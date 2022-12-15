using System;
using SiraUtil.Tools.FPFC;
using UnityEngine;
using Zenject;

namespace SquatToBegin.GameLogic {
	class SquatChecker : ILateTickable, IDisposable {
		public static bool enableOnNextSong = false;
		static int forcedSquatsOnNextStart = 0;

		readonly AudioTimeSyncController atsc;
		readonly PauseMenuManager pauseMenuManager;
		readonly Transform headTransform;

		public bool allowPlay { get; private set; } = false;

		public readonly Instructor instructor;

		static readonly IPA.Utilities.FieldAccessor<PlayerTransforms, Transform>.Accessor PlayerTransforms_Transform
			= IPA.Utilities.FieldAccessor<PlayerTransforms, Transform>.GetAccessor("_headTransform");

		public SquatChecker(
			AudioTimeSyncController atsc,
			PlayerTransforms playerTransforms,
			PauseMenuManager pauseMenuManager,
			IFPFCSettings FPFCSettings
		) {
#if !DEBUG
			if(FPFCSettings.Enabled) {
				allowPlay = true;
				return;
			}
#endif

			this.pauseMenuManager = pauseMenuManager;
			this.atsc = atsc;

			headTransform = PlayerTransforms_Transform(ref playerTransforms);

			instructor = new Instructor();

			pauseMenuManager.didPressRestartButtonEvent += PauseMenuManager_didPressRestartButtonEvent;

			if(!ShouldSquat())
				return;

			enableOnNextSong = false;
			forcedSquatsOnNextStart = 0;
		}

		public bool ShouldSquat(bool enableOverride = false) {
			if(forcedSquatsOnNextStart < 1 && ((!enableOnNextSong && !enableOverride) || Plugin.rng.NextDouble() >= Config.Instance.Chance)) {
				allowPlay = true;
				return false;
			}

			squatsNeeded = Math.Max(Config.Instance.SquatsNeeded, forcedSquatsOnNextStart);

			instructor?.SetText(squatsNeeded);
			instructor?.Show();

			allowPlay = false;

			return true;
		}

		public void Dispose() {
			pauseMenuManager.didPressRestartButtonEvent -= PauseMenuManager_didPressRestartButtonEvent;
		}

		private void PauseMenuManager_didPressRestartButtonEvent() {
			enableOnNextSong = true;
			forcedSquatsOnNextStart = squatsNeeded;
		}

		int squatsNeeded = 0;

		float standingHeight = 0;
		float targetHeight = 0;

		bool isUnsquatted = true;

		Action finishCallback;
		public void SetFinishCallback(Action callback) {
			finishCallback = callback;
		}

		public void LateTick() {
			if(headTransform == null || atsc == null)
				return;

			if(!allowPlay) {
				if(atsc.state == AudioTimeSyncController.State.Playing) {
					atsc.Pause();

					instructor.PlaySound();
				}
			} else if(!Config.Instance.CountSquatsDoneMidLevel) {
				return;
			}

			var p = headTransform.localPosition.y;

#if DEBUG
			if(Input.GetKeyDown(KeyCode.Space)) {
				p = -420;
			} else {
				p = 420;
			}
#endif

			if(p == 0)
				return;

			if(targetHeight == 0) {
				standingHeight = p - Math.Max(0.1f, Config.Instance.SquatAmount * .25f);
				targetHeight = p - Config.Instance.SquatAmount;
			}

			if(p >= standingHeight)
				isUnsquatted = true;

			if(isUnsquatted && p < targetHeight) {
				instructor.ConfirmSquat();

				if(squatsNeeded > 0) {
					if(--squatsNeeded == 0) {
						finishCallback?.Invoke();
						finishCallback = null;

						allowPlay = true;
						atsc.Resume();
						instructor.Hide();
					} else {
						instructor.SetText(squatsNeeded);
					}
				}
				
				isUnsquatted = false;
			}
		}
	}
}
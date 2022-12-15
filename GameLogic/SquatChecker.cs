using System;
using System.Linq;
using SiraUtil.Tools.FPFC;
using UnityEngine;
using Zenject;

namespace SquatToBegin.GameLogic {
	class SquatChecker : ILateTickable, IDisposable, IInitializable {
		public static bool enableOnNextSong = false;
		static int forcedSquatsOnNextStart = 0;

		readonly AudioTimeSyncController atsc;
		readonly PauseMenuManager pauseMenuManager;

		public bool allowPlay { get; private set; } = false;
		public bool isActive { get; private set; } = false;

		readonly Instructor instructor;

		Camera headCamera;

		public SquatChecker(
			AudioTimeSyncController atsc,
			PlayerTransforms playerTransforms,
			PauseMenuManager pauseMenuManager,
			IFPFCSettings FPFCSettings,
			Instructor instructor
		) {
#if !DEBUG
			if(FPFCSettings.Enabled) {
				allowPlay = true;
				return;
			}
#endif

			this.pauseMenuManager = pauseMenuManager;
			this.atsc = atsc;

			this.instructor = instructor;

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

		public void Initialize() => FindTheCamera();

		bool FindTheCamera() {
			headCamera = UnityEngine.Object.FindObjectsOfType<Camera>().FirstOrDefault(x => x.stereoEnabled && x.isActiveAndEnabled);

			return isActive = headCamera != null;
		}

		public void LateTick() {
			if(atsc == null)
				return;

#if !DEBUG
			if(headCamera == null) {
				isActive = false;
				return;
			}

			if(!headCamera.stereoEnabled || !headCamera.isActiveAndEnabled) {
				targetHeight = 0;
				if(!FindTheCamera())
					return;
			}
#endif

			if(!allowPlay) {
				if(atsc.state == AudioTimeSyncController.State.Playing) {
					atsc.Pause();

					instructor.Show();
					instructor.SetText(squatsNeeded);
					instructor.PlaySound();
				}
			} else if(!Config.Instance.CountSquatsDoneMidLevel) {
				return;
			}

			float p;

#if DEBUG
			if(Input.GetKeyDown(KeyCode.Space)) {
				p = -420;
			} else {
				p = 420;
			}
#else
			p = headCamera.transform.localPosition.y;

			if(p == 0)
				return;
#endif

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
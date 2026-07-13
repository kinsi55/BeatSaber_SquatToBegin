using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using SquatToBegin.AppLogic;
using TMPro;
using UnityEngine;

namespace SquatToBegin.GameLogic {
	class Instructor {
		readonly StatsTracker statsTracker;
		readonly UserSoundManager userSoundManager;

		GameObject cleanInfoText;
		TextMeshPro cleanLabel;

		static AudioSource source = null;

		public void ConfirmSquat() {
			statsTracker.AddSquats();

			if(Config.Instance.Ding)
				PlayRandomSound(okSounds, UserSoundManager.okSounds);
		}

		string action = "begin";
		public void SetActionText(string text) {
			action = text;
		}

		public void SetText(int requiredSquats) {
			if(cleanLabel == null)
				return;

			cleanLabel.text = $"<color=#3F3>Squat {requiredSquats}x to {action}</color> <color=#FC5>🏃</color>\n" +
				$"<size=3>{StatsTracker.sessionCounter} squat{(StatsTracker.sessionCounter != 1 ? "s" : "")} this session\n" + 
				$"<size=1.8><color=#BBB>{statsTracker.alltimeCounter} total";
		}

		public void Show() {
			if(cleanInfoText != null) {
				cleanInfoText.gameObject.SetActive(true);
			} else {
				cleanInfoText = new GameObject($"SquatToBegin", typeof(Canvas), typeof(TextMeshPro));

				cleanLabel = cleanInfoText.GetComponent<TextMeshPro>();
				cleanLabel.richText = true;
				cleanLabel.fontSize = 4f;
				cleanLabel.alignment = TextAlignmentOptions.Center;

				cleanInfoText.transform.position = new Vector3(0, 1.5f, 4f);
			}
		}

		public void Hide() {
			cleanInfoText?.gameObject.SetActive(false);
			source?.Stop();
		}

		static List<AudioClip> sounds;
		static List<AudioClip> okSounds;

		void PlayRandomSound(List<AudioClip> builtin, List<AudioClip> user) {
			var builtinCount = Config.Instance.AppendBuiltinSounds ? (builtin?.Count ?? 0) : 0;
			var max = builtinCount + (user?.Count ?? 0);

			var item = Plugin.rng.Next(max);
			var tList = builtin;

			if(item >= builtinCount || builtin == null) {
				tList = user;
				item -= builtinCount;
			}

			if((tList?.Count ?? 0) == 0)
				return;

			source.PlayOneShot(tList[item]);
		}

		public Instructor(StatsTracker statsTracker) {
			this.statsTracker = statsTracker;

			if((sounds == null && Config.Instance.Olaf) || (okSounds == null && Config.Instance.Ding)) {
				using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SquatToBegin.iloveolaf")) {
					var bundle = AssetBundle.LoadFromStream(stream);

					if(Config.Instance.Ding)
						okSounds ??= new List<AudioClip> { bundle.LoadAsset<AudioClip>("ok") };

					if(sounds == null && Config.Instance.Olaf) {
						sounds = new List<AudioClip> {
							bundle.LoadAsset<AudioClip>("1"),
							bundle.LoadAsset<AudioClip>("2"),
							bundle.LoadAsset<AudioClip>("3"),
							bundle.LoadAsset<AudioClip>("4")
						};
					}

					bundle.Unload(false);
				}

				Plugin.Log.Info(string.Format("Instructor sounds initialized", sounds.Count));
			}
		}

		public void PlaySound() {
			Plugin.Log.Info("PlaySound()");
			if(sounds == null)
				return;

			if(source == null) {
				source = new GameObject("SquatSoundPlayer").AddComponent<AudioSource>();

				source.volume = 0.5f;
				source.ignoreListenerPause = true;

				GameObject.DontDestroyOnLoad(source);
			}

			Plugin.Log.Info("Grabbed source");

			if(Config.Instance.Olaf) {
				source.Stop();

				PlayRandomSound(sounds, UserSoundManager.sounds);
			}
		}
	}
}

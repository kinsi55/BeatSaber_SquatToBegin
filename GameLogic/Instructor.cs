using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SquatToBegin.GameLogic {
	class Instructor {
		static int sessionCounter = 0;

		GameObject cleanInfoText;
		TextMeshPro cleanLabel;

		static AudioSource source = null;

		public void ConfirmSquat() {
			sessionCounter++;

			if(Config.Instance.Ding && okSound != null)
				source.PlayOneShot(okSound);
		}

		string action = "begin";
		public void SetActionText(string text) {
			action = text;
		}

		public void SetText(int requiredSquats) {
			cleanLabel.text = $"<color=#3F3>Squat {requiredSquats}x to {action}</color> <color=#FC5>🏃</color>\n<size=3>{sessionCounter} squat{(sessionCounter != 1 ? "s" : "")} this session";
		}

		public void Show() => cleanInfoText.gameObject.SetActive(true);
		public void Hide() => cleanInfoText.gameObject.SetActive(false);

		static AudioClip[] sounds;
		static AudioClip okSound;

		public Instructor() {
			cleanInfoText = new GameObject($"Label", typeof(Canvas), typeof(TextMeshPro));

			cleanLabel = cleanInfoText.GetComponent<TextMeshPro>();
			cleanLabel.richText = true;
			cleanLabel.fontSize = 4f;
			cleanLabel.alignment = TextAlignmentOptions.Center;

			cleanInfoText.transform.position = new Vector3(0, 1.5f, 4f);

			Hide();

			if(sounds == null && (Config.Instance.Olaf || Config.Instance.Ding)) {
				using(var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SquatToBegin.iloveolaf")) {
					var bundle = AssetBundle.LoadFromStream(stream);

					okSound = bundle.LoadAsset<AudioClip>("ok");
					sounds = new[] {
						bundle.LoadAsset<AudioClip>("1"),
						bundle.LoadAsset<AudioClip>("2"),
						bundle.LoadAsset<AudioClip>("3"),
						bundle.LoadAsset<AudioClip>("4")
					};

					bundle.Unload(false);
				}
			}
		}

		public void PlaySound() {
			if(sounds == null)
				return;

			if(source == null) {
				source = new GameObject("SquatSoundPlayer").AddComponent<AudioSource>();

				source.volume = 0.5f;
				source.ignoreListenerPause = true;

				GameObject.DontDestroyOnLoad(source);
			}

			if(Config.Instance.Olaf) {
				source.Stop();
				source.PlayOneShot(sounds[Plugin.rng.Next(sounds.Length)]);
			}
		}
	}
}

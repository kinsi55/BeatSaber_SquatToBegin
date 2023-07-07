using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IPA.Utilities;
using UnityEngine;
using Zenject;

namespace SquatToBegin.AppLogic {
	class UserSoundManager : IInitializable {
		IMediaAsyncLoader mediaAsyncLoader;
		public static List<AudioClip> sounds;
		public static List<AudioClip> okSounds;

		public UserSoundManager(IMediaAsyncLoader mediaAsyncLoader) {
			this.mediaAsyncLoader = mediaAsyncLoader;
		}

		public void Initialize() {
			if(sounds != null)
				return;

			LoadClips(Path.Combine(UnityGame.UserDataPath, "SquatToBegin", "begin")).ContinueWith(x => sounds = x.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
			LoadClips(Path.Combine(UnityGame.UserDataPath, "SquatToBegin", "ok")).ContinueWith(x => okSounds = x.Result, TaskContinuationOptions.OnlyOnRanToCompletion);
		}

		private async Task<List<AudioClip>> LoadClips(string path) {
			if(!Directory.Exists(path))
				return null;

			var files = Directory.GetFiles(path, "*.ogg");

			if(files.Length == 0)
				return null;

			var x = await Task.WhenAll(files.Select(file => {
				return mediaAsyncLoader.LoadAudioClipFromFilePathAsync(file).ContinueWith(t => t.IsFaulted ? null : t.Result);
			}));

			var outArr = x.Where(x => x != null).ToList();

			if(outArr.Count == 0)
				return null;

			return outArr;
		}
	}
}

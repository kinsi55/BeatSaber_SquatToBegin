using System;
using System.IO;
using System.Threading.Tasks;

namespace SquatToBegin.AppLogic {
	class StatsTracker {
		public static int sessionCounter { get; private set; } = 0;
		public int alltimeCounter { get; private set; } = 0;

		static readonly string statsFilePath = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "SquatToBeginCounter.txt");

		public StatsTracker() {
			if(File.Exists(statsFilePath)) {
				var content = File.ReadAllText(statsFilePath);
				var lines = content.Split('\n');
				if(int.TryParse(lines[0], out var alltime)) alltimeCounter = alltime;
				if(Config.Instance.TryPreserveSession && int.TryParse(lines[1], out var session) && DateTime.TryParse(lines[2], out var lastWrite)) {
					var now = DateTime.Now;
					if(now - lastWrite < TimeSpan.FromMinutes(10)) {
						Plugin.Log.Info("Restoring last session counter");
						sessionCounter = session;
					}
				}

				WriteSquats();
			}
		}

		public void AddSquats(int amount = 1) {
			sessionCounter += amount;
			alltimeCounter += amount;

			WriteSquats();
		}

		void WriteSquats() {
			Task.Run(() => {
				try {
					var lastWrite = DateTime.Now;
					File.WriteAllText(statsFilePath, $"{alltimeCounter}\n{sessionCounter}\n{lastWrite}");
				} catch { }
			});
		}
	}
}

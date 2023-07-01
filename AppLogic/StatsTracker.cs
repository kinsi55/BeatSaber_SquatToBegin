using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquatToBegin.AppLogic {
	class StatsTracker {
		public static int sessionCounter { get; private set; } = 0;
		public int alltimeCounter { get; private set; } = 0;

		static readonly string statsFilePath = Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "SquatToBeginCounter.txt");

		public StatsTracker() {
			if(File.Exists(statsFilePath)) {
				using(var f = File.OpenRead(statsFilePath)) {
					using(var r = new StreamReader(f)) {
						if(!r.EndOfStream && int.TryParse(r.ReadLine(), out var alltimeCounter))
							this.alltimeCounter = alltimeCounter;
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
					File.WriteAllText(statsFilePath, $"{alltimeCounter}\n{sessionCounter}");
				} catch { }
			});
		}
	}
}

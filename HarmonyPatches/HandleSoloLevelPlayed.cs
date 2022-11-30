using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using SquatToBegin.GameLogic;

namespace SquatToBegin.HarmonyPatches {
	[HarmonyPatch(typeof(SinglePlayerLevelSelectionFlowCoordinator), nameof(SinglePlayerLevelSelectionFlowCoordinator.StartLevel))]
	static class HandleSoloLevelPlayed {
		static void Prefix(bool practice) {
			if(practice && !Config.Instance.EnableInPractice)
				return;

			SquatChecker.enableOnNextSong = true;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiraUtil.Affinity;
using Zenject;

namespace SquatToBegin.GameLogic {
	class UnpauseHandler : IAffinity {
		[Inject] readonly SquatChecker squatChecker = null;

		[AffinityPatch(typeof(PauseController), nameof(PauseController.HandlePauseMenuManagerDidPressContinueButton))]
		[AffinityPrefix]
		bool Prefix(PauseController __instance, PauseMenuManager ____pauseMenuManager) {
			if(!squatChecker.allowPlay)
				return true;

			squatChecker.instructor.SetActionText("continue");
			if(squatChecker.ShouldSquat(true)) {
				____pauseMenuManager.gameObject.SetActive(false);

				squatChecker.SetFinishCallback(() => {
					____pauseMenuManager.gameObject.SetActive(true);
					__instance.HandlePauseMenuManagerDidPressContinueButton();
				});

				return false;
			}

			return true;
		}
	}
}

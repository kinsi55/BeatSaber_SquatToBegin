using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using SquatToBegin.GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace SquatToBegin {
	//[Plugin(RuntimeOptions.SingleStartInit)]
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin {
		internal static Plugin Instance;
		internal static IPALogger Log;

		public static System.Random rng = new System.Random();

		public static Harmony harmony;

		[Init]
		public Plugin(IPALogger logger, IPA.Config.Config conf, Zenjector zenjector) {
			Instance = this;
			Log = logger;
			Config.Instance = conf.Generated<Config>();

			zenjector.Install(Location.StandardPlayer, container => {
				if(Config.Instance.SquatsNeeded <= 0)
					return;

				container.BindInterfacesAndSelfTo<SquatChecker>().AsSingle().NonLazy();

				if(Config.Instance.EnableAfterPause)
					container.BindInterfacesTo<UnpauseHandler>().AsSingle();
			});

			BSMLSettings.instance.AddSettingsMenu("SquatToBegin", "SquatToBegin.UI.settings.bsml", Config.Instance);

			harmony = new Harmony("Kinsi55.BeatSaber.SquatToBegin");

			harmony.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}

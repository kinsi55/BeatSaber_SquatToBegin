using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace SquatToBegin {
	internal class Config {
		public static Config Instance;
		public virtual float SquatAmount { get; set; } = 0.4f;
		public virtual float Chance { get; set; } = 1f;
		public virtual int SquatsNeeded { get; set; } = 1;
		public virtual bool EnableInPractice { get; set; } = false;
		public virtual bool EnableAfterPause { get; set; } = false;
		public virtual bool Olaf { get; set; } = true;
		public virtual bool Ding { get; set; } = true;
		public virtual bool AppendBuiltinSounds { get; set; } = true;
		public virtual bool CountSquatsDoneMidLevel { get; set; } = false;
		public virtual bool TryPreserveSession { get; set; } = true;

		/*/// <summary>
		/// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
		/// </summary>
		public virtual void OnReload() {
			// Do stuff after config is read from disk.
		}

		/// <summary>
		/// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
		/// </summary>
		public virtual void Changed() {
			// Do stuff when the config is changed.
		}

		/// <summary>
		/// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
		/// </summary>
		public virtual void CopyFrom(Config other) {
			// This instance's members populated from other
		}*/
	}
}
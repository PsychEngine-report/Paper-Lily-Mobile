using Godot;

namespace LacieEngine.Settings
{
	public abstract class SettingParent
	{
		public abstract void WriteValue(ConfigFile configFile);

		public abstract void Apply();
	}
}

using Godot;

namespace LacieEngine.Core
{
	public static class LightBlendModeExtension
	{
		public static Light2D.ModeEnum ToGodotLight2DMode(this LightBlendMode value)
		{
			return value switch
			{
				LightBlendMode.Add => Light2D.ModeEnum.Add, 
				LightBlendMode.Sub => Light2D.ModeEnum.Sub, 
				LightBlendMode.Mix => Light2D.ModeEnum.Mix, 
				_ => Light2D.ModeEnum.Add, 
			};
		}
	}
}

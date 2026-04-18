using Godot;

namespace LacieEngine.API
{
	public static class EditorAreaExtension
	{
		public static Vector2 GetPixelPerfectOffset(this IEditorArea rect)
		{
			return new Vector2((rect.Area.x % 2f != 0f) ? 0.5f : 0f, (rect.Area.y % 2f != 0f) ? 0.5f : 0f);
		}

		public static Rect2 ToRect2(this IEditorArea rect)
		{
			return new Rect2(-1f * (rect.Area / 2f) + rect.GetPixelPerfectOffset(), rect.Area);
		}
	}
}

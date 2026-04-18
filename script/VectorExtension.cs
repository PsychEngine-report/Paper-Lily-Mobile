using Godot;

namespace LacieEngine.Core
{
	public static class VectorExtension
	{
		public static Vector2 FlattenX(this Vector2 vec)
		{
			return new Vector2(vec.x, 0f);
		}

		public static Vector2 FlattenY(this Vector2 vec)
		{
			return new Vector2(0f, vec.y);
		}

		public static Vector2 ReplaceX(this Vector2 vec, float newX)
		{
			return new Vector2(newX, vec.y);
		}

		public static Vector2 ReplaceY(this Vector2 vec, float newY)
		{
			return new Vector2(vec.x, newY);
		}

		public static Vector2 SignAwareCeil(this Vector2 vec)
		{
			float x = vec.x;
			float y = vec.y;
			x = ((x > 0f) ? Mathf.Ceil(x) : Mathf.Floor(x));
			y = ((y > 0f) ? Mathf.Ceil(y) : Mathf.Floor(y));
			return new Vector2(x, y);
		}
	}
}

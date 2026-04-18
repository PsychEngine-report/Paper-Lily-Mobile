using System;
using Godot;

namespace LacieEngine.Core
{
	public static class RandomExtension
	{
		public static float NextFloat(this Random random)
		{
			return (float)random.NextDouble();
		}

		public static Vector2 NextVector2(this Random random)
		{
			return new Vector2(random.NextFloat(), random.NextFloat());
		}

		public static Direction NextDirection(this Random random)
		{
			return random.Next(8) switch
			{
				0 => Direction.Left, 
				1 => Direction.UpLeft, 
				2 => Direction.Up, 
				3 => Direction.UpRight, 
				4 => Direction.Right, 
				5 => Direction.DownRight, 
				6 => Direction.Down, 
				_ => Direction.DownLeft, 
			};
		}
	}
}

using System;
using Godot;

namespace LacieEngine.Core
{
	public static class Node2DExtension
	{
		public static void PixelSnap(this Node2D node, bool x = true, bool y = true)
		{
			if (x || y)
			{
				Vector2 newPos = node.Position;
				if (x)
				{
					newPos.x = (int)Math.Round(newPos.x);
				}
				if (y)
				{
					newPos.y = (int)Math.Round(newPos.y);
				}
				node.Position = newPos;
			}
		}
	}
}

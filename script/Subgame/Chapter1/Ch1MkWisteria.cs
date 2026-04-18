using Godot;
using LacieEngine.API;
using LacieEngine.Core;

namespace LacieEngine.Rooms
{
	[Tool]
	public class Ch1MkWisteria : GameRoom
	{
		[GetNode("Background/Canvas")]
		private Sprite nCanvas;

		[GetNode("Background/Canvas/MkDarkness")]
		private Node2D nDarkness;

		[GetNode("Background/Canvas/Painting")]
		private Sprite nPainting;

		[GetNode("Background/WallPainting")]
		private Sprite nItemHolderPainting;

		private PVar varRevealedCanvas = "ch1.mk_revealed_canvas";

		private PVar varTookCanvas = "ch1.mk_took_canvas";

		private PVar varPainting = "ch1.mk_canvas_color";

		private PVar varItemHolderItem = "ch1.mk_wisteria_item";

		public override void _UpdateRoom()
		{
			nCanvas.Frame = (varTookCanvas.Value ? 2 : (((bool)varRevealedCanvas.Value) ? 1 : 0));
			nDarkness.Visible = nCanvas.Frame == 1;
			nPainting.Visible = !varTookCanvas.Value && (bool)varPainting.Value;
			if (nPainting.Visible)
			{
				Sprite sprite = nPainting;
				sprite.Frame = (string)varPainting.Value switch
				{
					"red" => 0, 
					"yellow" => 2, 
					"blue" => 1, 
					"orange" => 4, 
					"purple" => 3, 
					"green" => 5, 
					"all" => 6, 
					"brown" => 7, 
					_ => 0, 
				};
			}
			nItemHolderPainting.Frame = varItemHolderItem.Value;
		}
	}
}

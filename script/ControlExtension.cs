using Godot;

namespace LacieEngine.Core
{
	public static class ControlExtension
	{
		public static void SetAnchorsAndMarginsPreset(this Control control, Control.LayoutPreset layout, Vector2 size)
		{
			switch (layout)
			{
			case Control.LayoutPreset.LeftWide:
				control.AnchorLeft = 0f;
				control.AnchorTop = 0f;
				control.AnchorRight = 0f;
				control.AnchorBottom = 1f;
				control.MarginLeft = 0f;
				control.MarginTop = 0f;
				control.MarginRight = size.x;
				control.MarginBottom = 0f;
				break;
			case Control.LayoutPreset.TopWide:
				control.AnchorLeft = 0f;
				control.AnchorTop = 0f;
				control.AnchorRight = 1f;
				control.AnchorBottom = 0f;
				control.MarginLeft = 0f;
				control.MarginTop = 0f;
				control.MarginRight = 0f;
				control.MarginBottom = size.y;
				break;
			case Control.LayoutPreset.RightWide:
				control.AnchorLeft = 1f;
				control.AnchorTop = 0f;
				control.AnchorRight = 1f;
				control.AnchorBottom = 1f;
				control.MarginLeft = 0f - size.x;
				control.MarginTop = 0f;
				control.MarginRight = 0f;
				control.MarginBottom = 0f;
				break;
			case Control.LayoutPreset.BottomWide:
				control.AnchorLeft = 0f;
				control.AnchorTop = 1f;
				control.AnchorRight = 1f;
				control.AnchorBottom = 1f;
				control.MarginLeft = 0f;
				control.MarginTop = 0f - size.y;
				control.MarginRight = 0f;
				control.MarginBottom = 0f;
				break;
			case Control.LayoutPreset.TopLeft:
				control.AnchorLeft = 0f;
				control.AnchorTop = 0f;
				control.AnchorRight = 0f;
				control.AnchorBottom = 0f;
				control.MarginLeft = 0f;
				control.MarginTop = 0f;
				control.MarginRight = size.x;
				control.MarginBottom = size.y;
				break;
			case Control.LayoutPreset.CenterTop:
				control.AnchorLeft = 0.5f;
				control.AnchorTop = 0f;
				control.AnchorRight = 0.5f;
				control.AnchorBottom = 0f;
				control.MarginLeft = (0f - size.x) / 2f;
				control.MarginTop = 0f;
				control.MarginRight = size.x / 2f;
				control.MarginBottom = size.y;
				break;
			case Control.LayoutPreset.CenterBottom:
				control.AnchorLeft = 0.5f;
				control.AnchorTop = 1f;
				control.AnchorRight = 0.5f;
				control.AnchorBottom = 1f;
				control.MarginLeft = (0f - size.x) / 2f;
				control.MarginTop = 0f - size.y;
				control.MarginRight = size.x / 2f;
				control.MarginBottom = 0f;
				break;
			case Control.LayoutPreset.TopRight:
				control.AnchorLeft = 1f;
				control.AnchorTop = 0f;
				control.AnchorRight = 1f;
				control.AnchorBottom = 0f;
				control.MarginLeft = 0f - size.x;
				control.MarginTop = 0f;
				control.MarginRight = 0f;
				control.MarginBottom = size.y;
				break;
			case Control.LayoutPreset.BottomLeft:
				control.AnchorLeft = 0f;
				control.AnchorTop = 1f;
				control.AnchorRight = 0f;
				control.AnchorBottom = 1f;
				control.MarginLeft = 0f;
				control.MarginTop = 0f - size.y;
				control.MarginRight = size.x;
				control.MarginBottom = 0f;
				break;
			case Control.LayoutPreset.BottomRight:
				control.AnchorLeft = 1f;
				control.AnchorTop = 1f;
				control.AnchorRight = 1f;
				control.AnchorBottom = 1f;
				control.MarginLeft = 0f - size.x;
				control.MarginTop = 0f - size.y;
				control.MarginRight = 0f;
				control.MarginBottom = 0f;
				break;
			default:
				Log.Warn("Layout type not supported, using default behavior: " + layout);
				control.SetAnchorsAndMarginsPreset(layout);
				break;
			}
		}

		public static void SetAnchorsGrowFrom(this Control control, float pointX, float pointY, Direction growTowards)
		{
			float anchorLeft = (control.AnchorRight = pointX);
			control.AnchorLeft = anchorLeft;
			anchorLeft = (control.AnchorBottom = pointY);
			control.AnchorTop = anchorLeft;
			Control control2 = control;
			control2.GrowHorizontal = growTowards.ToEnum() switch
			{
				Direction.Enum.None => Control.GrowDirection.Both, 
				Direction.Enum.Left => Control.GrowDirection.Begin, 
				Direction.Enum.Up => Control.GrowDirection.Both, 
				Direction.Enum.UpLeft => Control.GrowDirection.Begin, 
				Direction.Enum.Right => Control.GrowDirection.End, 
				Direction.Enum.UpRight => Control.GrowDirection.End, 
				Direction.Enum.Down => Control.GrowDirection.Both, 
				Direction.Enum.DownLeft => Control.GrowDirection.Begin, 
				Direction.Enum.DownRight => Control.GrowDirection.End, 
				_ => Control.GrowDirection.Both, 
			};
			control2 = control;
			control2.GrowVertical = growTowards.ToEnum() switch
			{
				Direction.Enum.None => Control.GrowDirection.Both, 
				Direction.Enum.Left => Control.GrowDirection.Both, 
				Direction.Enum.Up => Control.GrowDirection.Begin, 
				Direction.Enum.UpLeft => Control.GrowDirection.Begin, 
				Direction.Enum.Right => Control.GrowDirection.Both, 
				Direction.Enum.UpRight => Control.GrowDirection.Begin, 
				Direction.Enum.Down => Control.GrowDirection.End, 
				Direction.Enum.DownLeft => Control.GrowDirection.End, 
				Direction.Enum.DownRight => Control.GrowDirection.End, 
				_ => Control.GrowDirection.Both, 
			};
		}

		public static void SetAnchorsGrowFrom(this Control control, float pointX, float pointY)
		{
			control.SetAnchorsGrowFrom(pointX, pointY, Direction.None);
		}

		public static void SetAnchorsGrowFrom(this Control control, Vector2 point, Direction growTowards)
		{
			control.SetAnchorsGrowFrom(point.x, point.y, growTowards);
		}

		public static void SetAnchorsGrowFrom(this Control control, Vector2 point)
		{
			control.SetAnchorsGrowFrom(point, Direction.None);
		}

		public static void SetSeparation(this BoxContainer box, int value)
		{
			box.Set("custom_constants/separation", value);
		}

		public static void SetSeparation(this GridContainer box, int xValue, int yValue)
		{
			box.Set("custom_constants/hseparation", xValue);
			box.Set("custom_constants/vseparation", yValue);
		}

		public static void SetContainerMarginLeft(this MarginContainer container, int value)
		{
			container.Set("custom_constants/margin_left", value);
		}

		public static void SetContainerMarginTop(this MarginContainer container, int value)
		{
			container.Set("custom_constants/margin_top", value);
		}

		public static void SetContainerMarginRight(this MarginContainer container, int value)
		{
			container.Set("custom_constants/margin_right", value);
		}

		public static void SetContainerMarginBottom(this MarginContainer container, int value)
		{
			container.Set("custom_constants/margin_bottom", value);
		}

		public static void SetContainerMargins(this MarginContainer container, int value)
		{
			container.SetContainerMarginLeft(value);
			container.SetContainerMarginTop(value);
			container.SetContainerMarginRight(value);
			container.SetContainerMarginBottom(value);
		}

		public static void SetFont(this Label label, string fontName)
		{
			string fontPath = (fontName.StartsWith("res://") ? fontName : ("res://resources/font/" + fontName + ".tres"));
			label.AddFontOverride("font", GD.Load<Font>(fontPath));
		}

		public static void SetDefaultFontAndColor(this Label label)
		{
			label.SetFont("res://resources/font/default.tres");
			label.SetFontColor(Constants.Colors.DefaultTextColor);
		}

		public static void SetFontColor(this Label label, Color color)
		{
			label.AddColorOverride("font_color", color);
		}

		public static void SetFontShadowColor(this Label label, Color color)
		{
			label.AddColorOverride("font_color_shadow", color);
		}

		public static void SetFontOutlineColor(this Label label, Color color)
		{
			label.AddColorOverride("font_outline_modulate", color);
		}

		public static void SetFont(this RichTextLabel label, string fontName)
		{
			string fontPath = (fontName.StartsWith("res://") ? fontName : ("res://resources/font/" + fontName + ".tres"));
			label.AddFontOverride("normal_font", GD.Load<Font>(fontPath));
		}

		public static void SetDefaultFontAndColor(this RichTextLabel label)
		{
			label.SetFont("res://resources/font/default.tres");
			label.SetFontColor(Constants.Colors.DefaultTextColor);
		}

		public static void SetFontColor(this RichTextLabel label, Color color)
		{
			label.AddColorOverride("default_color", color);
		}

		public static Color GetPixelAt(this TextureRect texture, Vector2 coords)
		{
			Image image = texture.Texture.GetData();
			image.Lock();
			Color pixel = image.GetPixel((int)coords.x, (int)coords.y);
			image.Unlock();
			return pixel;
		}

		public static void CollapseAll(this TreeItem treeItem)
		{
			treeItem.Collapsed = true;
			treeItem.GetChildren()?.CollapseAll();
			treeItem.GetNext()?.CollapseAll();
		}
	}
}

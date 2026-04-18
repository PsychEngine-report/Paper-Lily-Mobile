using System.Text.RegularExpressions;
using Godot;
using LacieEngine.Core;

namespace LacieEngine.UI
{
	public class LabelWithImages : HBoxContainer
	{
		private const string _pattern = "\\[img.*?\\](.*?)\\[\\/img\\]";

		private static readonly Vector2 _imageSize = new Vector2(14f, 14f);

		private string _bbcode;

		public string Text
		{
			get
			{
				return _bbcode;
			}
			set
			{
				_bbcode = value;
				UpdateContent();
			}
		}

		public LabelWithImages()
		{
			base.Name = "TextLine";
			_bbcode = "";
			this.SetSeparation(1);
		}

		public void SetLeftAlign()
		{
			base.Alignment = AlignMode.Begin;
		}

		public void SetCenterAlign()
		{
			base.Alignment = AlignMode.Center;
		}

		public void SetRightAlign()
		{
			base.Alignment = AlignMode.End;
		}

		private void UpdateContent()
		{
			this.Clear();
			int pointer = 0;
			int index = 0;
			foreach (Match m in Regex.Matches(_bbcode, "\\[img.*?\\](.*?)\\[\\/img\\]"))
			{
				if (m.Index - pointer > 0)
				{
					Label text = GDUtil.MakeNode<Label>(++index + "Text");
					string bbcode = _bbcode;
					int num = pointer;
					text.Text = bbcode.Substring(num, m.Index - num);
					text.SetDefaultFontAndColor();
					AddChild(text);
				}
				TextureRect image = GDUtil.MakeNode<TextureRect>(++index + "Image");
				image.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
				image.Expand = true;
				image.RectMinSize = _imageSize;
				image.Texture = GD.Load<Texture>(m.Groups[1].Value);
				AddChild(image);
				pointer = m.Index + m.Value.Length;
			}
			if (pointer < _bbcode.Length)
			{
				Label text2 = GDUtil.MakeNode<Label>(++index + "Text");
				string bbcode2 = _bbcode;
				int num = pointer;
				text2.Text = bbcode2.Substring(num, bbcode2.Length - num);
				text2.SetDefaultFontAndColor();
				AddChild(text2);
			}
		}
	}
}

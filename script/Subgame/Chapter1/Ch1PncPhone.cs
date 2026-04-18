using System.Collections.Generic;
using Godot;
using LacieEngine.API;
using LacieEngine.Core;
using LacieEngine.Minigames;
using LacieEngine.UI;

namespace LacieEngine.Subgame.Chapter1
{
	public class Ch1PncPhone : PointAndClick
	{
		[Export(PropertyHint.None, "")]
		public Texture TexDigits;

		[Export(PropertyHint.None, "")]
		public AudioStream[] SfxDialTones;

		public Dictionary<string, string> CorrectCombinations;

		[GetNode("Display")]
		protected Control nDigitContainer;

		private const int MaxDigits = 8;

		private string _curCombination = "";

		public override void _Ready()
		{
			base._Ready();
			if (CorrectCombinations.IsNullOrEmpty())
			{
				Log.Error("Phone combinations not initialized!");
				CorrectCombinations = new Dictionary<string, string>();
			}
		}

		public void Dial0()
		{
			DialNumber("0");
		}

		public void Dial1()
		{
			DialNumber("1");
		}

		public void Dial2()
		{
			DialNumber("2");
		}

		public void Dial3()
		{
			DialNumber("3");
		}

		public void Dial4()
		{
			DialNumber("4");
		}

		public void Dial5()
		{
			DialNumber("5");
		}

		public void Dial6()
		{
			DialNumber("6");
		}

		public void Dial7()
		{
			DialNumber("7");
		}

		public void Dial8()
		{
			DialNumber("8");
		}

		public void Dial9()
		{
			DialNumber("9");
		}

		public void DialAsterisk()
		{
			DialNumber("*");
		}

		public void DialPound()
		{
			DialNumber("#");
		}

		private void DialNumber(string digit)
		{
			PlayDialTone(digit);
			_curCombination += digit;
			nDigitContainer.AddChild(MakeDigitTexture(digit));
			if (CorrectCombinations.ContainsKey(_curCombination))
			{
				EndMinigame(CorrectCombinations[_curCombination]);
			}
			else if (_curCombination.Length >= 8)
			{
				EndMinigame("event_dial_wrong_number");
			}
		}

		private async void EndMinigame(string @event)
		{
			Game.InputProcessor = Inputs.Processor.None;
			nCursor.Visible = false;
			await DrkieUtil.DelaySeconds(1.0);
			Game.Minigames.End(@event);
		}

		private TextureRect MakeDigitTexture(string digit)
		{
			SlicedTextureRect tex = new SlicedTextureRect();
			tex.Texture = TexDigits;
			tex.Hframes = 3;
			tex.Vframes = 4;
			SlicedTextureRect slicedTextureRect = tex;
			slicedTextureRect.Frame = digit switch
			{
				"1" => 0, 
				"2" => 1, 
				"3" => 2, 
				"4" => 3, 
				"5" => 4, 
				"6" => 5, 
				"7" => 6, 
				"8" => 7, 
				"9" => 8, 
				"*" => 9, 
				"0" => 10, 
				"#" => 11, 
				_ => 0, 
			};
			return tex;
		}

		private void PlayDialTone(string digit)
		{
			AudioStream sfx = digit switch
			{
				"1" => SfxDialTones[1], 
				"2" => SfxDialTones[2], 
				"3" => SfxDialTones[3], 
				"4" => SfxDialTones[4], 
				"5" => SfxDialTones[5], 
				"6" => SfxDialTones[6], 
				"7" => SfxDialTones[7], 
				"8" => SfxDialTones[8], 
				"9" => SfxDialTones[9], 
				"*" => SfxDialTones[10], 
				"0" => SfxDialTones[0], 
				"#" => SfxDialTones[11], 
				_ => SfxDialTones[0], 
			};
			Game.Audio.StopSfx();
			Game.Audio.PlaySfx(sfx);
		}
	}
}

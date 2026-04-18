using System.Collections.Generic;

namespace LacieEngine.Subgame.Chapter1
{
	public class Ch1PncPhoneDay : Ch1PncPhone
	{
		public override void _Ready()
		{
			CorrectCombinations = new Dictionary<string, string>();
			CorrectCombinations.Add("195224#1", "ch1_event_phone_mother");
			base._Ready();
		}
	}
}

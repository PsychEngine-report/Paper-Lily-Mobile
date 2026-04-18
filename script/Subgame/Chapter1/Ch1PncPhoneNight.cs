using System.Collections.Generic;

namespace LacieEngine.Subgame.Chapter1
{
	public class Ch1PncPhoneNight : Ch1PncPhone
	{
		public override void _Ready()
		{
			CorrectCombinations = new Dictionary<string, string>();
			CorrectCombinations.Add("195224#1", "ch1_event_phone_mother_night");
			CorrectCombinations.Add("999281#0", "ch1_event_phone_ritual");
			base._Ready();
		}
	}
}

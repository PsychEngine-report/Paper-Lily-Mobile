namespace LacieEngine.Core
{
	internal static class LogicEvaluator
	{
		public static bool Evaluate(this LogicStatement s)
		{
			bool result = PerformCheck(s);
			if (s.Not)
			{
				result = !result;
			}
			Log.Trace("Logic evaluator: evaluated to ", result);
			return result;
		}

		private static bool PerformCheck(LogicStatement s)
		{
			if (s.Type == LogicStatement.EType.Variable)
			{
				string expectedValue = ((!s.Value.IsNullOrEmpty()) ? s.Value : null);
				string actualValue = Game.Variables.GetVariable(s.Variable);
				bool truthyCheck = expectedValue == null;
				Log.Trace("Logic evaluator: VAR: ", s.Variable, " expected: ", expectedValue ?? "<truthy>", " actual: ", actualValue ?? "<null>");
				if (actualValue != null)
				{
					if (double.TryParse(actualValue, out var numValue))
					{
						if (truthyCheck)
						{
							return numValue != 0.0;
						}
						double expectedValueNum = double.Parse(expectedValue);
						if (s.Operator == LogicStatement.EOperator.Eq && numValue == expectedValueNum)
						{
							return true;
						}
						if (s.Operator == LogicStatement.EOperator.Lt && numValue < expectedValueNum)
						{
							return true;
						}
						if (s.Operator == LogicStatement.EOperator.Gt && numValue > expectedValueNum)
						{
							return true;
						}
						if (s.Operator == LogicStatement.EOperator.Le && numValue <= expectedValueNum)
						{
							return true;
						}
						if (s.Operator == LogicStatement.EOperator.Ge && numValue >= expectedValueNum)
						{
							return true;
						}
					}
					else
					{
						if (expectedValue == actualValue)
						{
							return true;
						}
						if (truthyCheck && actualValue != "false")
						{
							return true;
						}
					}
				}
			}
			else
			{
				if (s.Type == LogicStatement.EType.Item)
				{
					int amount = Game.Items.GetOwnedAmount(s.Item);
					Log.Trace("Logic evaluator: ITEM: ", s.Item, ", expected: ", s.Amount, " actual: ", amount);
					return amount >= s.Amount;
				}
				if (s.Type == LogicStatement.EType.Character)
				{
					bool pass = Game.State.Party.Contains(s.Value);
					Log.Trace("Logic evaluator: CHARACTER: ", s.Value, ", result: ", pass);
					return pass;
				}
				if (s.Type == LogicStatement.EType.HasObjective)
				{
					Log.Trace("Logic evaluator: HAS OBJECTIVE: ", s.Value);
					return Game.Objectives.IsObjectiveInProgress(s.Value);
				}
				if (s.Type == LogicStatement.EType.ObjectiveDone)
				{
					Log.Trace("Logic evaluator: OBJECTIVE DONE: ", s.Value);
					return Game.Objectives.IsObjectiveCompleted(s.Value);
				}
				if (s.Type == LogicStatement.EType.Repeat)
				{
					int amount2 = Game.Events.GetEventInteractionCount(s.Value);
					Log.Trace("Logic evaluator: REPEAT: ", s.Value, " expected: ", s.Amount, " actual: ", amount2);
					return amount2 >= s.Amount;
				}
				if (s.Type == LogicStatement.EType.Random)
				{
					if (double.TryParse(s.Value, out var successChance))
					{
						return DrkieUtil.RollPercent(successChance);
					}
					Log.Error("Logic evaluator: Random Value invalid.");
				}
				else
				{
					if (s.Type == LogicStatement.EType.System)
					{
						Log.Trace("Logic evaluator: SYSTEM: ", s.Value);
						return LogicEvaluatorSystem.Evaluate(s.Value);
					}
					Log.Warn("Logic evaluator: Malformed statement");
				}
			}
			return false;
		}
	}
}

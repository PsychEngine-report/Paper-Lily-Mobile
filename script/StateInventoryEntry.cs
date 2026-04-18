namespace LacieEngine.Core
{
	public class StateInventoryEntry
	{
		public string itemId;

		public int amount;

		public StateInventoryEntry(string id, int amount)
		{
			itemId = id;
			this.amount = amount;
		}
	}
}

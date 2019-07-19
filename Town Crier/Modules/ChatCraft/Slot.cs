using System.Collections.Generic;

namespace TownCrier.Modules.ChatCraft
{
	public class Slot
	{
		public List<string> names = new List<string>();
		public List<ItemType> allowedTypes = new List<ItemType>();
		public int side = 0;
	}
}

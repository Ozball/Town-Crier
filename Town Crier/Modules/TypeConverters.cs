//using Alta.WebApi.Models;
//using Alta.WebApi.Models.DTOs.Responses;
//using Discord;
//using Discord.Addons.Interactive;
//using Discord.Commands;
//using Discord.WebSocket;
//using TownCrier;
//using TownCrier.Modules.ChatCraft;
//using Newtonsoft.Json;
//using RestSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace TownCrier
//{
//	public class RequireAdminAttribute : PreconditionAttribute
//	{
//		public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
//		{
//			Player player = ChatCraft.Instance.GetPlayer(context.User);

//			if (!player.isAdmin)
//			{
//				return PreconditionResult.FromError("You are not an admin.");
//			}

//			return PreconditionResult.FromSuccess();
//		}
//	}

//	public abstract class ChatCraftTypeReader<T> : TypeReader
//	{
//		public static string LastInput { get;  set; }

//		public static T LastValue { get;  set; }

//		public ICommandContext Context { get;  set; }

//		public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
//		{
//			Context = context;

//			string toLower = input.ToLower();

//			string error = null;
//			T result = Find(ChatCraft.Instance.State, toLower, ref error);

//			if (result == null)
//			{
//				if (error == null)
//				{
//					return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, $"That is not {GetName()}."));
//				}

//				return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, error));
//			}

//			LastInput = input;
//			LastValue = result;

//			return Task.FromResult(TypeReaderResult.FromSuccess(result));
//		}

//		public abstract T Find(ChatCraftState state, string nameToLower, ref string error);

//		public string GetName()
//		{
//			string name = typeof(T).Name.ToString().ToLower();

//			char first = name[0];

//			if (first == 'a' ||
//				first == 'e' ||
//				first == 'i' ||
//				first == 'o' ||
//				first == 'u')
//			{
//				name = "an " + name;
//			}
//			else
//			{
//				name = "a " + name;
//			}

//			return name;
//		}
//	}

//	public abstract class SimpleChatCraftTypeReader<T> : ChatCraftTypeReader<T>
//	{
//		public override T Find(ChatCraftState state, string nameToLower, ref string error)
//		{
//			Func<T, bool> check = GetCheck(nameToLower, ref error);

//			if (check == null)
//			{
//				return default(T);
//			}

//			return GetList(state).FirstOrDefault(check);
//		}

//		public abstract List<T> GetList(ChatCraftState state);

//		public abstract Func<T, bool> GetCheck(string nameLower, ref string error);
//	}

//	public class SlotTypeReader : SimpleChatCraftTypeReader<Slot>
//	{
//		public override Func<Slot, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.names.Contains(nameLower);
//		}

//		public override List<Slot> GetList(ChatCraftState state)
//		{
//			return state.slots;
//		}
//	}

//	public class UnitTypeReader : ChatCraftTypeReader<Unit>
//	{
//		public override Unit Find(ChatCraftState state, string nameToLower, ref string error)
//		{
//			Player player = ChatCraft.Instance.GetPlayer(Context.User);

//			if (player.combatState == null)
//			{
//				error = "You are not in combat!";
//				return null;
//			}

//			if (nameToLower.StartsWith("<@!") && nameToLower.Length > 5)
//			{
//				string number = nameToLower.Substring(3, nameToLower.Length - 4);

//				ulong id;

//				if (ulong.TryParse(number, out id))
//				{
//					IUser user = Context.Guild.GetUserAsync(id).Result;

//					if (user != null)
//					{
//						return ChatCraft.Instance.GetPlayer(user);
//					}
//				}
//			}

//			return (from team in player.combatState.instance.teams
//					from Unit item in team.currentUnits
//					where item.name.ToLower() == nameToLower
//					select item).FirstOrDefault();
//		}
//	}

//	public class ItemTypeReader : SimpleChatCraftTypeReader<Item>
//	{
//		public override Func<Item, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<Item> GetList(ChatCraftState state)
//		{
//			return state.items;
//		}
//	}

//	public class RecipeTypeReader : SimpleChatCraftTypeReader<Recipe>
//	{
//		public override Func<Recipe, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<Recipe> GetList(ChatCraftState state)
//		{
//			return state.recipes;
//		}
//	}

//	public class LocationTypeReader : SimpleChatCraftTypeReader<Location>
//	{
//		public override Func<Location, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<Location> GetList(ChatCraftState state)
//		{
//			return state.locations;
//		}
//	}

//	public class EncounterSetTypeReader : SimpleChatCraftTypeReader<EncounterSet>
//	{
//		public override Func<EncounterSet, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<EncounterSet> GetList(ChatCraftState state)
//		{
//			return state.encounterSets;
//		}
//	}

//	public class ItemSetTypeReader : SimpleChatCraftTypeReader<ItemSet>
//	{
//		public override Func<ItemSet, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<ItemSet> GetList(ChatCraftState state)
//		{
//			return state.itemSets;
//		}
//	}

//	public class RecipeSetTypeReader : SimpleChatCraftTypeReader<RecipeSet>
//	{
//		public override Func<RecipeSet, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<RecipeSet> GetList(ChatCraftState state)
//		{
//			return state.recipeSets;
//		}
//	}

//	public class ExploreSetTypeReader : SimpleChatCraftTypeReader<ExploreSet>
//	{
//		public override Func<ExploreSet, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<ExploreSet> GetList(ChatCraftState state)
//		{
//			return state.exploreSets;
//		}
//	}

//	public class StatTypeReader : SimpleChatCraftTypeReader<Stat>
//	{
//		public override Func<Stat, bool> GetCheck(string nameLower, ref string error)
//		{
//			return test => test.name.ToLower() == nameLower;
//		}

//		public override List<Stat> GetList(ChatCraftState state)
//		{
//			return state.stats;
//		}
//	}

//	public abstract class LimitedAttribute : ParameterPreconditionAttribute
//	{
//		protected abstract Type Type { get; }

//		protected virtual IEnumerable<Type> Types { get { return null; } }

//		protected ICommandContext Context { get;  set; }

//		protected ParameterInfo ParameterInfo { get;  set; }

//		protected IServiceProvider Services { get;  set; }

//		protected object Value { get;  set; }

//		public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
//		{
//			if (Type != null)
//			{
//				if (!Type.IsAssignableFrom(parameter.Type))
//				{
//					return PreconditionResult.FromError($"{GetType().Name} can only be used on {Type.Name} parameters. Not {parameter.Type}.");
//				}
//			}
//			else
//			{
//				if (!Types.Any(test => test.IsAssignableFrom(parameter.Type)))
//				{
//					return PreconditionResult.FromError($"{GetType().Name} can only be used on set types. {parameter.Type} is not one of them.");
//				}
//			}

//			Value = value;
//			Context = context;
//			ParameterInfo = parameter;
//			Services = services;

//			if (value != null)
//			{
//				Player player = ChatCraft.Instance.GetPlayer(context.User);

//				if (MeetsCondition(player, value))
//				{
//					return PreconditionResult.FromSuccess();
//				}
//			}

//			return PreconditionResult.FromError(GetError());
//		}

//		protected abstract bool MeetsCondition(Player player, object value);

//		protected abstract string GetError();
//	}

//	public class InCombatWith : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Unit); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			Unit otherUnit = value as Unit;

//			return player.combatState != null &&
//				player.combatState.instance.teams.Any(team => team.currentUnits.Contains(otherUnit));
//		}

//		protected override string GetError()
//		{
//			return $"You are not in combat with { ((IUser)Value).Username }.";
//		}
//	}

//	public class AllyAttribute : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Unit); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			Unit unit = value as Unit;

//			bool isInCombat = player.combatState != null &&
//				player.combatState.instance.teams[player.combatState.teamIndex].currentUnits.Contains(unit);

//			return isInCombat;
//		}

//		protected override string GetError()
//		{
//			return $"They are not an ally!";
//		}
//	}

//	public class EnemyAttribute : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Unit); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			Unit unit = value as Unit;

//			bool isEnemy = player.combatState != null &&
//				player.combatState.instance.teams[(player.combatState.teamIndex + 1) % 2].currentUnits.Contains(unit);

//			return isEnemy;
//		}

//		protected override string GetError()
//		{
//			return $"They are not an ally!";
//		}
//	}

//	public class FoundAttribute : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Location); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			return player.locations.Contains(value as Location);
//		}

//		protected override string GetError()
//		{
//			return $"You do not know a location called { LocationTypeReader.LastInput }. \nTry typing '!tc location list' for a list of known locations.";
//		}
//	}

//	public class HandAttribute : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Slot); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			Slot slot = value as Slot;

//			return slot.names.Contains("left") || slot.names.Contains("right");
//		}

//		protected override string GetError()
//		{
//			return $"You must provide a hand slot. \nTry using right/tool1 or left/tool2.";
//		}
//	}

//	public class LearntAttribute : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Recipe); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			return player.recipes.Contains(value as Recipe);
//		}

//		protected override string GetError()
//		{
//			return $"You do not know a recipe called { RecipeTypeReader.LastInput }. \nTry typing '!tc recipe list' for a list of learnt recipes.";
//		}
//	}

//	public class InInventoryAttribute : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Item); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			Item item = value as Item;

//			return player.items.Any(test => test.item == item);
//		}

//		protected override string GetError()
//		{
//			return $"You do not have an item called { ItemTypeReader.LastInput }. \nTry typing '!tc inventory' for a list of carried items.";
//		}
//	}

//	public class InEquipment : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Item); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			Item item = value as Item;

//			return player.equipped.Values.Any(test => test != null && test.item == item);
//		}

//		protected override string GetError()
//		{
//			return $"You do not have an item called { ItemTypeReader.LastInput } equipped. \nTry typing '!tc equipment' for a list of equipped items.";
//		}
//	}

//	public class InInventoryOrEquipment : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Item); } }

//		protected override bool MeetsCondition(Player player, object value)
//		{
//			Item item = value as Item;

//			return player.equipped.Values.Any(test => test != null && test.item == item) ||
//					player.items.Any(test => test.item == item);
//		}

//		protected override string GetError()
//		{
//			return $"You do not have an item called { ItemTypeReader.LastInput } equipped. \nTry typing '!tc inventory' for a list of carried and equipped items.";
//		}
//	}

//	public class ItemTypeSlot : ItemTypeAttribute
//	{
//		public override List<ItemType> ItemTypes
//		{
//			get
//			{
//				if (SlotTypeReader.LastValue == null)
//				{
//					return new List<ItemType>();
//				}

//				return SlotTypeReader.LastValue.allowedTypes;
//			}
//		}
//	}

//	public class ItemTypeAttribute : LimitedAttribute
//	{
//		protected override Type Type { get { return typeof(Item); } }

//		public virtual List<ItemType> ItemTypes { get;  set; }

//		public string ValidText { get;  set; }

//		public ItemTypeAttribute()
//		{

//		}

//		public ItemTypeAttribute(params ItemType[] types)
//		{
//			ItemTypes = new List<ItemType>(types);

//			GetValidText();
//		}

//		protected void GetValidText()
//		{
//			ValidText = "a";

//			char firstLetterToLower = ItemTypes[0].ToString().ToLower()[0];

//			if (firstLetterToLower == 'a' ||
//				firstLetterToLower == 'e' ||
//				firstLetterToLower == 'i' ||
//				firstLetterToLower == 'o' ||
//				firstLetterToLower == 'u')
//			{
//				ValidText += "n";
//			}

//			ValidText += $" {ItemTypes[0].ToString()}";

//			for (int i = 1; i < ItemTypes.Count - 1; i++)
//			{
//				ValidText += $", {ItemTypes[i].ToString()}";
//			}

//			if (ItemTypes.Count > 1)
//			{
//				ValidText += $", or {ItemTypes[ItemTypes.Count - 1].ToString()}";
//			}
//		}

//		protected override bool MeetsCondition(Player player, object value) => (value == null ? false : ItemTypes.Contains((value as Item).itemType));

//		protected override string GetError()
//		{
//			if (Value == null)
//			{
//				return "Item does not exist.";
//			}
//			else
//			{
//				string validText = ValidText;

//				if (ValidText == null)
//				{
//					GetValidText();
//					validText = ValidText;
//					ValidText = null;
//				}

//				return $"{(Value as Item).name} is not {validText}.";
//			}
//		}
//	}
//}
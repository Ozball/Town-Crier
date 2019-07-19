using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownCrier.Database;
using TownCrier.Services;

namespace TownCrier
{
	public class Migrator
	{
		public TownDatabase Database { get; }

		Dictionary<ulong, UserAltaInfo> infos = new Dictionary<ulong, UserAltaInfo>();

		void GetAltaLinks()
		{
			string target = "AltaLinks.txt";

			FileInfo fileInfo = new FileInfo($"./{target}");

			if (!fileInfo.Exists)
			{
				return;
			}

			using (StreamReader reader = new StreamReader($"./{target}"))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();

					string[] split = line.Split(' ');

					if (line.Length != 3)
					{
						ulong discord = ulong.Parse(split[0]);
						string name = split[1];
						int id = int.Parse(split[2]);

						infos[discord] = new UserAltaInfo()
						{
							Identifier = id,
							Username = name
						};
					}
				}
			}
		}

		public Migrator(TownDatabase database)
		{
			return;
			Database = database;

			GetAltaLinks();

			string target = "ChatCraft/craftConfig";	

			FileInfo fileInfo = new FileInfo($"../../{target}.json");

			if (!fileInfo.Exists)
			{
				return;
			}

			using (StreamReader reader = new StreamReader($"../../{target}.json"))
			{
				string json = reader.ReadToEnd();

				JsonSerializerSettings settings = new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects };

				ChatCraftState state = JsonConvert.DeserializeObject<ChatCraftState>(json, settings);

				Database.Users.InsertBulk(state.players.Select(item => new TownCrier.Database.TownUser()
				{
					Name = item.name,
					Description = item.description,
					Coins = item.coins,
					Scoring = new Database.UserScoring()
					{
						LastMessage = item.lastMessage,
						Score = item.score,
						UsedFirstHourPoint = item.usedFirstHourPoint,
						UsedHourPoints = item.usedHourPoints
					},
					InitialJoin = item.joined,
					UserId = item.identifier,
					AltaInfo = infos.ContainsKey(item.identifier) ? infos[item.identifier] : null
				}));
			}

			Console.WriteLine(Database.Users.Count());
		}
	}


	public class Player
	{
		public ulong identifier;

		public int coins;

		public string name;

		public DateTime joined;

		public DateTime lastMessage;
		public uint score;

		public uint usedHourPoints = 0;
		public DateTime usedFirstHourPoint;

		public string description;
	}


	public class ChatCraftState
	{
		public List<Player> players = new List<Player>();
	}
}

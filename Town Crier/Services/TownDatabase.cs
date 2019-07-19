using Discord;
using Discord.WebSocket;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownCrier.Database;

namespace TownCrier.Services
{
	public class TownDatabase
	{
		LiteDatabase database;

		public LiteCollection<TownGuild> Guilds { get; }
		public LiteCollection<TownUser> Users { get; }

		public TownDatabase(LiteDatabase database)
		{
			this.database = database;

			Guilds = database.GetCollection<TownGuild>("Guilds");
			Users = database.GetCollection<TownUser>("Users");
		}

		public TownGuild GetGuild(IGuild guild)
		{
			return Guilds.FindOne(x => x.GuildId == guild.Id);
		}

		public TownUser GetUser(IUser user)
		{
			TownUser result = Users.FindOne(x => x.UserId == user.Id);

			bool isChanged = false;

			if (result == null)
			{
				result = new TownUser() { UserId = user.Id, Name = user.Username };

				Users.Insert(result);
			}
			else if (result.Name != user.Username)
			{
				result.Name = user.Username;

				isChanged = true;
			}

			if (result.InitialJoin == default(DateTime) && user is IGuildUser guildUser && guildUser.JoinedAt.HasValue)
			{
				result.InitialJoin = guildUser.JoinedAt.Value.UtcDateTime;

				isChanged = true;
			}

			if (isChanged)
			{
				Users.Update(result);
			}

			return result;
		}
	}
}

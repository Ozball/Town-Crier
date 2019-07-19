using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownCrier.Database;
using TownCrier.Services;

namespace TownCrier.Services
{
	public class PointCounter
	{
		DiscordSocketClient discord;
		TownDatabase database;

		public PointCounter(DiscordSocketClient discord, TownDatabase database)
		{
			this.database = database;
			this.discord = discord;

			discord.MessageReceived += Handle;
		}

		Task Handle(SocketMessage message)
		{
			TownUser user = database.GetUser(message.Author);

			if (user.Scoring == null)
			{
				user.Scoring = new UserScoring();
			}

			UserScoring scoring = user.Scoring;

			DateTime now = DateTime.UtcNow;

			if ((now - scoring.LastMessage).TotalMinutes > 1)
			{
				if (scoring.UsedHourPoints > 0)
				{
					if ((now - scoring.UsedFirstHourPoint).TotalHours > 1)
					{
						scoring.UsedHourPoints = 0;
					}
				}

				if (scoring.UsedHourPoints < 20)
				{
					if (scoring.UsedHourPoints == 0)
					{
						scoring.UsedFirstHourPoint = now;
					}

					scoring.UsedHourPoints++;

					scoring.Score += 10;

					scoring.LastMessage = now;
				}
			}

			return Task.CompletedTask;
		}
	}
}

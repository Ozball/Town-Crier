using Discord;
using Discord.WebSocket;
using DiscordBot.Modules;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TownCrier.Database;

namespace TownCrier.Services
{
	public class NewcomerService
	{
		readonly DiscordSocketClient discord;
		IServiceProvider provider;
		readonly IConfiguration config;
		readonly TownDatabase database;

		public NewcomerService(DiscordSocketClient discord, TownDatabase database, IServiceProvider provider, IConfiguration config)
		{
			this.discord = discord;
			this.provider = provider;
			this.database = database;
			this.config = config;

			discord.UserJoined += UserJoined;
			discord.UserLeft += AlertTeam;
		}

		public async Task UserJoined(SocketGuildUser user)
		{
			// Locate TownGuild in the database
			var guild = database.GetGuild(user.Guild);

			if (guild != null)
			{
				// TextChannel where Notifications go
				var NotificationChannel = user.Guild.GetTextChannel(guild.NotificationChannel);

				//Not the most elegant, but can't afford to spend more time on it right now
				await WelcomeModule.SendTo(user);

				// Send welcome message, parsing the welcome message string from the TownGuild property
				if (guild.WelcomeMessage != "")
				{
					var welcome = await NotificationChannel.SendMessageAsync(guild.ParseMessage(user, discord));

					await welcome.AddReactionAsync(new Emoji("👋"));
				}

				if (guild.MilestoneMessage != "" && (user.Guild.Users.Count % guild.MilestoneMarker) == 0)
				{
					await NotificationChannel.SendMessageAsync($"We've now hit {user.Guild.Users.Count} members! Wooooo!");

					await Task.Delay(1000 * 20);

					await NotificationChannel.SendMessageAsync($"Partaayyy!");
				}
			}
		}

		public async Task AlertTeam(SocketGuildUser user)
		{
			// Locate TownGuild in the database
			var guild = database.GetGuild(user.Guild);
			
			// TextChannel where Notifications go
			var alertChannel = user.Guild.GetTextChannel(guild.LeaverChannel);

			// Fetch the user's TownResident entry
			var townUser = database.GetUser(user);

			await alertChannel.SendMessageAsync("The user: " + user.Username + " left. They joined: " + townUser.InitialJoin.ToString("dd/MMM/yyyy hh:mm: tt"));
		}
	}
}

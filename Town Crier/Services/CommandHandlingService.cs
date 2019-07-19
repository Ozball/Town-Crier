using Discord;
using Discord.Addons.CommandCache;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TownCrier.Database;

namespace TownCrier.Services
{
	public class CommandHandlingService
	{
		readonly DiscordSocketClient discord;
		readonly CommandService commands;
		readonly InteractiveService interactive;
		readonly IConfiguration config;
		readonly TownDatabase database;
		readonly CommandCacheService cache;

		IServiceProvider provider;
		
		public CommandHandlingService(IConfiguration config, IServiceProvider provider, TownDatabase database, DiscordSocketClient discord, CommandService commands, CommandCacheService cache, InteractiveService interactive)
		{
			this.discord = discord;
			this.commands = commands;
			this.provider = provider;
			this.config = config;
			this.interactive = interactive;
			this.cache = cache;
			this.database = database;

			this.discord.MessageReceived += MessageReceived;
			this.discord.ReactionAdded += OnReactAdded;
			this.discord.MessageUpdated += OnMessageUpdated;
		}

		public async Task InitializeAsync(IServiceProvider provider)
		{
			this.provider = provider;

			await commands.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);
			// Add additional initialization code here...
		}

		public async Task OnMessageUpdated(Cacheable<IMessage, ulong> _OldMsg, SocketMessage NewMsg, ISocketMessageChannel Channel)
		{
			var OldMsg = await _OldMsg.DownloadAsync();
			if (OldMsg.Source != MessageSource.User) return;


			if (cache.TryGetValue(NewMsg.Id, out var CacheMsg))
			{
				var reply = await Channel.GetMessageAsync(CacheMsg.First());
				await reply.DeleteAsync();
			}
			await MessageReceived(NewMsg);
		}


		public async Task OnReactAdded(Cacheable<IUserMessage, ulong> _msg, ISocketMessageChannel channel, SocketReaction reaction)
		{
			//Jira Implementation here
		}

		async Task MessageReceived(SocketMessage rawMessage)
		{
			// Ignore system messages and messages from bots
			if (!(rawMessage is SocketUserMessage message)) return;
			if (message.Source != MessageSource.User) return;

			var context = new SocketCommandContext(discord, message);
			var guild = (context.Guild == null) ? null : database.GetGuild(context.Guild);

			int argPos = 0;
			if (guild == null && !message.HasMentionPrefix(discord.CurrentUser, ref argPos)) return;
			if (guild != null && !message.HasStringPrefix(guild.Prefix, ref argPos) && !message.HasMentionPrefix(discord.CurrentUser, ref argPos)) return;
			
			var result = await commands.ExecuteAsync(context, argPos, provider);

			if (result.Error.HasValue && (result.Error.Value != CommandError.UnknownCommand))
			{
				Console.WriteLine(result.Error + "\n" + result.ErrorReason);
			}
		}
	}
}

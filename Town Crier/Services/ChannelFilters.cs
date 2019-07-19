using Discord;
using Discord.Commands;
using Discord.Addons;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownCrier.Database;
using TownCrier.Services;
using Discord.Addons.Interactive;

namespace TownCrier.Services
{
	public class ChannelFilters
	{
		DiscordSocketClient discord;
		TownDatabase database;
		InteractiveService interactive;

		public ChannelFilters(DiscordSocketClient discord, TownDatabase database, InteractiveService interactive)
		{
			this.database = database;
			this.discord = discord;
			this.interactive = interactive;

			discord.MessageReceived += Handle;
		}

		async Task Handle(SocketMessage socketMessage)
		{
			if (!(socketMessage is SocketUserMessage message && 
				message.Channel is ITextChannel channel) ||
				message.Author.Id == discord.CurrentUser.Id)
			{
				return;
			}

			TownGuild guild = database.GetGuild(channel.Guild);

			if (guild == null)
			{
				return;
			}

			ChannelFilter filter = guild.ChannelFilters.FirstOrDefault(x => x.Channel == channel.Id);

			if (filter == null)
			{
				return;
			}

			try
			{
				switch (filter.Type)
				{
					case ChannelFilter.FilterType.Heading:
						await FilterHeadings(filter, message);
						break;
					case ChannelFilter.FilterType.Image:
						await FilterScreenshots(filter, message);
						break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.StackTrace);
			}
		}
		
		async Task<bool> FilterScreenshots(ChannelFilter filter, SocketUserMessage message)
		{
			if (message.Attachments.Count == 0 && !message.Content.Contains(".jpg") && !message.Content.Contains(".png") && !message.Content.Contains(".gif") && !message.Content.Contains(".jpeg"))
			{
				if (filter.AlertChannel != 0)
				{
					ITextChannel discussion = await (message.Channel as IGuildChannel).Guild.GetChannelAsync(filter.AlertChannel) as ITextChannel;

					await discussion.SendMessageAsync(message.Author.Mention + " said in " + (message.Channel as ITextChannel).Mention + " : " + message.Content);

					await ReplyAndDelete(message, $"Hi {message.Author.Mention}! I've copied your message to {discussion.Mention}. If you move the discussion there, we can keep this channel filled with screenshots!", 10);
				}
				else
				{
					await ReplyAndDelete(message, $"Hi {message.Author.Mention}! Only images are allowed here!", 10);
				}

				await message.DeleteAsync();

				return false;
			}

			return true;
		}

		async Task<bool> FilterHeadings(ChannelFilter filter, SocketUserMessage message)
		{
			if (!message.Content.StartsWith("```"))
			{
				if (filter.AlertChannel != 0)
				{
					ITextChannel channel = (message.Channel as ITextChannel);
					ITextChannel discussion = await channel.Guild.GetChannelAsync(filter.AlertChannel) as ITextChannel;

					await discussion.SendMessageAsync($@"Hi {message.Author.Mention} - Messages in {channel.Mention} require a heading!

To help, I've created a template for you to copy & paste. Be sure to change the placeholder text!
If you were simply trying to discuss someone elses message, this is the place to do so.
Alternatively react with a :thumbsup: to show your support.

` ```Your Heading```
{message.Content}`");

					await ReplyAndDelete(message, $"Hi {message.Author.Mention}! Your message needs a heading! Let me help you out in {discussion.Mention}.", 10);
				}
				else
				{
					await ReplyAndDelete(message, $"Hi {message.Author.Mention}! Your message needs a heading! (Starting with \\```<Your heading>>>```)", 10);
				}

				await message.DeleteAsync();

				return false;
			}

			return true;
		}

		async Task<IUserMessage> ReplyAndDelete(SocketUserMessage original, string response, double seconds)
		{
			SocketCommandContext newContext = new SocketCommandContext(discord, original);

			return await interactive.ReplyAndDeleteAsync(newContext, response, timeout: TimeSpan.FromSeconds(seconds));
		}
	}
}

using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TownCrier.Services
{
	public class OutOfOffice
	{
		DiscordSocketClient discord;

		public OutOfOffice(DiscordSocketClient discord)
		{
			this.discord = discord;

			discord.MessageReceived += Process;
		}

		async Task Process(SocketMessage message)
		{
			//TODO: Move some logic here into a standardized time based response system?

			if (!(message.Channel is ITextChannel channel))
			{
				return;
			}

			DateTime now = DateTime.Now;

			if ((now.Hour >= 23 || now.Hour < 8) &&
				message.MentionedRoles.Any(item => item.Name == "devs" || item.Name == "admins"))
			{
				IReadOnlyCollection<IGuildChannel> channels = await channel.Guild.GetChannelsAsync(CacheMode.CacheOnly);
				ITextChannel bugs = channels.FirstOrDefault(item => item.Name == "bugs") as ITextChannel;
				ITextChannel feedback = channels.FirstOrDefault(item => item.Name == "feedback") as ITextChannel;
				ITextChannel tipsandhelp = channels.FirstOrDefault(item => item.Name == "tips-and-help") as ITextChannel;
				ITextChannel gettingstarted = channels.FirstOrDefault(item => item.Name == "getting-started") as ITextChannel;

				await message.Channel.SendMessageAsync($"Hi {message.Author.Mention}, unfortunately " +
					$"it's {DateTime.Now.ToShortTimeString()} in Sydney right now.\n" +
					$"If you're new, check out {gettingstarted.Mention}.\n" +
					$"If you've got a bug, hit up {bugs.Mention}.\n" +
					$"If you've got feedback, drop it at {feedback.Mention}.\n" +
					$"Otherwise visit {tipsandhelp.Mention} and hopefully someone else can help!");
			}
		}
	}
}

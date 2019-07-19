using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownCrier.Services
{
	public class DoYouCare
	{
		DiscordSocketClient discord;

		public DoYouCare(DiscordSocketClient discord)
		{
			this.discord = discord;

			discord.MessageReceived += Handle;
		}

		async Task Handle(SocketMessage message)
		{
			if (message.Content.ToLower().Contains("do you care") && message.Content.Contains("?"))
			{
				if (new Random().NextDouble() < 0.05f)
				{
					await message.Channel.SendMessageAsync(message.Author.Mention + " - I am care free.");
				}
				else
				{
					await message.Channel.SendMessageAsync(message.Author.Mention + " - No.");
				}
			}
		}
	}
}

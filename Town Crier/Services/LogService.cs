using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace TownCrier.Services
{
	public class LogService
	{
		readonly DiscordSocketClient discord;
		readonly CommandService commands;
		readonly ILoggerFactory loggerFactory;
		readonly ILogger discordLogger;
		readonly ILogger commandsLogger;

		public LogService(DiscordSocketClient discord, CommandService commands, ILoggerFactory loggerFactory)
		{
			this.discord = discord;
			this.commands = commands;

			this.loggerFactory = loggerFactory;

			discordLogger = this.loggerFactory.CreateLogger("discord");
			commandsLogger = this.loggerFactory.CreateLogger("commands");

			discord.Log += LogDiscord;
			commands.Log += LogCommand;
		}

		Task LogDiscord(LogMessage message)
		{
			discordLogger.Log(
				LogLevelFromSeverity(message.Severity),
				0,
				message,
				message.Exception,
				(_1, _2) => message.ToString(prependTimestamp: false));

			return Task.CompletedTask;
		}

		Task LogCommand(LogMessage message)
		{
			// Return an error message for async commands
			if (message.Exception is CommandException command)
			{
				// Don't risk blocking the logging task by awaiting a message send; ratelimits!?
				var _ = command.Context.Channel.SendMessageAsync($"Error: {command.Message}");
			}

			commandsLogger.Log(
				LogLevelFromSeverity(message.Severity),
				0,
				message,
				message.Exception,
				(_1, _2) => message.ToString(prependTimestamp: false));
			return Task.CompletedTask;
		}

		static LogLevel LogLevelFromSeverity(LogSeverity severity)
		   => (LogLevel)(Math.Abs((int)severity - 5));
	}
}

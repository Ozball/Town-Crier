using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LiteDB;
using Discord;
using Discord.WebSocket;

namespace TownCrier.Database
{
	public class TownGuild
	{
		/// <summary>
		/// Guild's Discord ID
		/// </summary>
		[BsonId]
		public ulong GuildId { get; set; }

		/// <summary>
		/// Guild's Prefix
		/// </summary>
		public string Prefix { get; set; } = "!";

		/// <summary>
		/// These roles are auto-assigned to users based on their activity.
		/// </summary>
		public List<ActivityRole> ActivityRoles { get; set; } = new List<ActivityRole>();

		/// <summary>
		/// These channels are pinged when the role is mentioned
		/// </summary>
		public List<CrossAlert> CrossAlerts { get; set; } = new List<CrossAlert>();

		/// <summary>
		/// These filters only allow particular content in channels
		/// </summary>
		public List<ChannelFilter> ChannelFilters { get; set; } = new List<ChannelFilter>();

		/// <summary>
		/// This role is assinged to Alta supporters.
		/// </summary>
		public SocketRole SupporterRole { get; set; } = null;

		/// <summary>
		/// This role is labeled as the Admin role, assignable by the server owner.
		/// Allows bypassing of several permission checks
		/// </summary>
		public ulong AdminRole { get; set; } = 0;

		/// <summary>
		/// Channel assigned for public-level notifications.
		/// </summary>
		public ulong NotificationChannel { get; set; } = 0;

		/// <summary>
		/// Channel assigned for leaving notifications
		/// </summary>
		public ulong LeaverChannel { get; set; } = 0;

		/// <summary>
		/// A dictionary with some server settings:
		/// <list type="bullet">
		/// <item>
		/// <description> ModuleName: Whether or not this module is enabled/disabled.</description>
		/// </item>
		/// </list>
		/// </summary>
		public Dictionary<string, bool> Settings { get; set; } = new Dictionary<string, bool>();
		
		/// <summary>
		/// Message to be displayed when a user joins.
		/// <list type="bullet">
		/// <item>
		/// <description><para>"{user}" is replaced with a mention of the user who joined.</para></description>
		/// </item>
		/// <item>
		/// <description><para>"{server}" is replaced with the server's name.</para></description>
		/// </item>
		/// <item>
		/// <description><para>"{server:count}" is replaced with the amount of users in the server.</para></description>
		/// </item>
		/// <item>
		/// <description><para>"{staff}" is replaced with a mention of the staff role (if it exists).</para></description>
		/// </item>
		/// </list>
		/// </summary>
		public string WelcomeMessage { get; set; } = "";

		/// <summary>
		/// Message to be displayed once the server usercount reaches a certain number defined by the Milestone variable
		/// </summary>
		/// <see cref="WelcomeMessage"/>
		public string MilestoneMessage { get; set; } = "";

		/// <summary>
		/// Indicates how often should the server anounce a new milestone in server user count.
		/// </summary>
		public int MilestoneMarker { get; set; } = 1000;

		public string WikiName { get; set; } //A Township Tale Wiki
		public string WikiUrl { get; set; } //https://townshiptale.gamepedia.com
		public string WikiIcon { get; set; } //https://d1u5p3l4wpay3k.cloudfront.net/atownshiptale_gamepedia_en/9/9e/WikiOnly.png

		public string ParseMessage(SocketGuildUser user, DiscordSocketClient client)
		{
			string returnstring = WelcomeMessage.Replace("{user}", user.Mention).Replace("{server}", client.GetGuild(GuildId).Name).Replace("{server:count}", client.GetGuild(GuildId).Users.Count.ToString());
			returnstring = AdminRole != 0 ? returnstring.Replace("{admin}", user.Guild.GetRole(AdminRole)?.Mention) : returnstring;
			return returnstring;
		}
	}

	public class ActivityRole
	{
		public ActivityFlag ActivityType { get; set; }
		public string ActivityName { get; set; }
		public ulong AssociatedRole { get; set; }
	}

	public class CrossAlert
	{
		public ulong Channel { get; set; }
		public ulong Role { get; set; }
	}

	public class ChannelFilter
	{
		public enum FilterType
		{
			Heading,
			Image
		}

		public ulong Channel { get; set; }
		public ulong AlertChannel { get; set; }
		public FilterType Type { get; set; }
	}
}

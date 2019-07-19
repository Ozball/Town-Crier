
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownCrier.Modules
{
	public class PlaytimeModule : InteractiveBase<SocketCommandContext>
	{
		AltaAPI AltaAPI { get; set; }

		public TownDatabase Database { get; set; }

		public PlaytimeModule(AltaAPI altaAPI, TownDatabase database)
		{
			AltaAPI = altaAPI;
			Database = database;
		}

		[Command("playtime")]
		public async Task PlayTime()
		{
			await PlayTime2(Context.User);
		}

		[Command("playtime"), RequireUserPermission(GuildPermission.BanMembers)]
		public async Task PlayTime2(IUser userArg)
		{
			TownUser user = Database.GetUser(userArg);

			if (user == null || user.AltaInfo == null || user.AltaInfo.Identifier == 0)
			{
				await ReplyAsync(Context.User.Mention + ", " + "You have not linked to an Alta account! To link, visit the 'Account Settings' page in the launcher.");
			}
			else
			{
				var stats = await AltaAPI.ApiClient.UserClient.GetUserStatisticsAsync(user.AltaInfo.Identifier);

				await ReplyAsync("Play time: " + stats.PlayTime.TotalHours.ToString("0.00") + " hours");
			}
		}
	}

	public class InfoModule : InteractiveBase<SocketCommandContext>
	{
		public TownDatabase Database { get; set;  }

		[Command("blog")]
		public Task Info()
			=> ReplyAsync(
				$"Were you looking for this?\nhttps://www.townshiptale.com/blog/\n");

		[Command("wiki")]
		public Task Wiki()
			=> ReplyAsync(
				$"Were you looking for this?\nhttps://www.townshiptale.com/wiki/\n");

		[Command("invite")]
		public Task Invite()
			=> ReplyAsync(
				$"Were you looking for this?\nhttps://discord.gg/townshiptale\n");

		[Command("reddit")]
		public Task Reddit()
			=> ReplyAsync(
				$"Were you looking for this?\nhttps://reddit.com/r/townshiptale\n");

		[Command("resetpassword")]
		public Task ResetPassword()
			=> ReplyAsync(
				$"Were you looking for this?\nhttps://townshiptale.com/reset-password\n");

		[Command("launcher")]
		public Task Launcher()
			=> ReplyAsync(
				$"Were you looking for this?\nhttps://townshiptale.com/launcher\n");


		[Command("supporter"), Alias("support", "donate")]
		public Task Supporter()
			=> ReplyAsync(
				"To become a supporter, visit the following URL, or click the 'Become a Supporter' button in the Alta Launcher.\nhttps://townshiptale.com/supporter");

		class TrelloCard
		{
			public string name;
			public string url;
		}

		[Command("faq")]
		public async Task Faq([Remainder]string query = null)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				await ReplyAsync($"Were you looking for this?\n<https://trello.com/b/Dnaxu0Mk/a-township-tale-faq-help>\n");
			}
			else
			{
				query = query.ToLower();

				var client = new RestClient("https://api.trello.com/1/boards/Dnaxu0Mk/cards/visible?key=3e7b77be622f7578d998feb1e663561b&token=83df6272cd4b14650b15fc4d6a9960c6090da2ea1287e5cbce09b99d9549fc61");
				var request = new RestRequest(Method.GET);

				IRestResponse response = client.Execute(request);

				TrelloCard[] cards = JsonConvert.DeserializeObject<TrelloCard[]>(response.Content);

				foreach (TrelloCard card in cards)
				{
					if (card.name.ToLower().Contains(query))
					{
						await ReplyAsync($"Were you looking for this?\n{card.url}\n");
						return;
					}
				}

				await ReplyAsync($"Were you looking for this?\n<https://trello.com/b/Dnaxu0Mk/a-township-tale-faq-help>\n");
			}
		}

		[Command("roadmap")]
		public async Task Roadmap([Remainder]string query = null)
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				await ReplyAsync($"Were you looking for this?\n<https://trello.com/b/0rQGM8l4/a-township-tales-roadmap>\n");
			}
			else
			{
				query = query.ToLower();

				var client = new RestClient("https://api.trello.com/1/boards/0rQGM8l4/cards/visible?key=3e7b77be622f7578d998feb1e663561b&token=83df6272cd4b14650b15fc4d6a9960c6090da2ea1287e5cbce09b99d9549fc61");
				var request = new RestRequest(Method.GET);

				IRestResponse response = client.Execute(request);

				TrelloCard[] cards = JsonConvert.DeserializeObject<TrelloCard[]>(response.Content);

				foreach (TrelloCard card in cards)
				{
					if (card.name.ToLower().Contains(query))
					{
						await ReplyAsync($"Were you looking for this?\n{card.url}\n");
						return;
					}
				}

				await ReplyAsync($"Were you looking for this?\n<https://trello.com/b/0rQGM8l4/a-township-tales-roadmap>\n");
			}
		}

		[Command("joined")]
		public async Task Joined()
		{
			TownUser user = Database.GetUser(Context.User);
			
			await ReplyAsync($"{Context.User.Mention} joined on {user.InitialJoin.ToString("dd/MMM/yyyy")}");
		}

		[Command("joined"), RequireUserPermission(GuildPermission.KickMembers)]
		public async Task Joined(IUser discordUser)
		{
			TownUser user = Database.GetUser(discordUser);

			await ReplyAsync($"{discordUser.Username} joined on {user.InitialJoin.ToString("dd/MMM/yyyy")}");
		}


		[Command("title"), Alias("heading", "header")]
		public async Task Title([Remainder]string text)
		{
			IUserMessage response = await ReplyAsync("\\```css\n" + text + "\n\\```");
			await Context.Message.DeleteAsync();

			Task _ = Task.Run(async () =>
			{
				await Task.Delay(20000);
				await response.DeleteAsync();
			});
		}

		[Command("userlist")]
		public async Task UserList()
		{
			if (Context.Guild == null)
			{
				return;
			}

			if (!(Context.User as IGuildUser).RoleIds.Contains<ulong>(334935631149137920))
			{
				return;
			}

			await ReplyAsync("Starting...");

			StringBuilder result = new StringBuilder();

			result
				.Append("ID")
				.Append(',')
				.Append("Username")
				.Append(',')
				.Append("Nickname")
				.Append(',')
				.Append("Joined")
				.Append(',')
				.Append("Last Message")
				.Append(',')
				.Append("Score")
				.Append('\n');

			foreach (IGuildUser user in (Context.Guild as SocketGuild).Users)
			{
				TownUser townUser = Database.GetUser(user);

				result
					.Append(user.Id)
					.Append(',')
					.Append(user.Username.Replace(',', '_'))
					.Append(',')
					.Append(user.Nickname?.Replace(',', '_'))
					.Append(',')
					.Append(townUser.InitialJoin.ToString("dd-MM-yy"))
					.Append(',')
					.Append(townUser.Scoring?.LastMessage.ToString("dd-MM-yy"))
					.Append('\n');
			}

			System.IO.File.WriteAllText("D:/Output/Join Dates.txt", result.ToString());

			await ReplyAsync("I'm done now :)");
		}

		[Command("alerton")]
		public async Task AlertOn()
		{
			if (Context.Guild == null)
			{
				return;
			}

			if (!(Context.User as IGuildUser).RoleIds.Contains<ulong>(334935631149137920))
			{
				return;
			}

			IRole role = Context.Guild.Roles.FirstOrDefault(test => test.Name == "followers");

			await role.ModifyAsync(properties => properties.Mentionable = true);
		}

		[Command("alertoff")]
		public async Task AlertOff()
		{
			if (Context.Guild == null)
			{
				return;
			}

			if (!(Context.User as IGuildUser).RoleIds.Contains<ulong>(334935631149137920))
			{
				return;
			}

			IRole role = Context.Guild.Roles.FirstOrDefault(test => test.Name == "followers");

			await role.ModifyAsync(properties => properties.Mentionable = false);
		}

		[Command("follow"), Alias("optin", "keepmeposted")]
		public async Task OptIn()
		{
			if (Context.Guild == null)
			{
				await ReplyAsync("You must call this from within a server channel.");
				return;
			}

			IGuildUser user = Context.User as IGuildUser;
			IRole role = Context.Guild.Roles.FirstOrDefault(test => test.Name == "followers");

			if (role == null)
			{
				await ReplyAsync("Role not found");
				return;
			}

			if (user.RoleIds.Contains(role.Id))
			{
				await ReplyAsync("You are already a follower!\nUse !unfollow to stop following.");
				return;
			}

			await user.AddRoleAsync(role);
			await ReplyAsync("You are now a follower!");
		}

		[Command("unfollow"), Alias("optout", "leavemealone")]
		public async Task OptOut()
		{
			if (Context.Guild == null)
			{
				await ReplyAsync("You must call this from within a server channel.");
				return;
			}

			IGuildUser user = Context.User as IGuildUser;
			IRole role = Context.Guild.Roles.FirstOrDefault(test => test.Name == "followers");

			if (role == null)
			{
				await ReplyAsync("Role not found");
				return;
			}

			if (!user.RoleIds.Contains(role.Id))
			{
				await ReplyAsync("You aren't a follower!\nUse !follow to start following.");
				return;
			}

			await user.RemoveRoleAsync(role);
			await ReplyAsync("You are no longer a follower.");
		}

		//[Command("help"), Alias("getstarted", "gettingstarted")]
		//public async Task GetStarted()
		//{
		//	List<string> commands = new List<string>();
		//	List<string> descriptions = new List<string>();

		//	string message = $"Welcome! I am the Town Crier.\n" +
		//		$"I can help with various tasks.\n\n" +
		//		$"Here are some useful commands:\n\n";

		//	commands.Add("help");
		//	descriptions.Add("In case you get stuck");

		//	commands.Add("follow");
		//	descriptions.Add("Get alerted with news.");

		//	commands.Add("blog");
		//	descriptions.Add("For a good read");

		//	commands.Add("whois [developer]");
		//	descriptions.Add("A brief bio on who a certain developer is");

		//	commands.Add("flip");
		//	descriptions.Add("Flip a coin!");

		//	commands.Add("roll");
		//	descriptions.Add("Roll a dice!");


		//	//commands.Add("tc help");
		//	//descriptions.Add("An introduction to A Chatty Township Tale");

		//	message += ShowCommands("!", commands, descriptions);

		//	await ReplyAsync(message);
		//	//RestUserMessage messageResult = (RestUserMessage)
		//	//await messageResult.AddReactionAsync(Emote.Parse("<:hand_splayed:360022582428303362>"));
		//}

	}
}
using TownCrier.Database;
using TownCrier.Services;

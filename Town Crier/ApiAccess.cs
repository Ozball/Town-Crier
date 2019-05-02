using Alta.WebApi.Client;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

public static class ApiAccess
{
	const int Timeout = 40;

	public static IHighLevelApiClient ApiClient { get; private set; }

	static SHA512 sha512 = new SHA512Managed();

	static ApiAccess()
	{
		StartWithEndpoint(HighLevelApiClientFactory.ProductionEndpoint);
	}

	public static void StartWithEndpoint(string endpoint)
	{
		if (ApiClient != null)
		{
			Console.WriteLine("Already have an Api Client");
			return;
		}

		SetApiClientLogging();

		ApiClient = HighLevelApiClientFactory.CreateHighLevelClient(endpoint, Timeout);
	}

	static void SetApiClientLogging()
	{
		//HighLevelApiClientFactory.SetLogging(new AltaLoggerFactory());
	}

	public static void StartOffline(LoginCredentials credentials)
	{
		if (ApiClient != null)
		{
			Console.WriteLine("Already have an Api Client");
			return;
		}

		SetApiClientLogging();

		ApiClient = HighLevelApiClientFactory.CreateOfflineHighLevelClient(credentials);
	}

	public async static Task EnsureLoggedIn()
	{
		if (!ApiClient.IsLoggedIn)
		{
			if (!File.Exists("account.txt"))
			{
				Console.WriteLine("`account.txt` expected next to be next to the .exe " +
					"with the contents `username|password` (for your Alta account)");
				Console.ReadLine();
				throw new Exception("No credentials provided");
			}

			string username;
			string password;

			try
			{
				string[] account = System.IO.File.ReadAllText("account.txt").Trim().Split('|');

				username = account[0].Trim();
				password = account[1].Trim();

				if (password.Length < 64)
				{
					password = HashString(password);

					Console.WriteLine("Detected a password in the account file." +
						" Replaced it with a hash for security reasons.");

					File.WriteAllText("account.txt", username + "|" + password);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("`account.txt` found, but failed reading the contents." +
					" Expected format: `username|password` (for your Alta account)");
				Console.ReadLine();
				throw new Exception("Invalid credential format");
			}

			try
			{
				await ApiClient.LoginAsync(username, password);
				Console.WriteLine($"Logged in as {username} \n");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}

	static string HashString(string text)
	{
		//return Convert.ToBase64String(sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
		return BitConverter.ToString(sha512.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text))).Replace("-", String.Empty).ToLowerInvariant();
	}
}
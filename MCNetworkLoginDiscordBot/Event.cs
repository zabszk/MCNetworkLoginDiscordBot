using System;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;

namespace MCNetworkLoginDiscordBot
{
	internal class Event
	{
		internal static Task ClientReady(ReadyEventArgs e)
		{
			Console.WriteLine("Discord bot ready.");
			return Task.CompletedTask;
		}

		internal static Task ClientError(ClientErrorEventArgs e)
		{
			Console.WriteLine($"Discord bot ERROR: {e.Exception.GetType()}, {e.Exception.Message}");
			return Task.CompletedTask;
		}

		internal static Task MessageReceived(MessageCreateEventArgs e)
		{
			if (!e.Message.ToString().StartsWith(Program.Config.CommandPrefix)) return Task.CompletedTask;
			var message = e.Message.ToString().Substring(Program.Config.CommandPrefix.Length).Split(' ');
			switch (message[0].ToLower())
			{
				case "register":
					if (e.Channel.IsPrivate || e.Channel.Id != Program.Config.RegisterChannel)
					{
						e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Command \"register\" can only be executed on channel <#{Program.Config.RegisterChannel}>.");

						return Task.CompletedTask;
					}
					else
					{
						if (message.Length != 2)
						{
							e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
								$"Correct syntax is register YourMinecraftUsername");

							return Task.CompletedTask;
						}

						if (message[1].Length < 4 || message[1].Length > 16)
						{
							e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
								$"Username must be between 4 and 16 characters.");

							return Task.CompletedTask;
						}

						var result = HttpQuery.Post(Program.Config.ApiUrl, $"action=register&token={Program.Config.ApiUrl}&DiscordID={e.Author.Id}&username={Base64.Base64Encode(message[1])}");
						e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Request processed, result: " + result + $". To change your password use **IN DIRECT MESSAGE** (!) : {Program.Config.CommandPrefix}passwd YourNewPassword");
					}
					break;

				case "unregister":
					if (e.Channel.IsPrivate || e.Channel.Id != Program.Config.RegisterChannel)
					{
						e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Command \"unregister\" can only be executed on channel <#{Program.Config.RegisterChannel}>.");

						return Task.CompletedTask;
					}
					else
					{
						var result = HttpQuery.Post(Program.Config.ApiUrl, $"action=unregister&token={Program.Config.ApiUrl}&DiscordID={e.Author.Id}");
						e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Request processed, result: " + result);
					}
					break;

				case "passwd":
					if (!e.Channel.IsPrivate)
					{
						e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Command \"passwd\" can only be executed in direct message.");

						return Task.CompletedTask;
					}
					else
					{
						if (message.Length != 2)
						{
							e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
								$"Correct syntax is passwd YourNewPassword");

							return Task.CompletedTask;
						}

						if (message[1].Length < 8)
						{
							e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
								$"Password must have at least 8 characters.");

							return Task.CompletedTask;
						}

						var result = HttpQuery.Post(Program.Config.ApiUrl, $"action=passwd&token={Program.Config.ApiUrl}&DiscordID={e.Author.Id}&password={Base64.Base64Encode(message[1])}");
						e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Request processed, result: " + result);
					}
					break;
			}

			return Task.CompletedTask;
		}
	}
}

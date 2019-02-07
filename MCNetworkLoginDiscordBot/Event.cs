using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DSharpPlus.Entities;
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
			ProcessMessage(e).GetAwaiter().GetResult();
			return Task.CompletedTask;
		}

		private static async Task ProcessMessage(MessageCreateEventArgs e)
		{
			try
			{
				if (!e.Message.Content.StartsWith(Program.Config.CommandPrefix)) return;
				var message = e.Message.Content.Substring(Program.Config.CommandPrefix.Length).Split(' ');

				DiscordMember sender = null;

				if (!e.Channel.IsPrivate)
				{
					var senderTask = e.Guild.GetMemberAsync(e.Author.Id);
					await senderTask;
					sender = senderTask.Result;
				}

				switch (message[0].ToLower())
				{
					case "register":
						if (sender == null || e.Channel.IsPrivate || e.Channel.Id != Program.Config.RegisterChannel)
						{
							if (sender != null)
								await sender.SendMessageAsync(
								$"Command \"register\" can only be executed on channel <#{Program.Config.RegisterChannel}>.");

							else
							await e.Channel.SendMessageAsync($"Command \"register\" can only be executed on channel <#{Program.Config.RegisterChannel}>.");

							return;
						}

						if (message.Length != 2)
						{
							await e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
								$"Correct syntax is register YourMinecraftUsername");

							return;
						}

						if (message[1].Length < 4 || message[1].Length > 16)
						{
							await e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
								$"Username must be between 4 and 16 characters.");

							return;
						}

						var result = HttpQuery.Post(Program.Config.ApiUrl,
							$"action=register&token={Program.Config.ApiToken}&DiscordID={e.Author.Id}&username={Base64.Base64Encode(message[1])}");
						await e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Request processed, result: " + result +
							$". To change your password use **IN DIRECT MESSAGE** (!) : {Program.Config.CommandPrefix}passwd YourNewPassword");
						break;

					case "unregister":
						if (sender == null || e.Channel.IsPrivate || e.Channel.Id != Program.Config.RegisterChannel)
						{
							if (sender != null)
								await sender.SendMessageAsync(
									$"Command \"unregister\" can only be executed on channel <#{Program.Config.RegisterChannel}>.");

							else
								await e.Channel.SendMessageAsync($"Command \"unregister\" can only be executed on channel <#{Program.Config.RegisterChannel}>.");

							return;
						}

						var result2 = HttpQuery.Post(Program.Config.ApiUrl,
							$"action=unregister&token={Program.Config.ApiToken}&DiscordID={e.Author.Id}");
						await e.Guild.GetMemberAsync(e.Author.Id).Result.SendMessageAsync(
							$"Request processed, result: " + result2);
						break;

					case "passwd":
						if (!e.Channel.IsPrivate)
						{
							await sender.SendMessageAsync(
								$"Command \"passwd\" can only be executed in direct message.");

							return;
						}

						if (message.Length != 2)
						{
							await e.Channel.SendMessageAsync(
								$"Correct syntax is passwd YourNewPassword");

							return;
						}

						if (message[1].Length < 8)
						{
							await e.Channel.SendMessageAsync(
								$"Password must have at least 8 characters.");

							return;
						}

						var result3 = HttpQuery.Post(Program.Config.ApiUrl,
							$"action=passwd&token={Program.Config.ApiToken}&DiscordID={e.Author.Id}&password={Base64.Base64Encode(message[1])}");
						await e.Channel.SendMessageAsync(
							$"Request processed, result: " + result3);
						break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("[EXCEPTION] " + ex.GetType() + ": " + ex.Message + "\r\n" + ex.StackTrace);
			}
		}
	}
}

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json;

namespace MCNetworkLoginDiscordBot
{
	class Program
	{
		internal static DiscordClient Client;
		internal static ConfigJson Config;

		static void Main(string[] args)
		{
			Console.WriteLine("MCNetworkLoginDiscordBot");
			Console.WriteLine("Copyright by Łukasz \"zabszk\" Jurczyk, 2019");
			Console.WriteLine("Init...");
			RunBot().GetAwaiter().GetResult();
		}

		static async Task RunBot()
		{
			var json = "{\"token\": \"\", \"prefix\": \"!\", \"RegisterChannel\": \"\", \"ApiUrl\": \"https://\", \"ApiToken\": \"\"}";
			if (File.Exists("config.json"))
			{
				using (var fs = File.OpenRead("config.json"))
				using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
					json = await sr.ReadToEndAsync();

				Console.WriteLine("Config loaded.");
			}
			else
			{
				using (var fs = File.OpenWrite("config.json"))
				using (var sw = new StreamWriter(fs, new UTF8Encoding(false)))
					await sw.WriteLineAsync(json);

				Console.WriteLine("\"config.json\" generated. Please configure the bot and run it again.");
				return;
			}

			Config = JsonConvert.DeserializeObject<ConfigJson>(json);
			var cfg = new DiscordConfiguration
			{
				Token = Config.Token,
				TokenType = TokenType.Bot,

				AutoReconnect = true,
				LogLevel = LogLevel.Debug,
				UseInternalLogHandler = true
			};

			Client = new DiscordClient(cfg);

			Client.Ready += Event.ClientReady;
			Client.ClientErrored += Event.ClientError;
			Client.MessageCreated += Event.MessageReceived;

			await Client.ConnectAsync();
			await Task.Delay(-1);
		}
	}
}

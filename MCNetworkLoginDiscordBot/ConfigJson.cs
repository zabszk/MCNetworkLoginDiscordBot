using Newtonsoft.Json;

namespace MCNetworkLoginDiscordBot
{
	public struct ConfigJson
	{
		[JsonProperty("token")]
		internal string Token { get; set; }

		[JsonProperty("prefix")]
		internal string CommandPrefix { get; set; }

		[JsonProperty("RegisterChannel")]
		internal ulong RegisterChannel { get; set; }

		[JsonProperty("ApiUrl")]
		internal string ApiUrl { get; set; }

		[JsonProperty("ApiToken")]
		internal string ApiToken { get; set; }
	}
}

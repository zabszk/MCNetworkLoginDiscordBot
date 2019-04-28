# MCNetworkLoginDiscordBot
Discord bot and WebAPI for https://github.com/zabszk/NetworkLogin minecraft plugin.

Bot written in C# in .NET Core - compatible with Windows and Linux x64.
WebAPI written in PHP.

# config.json for the bot
```json
{
"token": "Discord application token",
"prefix": "!",
"RegisterChannel": ChannelIdForCommands,
"ApiUrl": "https://url.of.your.webapi/DiscordNetworkLoginProcessor.php",
"ApiToken": "Random API token - must be the same as in DiscordNetworkLoginProcessor.php"
}
```

# WebAPI files
* authenticator.php - used by the minecraft plugin for password verification
* DiscordNetworkLoginProcessor.php - used by this bot for accounts management

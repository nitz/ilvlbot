# ilvlbot
A Discord bot that fetches World of Warcraft character info: item level, azerite level, achievments, etc. Can also pull a whole guild of characters and report their top item levels.

## Prerequisites

I've built this with vs15 and try to stay up with patches. It's built against .NET 4.7.2.

## Building

Fire up Visual Studio 2017 and smash build. It should grab the required nuget packages and go to town.

## Running

You'll need to set up a few configuation options. Run the compiled binary once and it'll spit out a default `settings.conf` for you. Add your Discord and battlenet API keys and you should be solid.

## Todo

Provide some useful info on what is actually going on here.

## Thanks

Big thanks to [Discord.Net](https://github.com/RogueException/Discord.Net) and [Json.NET](https://github.com/JamesNK/Newtonsoft.Json). Thanks as well to [wowtoken.info](https://wowtoken.info) for the easy, non-oauth access to the token pricing. Finally, thanks to Blizzard; You know, for the game that this tool even exists for.

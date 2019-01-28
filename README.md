# ilvlbot
A Discord bot that fetches World of Warcraft character info: item level, azerite level, achievments, etc. Can also pull a whole guild of characters and report their top item levels.

## Prerequisites

* Visual Studio 15.9 or later.
* .NET Core 2.2 SDK

I build with vs15 and try to stay up with patches. It's built with .NET Core 2.2, which is supported by vs15.9 or later. You'll also need the .NET Core 2.2 SDK installed. As of writing, it doesn't seem to come as a component in the msvs installer, and instead is installed seperatedly from [Microsoft's Site](https://dotnet.microsoft.com/download). The ".NET Core cross-platform development" workload does install .NET Core 2.1 and some tooling, but 2.2 still needs to be seperately installed for now.

## Building

Fire up Visual Studio 2017 and smash build. It should grab the required nuget packages and go to town.

## Running

You'll need to set up a few configuation options. Run the compiled application once and it'll spit out a default `settings.conf` for you (if it didn't copy the blank one from the project folder). Add your Discord and Blizzard API keys and you should be solid.

## Todo

Provide some useful info on what is actually going on here. Still have some bits I want to clean up with better DI (settings in particular.) Still a few places I could move to some newer C# 7 syntax.

## Thanks

Big thanks to [Discord.Net](https://github.com/RogueException/Discord.Net) and [Json.NET](https://github.com/JamesNK/Newtonsoft.Json). Thanks as well to [wowtoken.info](https://wowtoken.info) for the easy, non-oauth access to the token pricing. Finally, thanks to Blizzard; You know, for the game that this tool even exists for.

using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace ilvlbot.Access
{
	/// <summary>
	/// An attribute used to disable commands from being executed.
	/// Should only be used as a debugging tool.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class DisabledAttribute : PreconditionAttribute
	{
		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider service)
		{
			// Disabled commands can't execute.
			return Task.FromResult(PreconditionResult.FromError("Command disabled."));
		}
	}
}

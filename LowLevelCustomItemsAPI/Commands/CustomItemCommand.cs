namespace Core.Commands.CustomItems;

using API.Extensions;
using API.Interfaces;
using CommandSystem;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CustomItemCommand : ParentCommand
{
	public CustomItemCommand() => LoadGeneratedCommands();

	public override string Command => "customitem";
	public override string[] Aliases => ["cui"];
	public override string Description => "Custom item commands.";

	public sealed override void LoadGeneratedCommands()
	{
		RegisterCommand(new List());
		RegisterCommand(new Give());
	}

	protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.CheckRemoteAdmin(out response))
			return false;

		response = AllCommands.Aggregate("Please enter a valid subcommand:",
			(current,
			 command) => current +
						 ($"\n\n<color=yellow><b>- {command.Command}{(!command.Aliases.IsEmpty() ? $" ({string.Join(", ", command.Aliases)})" : "")}\n" +
						  $"{((IUsageHelper)command).Usage}</b></color>\n" +
						  $"<color=white>{command.Description}</color>"));
		return false;
	}
}
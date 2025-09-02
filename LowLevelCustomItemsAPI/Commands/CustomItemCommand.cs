namespace LowLevelCustomItemsAPI.Commands;

using CommandSystem;
using LabApi.Features.Permissions;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CustomItemCommand : ParentCommand
{
	public CustomItemCommand() => LoadGeneratedCommands();

	public override string Command => "customitems";
	public override string[] Aliases => ["llci", "ci", "citems"];
	public override string Description => "Custom item commands.";

	public sealed override void LoadGeneratedCommands()
	{
		RegisterCommand(new List());
		RegisterCommand(new Give());
	}

	protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission("llci.customitems"))
		{
			response = "You do not have permission to use this command.";
			return false;
		}

		response = "Please enter a valid subcommand:\n";
		int i = 0;
		foreach (ICommand command in AllCommands)
		{
			i++;
			response += $"{i}.\t{command.Command}\n";
			response += $"\t{command.Description}\n\n";
		}
		return false;
	}
}
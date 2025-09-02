namespace LowLevelCustomItemsAPI.Commands;

using CommandSystem;
using Internal;

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
		if (!sender.HasPermission("llci.customitems", out response))
			return false;

		response = "Please enter a valid subcommand:\n";
		int i = 0;
		foreach (ICommand command in AllCommands)
		{
			i++;
			response += $"{i}. {command.Command}\n";
			response += $"   {command.Description}\n\n";
		}
		return false;
	}
}
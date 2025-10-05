namespace Examples.Commands;

using CommandSystem;
using Items;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using LowLevelCustomItemsAPI.API;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CreateCustomItem : ICommand
{
	public string Command => "createcustomitem";
	public string[] Aliases => ["cci"];
	public string Description => "Creates an item with a custom name.";
	private const string USAGE = "cci (item type) (name)";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission("llci.examples.createcustomitem"))
		{
			response = "You do not have permission to use this command.";
			return false;
		}

		if (arguments.Count < 2)
		{
			response = $"Invalid arguments. Usage: {USAGE}";
			return false;
		}

		if (!Enum.TryParse(arguments.At(0), out ItemType itemType))
		{
			response = $"Invalid item type. Usage: {USAGE}";
			return false;
		}

		Player player = Player.Get(sender)!;
		string name = string.Join(" ", arguments.Skip(1));

		CustomItem.Get<NamedItem>()!.GiveItem(player, itemType, name);

		response = "Done!";
		return true;
	}
}
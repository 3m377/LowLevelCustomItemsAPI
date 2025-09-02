namespace LowLevelCustomItemsAPI.Commands;

using API;
using CommandSystem;
using LabApi.Features.Permissions;

public class List : ICommand
{
	public string Command => "list";
	public string[] Aliases => ["l"];
	public string Description => "Lists all custom items.";
	private const string USAGE = "cui list"; // I don't know how to use IUsageProvider :(

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission("llci.customitems"))
		{
			response = "You do not have permission to use this command.";
			return false;
		}

		if (!CustomItemManager.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Any())
		{
			response = "No custom items found.";
			return false;
		}

		if (arguments.Count != 0)
		{
			response = $"Invalid syntax! Usage: {USAGE}";
			return false;
		}

		response = CustomItemManager.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Aggregate("Custom Items:\n",
			(current, handler) => current + $"- {handler.Name} {(handler.Identifiers.Length != 0 ? "(" : "") + string.Join(", ", handler.Identifiers) + (handler.Identifiers.Length != 0 ? ")" : "")}\n");
		return true;
	}
}
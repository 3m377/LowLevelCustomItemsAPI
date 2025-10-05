namespace LowLevelCustomItemsAPI.Commands;

using API;
using CommandSystem;
using Internal;

public class List : ICommand
{
	public string Command => "list";
	public string[] Aliases => ["l"];
	public string Description => "Lists all custom items.";
	private const string USAGE = "cui list"; // I don't know how to use IUsageProvider :(

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasPermission("llci.customitems", out response))
			return false;

		if (!CustomItem.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Any())
		{
			response = "No custom items found.";
			return false;
		}

		if (arguments.Count != 0)
		{
			response = $"Invalid syntax! Usage: {USAGE}";
			return false;
		}

		response = CustomItem.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Aggregate("Custom Items:\n",
			(current, handler) => current + $"- {handler.Name} {(handler.Identifiers.Length != 0 ? "(" : "") + string.Join(", ", handler.Identifiers) + (handler.Identifiers.Length != 0 ? ")" : "")}\n");
		return true;
	}
}
namespace Core.Commands.CustomItems;

using API.CustomItems;
using API.Extensions;
using API.Interfaces;
using CommandSystem;

public class List : ICommand, IUsageHelper
{
	public string Command => "list";
	public string[] Aliases => ["l"];
	public string Description => "Lists all custom items.";
	public string Usage => "cui list";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.CheckRemoteAdmin(out response))
			return false;

		if (!CustomItemManager.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Any())
		{
			response = "No custom items found.";
			return false;
		}

		if (arguments.Count != 0)
		{
			response = $"Invalid syntax! Usage: {Usage}";
			return false;
		}

		response = CustomItemManager.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Aggregate("Custom Items:\n",
			(current, handler) => current + $"- {handler.Name} {(handler.Identifiers.Length != 0 ? "(" : "") + string.Join(", ", handler.Identifiers) + (handler.Identifiers.Length != 0 ? ")" : "")}\n");
		return true;
	}
}
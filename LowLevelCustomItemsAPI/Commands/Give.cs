namespace LowLevelCustomItemsAPI.Commands;

using API;
using CommandSystem;
using Internal;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

public class Give : ICommand
{
	public string Command => "give";
	public string[] Aliases => ["g"];
	public string Description => "Gives a custom item.";
	private const string USAGE = "cui give (item) (args) ?(player)"; // I don't know how to use IUsageProvider :(

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasAnyPermission("llci.customitems"))
		{
			response = "You do not have permission to use this command.";
			return false;
		}

		if (arguments.Count < 1)
		{
			response = $"Invalid syntax! Usage: {USAGE}";
			return false;
		}

		string itemName;
		Player player;

		// Parse item name, additional arguments, and player
		ArraySegment<string> itemArgs;
		if (arguments.Count >= 2)
		{
			string potentialPlayer = arguments.Last();
			if (Utils.TryGetPlayer(potentialPlayer, out player))
			{
				itemName = arguments.First();
				itemArgs = new ArraySegment<string>(arguments.ToArray(), arguments.Offset + 1, arguments.Count - 2);
			}
			else
			{
				itemName = arguments.First();
				itemArgs = new ArraySegment<string>(arguments.ToArray(), arguments.Offset + 1, arguments.Count - 1);
				player = Player.Get(sender)!;
			}
		}
		else
		{
			itemName = arguments.First();
			itemArgs = [];
			player = Player.Get(sender)!;
		}

		CustomItem handler = CustomItemManager.Items.Values.Select(dict => dict.Values).SelectMany(list => list).FirstOrDefault(
			item => Enumerable.Contains(item.Identifiers, itemName.ToLower()));

		if (handler == null)
		{
			response = CustomItemManager.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Aggregate("Custom Items:\n",
				(current, handler2) => current + $"- {handler2.Name} {(handler2.Identifiers.Length != 0 ? "(" : "") + string.Join(", ", handler2.Identifiers) + (handler2.Identifiers.Length != 0 ? ")" : "")}\n");
			return false;
		}

		if (player.IsInventoryFull)
		{
			response = $"<color=red>Player {player.Nickname}'s inventory is full.";
			return false;
		}

		handler.GiveDefault(player, itemArgs, null);

		response = $"<color=green>You gave {player.Nickname} a {handler.Name}";
		return true;
	}
}
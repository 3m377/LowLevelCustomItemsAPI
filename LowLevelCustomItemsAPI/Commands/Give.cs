namespace LowLevelCustomItemsAPI.Commands;

using API;
using CommandSystem;
using Internal;
#if EXILED
using Exiled.API.Features;
#else
using LabApi.Features.Wrappers;
#endif

public class Give : ICommand
{
	public string Command => "give";
	public string[] Aliases => ["g"];
	public string Description => "Gives a custom item.";
	private const string USAGE = "cui give (item) ?(player)"; // I don't know how to use IUsageProvider :(

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.HasPermission("llci.customitems", out response))
			return false;

		if (arguments.Count < 1)
		{
			response = $"Invalid syntax! Usage: {USAGE}";
			return false;
		}

		string itemName;
		Player player;

		// Parse item name and player
		if (arguments.Count >= 2)
		{
			if (Utils.TryGetPlayer(arguments.Last(), out player))
				itemName = string.Join(" ", arguments.Take(arguments.Count - 1));
			else
			{
				itemName = string.Join(" ", arguments);
				player = Player.Get(sender)!;
			}
		}
		else
		{
			itemName = arguments.First();
			player = Player.Get(sender)!;
		}

		CustomItem handler = CustomItem.Items.Values.Select(dict => dict.Values).SelectMany(list => list)
											  .FirstOrDefault(item =>
												  string.Equals(item.Name, itemName,
													  StringComparison.CurrentCultureIgnoreCase) ||
												  Enumerable.Contains(item.Identifiers, itemName.ToLower()));

		if (handler == null)
		{
			response = CustomItem.Items.Values.Select(dict => dict.Values).SelectMany(list => list).Aggregate("Custom Items:\n",
				(current, handler2) => current + $"- {handler2.Name} {(handler2.Identifiers.Length != 0 ? "(" : "") + string.Join(", ", handler2.Identifiers) + (handler2.Identifiers.Length != 0 ? ")" : "")}\n");
			return false;
		}

		if (player.IsInventoryFull)
		{
			response = $"<color=red>Player {player.Nickname}'s inventory is full.";
			return false;
		}

		handler.GiveDefault(player, null);

		response = $"<color=green>You gave {player.Nickname} a {handler.Name}";
		return true;
	}
}
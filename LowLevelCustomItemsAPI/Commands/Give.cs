namespace Core.Commands.CustomItems;

using API.CustomItems;
using API.Extensions;
using API.Interfaces;
using CommandSystem;
using LabApi.Features.Wrappers;

public class Give : ICommand, IUsageHelper
{
	public string Command => "give";
	public string[] Aliases => ["g"];
	public string Description => "Gives a custom item.";
	public string Usage => "cui give (item) ?(player)";

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (!sender.CheckRemoteAdmin(out response))
			return false;

		if (arguments.Count < 1)
		{
			response = $"Invalid syntax! Usage: {Usage}";
			return false;
		}

		string itemName;
		Player player;

		// Parse item name and player
		if (arguments.Count >= 2)
		{
			string potentialPlayer = arguments.Last();
			if (Player.TryGet(potentialPlayer, out player))
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

		CustomItem handler = CustomItemManager.Items.Values.Select(dict => dict.Values).SelectMany(list => list).FirstOrDefault(
			item => string.Equals(item.Name, itemName, StringComparison.CurrentCultureIgnoreCase) || Enumerable.Contains(item.Identifiers, itemName.ToLower()));

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

		handler.GiveDefault(player, [], null);

		response = $"<color=green>You gave {player.Nickname} a {handler.Name}";
		return true;
	}
}
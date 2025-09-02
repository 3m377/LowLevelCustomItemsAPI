namespace Examples;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LowLevelCustomItemsAPI.API;

public class NamedItem : CustomItem<string>
{
	public override string Name => "Named Item";
	public override string[] Identifiers => ["named item", "named", "custom"];

	public override void OnEnabled()
	{
		PlayerEvents.ChangedItem += ChangedItem;
	}

	public override void OnDisabled()
	{
		PlayerEvents.ChangedItem -= ChangedItem;
	}

	private void ChangedItem(PlayerChangedItemEventArgs ev)
	{
		if (ev.NewItem == null)
			return;

		if (Check(ev.NewItem.Serial, out string name))
			ev.Player.SendHint(name, 4f);
	}

	public override Item GiveDefault(Player player, ArraySegment<string> args, object data)
	{
		string name = "Custom Item";
		ItemType type = ItemType.Coin;
		switch (args.Count)
		{
			case 1:
				if (Enum.TryParse(args.At(0), true, out type))
					break;
				name = args.At(0);
				break;
			case >= 2:
				name = string.Join(" ", Enum.TryParse(args.At(0), true, out type) ? args : args.Skip(1));
				break;
		}
		Item item = player.AddItem(type)!;
		Register(item.Serial, name);
		return item;
	}
}
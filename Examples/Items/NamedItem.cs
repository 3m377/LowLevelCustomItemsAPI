namespace Examples.Items;

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

	public override Item GiveDefault(Player player, object data)
	{
		Item item = player.AddItem(ItemType.Coin)!;
		Register(item.Serial, "Custom Item");
		return item;
	}

	public Item GiveItem(Player player, ItemType itemType, string name)
	{
		Item item = player.AddItem(itemType)!;
		Register(item.Serial, name);
		return item;
	}
}
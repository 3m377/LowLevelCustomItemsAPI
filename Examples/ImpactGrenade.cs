namespace Examples;

using LabApi.Features.Wrappers;
using LowLevelCustomItemsAPI.API;

public class ExampleItem : CustomItem
{
	public override string Name { get; }
	public override string[] Identifiers { get; }

	public override void OnEnabled()
	{
		throw new NotImplementedException();
	}

	public override void OnDisabled()
	{
		throw new NotImplementedException();
	}

	public override Item GiveDefault(Player player, ArraySegment<string> args, object data)
	{
		Item item = player.AddItem(ItemType.GrenadeHE)
	}
}
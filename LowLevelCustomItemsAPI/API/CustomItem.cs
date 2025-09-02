#nullable enable

namespace Core.API.CustomItems;

using LabApi.Features.Wrappers;

public abstract class CustomItem
{
	public abstract string Name { get; }
	public abstract string[] Identifiers { get; }

	public abstract void Enable();

	public abstract void Disable();

	public abstract Item GiveDefault(Player player, ArraySegment<string> args, object? data);

	public HashSet<int> Serials => CustomItemManager.RegisteredSerials[this].Keys.ToHashSet();

	public bool Check(int serial, out object? data) => CustomItemManager.Check(this, serial, out data);
	public bool Check(int serial) => Check(serial, out _);

	public void Register(ushort serial, object? data = null) => CustomItemManager.RegisterSerial(this, serial, data);
	public bool Unregister(ushort serial) => CustomItemManager.RemoveSerial(this, serial);
	public void Clear() => CustomItemManager.ClearSerials(this);
}


public abstract class CustomItem<T> : CustomItem
{
	public bool Check(ushort serial, out T? data)
	{
		data = default;
		if (!CustomItemManager.Check(this, serial, out object? raw) || raw is null)
			return false;
		if (raw is not T t)
			return false;
		data = t;
		return true;
	}

	public void Register(ushort serial, T data) => CustomItemManager.RegisterSerial(this, serial, data);
}
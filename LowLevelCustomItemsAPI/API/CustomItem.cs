#nullable enable

namespace LowLevelCustomItemsAPI.API;
#if EXILED
using Exiled.API.Features;
using Exiled.API.Features.Items;
#else
using LabApi.Features.Wrappers;
#endif

/// <summary>
/// The base class for custom items.
/// </summary>
public abstract class CustomItem
{
	/// <summary>
	/// The name of the item.
	/// </summary>
	public abstract string Name { get; }

	/// <summary>
	/// The identifiers of the item. These are used for giving items through the CustomItems command.
	/// </summary>
	public abstract string[] Identifiers { get; }

	/// <summary>
	/// Run when the item is registered.
	/// </summary>
	public abstract void OnEnabled();

	/// <summary>
	/// Run when the item is unregistered.
	/// </summary>
	public abstract void OnDisabled();

	/// <summary>
	/// Provides a default instance of the custom item to the specified player.
	/// </summary>
	/// <param name="player">The player receiving the item.</param>
	/// <param name="data">Optional additional data to store with the item.</param>
	/// <returns>The item that was created for the player.</returns>
	public abstract Item GiveDefault(Player player, object? data);

	/// <summary>
	/// A collection of serials that are registered to the custom item.
	/// </summary>
	public virtual HashSet<int> Serials => CustomItemManager.RegisteredSerials[this].Keys.ToHashSet();

	/// <summary>
	/// Checks if the specified serial is registered for the custom item and retrieves associated data.
	/// </summary>
	/// <param name="serial">The serial number to check for registration.</param>
	/// <param name="data">The data associated with the item.
	/// This will be null if this custom item it is registered to is not a <see cref="CustomItem{T}"/>.</param>
	/// <returns>True if the serial is registered for the custom item, otherwise false.</returns>
	public virtual bool Check(int serial, out object? data) => CustomItemManager.Check(this, serial, out data);

	/// <summary>
	/// Checks if the specified serial is registered for the custom item.
	/// </summary>
	/// <param name="serial">The serial number to check for registration.</param>
	/// <returns>True if the serial is registered for the custom item, otherwise false.</returns>
	public virtual bool Check(int serial) => Check(serial, out _);

	/// <summary>
	/// Registers the specified serial to the custom item.
	/// </summary>
	/// <param name="serial">The serial number to register.</param>
	/// <param name="data">Data to associate with the serial.
	/// It is recommended that if your item stores data to instead make it a <see cref="CustomItem{T}"/>.</param>
	public virtual void Register(ushort serial, object? data = null) => CustomItemManager.RegisterSerial(this, serial, data);

	/// <summary>
	/// Unregisters a custom item associated with the given serial number.
	/// </summary>
	/// <param name="serial">The serial number associated with the custom item to be unregistered.</param>
	/// <returns>True if the item was successfully unregistered, otherwise false.</returns>
	public virtual bool Unregister(ushort serial) => CustomItemManager.UnregisterSerial(this, serial);

	/// <summary>
	/// Clears all serials associated with the custom item.
	/// </summary>
	public virtual void Clear() => CustomItemManager.ClearSerials(this);
}

/// <summary>
/// The base class for custom items that contain data.
/// </summary>
/// <typeparam name="T">The type of data contained in the item.</typeparam>
public abstract class CustomItem<T> : CustomItem
{
	/// <summary>
	/// Checks if the specified serial is registered for the custom item and retrieves associated data.
	/// </summary>
	/// <param name="serial">The serial number to check for registration.</param>
	/// <param name="data">The data associated with the item.</param>
	/// <returns>True if the serial is registered for the custom item, otherwise false.</returns>
	public virtual bool Check(int serial, out T? data)
	{
		data = default;
		if (!CustomItemManager.Check(this, serial, out object? raw) || raw is null)
			return false;
		if (raw is not T t)
			return false;
		data = t;
		return true;
	}

	/// <summary>
	/// Registers the specified serial to the custom item.
	/// </summary>
	/// <param name="serial">The serial number to register.</param>
	/// <param name="data">The data to associate with the serial.</param>
	public virtual void Register(int serial, T data) => CustomItemManager.RegisterSerial(this, serial, data);
}
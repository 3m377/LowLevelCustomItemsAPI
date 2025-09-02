#nullable enable

namespace LowLevelCustomItemsAPI.API;

using System.Reflection;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

/// <summary>
/// Provides functionality for managing custom items, including registration, retrieval, and validation of item instances.
/// </summary>
public static class CustomItemManager
{
	internal static Dictionary<Assembly, Dictionary<Type, CustomItem>> Items { get; } = [];
	internal static Dictionary<CustomItem, Dictionary<int, object?>> RegisteredSerials { get; } = [];

	internal static void Initialize()
	{
		ServerEvents.WaitingForPlayers += WaitingForPlayers;
	}

	internal static void Uninitialize()
	{
		ServerEvents.WaitingForPlayers -= WaitingForPlayers;
	}

	/// <summary>
	/// Tries to register all custom items in the calling assembly.
	/// </summary>
	/// <returns>True if all items were successfully registered, otherwise false.</returns>
	public static bool TryRegisterItems()
	{
		try
		{
			Assembly assembly = Assembly.GetCallingAssembly();

			Items.Add(assembly, []);

			Type[] types = assembly.GetTypes();

			foreach (Type type in types)
			{
				// We want concrete subclasses of CustomItem
				if (!type.IsClass || type.IsAbstract)
					continue;
				if (!typeof(CustomItem).IsAssignableFrom(type))
					continue;

				CustomItem item = (CustomItem)Activator.CreateInstance(type, nonPublic: true);
				Items[assembly].Add(type, item);
				if (!RegisteredSerials.ContainsKey(item))
					RegisteredSerials[item] = new Dictionary<int, object?>();
				item.OnEnabled();
			}

			return true;
		}
		catch (Exception ex)
		{
			Logger.Error($"Error while registering items: {ex}");
			return false;
		}
	}

	/// <summary>
	/// Unregisters all custom items in the calling assembly.
	/// </summary>
	public static void UnregisterItems()
	{
		Assembly assembly = Assembly.GetCallingAssembly();

		foreach (CustomItem item in Items[assembly].Values)
			item.OnDisabled();

		Items.Remove(assembly);
	}

	/// <summary>
	/// Gets the registered instance of the specified custom item.
	/// </summary>
	/// <typeparam name="T">The type of custom item to retrieve.</typeparam>
	/// <returns>The registered instance of the specified custom item, or null if it is not registered.</returns>
	public static T? Get<T>() where T : CustomItem =>
		Items.Values
			 .SelectMany(dict => dict.Values)
			 .OfType<T>()
			 .FirstOrDefault();

	/// <summary>
	/// Attempts to retrieve the registered instance of the specified custom item.
	/// </summary>
	/// <typeparam name="T">The type of the custom item to retrieve.</typeparam>
	/// <param name="item">The registered instance of the specified custom item if it is found, or null if it is not found.</param>
	/// <returns>True if the specified custom item is successfully retrieved, otherwise false.</returns>
	public static bool TryGet<T>(out T? item) where T : CustomItem => (item = Get<T>()) != null;

	/// <summary>
	/// Checks if the specified serial is registered for any custom item.
	/// </summary>
	/// <param name="serial">The serial number to check for registration.</param>
	/// <returns>True if the serial is registered to any custom item, otherwise false.</returns>
	public static bool Check(int serial) => RegisteredSerials.Values.Any(x => x.ContainsKey(serial));

	/// <summary>
	/// Checks if a given serial is associated with the specified custom item.
	/// </summary>
	/// <param name="item">The custom item to check for the association.</param>
	/// <param name="serial">The serial number to check for registration.</param>
	/// <returns>True if the serial is registered to the specified custom item, otherwise false.</returns>
	public static bool Check(CustomItem item, int serial) =>
		RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map) && map.ContainsKey(serial);

	/// <summary>
	/// Checks if a given serial is present in the registered serials.
	/// </summary>
	/// <param name="serial">The serial to check.</param>
	/// <param name="data">The data associated with the serial.
	/// This will be null if there is no data associated with it.</param>
	/// <returns>True if the serial exists in the registered serials, otherwise false.</returns>
	public static bool Check(int serial, out object? data)
	{
		data = null;
		foreach (KeyValuePair<CustomItem, Dictionary<int, object?>> pair in RegisteredSerials)
			if (pair.Value.TryGetValue(serial, out data))
				return true;
		return false;
	}

	/// <summary>
	/// Checks whether the specified serial number exists in the registered serials.
	/// </summary>
	/// <param name="item">The custom item to check.</param>
	/// <param name="serial">The serial number to check.</param>
	/// <param name="data">The data associated with the serial.
	/// This will be null if there is no data associated with it.</param>
	/// <returns>True if the serial number exists; otherwise, false.</returns>
	public static bool Check(CustomItem item, int serial, out object? data)
	{
		data = null;
		return RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map) && map.TryGetValue(serial, out data);
	}

	/// <summary>
	/// Registers the given serial to the specified custom item.
	/// </summary>
	/// <param name="item">The custom item to register the serial to.</param>
	/// <param name="serial">The serial number to register.</param>
	/// <param name="data">The data to associate with the serial.
	/// It is recommended to only register data if the custom item is a <see cref="CustomItem{T}"/>.</param>
	public static void RegisterSerial(CustomItem item, int serial, object? data = null)
	{
		if (!RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map))
		{
			map = new Dictionary<int, object?>();
			RegisteredSerials[item] = map;
		}
		map[serial] = data;
	}

	/// <summary>
	/// Unregisters the serial number associated with the specified custom item.
	/// </summary>
	/// <param name="item">The custom item with which the serial number is associated.</param>
	/// <param name="serial">The serial number to be removed.</param>
	/// <returns>True if the serial number was successfully unregistered, otherwise false.</returns>
	public static bool UnregisterSerial(CustomItem item, int serial) =>
		RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map) && map.Remove(serial);

	/// <summary>
	/// Clears all serials registered to the specified custom item.
	/// </summary>
	/// <param name="item">The custom item to clear serials for.</param>
	public static void ClearSerials(CustomItem item)
	{
		if (RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map))
			map.Clear();
	}

	private static void WaitingForPlayers()
	{
		foreach (CustomItem item in Items.Values.Select(x => x.Values).SelectMany(x => x))
			ClearSerials(item);
	}
}
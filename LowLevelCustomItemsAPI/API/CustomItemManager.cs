#nullable enable

namespace Core.API.CustomItems;

using System.Reflection;
using Attributes;
using LabApi.Events.Handlers;

public static class CustomItemManager
{
	internal static Dictionary<Assembly, Dictionary<Type, CustomItem>> Items { get; } = [];
	public static Dictionary<CustomItem, Dictionary<int, object?>> RegisteredSerials { get; } = [];

	[OnPluginEnabled]
	private static void Initialize()
	{
		ServerEvents.WaitingForPlayers += WaitingForPlayers;
	}

	public static bool TryRegisterItems(Assembly assembly)
	{
		try
		{
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
				item.Enable();
			}

			return true;
		}
		catch (Exception ex)
		{
			Log.Error($"Error while registering items from assembly {assembly.FullName}: {ex}");
			return false;
		}
	}

	public static void UnregisterItems(Assembly assembly)
	{
		foreach (CustomItem item in Items[assembly].Values)
			item.Disable();
		Items.Remove(assembly);
	}

	public static T? Get<T>() where T : CustomItem =>
		Items.Values
			 .SelectMany(dict => dict.Values)
			 .OfType<T>()
			 .FirstOrDefault();

	public static bool TryGet<T>(out T? item) where T : CustomItem => (item = Get<T>()) != null;

	public static bool Check(int serial, out object? data)
	{
		data = null;
		foreach (KeyValuePair<CustomItem, Dictionary<int, object?>> pair in RegisteredSerials)
			if (pair.Value.TryGetValue(serial, out data))
				return true;
		return false;
	}

	public static bool Check(CustomItem item, int serial, out object? data)
	{
		data = null;
		return RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map) && map.TryGetValue(serial, out data);
	}

	public static void RegisterSerial(CustomItem item, ushort serial, object? data = null)
	{
		if (!RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map))
		{
			map = new Dictionary<int, object?>();
			RegisteredSerials[item] = map;
		}
		map[serial] = data;
	}

	public static bool RemoveSerial(CustomItem item, ushort serial) =>
		RegisteredSerials.TryGetValue(item, out Dictionary<int, object?>? map) && map.Remove(serial);

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
# LLCI - A Low-Level Custom Items API for LabAPI or EXILED
A powerful and flexible low-level API for creating custom items using LabAPI or EXILED.

## Features
 - Direct access to item mechanics without high-level abstractions
 - Support for associating custom data with items using generics
 - Built-in commands for giving custom items
 - Support for both simple and complex custom item implementations

### Dependencies
 - None

## Quick Start

### Installation
1. Download the [Latest Release](https://github.com/3m377/LowLevelCustomItemsAPI/releases/latest).
2. Place the DLL in your preferred plugin manager's `Plugins` folder.
3. Reference the DLL in your plugin. (nuget coming never idk how to do that)

### Permissions
This plugin has one permission:
 - `llci.customitems`: Allows the player to use the `customitems` command/subcommands (ci give, ci list)

### Using the API
1. **Register your items** in your plugin's `OnEnabled` method:
```csharp
public override void Enable()
{
    CustomItemManager.TryRegisterItems();
}

public override void Disable()
{
    CustomItemManager.UnregisterItems();
}
```

2. **Create a new class** inheriting from `CustomItem`. EX:
```csharp
using LowLevelCustomItemsAPI.API;
using LabApi.Features.Wrappers;

public class MyCustomItem : CustomItem
{
    public override string Name => "My Custom Item";
    public override string[] Identifiers => ["my custom item", "custom item"];

    public override void OnEnabled()
    {
        // Initialize your item (subscribe to events, etc.)
    }

    public override void OnDisabled()
    {
        // Cleanup (unsubscribe from events, etc.)
    }

    public override Item GiveDefault(Player player, ArraySegment<string> args, object data)
    {
        Item item = player.AddItem(ItemType.Coin);
        Register(item.Serial);
        return item;
    }
}
```

Examples can be found in the [Examples](Examples) folder.

## License
MIT License, do whatever you want with the code, I don't care.

```
MIT License
Copyright (c) 2025 3m377
```

## Acknowledgments
 - Me!!!

## Support
 - **Issues**: [GitHub Issues](https://github.com/your-repo/issues)

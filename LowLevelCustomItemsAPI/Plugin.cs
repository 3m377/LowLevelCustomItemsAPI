// this is all a fucking mess

namespace LowLevelCustomItemsAPI;

using API;
#if EXILED
using Exiled.API.Features;
using Exiled.API.Interfaces;
#else
using LabApi.Features;
#endif

#if EXILED
public class Config : IConfig
{
	public bool IsEnabled { get; set; } = true;
	public bool Debug { get; set; } = false;
}

public class Plugin : Plugin<Config>
#else
public class Plugin : LabApi.Loader.Features.Plugins.Plugin
#endif
{
#if EXILED
	public override string Name => "LowLevelCustomItemsAPI.EXILED";
	public override string Prefix => "LLCI";
#else
	public override string Name => "LowLevelCustomItemsAPI.LabAPI";
	public override string Description => "A low-level API for custom items.";
	public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;
#endif
	public override string Author => "3m377";
	public override Version Version => new(1, 0, 2);

#if EXILED
	public override void OnEnabled()
#else
	public override void Enable()
#endif
	{
		CustomItem.Initialize();
	}

#if EXILED
	public override void OnDisabled()
#else
	public override void Disable()
#endif
	{
		CustomItem.Uninitialize();
	}
}
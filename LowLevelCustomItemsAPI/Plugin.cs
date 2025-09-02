namespace LowLevelCustomItemsAPI;

using API;
using LabApi.Features;

public class Plugin : LabApi.Loader.Features.Plugins.Plugin
{
	public override string Name => "LowLevelCustomItemsAPI";
	public override string Description => "A low-level API for custom items.";
	public override string Author => "3m377";
	public override Version Version => new(1, 0, 0);
	public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

	public override void Enable()
	{
		CustomItemManager.Initialize();
	}

	public override void Disable()
	{
		CustomItemManager.Uninitialize();
	}
}
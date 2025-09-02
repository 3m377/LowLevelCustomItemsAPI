namespace Examples;

using LabApi.Features;
using LowLevelCustomItemsAPI.API;

public class Plugin : LabApi.Loader.Features.Plugins.Plugin
{
	public override string Name => "Example LLCI plugin";
	public override string Description => "An example plugin for the LLCI API.";
	public override string Author => "3m377";
	public override Version Version => new(1, 0, 0);
	public override Version RequiredApiVersion => LabApiProperties.CurrentVersion;

	public override void Enable()
	{
		CustomItemManager.TryRegisterItems();
	}

	public override void Disable()
	{
		CustomItemManager.UnregisterItems();
	}
}
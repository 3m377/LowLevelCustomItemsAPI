namespace LowLevelCustomItemsAPI.Internal;

using LabApi.Features.Wrappers;

internal static class Utils
{
	internal static bool TryGetPlayer(string str, out Player player)
	{
		player = null;
		if (Player.TryGet(str, out player))
			return true;
		return int.TryParse(str, out int id) && Player.TryGet(id, out player);
	}
}
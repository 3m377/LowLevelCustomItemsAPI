namespace LowLevelCustomItemsAPI.Internal;

using LabApi.Features.Wrappers;

internal static class Extensions
{
	internal static bool TryGetPlayer(object obj, out Player player)
	{
		player = null;
		return obj switch
		{
			string userId => Player.TryGet(userId, out player),
			int id => Player.TryGet(id, out player),
			_ => false
		};
	}
}
namespace LowLevelCustomItemsAPI.Internal;

using CommandSystem;
#if EXILED
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
#else
using LabApi.Features.Wrappers;
using LabApi.Features.Permissions;
#endif

internal static class Utils
{
	internal static bool TryGetPlayer(string str, out Player player)
	{
#if EXILED
		return Player.TryGet(str, out player);
#else
		player = null;
		if (Player.TryGet(str, out player))
			return true;
		return int.TryParse(str, out int id) && Player.TryGet(id, out player);
#endif
	}

	internal static bool HasPermission(this ICommandSender sender, string permission, out string response)
	{
		response = "You do not have permission to use this command.";
#if EXILED
		return sender.CheckPermission(permission);
#else
		return sender.HasAnyPermission(permission);
#endif
	}
}
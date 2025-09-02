namespace Examples;

using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LowLevelCustomItemsAPI.API;
using MEC;
using UnityEngine;

public class ImpactGrenade : CustomItem
{
	public override string Name => "Impact Grenade";
	public override string[] Identifiers => ["impact grenade", "igrenade", "ig"];

	public override void OnEnabled()
	{
		PlayerEvents.ChangedItem += ChangedItem;
		PlayerEvents.ThrewProjectile += ThrewProjectile;
	}

	public override void OnDisabled()
	{
		PlayerEvents.ChangedItem -= ChangedItem;
		PlayerEvents.ThrewProjectile -= ThrewProjectile;
	}

	private void ChangedItem(PlayerChangedItemEventArgs ev)
	{
		if (ev.NewItem == null)
			return;

		if (Check(ev.NewItem.Serial))
			ev.Player.SendHint($"You are holding an {Name}", 4f);
	}

	private void ThrewProjectile(PlayerThrewProjectileEventArgs ev)
	{
		if (!Check(ev.Projectile.Serial) ||
			ev.Projectile is not TimedGrenadeProjectile timedGrenade)
			return;

		timedGrenade.RemainingTime = 1000;
		ev.Projectile.Base.gameObject.AddComponent<ImpactGrenadeHandler>().Init(ev.Player, timedGrenade);
	}

	public override Item GiveDefault(Player player, object data)
	{
		Item item = player.AddItem(ItemType.GrenadeHE)!;
		Register(item.Serial);
		return item;
	}

	private class ImpactGrenadeHandler : MonoBehaviour
	{
		private Player _owner;
		private TimedGrenadeProjectile _grenade;
		private bool _initialized;

		public void Init(Player owner, TimedGrenadeProjectile grenade)
		{
			_owner = owner;
			_grenade = grenade;
			_initialized = true;
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!_initialized ||
				collision.gameObject == _owner.GameObject ||
				collision.gameObject == _grenade.Base.gameObject)
				return;

			Timing.CallDelayed(0.4f, () => _grenade.FuseEnd());
		}
	}
}
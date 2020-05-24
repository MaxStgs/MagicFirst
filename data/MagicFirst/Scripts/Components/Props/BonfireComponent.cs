using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Unigine;

[Component(PropertyGuid = "68bd7eaa6d201e01811ddf6c838453dbd3f3777e")]
public class BonfireComponent : Component
{
	private WorldTrigger worldTrigger;

	private float Damage { get; set; }

	private float TicksPerMinute { get; set; }

	private class PlayerTimer
	{
		public HealthComponent HealthComponent { get; }
		private float Time { get; set; }

		private float TicksPerMinute { get; }

		internal delegate void DelegateTakeDamage(HealthComponent healthComponent);

		private readonly DelegateTakeDamage delegateTakeDamage;

		public PlayerTimer(HealthComponent healthComponent, float ticksPerMinute, DelegateTakeDamage delegateTakeDamage)
		{
			HealthComponent = healthComponent;
			TicksPerMinute = ticksPerMinute;
			this.delegateTakeDamage = delegateTakeDamage;
			Time = 0.0f;
			RefreshTimer();
		}

		private void RefreshTimer()
		{
			Time = 60.0f / TicksPerMinute;
		}

		public void Tick(float iFps)
		{
			if (Time <= 0.0f)
			{
				delegateTakeDamage(HealthComponent);
				RefreshTimer();
				return;
			}

			Time -= iFps;
		}
	}

	private static void TakeDamage(HealthComponent healthComponent)
	{
		Log.Message("Take damage\n");
		healthComponent.TakeDamage(10);
	}

	private readonly List<PlayerTimer> timers = new List<PlayerTimer>();
	
	private void Init()
	{
		// write here code to be called on component initialization
		InitWorldTrigger();
		LoadVariables();
	}

	private void LoadVariables()
	{
		Damage = PropLib.LoadValue(node, "BonfireProp", "Damage").Float;
		TicksPerMinute = PropLib.LoadValue(node, "BonfireProp", "TicksPerMinute").Float;
	}

	private void InitWorldTrigger()
	{
		var worldTriggerIndex = node.FindChild("WorldTrigger");
		if (worldTriggerIndex == -1) return;
		
		worldTrigger = node.GetChild(worldTriggerIndex) as WorldTrigger;
		if (worldTrigger == null) return;
		
		worldTrigger.AddEnterCallback(ObjectEntered);
		worldTrigger.AddLeaveCallback(ObjectLeaved);
	}

	private void Update()
	{
		var iFps = Game.IFps;
		foreach (var timer in timers)
		{
			timer.Tick(iFps);
		}
	}

	private void ObjectEntered(Node enteredNode)
	{
		var healthComponent = enteredNode.GetComponentInChildren<HealthComponent>();
		if (healthComponent == null)
		{
			return;
		}

		var playerTimer = new PlayerTimer(healthComponent, TicksPerMinute, TakeDamage);
		timers.Add(playerTimer);
	}

	private void ObjectLeaved(Node leavedNode)
	{
		var healthComponent = leavedNode.GetComponentInChildren<HealthComponent>();
		if (healthComponent == null)
		{
			return;
		}

		var playerTimer = timers.FirstOrDefault(timer => timer.HealthComponent == healthComponent);

		if (playerTimer == null)
		{
			return;
		}
		timers.Remove(playerTimer);
	}
}
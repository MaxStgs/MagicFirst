using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unigine;

[Component(PropertyGuid = "68bd7eaa6d201e01811ddf6c838453dbd3f3777e")]
public class BonfireComponent : Component
{
	private WorldTrigger worldTrigger;

	public float Damage { get; set; }

	public float TicksPerMinute { get; set; }

	struct PlayerTimer
	{
		public HealthComponent healthComponent { get; }
		public float time { get; set; }

		public PlayerTimer(HealthComponent healthComponent, float time)
		{
			this.healthComponent = healthComponent;
			this.time = time;
		}

		public void Tick()
		{
			if (time <= 0.0f)
			{
								
			}
		}
	}

	private List<PlayerTimer> timers = new List<PlayerTimer>();
	
	private void Init()
	{
		// write here code to be called on component initialization
		InitWorldTrigger();
		LoadVariables();
		// PropLib.LoadVariables(node, "BonfireProp", new []{"Damage", "TicksPerMinute"});
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
		// write here code to be called before updating each render frame
		timers = timers.Select(TimerTick).ToList();
	}

	private PlayerTimer TimerTick(PlayerTimer playerTimer)
	{
		if (!(playerTimer.time <= 0.0f))
			return new PlayerTimer(playerTimer.healthComponent, playerTimer.time - Game.IFps);
		
		TakeDamage(playerTimer.healthComponent);
		return new PlayerTimer(playerTimer.healthComponent, playerTimer.time = 60.0f / TicksPerMinute);

	}

	private void ObjectEntered(Node enteredNode)
	{
		var healthComponent = enteredNode.GetComponentInChildren<HealthComponent>();
		if (healthComponent == null)
		{
			return;
		}
		timers.Add(new PlayerTimer(healthComponent, 60.0f / TicksPerMinute));
	}
	
	private void ObjectLeaved(Node leavedNode)
	{
		var healthComponent = leavedNode.GetComponentInChildren<HealthComponent>();
		if (healthComponent == null)
		{
			return;
		}

		PlayerTimer? playerTimer = null;
		foreach (var timer in timers.Where(timer => timer.healthComponent == healthComponent))
		{
			playerTimer = timer;
			break;
		}

		if (playerTimer == null)
		{
			
			return;
		}
		timers.Remove((PlayerTimer)playerTimer);
	}

	private void TakeDamage(HealthComponent healthComponent)
	{
		Log.Message($"Take damage\n");
		healthComponent.TakeDamage(10);
	}
}
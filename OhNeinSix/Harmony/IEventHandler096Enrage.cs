using System;
using Smod2.EventHandlers;
using Smod2.API;

namespace OhNeinSix
{
	public interface IEventHandler096Enrage : IEventHandler
	{
		void OnSetEnrage(Player096EnrageEvent ev);
	}

	public class Player096EnrageEvent : Smod2.Events.Event
	{
		public Player Player;
		public float RageProgress;
		public bool AllowEnrage = true;
		public Object Script;
		public EnrageState EnrageState;

		public override void ExecuteHandler(IEventHandler handler)
		{
			((IEventHandler096Enrage)handler).OnSetEnrage(this);
		}
	}

	public enum EnrageState
	{
		NotEnraged,
		Panic,
		Enraged,
		Cooldown
	}
}
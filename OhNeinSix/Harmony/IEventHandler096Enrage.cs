using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		public float rageProgress;
		public bool allowEnrage = true;
		public Object Script;
		public EnrageState enrageState;

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
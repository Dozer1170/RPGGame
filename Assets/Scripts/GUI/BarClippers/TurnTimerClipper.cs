using UnityEngine;
using System.Collections;

public class TurnTimerClipper : StatusBarClipper 
{
	private CombatantTurnTime _timer;
	public CombatantTurnTime Timer
	{
		set
		{
			_timer = value;
		}
	}
	
	protected override void GetTargetPercent ()
	{
		if(_timer != null && CombatManager.Instance.InCombat)
		{
			_targetPercent = (Time.fixedTime - _timer.startTime)/_timer.waitTime;
			if(_targetPercent >= 1)
			{
				Destroy(transform.parent.gameObject);
			}
		}
		else
		{
			Destroy(transform.parent.gameObject);
		}
	}
}

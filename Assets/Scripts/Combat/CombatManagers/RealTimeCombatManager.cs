using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RealTimeCombatManager : CombatManager 
{
	private List<CombatantTurnTime> _combatantTimes = new List<CombatantTurnTime>();
	private Dictionary<int,Queue<CombatEntity>> _readyPlayerControlledEntities = new Dictionary<int,Queue<CombatEntity>>();
	
	void Awake()
	{
		_instance = this;
	}
	
	public override void StartCombat (List<CombatEntity> combatants)
	{
		if(!_inCombat)
		{
			base.StartCombat (combatants);
			_readyPlayerControlledEntities.Clear();
			_combatantTimes.Clear();
			
			for(int i = 0; i < combatants.Count; i++)
			{
				AddTimerForEntity(combatants[i]);
				_combatants.Add(i,combatants[i]);
			}
		}
	}
	
	public override void CombatantDied (CombatEntity entity)
	{
		base.CombatantDied (entity);
		
		for(int i = 0; i < _combatantTimes.Count; i++)
		{
			if(_combatantTimes[i].entity == entity)
			{
				RemoveTimer(i);
				break;
			}
		}
	}
	
	public override void CombatantRevived (CombatEntity entity)
	{
		base.CombatantRevived (entity);
		
		AddTimerForEntity(entity);
	}
	
	public override void CombatantFinishedTurn (CombatEntity entity)
	{
		base.CombatantFinishedTurn (entity);
		
		if(entity.Controller is PlayerCombatEntityController)
		{
			if(_readyPlayerControlledEntities[entity.PlayerNum].Count > 0)
			{
				_readyPlayerControlledEntities[entity.PlayerNum].Dequeue().BeginTurn();
			}
		}
		
		AddTimerForEntity(entity);
	}
	
	void Update()
	{
		if(InCombat)
		{
			for(int i = 0; i < _combatantTimes.Count; i++)
			{
				CombatEntity entity = _combatantTimes[i].entity;
				if(_combatantTimes[i].ReadyForTurn())
				{
					if(!entity.IsDead)
					{
						if(entity.Controller is AICombatEntityController)
						{
							entity.BeginTurn();
						}
						else
						{
							//Controlled by player
							if(!_readyPlayerControlledEntities.ContainsKey(entity.PlayerNum))
							{
								_readyPlayerControlledEntities[entity.PlayerNum] = new Queue<CombatEntity>();
							}
							
							bool startTurn = _readyPlayerControlledEntities[entity.PlayerNum].Count == 0;
							_readyPlayerControlledEntities[entity.PlayerNum].Enqueue(entity);
							
							if(startTurn)
							{
								_readyPlayerControlledEntities[entity.PlayerNum].Dequeue().BeginTurn();
							}
						}
					}
					RemoveTimer(i);
				}
			}
		}
	}
	
	private void AddTimerForEntity(CombatEntity entity)
	{
		CombatantTurnTime ctt = new CombatantTurnTime();
		ctt.entity = entity;
		ctt.startTime = Time.fixedTime;
		ctt.waitTime = CalculateWaitTime(entity);
		ctt.turnTimer = (GameObject) Instantiate(GameManager.Instance.TurnTimerPrefab);
		ctt.turnTimer.transform.parent = entity.TurnTimerMount;
		ctt.turnTimer.transform.localPosition = Vector3.zero;
		ctt.turnTimer.transform.rotation = Quaternion.identity;
		ctt.InitTurnTimer();
		
		_combatantTimes.Add(ctt);
	}
	
	private void RemoveTimer(int index)
	{
		_combatantTimes.RemoveAt(index);
	}
	
	private float CalculateWaitTime(CombatEntity entity)
	{
		return 3.0f;
	}
}

public class CombatantTurnTime
{
	public float startTime;
	public float waitTime;
	public CombatEntity entity;
	public GameObject turnTimer;
	
	public bool ReadyForTurn()
	{
		return Time.fixedTime >= startTime + waitTime;
	}
	
	public void InitTurnTimer()
	{
		TurnTimerClipper clipper = (TurnTimerClipper) turnTimer.GetComponentInChildren(typeof(TurnTimerClipper));
		if(clipper != null)
		{
			clipper.Timer = this;
			entity.SpriteController.TurnTimer = clipper.gameObject;
		}
	}
}

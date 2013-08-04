using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatManager : MonoBehaviour
{
	protected static CombatManager _instance;
	public static CombatManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
	protected bool _inCombat = false;
	public bool InCombat
	{
		get
		{
			return _inCombat;
		}
	}
	
	protected Dictionary<float, CombatEntity> _combatants = new Dictionary<float, CombatEntity>();
	
	public virtual void StartCombat(List<CombatEntity> combatants)
	{
		_inCombat = true;
		_combatants.Clear();
	}
	
	public virtual void CombatantFinishedTurn(CombatEntity entity)
	{
		if(GetEnemyTargets(Faction.PLAYER).Count == 0)
		{
			//Player Won
			StageManager.Instance.FinishedCombat(true);
			_inCombat = false;
			return;
		}
		else if(GetEnemyTargets(Faction.ENEMY).Count == 0)
		{
			//NPCs Won
			StageManager.Instance.FinishedCombat(false);
			_inCombat = false;
			return;
		}
	}
	
	public virtual void CombatantDied(CombatEntity entity)
	{
		AwardXpToOtherSide(entity.FactionType, entity.Stats.GetKillXPValue());
	}
	
	private void AwardXpToOtherSide(Faction faction, int xp)
	{
		//Award Xp to any enemy combatants
		List<CombatEntity> enemies = GetEnemyTargets(faction);
		foreach(CombatEntity entity in enemies)
		{
			entity.Stats.AddXP(xp);
		}
	}
	
	public virtual void CombatantRevived(CombatEntity entity)
	{
		
	}
	
	public virtual void RemoveCombatant(CombatEntity entity)
	{
		float key = -1;
		
		foreach(KeyValuePair<float,CombatEntity> kvp in _combatants)
		{
			if(kvp.Value == entity)
			{
				key = kvp.Key;
				break;
			}
		}
		
		_combatants.Remove(key);
	}
	
	public List<CombatEntity> GetEnemyTargets(Faction factionType)
	{
		List<CombatEntity> targets = new List<CombatEntity>();
		foreach(CombatEntity entity in _combatants.Values)
		{
			if(entity.FactionType != factionType && !entity.IsDead)
			{
				targets.Add(entity);
			}
		}
		
		return targets;
	}
	
	public List<CombatEntity> GetFriendlyTargets(Faction factionType)
	{
		List<CombatEntity> targets = new List<CombatEntity>();
		foreach(CombatEntity entity in _combatants.Values)
		{
			if(entity.FactionType == factionType && !entity.IsDead)
			{
				targets.Add(entity);
			}
		}
		
		return targets;
	}
}


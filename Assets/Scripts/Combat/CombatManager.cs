using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
	private static readonly float TURN_START_DELAY = 0.0f;
	private static readonly float _epsilon = 0.05f;

	private static CombatManager _instance;
	public static CombatManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
	private bool _inCombat = false;
	public bool InCombat
	{
		get
		{
			return _inCombat;
		}
	}
	
	private int _turnNumber = 1;
	public int TurnNumber
	{
		get
		{
			return _turnNumber;
		}
	}
	
	private Dictionary<float, CombatEntity> _combatants = new Dictionary<float, CombatEntity>();
	private List<float> _sortedInitiatives;
	private int _currentInitiativeIndex;
	
	void Awake()
	{
		_instance = this;
	}
	
	public void StartCombat(List<CombatEntity> combatants)
	{
		if(!_inCombat)
		{
			_combatants.Clear();
			_turnNumber = 1;
			
			foreach(CombatEntity combatant in combatants)
			{
				float initiative = combatant.RollInitiative();
				//Make sure it is a unique value
				while(_combatants.ContainsKey(initiative))
				{
					initiative -= _epsilon;
				}
				
				_combatants.Add(initiative, combatant);
			}
			
			_sortedInitiatives = new List<float>(_combatants.Keys);
			_sortedInitiatives.Sort();
			_currentInitiativeIndex = _sortedInitiatives.Count - 1;
			
			StartCoroutine(CombatantStartTurn(_currentInitiativeIndex, TURN_START_DELAY));
		}
	}
	
	private void IncreaseInitiativeIndex()
	{
		_currentInitiativeIndex--;
		
		if(_currentInitiativeIndex < 0)
		{
			_currentInitiativeIndex = _sortedInitiatives.Count - 1;
			_turnNumber++;
		}
	}
	
	private IEnumerator CombatantStartTurn(int initiativeIndex, float timeDelay)
	{
		yield return new WaitForSeconds(timeDelay);
		
		_combatants[_sortedInitiatives[_currentInitiativeIndex]].BeginTurn();
	}
	
	public void CombatantFinishedTurn()
	{
		IncreaseInitiativeIndex();
		
		if(GetEnemyTargets(Faction.PLAYER).Count == 0)
		{
			//Player Won
			StageManager.Instance.FinishedCombat(true);
			return;
		}
		else if(GetEnemyTargets(Faction.ENEMY).Count == 0)
		{
			//NPCs Won
			StageManager.Instance.FinishedCombat(false);
			_inCombat = false;
			return;
		}
		
		while(_combatants[_sortedInitiatives[_currentInitiativeIndex]].IsDead)
		{
			IncreaseInitiativeIndex();
		}
		
		StartCoroutine(CombatantStartTurn(_currentInitiativeIndex, TURN_START_DELAY));
	}
	
	public void CombatantDied(CombatEntity entity)
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
	
	public void CombatantRevived(CombatEntity entity)
	{
		
	}
	
	public void RemoveCombatant(CombatEntity entity)
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
	
	private void LogInitiativeValues()
	{
		foreach(float val in _sortedInitiatives)
		{
			Debug.Log(_combatants[val].name + " got an initiative of " + val);
		}
	}
}

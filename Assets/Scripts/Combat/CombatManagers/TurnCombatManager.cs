using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnCombatManager : CombatManager
{
	private static readonly float TURN_START_DELAY = 0.0f;
	private static readonly float _epsilon = 0.05f;
	
	private int _turnNumber = 1;
	public int TurnNumber
	{
		get
		{
			return _turnNumber;
		}
	}
	
	private List<float> _sortedInitiatives;
	private int _currentInitiativeIndex;
	
	void Awake()
	{
		_instance = this;
	}
	
	public override void StartCombat(List<CombatEntity> combatants)
	{
		if(!_inCombat)
		{
			base.StartCombat(combatants);
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
	
	public override void CombatantFinishedTurn(CombatEntity entity)
	{
		IncreaseInitiativeIndex();
		
		base.CombatantFinishedTurn(entity);
		
		while(_combatants[_sortedInitiatives[_currentInitiativeIndex]].IsDead)
		{
			IncreaseInitiativeIndex();
		}
		
		StartCoroutine(CombatantStartTurn(_currentInitiativeIndex, TURN_START_DELAY));
	}
	
	private void LogInitiativeValues()
	{
		foreach(float val in _sortedInitiatives)
		{
			Debug.Log(_combatants[val].name + " got an initiative of " + val);
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICombatEntityController : CombatEntityController 
{
	private static readonly float DECISION_DELAY = 1.00f;
	private static readonly float YIELD_TURN_DELAY = 0.33f;
	
	public override void BeginTurn ()
	{
		StartCoroutine(TakeTurn(DECISION_DELAY));
	}
	
	public override void EndTurn()
	{
		_entity.EndTurn();
	}
	
	private IEnumerator TakeTurn(float decisionDelay)
	{
		yield return new WaitForSeconds(DECISION_DELAY);
		
		if(CombatManager.Instance.InCombat && !_entity.IsDead)
		{
			Spell spell = DecideOnSpell();
			CombatEntity target = DecideOnTarget(spell.targetType);
		
			_entity.UseSpell(spell, target);
	
			yield return new WaitForSeconds(YIELD_TURN_DELAY);
			
			EndTurn();
		}
	}
	
	protected virtual Spell DecideOnSpell()
	{
		List<Spell> possibleSpells = GetPossibleSpells();
		int spellIndex = Random.Range(0, possibleSpells.Count);
		Spell spell = null;
		int index = 0;
		foreach(Spell possibleSpell in possibleSpells)
		{
			if(index == spellIndex)
			{
				spell = possibleSpell;
				break;
			}
			index++;
		}
		
		return spell;
	}
	
	protected virtual CombatEntity DecideOnTarget(TargetType spellTargetType)
	{
		List<CombatEntity> possibleTargets = null;
		CombatEntity target = null;
		if(spellTargetType == TargetType.ENEMY)
		{
			possibleTargets = CombatManager.Instance.GetEnemyTargets(_entity.FactionType);
			target = PickEnemyTarget(possibleTargets);
		}
		else if(spellTargetType == TargetType.FRIENDLY)
		{
			possibleTargets = CombatManager.Instance.GetFriendlyTargets(_entity.FactionType);
			target = PickFriendlyTarget(possibleTargets);
		}
		else if(spellTargetType == TargetType.SELF)
		{
			return _entity;
		}
		
		return target;
	}
	
	private CombatEntity PickEnemyTarget(List<CombatEntity> possibleTargets)
	{
		CombatEntity target = null;
		int targetIndex = Random.Range(0, possibleTargets.Count);
		int index = 0;
		foreach(CombatEntity possibleTarget in possibleTargets)
		{
			if(index == targetIndex)
			{
				target = possibleTarget;
				break;
			}
			index++;
		}
		
		return target;
	}
	
	private CombatEntity PickFriendlyTarget(List<CombatEntity> possibleTargets)
	{
		CombatEntity target = null;
		foreach(CombatEntity possibleTarget in possibleTargets)
		{
			if(target == null)
			{
				target = possibleTarget;
			}
			else
			{
				if(possibleTarget.CurrentHp < target.CurrentHp)
				{
					target = possibleTarget;
				}
			}
		}
		return target;
	}
	
	private List<Spell> GetPossibleSpells()
	{
		List<Spell> usableSpells = new List<Spell>();
		foreach(Spell spell in _spellBook.Values)
		{
			if(spell.secondaryCost + _entity.Stats.Level * spell.secondaryCostPerLevel 
				<= _entity.SRManager.CurrentSr)
			{
				usableSpells.Add(spell);
			}
		}
		return usableSpells;
	}
}

using UnityEngine;
using System.Collections;

public class SecondaryResourceManager
{
	protected CombatEntity _entity;
	
	protected float _currentSr;
	public float CurrentSr
	{
		get
		{
			return _currentSr;
		}
		set
		{
			_currentSr = value;
		}
	}
	
	public virtual void Init(CombatEntity entity)
	{
		_entity = entity;
	}
	
	public virtual void TookDamage(float damage, bool crit)
	{
		
	}
	
	public virtual void DealtDamage(float damage, bool crit, Spell spell)
	{
		
	}
	
	public virtual void BeginTurn()
	{
		
	}
	
	public virtual void EndTurn()
	{
		
	}
	
	public virtual void UsedSpell(float secondaryCost)
	{
		SubtractFromSR(secondaryCost);
	}
	
	public virtual void AddToSR(float amount)
	{
		if(_currentSr + amount < _entity.Stats.MaxSecondaryResource)
		{
			_currentSr += amount;
		}
		else
		{
			_currentSr = _entity.Stats.MaxSecondaryResource;
		}
	}
	
	public virtual void SubtractFromSR(float amount)
	{
		if(_currentSr - amount >= 0)
		{
			_currentSr -= amount;
		}
		else
		{
			_currentSr = 0;
		}
	}
}

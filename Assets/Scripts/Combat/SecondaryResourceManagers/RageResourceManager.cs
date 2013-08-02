using UnityEngine;
using System.Collections;

public class RageResourceManager : SecondaryResourceManager 
{
	private static readonly int[] _rageGeneratingSpells = { 1 };
	private static readonly float _hitFactor = 0.25f;
	
	public override void DealtDamage(float damage, bool crit, Spell spell)
	{
		foreach(int spellId in _rageGeneratingSpells)
		{
			if(spell.spellId == spellId)
			{
				float rageGenerated = ((15 * damage)/ CalculateLevelFactor()) * (crit ? _hitFactor * 2 : _hitFactor);
				AddToSR(rageGenerated);
				break;
			}
		}
	}
	
	public override void TookDamage(float damage, bool crit)
	{
		float rageGenerated = (5f/2f) * (damage/CalculateLevelFactor());
		AddToSR(rageGenerated);
	}
	
	private float CalculateLevelFactor()
	{
		return 0.0091f * _entity.Stats.Level * _entity.Stats.Level + 3.23f * _entity.Stats.Level + 4.27f;
	}
}

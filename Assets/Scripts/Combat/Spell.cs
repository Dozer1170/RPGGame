using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell
{
	public string name;
	public int spellId;
	public int requiredLevel;
	public EntityClass entityClass;
	public bool classRestricted;
	public TargetType targetType;
	public float secondaryCost;
	public float secondaryCostPerLevel;
	public Stat mainStat;
	public float mainStatRatio;
	public bool weaponAttack;
	public float vampPercent;
	public float healBase;
	public float healBonusPerLevel;
	public float damageBase;
	public float damageBonusPerLevel;
	public float damageOverTimeBase;
	public float damageOverTimeBonusPerLevel;
	public int dotDuration;
	public bool stackable;
	public StatMod casterMod;
	public StatMod targetMod;
	public EffectTimes healingTimes;
	public EffectTimes damageTimes;
	public string effectName;
	
	public override string ToString ()
	{
		return "Name: " + name + 
				"\nSpell ID: " + spellId + 
				"\nLevel: " + requiredLevel + 
				"\nClass: " + entityClass + 
				"\nClass Restricted: " + classRestricted +
				"\nTarget: " + targetType + 
				"\nCost: " + secondaryCost + 
				"\nCost per Level: " + secondaryCostPerLevel + 
				"\nMain Stat: " + mainStat + 
				"\nMain Stat Ratio: " + mainStatRatio + 
				"\nWeapon Attack: " + weaponAttack + 
				"\nVamp Percent: " + vampPercent +
				"\nDamage: " + damageBase +
				"\nDamage Per Level: " + damageBonusPerLevel +
				"\nHealing: " + healBase + 
				"\nHeal Bonus Per Level: " + healBonusPerLevel +
				"\nDOT: " + damageOverTimeBase + 
				"\nDOT per Level: " + damageOverTimeBonusPerLevel + 
				"\nDOT Duration: " + dotDuration + 
				"\nStackable: " + stackable +
				(casterMod != null ? "\nCaster Mod: " + casterMod.statAffected + ", Amount: " +
						casterMod.amount + ", Percentage: " + casterMod.percentage + ", Duration: " + casterMod.durationLeft : "")  +
				(targetMod != null ? "\nTarget Mod: " + targetMod.statAffected + ", Amount: " +
						targetMod.amount + ", Percentage: " + targetMod.percentage + ", Duration: " + targetMod.durationLeft : "")  +
				(healingTimes == null ? "" : healingTimes.ToString()) +
				(damageTimes == null ? "" : damageTimes.ToString()) +
				"\nEffect Name: " + effectName;
	}
}

public class EffectTimes
{
	public bool healing;
	public List<EffectTime> times = new List<EffectTime>();
	
	public void AddEffectTime(float time, float mult)
	{
		EffectTime effectTime = new EffectTime();
		effectTime.time = time;
		effectTime.mult = mult;
		times.Add(effectTime);
	}
	
	public void AddEffectTime(EffectTime time)
	{
		times.Add(time);
	}
	
	public override string ToString ()
	{
		string rval = "";
		rval += healing ? "\nHealing Times:" : "\nDamage Times:";
		foreach(EffectTime effectTime in times)
		{
			rval += "\n\tTime: " + effectTime.time + " Mult: " + effectTime.mult;
		}
		return rval;
	}
}

[System.Serializable]
public class EffectTime
{
	public float time;
	public float mult;
}

public enum TargetType
{
	FRIENDLY, ENEMY, SELF	
}

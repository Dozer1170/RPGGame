using UnityEngine;
using System.Collections;

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
						targetMod.amount + ", Percentage: " + targetMod.percentage + ", Duration: " + targetMod.durationLeft : "");
	}
}

public enum TargetType
{
	FRIENDLY, ENEMY, SELF	
}

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class SpellParser 
{
	private static readonly string _spellConfigLocation = "/SpellConfig/SpellList.txt";
	
	private static SpellParser _instance;
	public static SpellParser Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new SpellParser();
			}
			
			return _instance;
		}
	}
	
	public void ParseSpells()
	{
		Dictionary<string,Spell> spells = new Dictionary<string,Spell>();
		
		System.IO.StreamReader file = new System.IO.StreamReader(@Application.streamingAssetsPath + _spellConfigLocation);
		
		bool parsingCasterMod = false;
		bool parsingTargetMod = false;
		Spell spell = new Spell();
		StatMod mod = new StatMod();
		string line;
		while((line = file.ReadLine()) != null)
		{
			string trimmedLine = Regex.Replace(line, @"\s+", "");
			if(trimmedLine.Contains("{"))
			{
				continue;
			}
			else if(trimmedLine.Contains("}"))
			{
				Debug.Log(spell);
				spells.Add(spell.name, spell);
				spell = new Spell();
				continue;
			}
			else if(trimmedLine.Contains("["))
			{
				continue;
			}
			else if(trimmedLine.Contains("]"))
			{
				if(parsingCasterMod)
				{
					spell.casterMod = mod;
					parsingCasterMod = false;
				}
				else if(parsingTargetMod)
				{
					spell.targetMod = mod;
					parsingTargetMod = false;
				}
				mod = new StatMod();
				continue;
			}
			else if(!trimmedLine.Contains(":") && trimmedLine == "casterMod")
			{
				parsingCasterMod = true;
			}
			else if(!trimmedLine.Contains(":") && trimmedLine == "targetMod")
			{
				parsingTargetMod = true;
			}
			else if(!trimmedLine.Contains(":"))
			{
				//Its the name of the spell
				spell.name = line;
			}
			else
			{
				//It is a property
				string[] property = trimmedLine.Split(':');
				switch(property[0])
				{
					case "spellId":
						spell.spellId = int.Parse(property[1]);
					break;
					case "requiredLevel":
						spell.requiredLevel = int.Parse(property[1]);
					break;
					case "classRestricted":
						spell.classRestricted = property[1] == "true";
					break;
					case "targetType":
						spell.targetType = GetTargetTypeFromName(property[1]);
					break;
					case "entityClass":
						spell.entityClass = GetClassFromName(property[1]);
					break;
					case "secondaryCost":
						spell.secondaryCost = float.Parse(property[1]);
					break;
					case "secondaryCostPerLevel":
						spell.secondaryCostPerLevel = float.Parse(property[1]);
					break;
					case "mainStat":
						spell.mainStat = GetStatFromName(property[1]);
					break;
					case "mainStatRatio":
						spell.mainStatRatio = float.Parse(property[1]);
					break;
					case "weaponAttack":
						spell.weaponAttack = property[1] == "true";
					break;
					case "vampPercent":
						spell.vampPercent = float.Parse(property[1]);
					break;
					case "healBase":
						spell.healBase = float.Parse(property[1]);
					break;
					case "healBonusPerLevel":
						spell.healBonusPerLevel = float.Parse(property[1]);
					break;
					case "damageBase":
						spell.damageBase = float.Parse(property[1]);
					break;
					case "damageBonusPerLevel":
						spell.damageBonusPerLevel = float.Parse(property[1]);
					break;
					case "damageOverTimeBase":
						spell.damageOverTimeBase = float.Parse(property[1]);
					break;
					case "damageOverTimeBonusPerLevel":
						spell.damageOverTimeBonusPerLevel = float.Parse(property[1]);
					break;
					case "dotDuration":
						spell.dotDuration = int.Parse(property[1]);
					break;
					case "stackable":
						spell.stackable = property[1] == "true";
					break;
					case "statAffected":
						mod.statAffected = GetStatFromName(property[1]);
					break;
					case "amount":
						mod.amount = float.Parse(property[1]);
					break;
					case "percentage":
						mod.percentage = float.Parse(property[1]);
					break;
					case "duration":
						mod.durationLeft = int.Parse(property[1]);
					break;
					default:
						Debug.LogError("Could not parse " + property[0] + " property");
					break;
				}
			}
		}
		
		GameManager.Instance.SpellList = spells;
	}
	
	private EntityClass GetClassFromName(string className)
	{
		EntityClass rval = EntityClass.MAGE;
		foreach(EntityClass entityClass in System.Enum.GetValues(typeof(EntityClass)))
		{
			if(entityClass.ToString() == className)
			{
				rval = entityClass;
				break;
			}
		}
		return rval;
	}
	
	private Stat GetStatFromName(string name)
	{
		Stat rval = Stat.STRENGTH;
		foreach(Stat stat in System.Enum.GetValues(typeof(Stat)))
		{
			if(stat.ToString() == name)
			{
				rval = stat;
				break;
			}
		}
		
		return rval;
	}
	
	private TargetType GetTargetTypeFromName(string name)
	{
		TargetType rval = TargetType.ENEMY;
		foreach(TargetType targetType in System.Enum.GetValues(typeof(TargetType)))
		{
			if(targetType.ToString() == name)
			{
				rval = targetType;
				break;
			}
		}
		
		return rval;
	}
}

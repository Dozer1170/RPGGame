using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CombatStats
{	
	private static readonly float HEALTH_AND_SECONDARY_COEFFICIENT = 0.3f;
	
	private static readonly int BOSS_XP_MULT = 8;
	
	[SerializeField] private float _level = 1;
	[SerializeField] private float _baseHealth = 200;
	[SerializeField] private float _baseStrength = 30;
	[SerializeField] private float _baseDexterity = 30;
	[SerializeField] private float _baseConstitution = 30;
	[SerializeField] private float _baseIntelligence = 30;
	[SerializeField] private float _baseWisdom = 30;
	[SerializeField] private float _baseCharisma = 30;
	[SerializeField] private float _baseCriticalChance = 0.05f;
	[SerializeField] private float _baseSecondaryResource = 150f;
	[SerializeField] private bool _giveBossXP = false;
	
	private CombatEntity _entity;
	
	private int _lastLevelXp;
	private int _currentXP;
	private int _neededXpForLevel;
	
	private Dictionary<Stat,float> _currentStats = new Dictionary<Stat, float>();
	
	public float Level
	{
		get
		{
			return _currentStats[Stat.LEVEL];
		}
	}
	
	public float Strength
	{
		get
		{
			return _currentStats[Stat.STRENGTH];
		}
	}
	public float Dexterity
	{
		get
		{
			return _currentStats[Stat.DEXTERITY];
		}
	}
	public float Constitution
	{
		get
		{
			return _currentStats[Stat.CONSTITUTION];
		}
	}
	public float Intelligence
	{
		get
		{
			return _currentStats[Stat.INTELLIGENCE];
		}
	}
	public float Wisdom
	{
		get
		{
			return _currentStats[Stat.WISDOM];
		}
	}
	public float Charisma
	{
		get
		{
			return _currentStats[Stat.CHARISMA];
		}
	}
	
	public float MaxHealth
	{
		get
		{
			return _currentStats[Stat.HEALTH];
		}
	}
	
	public float MaxSecondaryResource
	{
		get
		{
			return _currentStats[Stat.SECONDARY_RESOURCE];
		}
	}
	
	public float CriticalChance
	{
		get
		{
			return _currentStats[Stat.CRITICAL_CHANCE];
		}
	}
	
	public int XPLevelProgress
	{
		get
		{
			return _currentXP - _lastLevelXp;
		}
	}
	
	public int XPDiffForNextLevel
	{
		get
		{
			return _neededXpForLevel - _lastLevelXp;
		}
	}
	
	public int XPForLevel
	{
		get
		{
			return _neededXpForLevel;
		}
	}
	
	public float GetStatValue(Stat stat)
	{
		return _currentStats[stat];
	}
	
	/// <summary>
	/// The bonuses to stats.
	/// 
	/// Store in form of spell id to StatBonus
	/// </summary>
	private Dictionary<int,List<StatMod>> _statMods = new Dictionary<int,List<StatMod>>();
	
	#region XP Methods
	private static int LEVEL_CAP = 300;
	private static Dictionary<int,int> _xpTable = new Dictionary<int, int>();
	
	public static void InitXPTable()
	{
		_xpTable[0] = 0;
		for(int i = 1; i < LEVEL_CAP; i++)
		{
			int xpToNextLevel = XPToNextLevel(i);
			_xpTable[i] = _xpTable[i - 1] + xpToNextLevel;
		}
	}
	
	public void AddXP(int xp)
	{
		_currentXP += xp;
		
		while(_currentXP >= _neededXpForLevel)
		{
			LevelUp(_entity.Class);
		}
	}
	
	public int GetKillXPValue()
	{
		int rval = GetMobXPValue(_level);
		if(_giveBossXP)
		{
			rval *= BOSS_XP_MULT;
		}
		return rval;
	}
	
	public static int GetMobXPValue(float level)
	{
		return (int) (level * 15) + 45;
	}
	
	private static int XPToNextLevel(float level)
	{
		int mobXPVal = GetMobXPValue(level);
		int xpToLevel = (int) ((level)/2.1115f) * mobXPVal;
		return xpToLevel < mobXPVal * 2 ? 2 * mobXPVal : xpToLevel;
	}
	
	public static int GetLevelXPValue(float level)
	{
		return _xpTable[(int)level];
	}
	
	public void RunXPSimulation()
	{
		string output = "";
		for(int i = 1; i < 61; i++)
		{
			int mobsKilled = 0;
			while(_level == i)
			{
				AddXP(GetKillXPValue());
				mobsKilled++;
			}
			
			output += "Level " + i + " took: " + mobsKilled + " mobs and total xp is: " + GetLevelXPValue(i) + "\n";
		}
		Debug.Log(output);
	}
	#endregion
	
	public void Init(CombatEntity entity)
	{
		_entity = entity;
		
		if(_level == 1)
		{
			_lastLevelXp = 0;
		}
		_currentXP = _lastLevelXp;
		_neededXpForLevel = GetLevelXPValue(_level);
		
		
		ResetStats();
	}
	
	public void AddStatMod(int spellId, StatMod bonus)
	{
		AddStatBonus(spellId, bonus, _statMods);
	}
	
	private void AddStatBonus(int spellId, StatMod bonus, Dictionary<int,List<StatMod>> dict)
	{
		if(!dict.ContainsKey(spellId))
		{
			List<StatMod> mods = new List<StatMod>();
			mods.Add(bonus);
			dict.Add(spellId, mods);
		}
		else
		{
			if(GameManager.Instance.GetSpellById(spellId).stackable)
			{
				dict[spellId].Add(bonus);
			}
			else
			{
				//Not stackable and already there, so remove the old one and use this one
				dict[spellId].Clear();
				dict[spellId].Add(bonus);
			}
		}
		
		ReCalculateStatMods();
	}
	
	private void ReCalculateStatMods()
	{
		ResetStats();
		
		foreach(List<StatMod> spellMods in _statMods.Values)
		{
			foreach(StatMod mod in spellMods)
			{
				CalculateStatMod(mod);
			}
		}
	}
	
	public void ResetStats()
	{
		_currentStats[Stat.LEVEL] = _level;
		_currentStats[Stat.STRENGTH] = _baseStrength;
		_currentStats[Stat.DEXTERITY] = _baseDexterity;
		_currentStats[Stat.CONSTITUTION] = _baseConstitution;
		_currentStats[Stat.INTELLIGENCE] = _baseIntelligence;
		_currentStats[Stat.WISDOM] = _baseWisdom;
		_currentStats[Stat.CHARISMA] = _baseCharisma;
		_currentStats[Stat.HEALTH] = _baseHealth;
		_currentStats[Stat.CRITICAL_CHANCE] = _baseCriticalChance;
		_currentStats[Stat.SECONDARY_RESOURCE] = _baseSecondaryResource;
	}
	
	private void CalculateStatMod(StatMod bonus)
	{
		if(bonus.amount != 0)
		{
			_currentStats[bonus.statAffected] += bonus.amount;
		}
		
		if(bonus.percentage != 0)
		{
			_currentStats[bonus.statAffected] *= bonus.percentage;
		}
	}
	
	public void LevelUp(EntityClass classType)
	{
		_level++;
		_lastLevelXp = _neededXpForLevel;
		_neededXpForLevel = GetLevelXPValue(_level + 1);
		
		LevelStrengthForClass(classType);
		LevelDexterityForClass(classType);
		LevelConstitutionForClass(classType);
		LevelIntelligenceForClass(classType);
		LevelWisdomForClass(classType);
		LevelCharismaForClass(classType);
		LevelHealthForClass(classType);
		LevelSecondaryResourceForClass(classType);
		
		ReCalculateStatMods();
		_entity.LeveledUp();
	}
	
	#region Level Stat Methods
	private void LevelHealthForClass(EntityClass classType)
	{
		_baseHealth += _level * HEALTH_AND_SECONDARY_COEFFICIENT + _baseConstitution * HEALTH_AND_SECONDARY_COEFFICIENT;
	}
	
	private void LevelSecondaryResourceForClass(EntityClass classType)
	{
		switch(classType)
		{
			case EntityClass.MAGE:
				_baseSecondaryResource += _level * HEALTH_AND_SECONDARY_COEFFICIENT + _baseIntelligence * HEALTH_AND_SECONDARY_COEFFICIENT;
			break;
			case EntityClass.SHADOW_MANCER:
				_baseSecondaryResource += _level * HEALTH_AND_SECONDARY_COEFFICIENT + _baseConstitution * HEALTH_AND_SECONDARY_COEFFICIENT;
			break;
			case EntityClass.LIGHT_WEAVER:
				_baseSecondaryResource += _level * HEALTH_AND_SECONDARY_COEFFICIENT + _baseCharisma * HEALTH_AND_SECONDARY_COEFFICIENT;
			break;
		}
	}
	
	private void LevelStrengthForClass(EntityClass classType)
	{
		switch(classType)
		{
			case EntityClass.MAGE:
				_baseStrength += 4 + _level/3;
			break;
			case EntityClass.ROGUE:
				_baseStrength += 6 + _level/3;
			break;
			case EntityClass.SHADOW_MANCER:
				_baseStrength += 4 + _level/3;
			break;
			case EntityClass.LIGHT_WEAVER:
				_baseStrength += 4 + _level/3;
			break;
			case EntityClass.WARRIOR:
				_baseStrength += 7 + _level/3;
			break;
		}
	}
	
		
	private void LevelDexterityForClass(EntityClass classType)
	{
		switch(classType)
		{
			case EntityClass.MAGE:
				_baseDexterity += 4 + _level/3;
			break;
			case EntityClass.ROGUE:
				_baseDexterity += 7 + _level/3;
			break;
			case EntityClass.SHADOW_MANCER:
				_baseDexterity += 4 + _level/3;
			break;
			case EntityClass.LIGHT_WEAVER:
				_baseDexterity += 4 + _level/3;
			break;
			case EntityClass.WARRIOR:
				_baseDexterity += 5 + _level/3;
			break;
		}
	}
	
		
	private void LevelConstitutionForClass(EntityClass classType)
	{
		switch(classType)
		{
			case EntityClass.MAGE:
				_baseConstitution += 4 + _level/3;
			break;
			case EntityClass.ROGUE:
				_baseConstitution += 4 + _level/3;
			break;
			case EntityClass.SHADOW_MANCER:
				_baseConstitution += 5 + _level/3;
			break;
			case EntityClass.LIGHT_WEAVER:
				_baseConstitution += 4 + _level/3;
			break;
			case EntityClass.WARRIOR:
				_baseConstitution += 7 + _level/3;
			break;
		}
	}
	
		
	private void LevelIntelligenceForClass(EntityClass classType)
	{
		switch(classType)
		{
			case EntityClass.MAGE:
				_baseIntelligence += 7 + _level/3;
			break;
			case EntityClass.ROGUE:
				_baseIntelligence += 4 + _level/3;
			break;
			case EntityClass.SHADOW_MANCER:
				_baseIntelligence += 6 + _level/3;
			break;
			case EntityClass.LIGHT_WEAVER:
				_baseIntelligence += 6 + _level/3;
			break;
			case EntityClass.WARRIOR:
				_baseIntelligence += 4 + _level/3;
			break;
		}
	}
	
		
	private void LevelWisdomForClass(EntityClass classType)
	{
		switch(classType)
		{
			case EntityClass.MAGE:
				_baseWisdom += 7 + _level/3;
			break;
			case EntityClass.ROGUE:
				_baseWisdom += 4 + _level/3;
			break;
			case EntityClass.SHADOW_MANCER:
				_baseWisdom += 6 + _level/3;
			break;
			case EntityClass.LIGHT_WEAVER:
				_baseWisdom += 6 + _level/3;
			break;
			case EntityClass.WARRIOR:
				_baseWisdom += 4 + _level/3;
			break;
		}
	}
	
		
	private void LevelCharismaForClass(EntityClass classType)
	{
		switch(classType)
		{
			case EntityClass.MAGE:
				_baseCharisma += 4 + _level/3;
			break;
			case EntityClass.ROGUE:
				_baseCharisma += 4 + _level/3;
			break;
			case EntityClass.SHADOW_MANCER:
				_baseCharisma += 6 + _level/3;
			break;
			case EntityClass.LIGHT_WEAVER:
				_baseCharisma += 7 + _level/3;
			break;
			case EntityClass.WARRIOR:
				_baseCharisma += 4 + _level/3;
			break;
		}
	}
	#endregion
	
	public void LogStats()
	{
		string statString = "";
		foreach(KeyValuePair<Stat,float> stat in _currentStats)
		{
			statString += stat.Key + ": " + stat.Value + "\n";
		}
		Debug.Log(statString);
	}
}

public class StatMod
{
	public Stat statAffected;
	public float amount;
	public float percentage;
	public int durationLeft;
};

public enum Stat
{
	LEVEL, STRENGTH, DEXTERITY, CONSTITUTION, INTELLIGENCE, WISDOM, CHARISMA, DAMAGE_BONUS, ATTACK_BONUS, CRITICAL_CHANCE, HEALTH, SECONDARY_RESOURCE
};

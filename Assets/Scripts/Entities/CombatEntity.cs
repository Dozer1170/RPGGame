using UnityEngine;
using System.Collections;

public class CombatEntity : Entity
{
	[SerializeField] private CombatStats _stats;
	[SerializeField] private Faction _faction;
	[SerializeField] private EntityClass _class;
	[SerializeField] private CombatEntityController _entityController;
	[SerializeField] private EntitySpriteController _spriteController;
	[SerializeField] private Transform _combatTextMount;
	[SerializeField] private Transform _statusBarMount;
	[SerializeField] private Transform _turnTimerMount;
	
	private static readonly float _healVariability = 0.10f;
	
	public int PlayerNum
	{
		get; set;
	}
	
	public Transform TargetMount
	{
		get
		{
			return _combatTextMount;
		}
	}
	
	public Transform StatusBarMount
	{
		get
		{
			return _statusBarMount;
		}
	}
	
	public Transform TurnTimerMount
	{
		get
		{
			return _turnTimerMount;
		}
	}
	
	private float _currentHp;
	public float CurrentHp
	{
		get
		{
			return _currentHp;
		}
	}
	
	private SecondaryResourceManager _srManager;
	public SecondaryResourceManager SRManager
	{
		get
		{
			return _srManager;
		}
	}
	
	public float HealthPercent
	{
		get
		{
			return _currentHp / _stats.MaxHealth;
		}
	}
	
	public float SecondaryPercent
	{
		get
		{
			return _srManager.CurrentSr / _stats.MaxSecondaryResource;
		}
	}
	
	public Faction FactionType
	{
		get
		{
			return _faction;
		}
	}
	
	public EntityClass Class
	{
		get
		{
			return _class;
		}
	}
	
	public CombatEntityController Controller
	{
		get
		{
			return _entityController;
		}
	}
	
	public EntitySpriteController SpriteController
	{
		get
		{
			return _spriteController;
		}
	}
	
	public CombatStats Stats
	{
		get
		{
			return _stats;
		}
	}
	
	public bool IsDead
	{
		get
		{
			return _currentHp <= 0;
		}
	}
	
	void Awake()
	{
		_stats.Init(this);
		_currentHp = _stats.MaxHealth;
		InitSRManager();
	}
	
	private void InitSRManager()
	{
		switch(_class)
		{
			case EntityClass.LIGHT_WEAVER:
			case EntityClass.MAGE:
			case EntityClass.SHADOW_MANCER:
				_srManager = new ManaResourceManager();			
			break;
			case EntityClass.WARRIOR:
				_srManager = new RageResourceManager();
			break;
			case EntityClass.ROGUE:
			
			break;
		}
		
		_srManager.Init(this);
	}
	
	public float RollInitiative()
	{
		return DiceSimulator.Instance.RollD20() + _stats.Dexterity/2;
	}
	
	private bool RollForCritical(float chance)
	{
		return Random.Range(0f, 100f) < chance * 100;
	}
	
	private float RollWeaponDamage()
	{
		return Random.Range(28f,38f);
	}
	
	private float RollForHealVariability()
	{
		return 1f + Random.Range(-_healVariability,_healVariability);
	}
	
	//Called when combat manager tells us it is our turn
	public void BeginTurn()
	{
		_srManager.BeginTurn();
		_spriteController.BeginTurn();
		_entityController.BeginTurn();
	}
	
	public void EndTurn()
	{
		CombatManager.Instance.CombatantFinishedTurn(this);
		_spriteController.EndTurn();
		_srManager.EndTurn();
	}
	
	public void UseSpell(Spell spell, CombatEntity target)
	{
		if(target != null)
		{
			_srManager.UsedSpell(spell.secondaryCost + _stats.Level * spell.secondaryCostPerLevel);
			
			//TODO Hit roll calculation
			HandleDamageFromSpell(spell, target);
			HandleHealingFromSpell(spell, target);
			HandleBuffsAndDebuffsFromSpell(spell, target);
		}
	}
	
	private void HandleDamageFromSpell(Spell spell, CombatEntity target)
	{
		float damage = 0;
		if(spell.weaponAttack)
		{
			//TODO actually fill in weapon type in field
			damage = RollWeaponDamage();
		}
		else if(spell.damageBase > 0)
		{
			damage = spell.damageBase;
		}
		else if(spell.damageOverTimeBase == 0)
		{
			//Has no damage, and is not a DOT so we dont need to do anything
			return;
		}
		
		if(damage != 0)
		{
			damage += spell.damageBonusPerLevel * _stats.Level + 
				(_stats.GetStatValue(spell.mainStat) * spell.mainStatRatio);
		}
		//Check for critical hit
		bool critical = RollForCritical(_stats.CriticalChance);
		if(critical)
		{
			damage *= 2;
		}
		
		//TODO manage damage over time effects
		
		float actualDamageDealt = target.TakeDamage(damage, critical);
		_srManager.DealtDamage(actualDamageDealt, critical, spell);
		
		if(spell.vampPercent > 0)
		{
			bool vampHealCrit = RollForCritical(_stats.CriticalChance);
			float vampAmount = vampHealCrit ? damage * spell.vampPercent * 2 : damage * spell.vampPercent;
			HealDamage(vampAmount, vampHealCrit);
		}
	}
	
	private void HandleHealingFromSpell(Spell spell, CombatEntity target)
	{
		if(spell.healBase > 0)
		{
			float healAmount = (spell.healBase + spell.healBonusPerLevel * _stats.Level +
				(_stats.GetStatValue(spell.mainStat) * spell.mainStatRatio)) * RollForHealVariability();
			
			bool critical = RollForCritical(_stats.CriticalChance);
			if(critical)
			{
				healAmount *= 2;
			}
			
			target.HealDamage(healAmount, critical);
		}
	}
	
	private void HandleBuffsAndDebuffsFromSpell(Spell spell, CombatEntity target)
	{
		if(spell.casterMod != null)
		{
			if(spell.casterMod.percentage != 0 || spell.casterMod.amount != 0)
			{
				_stats.AddStatMod(spell.spellId, spell.casterMod);
			}
		}
		
		if(spell.targetMod != null)
		{
			if(spell.targetMod.percentage != 0 || spell.casterMod.amount != 0)
			{
				target.Stats.AddStatMod(spell.spellId, spell.targetMod);
			}
		}
	}
	
	public float TakeDamage(float damage, bool critical)
	{
		_currentHp -= damage;
		CombatTextManager.Instance.CreateCombatTextAtPosition(_combatTextMount.position, ((int)damage).ToString(), critical, false);
		
		if(_currentHp <= 0)
		{
			OnDeath();
		}
		else
		{
			_spriteController.EntityWasHit();
			_srManager.TookDamage(damage, critical);
		}
		
		return damage;
	}
	
	public void HealDamage(float healAmount, bool critical)
	{
		_currentHp += healAmount;
		_currentHp = _currentHp > _stats.MaxHealth ? _stats.MaxHealth : _currentHp;
		CombatTextManager.Instance.CreateCombatTextAtPosition(_combatTextMount.position, ((int)healAmount).ToString(), critical, true);
		
		_spriteController.EntityWasHealed();
	}
	
	public void LeveledUp()
	{
		_currentHp = _stats.MaxHealth;
		_srManager.CurrentSr = _stats.MaxSecondaryResource;
	}
	
	protected virtual void OnDeath()
	{
		CombatManager.Instance.CombatantDied(this);
		_spriteController.EntityDied();
	}
}

public enum Faction
{
	PLAYER, ENEMY
};

public enum EntityClass
{
	MAGE, LIGHT_WEAVER ,SHADOW_MANCER, WARRIOR, ROGUE
};
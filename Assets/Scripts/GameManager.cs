using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	[SerializeField] private GameObject _playerCharacterPrefab;
	[SerializeField] private GameObject _mageBars;
	[SerializeField] private GameObject _rogueBars;
	[SerializeField] private GameObject _shadowMancerBars;
	[SerializeField] private GameObject _lightWeaverBars;
	[SerializeField] private GameObject _warriorBars;
	
	private static GameManager _instance;
	public static GameManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
	private CombatEntity _player;
	public CombatEntity Player
	{
		get
		{
			return _player;
		}
	}
	
	public Dictionary<string,Spell> SpellList
	{
		get; set;
	}
	
	void Awake()
	{
		_instance = this;
		SpellParser.Instance.ParseSpells();
		CombatStats.InitXPTable();
		
		GameObject playerObject = (GameObject) Instantiate(_playerCharacterPrefab);
		_player = (CombatEntity) playerObject.GetComponent(typeof(CombatEntity));
		_player.Controller.InitSpellBook();
	}
	
	public GameObject GetStatusBarPrefabForClass(EntityClass entityClass)
	{
		GameObject rval = null;
		switch(entityClass)
		{
			case EntityClass.MAGE:
				rval = _mageBars;
				break;
			case EntityClass.ROGUE:
				rval = _rogueBars;
				break;
			case EntityClass.SHADOW_MANCER:
				rval = _shadowMancerBars;
				break;
			case EntityClass.LIGHT_WEAVER:
				rval = _lightWeaverBars;
				break;
			case EntityClass.WARRIOR:
				rval = _warriorBars;
				break;
		}
		return rval;
	}
	
	public Spell GetSpellByName(string name)
	{
		return SpellList[name];
	}
	
	public Spell GetSpellById(int id)
	{
		Spell rval = null;
		foreach(Spell spell in SpellList.Values)
		{
			if(spell.spellId == id)
			{
				rval = spell;
				break;
			}
		}
		
		return rval;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class CombatEntityController : MonoBehaviour 
{
	[SerializeField] private List<string> _spells;
	
	protected Dictionary<string,Spell> _spellBook = new Dictionary<string,Spell>();
	public Dictionary<string,Spell> SpellBook
	{
		get
		{
			return _spellBook;
		}
	}
	
	protected CombatEntity _entity;
	
	public abstract void BeginTurn();
	
	public abstract void EndTurn();
	
	protected virtual void Awake()
	{
		_entity = (CombatEntity) GetComponent(typeof(CombatEntity));
	}
	
	protected virtual void Start()
	{
		
	}
	
	//Called at beginning of combat and after spell parser has built the spell list
	public void InitSpellBook()
	{
		foreach(string spellName in _spells)
		{
			Spell spell = GameManager.Instance.GetSpellByName(spellName);
			_spellBook.Add(spell.name, spell);
		}
	}
}

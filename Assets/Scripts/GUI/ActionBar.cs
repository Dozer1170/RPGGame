using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionBar : MonoBehaviour 
{
	private static readonly string _spellIconResourceDirectory = "GUIPrefabs/SpellIcons/";
	
	[SerializeField] private GameObject _actionSlotPrefab;
	[SerializeField] private float _buttonXOffset;
	
	private Vector3 _startingPosition;
	
	private Dictionary<int,GameObject> _loadedIcons = new Dictionary<int, GameObject>();
	private List<ActionButton> _actionBar = new List<ActionButton>();
	private int _selectedAction;
	
	private Dictionary<CombatEntity,int> _lastSelectionMap = new Dictionary<CombatEntity, int>();
	private CombatEntity _currentEntity;
	
	private static ActionBar _instance;
	public static ActionBar Instance
	{
		get
		{
			return _instance;
		}
	}
	
	void Awake()
	{
		_instance = this;
		_startingPosition = transform.localPosition;
	}
	
	public void LoadSpellIconsIntoMemory(Dictionary<string,Spell> spells)
	{
		foreach(Spell spell in spells.Values)
		{
			_loadedIcons.Add(spell.spellId, (GameObject) Resources.Load(_spellIconResourceDirectory + spell.name));
		}
	}
	
	public void SetupActionBarForEntity(CombatEntity entity, Dictionary<string,Spell> spells)
	{
		_selectedAction = 0;
		_currentEntity = entity;
		CleanBar();
		transform.localPosition = _startingPosition;
		foreach(Spell spell in spells.Values)
		{
			CreateActionButton(spell);
		}
		
		HighlightLastSelection();
	}
	
	private void CleanBar()
	{
		foreach(ActionButton button in _actionBar)
		{
			Destroy(button.gameObject);
		}
		
		_actionBar.Clear();
	}
	
	private void CreateActionButton(Spell spell)
	{
		GameObject slot = (GameObject) Instantiate(_actionSlotPrefab);
		slot.transform.parent = transform;
		slot.transform.localPosition = new Vector3(_buttonXOffset * _actionBar.Count, 0, 0);
		slot.transform.localRotation = Quaternion.identity;
		ActionButton button = (ActionButton) slot.GetComponent(typeof(ActionButton));
		button.SpellId = spell.spellId;
		button.SpellName = spell.name;
		button.CreateIcon(_loadedIcons[spell.spellId]);
		button.UnSelected();
		
		if(_actionBar.Count > 0)
		{
			transform.localPosition = new Vector3(transform.localPosition.x - _buttonXOffset/2,
				transform.localPosition.y, transform.localPosition.z);
		}
		else
		{
			//Select the first one
			button.Selected();
		}
		
		_actionBar.Add(button);
	}
	
	public void HighlightActionButton(int index)
	{
		for(int i = 0; i < _actionBar.Count; i++)
		{
			if(index == i)
			{
				_actionBar[i].Selected();
			}
			else
			{
				_actionBar[i].UnSelected();
			}
		}
	}
	
	public void HighlightLastSelection()
	{
		if(_lastSelectionMap.ContainsKey(_currentEntity))
		{
			_selectedAction = _lastSelectionMap[_currentEntity];
			HighlightActionButton(_selectedAction);
		}
	}
	
	public void HighlightNextActionButton()
	{
		if(_selectedAction + 1 < _actionBar.Count)
		{
			_actionBar[_selectedAction].UnSelected();
			_selectedAction++;
			_actionBar[_selectedAction].Selected();
		}
	}
	
	public void HighlightPrevActionButton()
	{
		if(_selectedAction - 1 >= 0)
		{
			_actionBar[_selectedAction].UnSelected();
			_selectedAction--;
			_actionBar[_selectedAction].Selected();
		}
	}
	
	public void UnHighlightAll()
	{
		foreach(ActionButton button in _actionBar)
		{
			button.UnSelected();
		}
	}
	
	public Spell ChooseSpell()
	{
		_lastSelectionMap[_currentEntity] = _selectedAction;
		return GameManager.Instance.GetSpellByName(_actionBar[_selectedAction].SpellName);
	}
}

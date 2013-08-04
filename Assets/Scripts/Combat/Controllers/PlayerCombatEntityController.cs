using UnityEngine;
using System.Collections;

public class PlayerCombatEntityController : CombatEntityController
{
	private bool _myTurn = false;
	private Spell _selectedSpell = null;
	
	protected override void Start ()
	{
		base.Start ();
		ActionBar.Instance.LoadSpellIconsIntoMemory(_spellBook);
	}
	
	public override void BeginTurn ()
	{
		_myTurn = true;
		ActionBar.Instance.SetupActionBarForEntity(_entity, _spellBook);
		TargetManager.Instance.SetupForEntity(_entity);
	}
	
	public override void EndTurn()
	{
		_myTurn = false;
		ActionBar.Instance.UnHighlightAll();
		_entity.EndTurn();
	}
	
	void Update()	
	{
		if(_myTurn)
		{
			if(_selectedSpell == null)
			{
				//Need to select spell
				if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
				{
					ActionBar.Instance.HighlightPrevActionButton();
				}
				 
				if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
				{
					ActionBar.Instance.HighlightNextActionButton();
				}
				
				if(Input.GetKeyDown(KeyCode.Return))
				{
					SelectSpell();
				}
			}
			else
			{
				//Need to select target
				if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
				{
					TargetManager.Instance.PrevTarget(_selectedSpell.targetType == TargetType.ENEMY);
				}
				 
				if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
				{
					TargetManager.Instance.NextTarget(_selectedSpell.targetType == TargetType.ENEMY);
				}
				
				if(Input.GetKeyDown(KeyCode.Return))
				{
					SelectTarget(TargetManager.Instance.ChooseTarget(_selectedSpell.targetType == TargetType.ENEMY));
				}
				else if(Input.GetKeyDown(KeyCode.Escape))
				{
					_selectedSpell = null;
					TargetManager.Instance.HideTargetMarker();
				}
			}
		}
	}
	
	private void SelectSpell()
	{
		_selectedSpell = ActionBar.Instance.ChooseSpell();
		if(_selectedSpell.secondaryCost + _entity.Stats.Level * _selectedSpell.secondaryCostPerLevel <= _entity.SRManager.CurrentSr)
		{
			TargetManager.Instance.ShowFirstTarget(_selectedSpell.targetType == TargetType.ENEMY);
		}
		else
		{
			_selectedSpell = null;
		}
	}
	
	private void SelectTarget(CombatEntity entity)
	{
		if(CombatManager.Instance.InCombat && !_entity.IsDead)
		{
			_entity.UseSpell(_selectedSpell, entity);
			_selectedSpell = null;
			EndTurn();
		}
	}
}

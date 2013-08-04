using UnityEngine;
using System.Collections;

public class StatusBarClipper : MonoBehaviour 
{
	private static readonly float percentPerSecond = 0.5f;
	
	[SerializeField] private StatusBarType _type;
	[SerializeField] private bool _showText;
	
	private CombatEntity _entity;
	public CombatEntity Entity
	{
		get
		{
			return _entity;
		}
		set
		{
			_entity = value;
			
			if(_showText)
			{
				//Update text with values since we just got our entity
				SetTextDisplay();
			}
		}
	}
	
	protected float _currentPercent = 1f;
	protected float _targetPercent = 1f;
	protected float _lastTargetChangeTime;
	protected Rect _currentRect = new Rect(0,0,1,1);
	protected tk2dClippedSprite _sprite;
	protected BarTextManager _text;
	
	void Awake()
	{
		_sprite = (tk2dClippedSprite) GetComponent(typeof(tk2dClippedSprite));
		_text = (BarTextManager) GetComponentInChildren(typeof(BarTextManager));
		_currentPercent = _sprite.ClipRect.width;
	}
	
	protected virtual void Update()
	{
		GetTargetPercent();
		
		if(_currentPercent != _targetPercent && _sprite != null)
		{
			_currentPercent = Mathf.Lerp(_currentPercent, _targetPercent, (Time.fixedTime - _lastTargetChangeTime) * percentPerSecond);
			_currentRect.width = _currentPercent;
			_sprite.ClipRect = _currentRect;
			
			if(_showText)
			{
				SetTextDisplay();
			}
		}
	}
	
	protected virtual void SetTextDisplay()
	{
		if(_type == StatusBarType.HEALTH)
		{
			_text.CurrentStatText = ((int)_entity.CurrentHp).ToString();
			_text.TotalStat = ((int)_entity.Stats.MaxHealth).ToString();
		}
		else if(_type == StatusBarType.SECONDARY)
		{
			_text.CurrentStatText = ((int)_entity.SRManager.CurrentSr).ToString();
			_text.TotalStat = ((int)_entity.Stats.MaxSecondaryResource).ToString();
		}
	}
	
	protected virtual void GetTargetPercent()
	{
		if(_entity != null)
		{
			if(_type == StatusBarType.HEALTH)
			{
				UpdateTargetPercent(_entity.HealthPercent);
			}
			else if(_type == StatusBarType.SECONDARY)
			{
				UpdateTargetPercent(_entity.SecondaryPercent);
			}
		}
	}
	
	protected void UpdateTargetPercent(float percent)
	{
		if(_targetPercent != percent)
		{
			_lastTargetChangeTime = Time.fixedTime;
			_targetPercent = percent;
		}
	}
	
	protected void SetPercentHard(float percent)
	{
		_currentPercent = 0f;
		_targetPercent = 0f;
		_currentRect.width = _currentPercent;
		_sprite.ClipRect = _currentRect;
	}
}

public enum StatusBarType
{
	HEALTH, SECONDARY
}
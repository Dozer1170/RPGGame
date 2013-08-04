using UnityEngine;
using System.Collections;

public class XpBarManager : StatusBarClipper 
{
	private bool _isMovingToNextLevel;
	private int _xpDiffToLevel;
	private CombatStats _stats;
	
	void Start()
	{
		_stats = GameManager.Instance.Player.Stats;
		_xpDiffToLevel = _stats.XPDiffForNextLevel;
		
		_text.CurrentStatText = _stats.XPLevelProgress.ToString();
		_text.TotalStat = _xpDiffToLevel.ToString();
	}
	
	protected override void SetTextDisplay ()
	{
		_text.CurrentStatText = Mathf.RoundToInt(_currentPercent * _xpDiffToLevel).ToString();
		_text.TotalStat = _xpDiffToLevel.ToString();
	}
	
	protected override void GetTargetPercent ()
	{
		if(_stats.XPDiffForNextLevel != _xpDiffToLevel)
		{
			//Leveled up
			if(!_isMovingToNextLevel)
			{
				_isMovingToNextLevel = true;
				StartCoroutine(MoveUpToNextLevel());
			}
		}
		else
		{
			UpdateTargetPercent(((float)_stats.XPLevelProgress) / ((float)_xpDiffToLevel));
		}
	}
			
	private IEnumerator MoveUpToNextLevel()
	{
		_lastTargetChangeTime = Time.fixedTime;
		_targetPercent = 1f;
		
		while(_currentPercent != _targetPercent)
		{
			yield return null;
		}
		
		_xpDiffToLevel = _stats.XPDiffForNextLevel;
		_text.CurrentStatText = _stats.XPLevelProgress.ToString();
		_text.TotalStat = _xpDiffToLevel.ToString();
		_isMovingToNextLevel = false;
		SetPercentHard(0f);
	}
}

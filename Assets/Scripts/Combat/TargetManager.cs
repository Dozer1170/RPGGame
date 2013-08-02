using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetManager : MonoBehaviour
{
	[SerializeField] private GameObject _targetMarkerPrefab;
	
	private Vector3 _markerOffset = new Vector3(0,0,-5);
	
	private List<CombatEntity> _enemyTargets;
	private List<CombatEntity> _friendlyTargets;
	
	private int _enemyTargetIndex = 0;
	private int _friendlyTargetIndex = 0;
	
	private static TargetManager _instance;
	public static TargetManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
	private GameObject _targetMarker;
	
	void Awake()
	{
		_instance = this;
		
		_targetMarker = (GameObject) Instantiate(_targetMarkerPrefab);
		_targetMarker.SetActive(false);
	}
	
	public void SetupForEntity(CombatEntity _entity)
	{
		_enemyTargets = CombatManager.Instance.GetEnemyTargets(_entity.FactionType);
		_friendlyTargets = CombatManager.Instance.GetFriendlyTargets(_entity.FactionType);
		
		_enemyTargetIndex = 0;
		_friendlyTargetIndex = 0;
	}
	
	public void ShowFirstTarget(bool enemyTarget)
	{
		if(enemyTarget)
		{
			MoveTarget(_enemyTargets[0]);
		}
		else
		{
			MoveTarget(_friendlyTargets[0]);
		}
	}
	
	public void HideTargetMarker()
	{
		_targetMarker.SetActive(false);
	}
	
	public void NextTarget(bool enemyTarget)
	{
		if(enemyTarget)
		{
			if(_enemyTargetIndex + 1 < _enemyTargets.Count)
			{
				_enemyTargetIndex++;
			}
			else
			{
				_enemyTargetIndex = 0;
			}
			MoveTarget(_enemyTargets[_enemyTargetIndex]);
		}
		else if(!enemyTarget)
		{
			if(_friendlyTargetIndex + 1 < _friendlyTargets.Count)
			{
				_friendlyTargetIndex++;
			}
			else
			{
				_friendlyTargetIndex = 0;
			}
			MoveTarget(_friendlyTargets[_friendlyTargetIndex]);
		}
	}
	
	public void PrevTarget(bool enemyTarget)
	{
		if(enemyTarget)
		{
			if(_enemyTargetIndex - 1 >= 0)
			{
				_enemyTargetIndex--;
			}
			else
			{
				_enemyTargetIndex = _enemyTargets.Count - 1;
			}
			MoveTarget(_enemyTargets[_enemyTargetIndex]);
		}
		else if(!enemyTarget && _friendlyTargetIndex - 1 >= 0)
		{
			if(_enemyTargetIndex - 1 >= 0)
			{
				_friendlyTargetIndex--;
			}
			else
			{
				_friendlyTargetIndex = _friendlyTargets.Count - 1;
			}
			MoveTarget(_friendlyTargets[_friendlyTargetIndex]);
		}
	}
	
	private void MoveTarget(CombatEntity _entity)
	{
		_targetMarker.SetActive(true);
		_targetMarker.transform.position = _entity.TargetMount.position + _markerOffset;
	}
	
	public CombatEntity ChooseTarget(bool enemyTarget)
	{
		_targetMarker.SetActive(false);
		return enemyTarget ? _enemyTargets[_enemyTargetIndex] : _friendlyTargets[_friendlyTargetIndex];
	}
}

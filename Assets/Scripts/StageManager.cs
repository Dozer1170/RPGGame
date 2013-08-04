using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
	[SerializeField] private bool _makeInfinite = false;
	[SerializeField] private bool _scaleToPlayerLevel = false;
	[SerializeField] private Encounter[] _encounters;
	[SerializeField] private float _delayBetweenEncounters = 4f;
	[SerializeField] private Transform[] _leftCharacterMounts;
	[SerializeField] private Transform[] _rightCharacterMounts;
	
	private int _leftMountIndex = 0, _rightMountIndex = 0;
	
	private static StageManager _instance;
	public static StageManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
	private int _fightProgress = 0;
	
	void Awake()
	{
		_instance = this;
	}
	
	// Use this for initialization
	void Start ()
	{
		MountToSide(GameManager.Instance.Player, _leftCharacterMounts, ref _leftMountIndex);
		NextFight();
	}
	
	public void FinishedCombat(bool playerWon)
	{
		if(playerWon)
		{
			_fightProgress++;
			
			if(_fightProgress < _encounters.Length)
			{
				StartCoroutine(StartNextFight());
			}
			else
			{
				if(_makeInfinite)
				{
					_fightProgress = 0;
					StartCoroutine(StartNextFight());
				}
				else
				{
					Debug.Log("Congrats you won the stage!");
				}
			}
		}
		else
		{
			Debug.Log("Game Over");
		}
	}
	
	private IEnumerator StartNextFight()
	{
		yield return new WaitForSeconds(_delayBetweenEncounters);
		
		ClearMounts(_rightCharacterMounts);
		NextFight();
	}
	
	private void NextFight()
	{
		_rightMountIndex = 0;
		
		List<CombatEntity> combatants = new List<CombatEntity>();
		foreach(GameObject entity in _encounters[_fightProgress].Combatants)
		{
			GameObject entityObj = (GameObject) Instantiate(entity);
			CombatEntity combatEntity = (CombatEntity)entityObj.GetComponent(typeof(CombatEntity));
			combatEntity.Controller.InitSpellBook();
			combatEntity.SpriteController.FlipSprite();
			combatants.Add(combatEntity);
			
			if(_scaleToPlayerLevel)
			{
				combatEntity.Stats.AddXP(CombatStats.GetLevelXPValue(GameManager.Instance.Player.Stats.Level - 1));
			}
			
			MountToSide(combatEntity, _rightCharacterMounts, ref _rightMountIndex);
		}
		
		//TODO Add player's team
		combatants.Add(GameManager.Instance.Player);
		
		CombatManager.Instance.StartCombat(combatants);
	}
	
	private void MountToSide(CombatEntity entity, Transform[] characterMounts, ref int mountIndex)
	{
		entity.transform.parent = characterMounts[mountIndex++];
		entity.transform.localPosition = Vector3.zero;
		entity.transform.localRotation = Quaternion.identity;
		entity.transform.localScale = Vector3.one;
		GameObject bars = (GameObject) Instantiate(GameManager.Instance.GetStatusBarPrefabForClass(entity.Class));
		bars.transform.parent = entity.StatusBarMount;
		bars.transform.localPosition = Vector3.zero;
		bars.transform.rotation = Quaternion.identity;
		bars.transform.localScale = Vector3.one;
		
		StatusBarClipper[] clippers = bars.GetComponentsInChildren<StatusBarClipper>();
		foreach(StatusBarClipper clipper in clippers)
		{
			clipper.Entity = entity;
		}
		
		entity.SpriteController.StatusBars = bars;
	}
	
	private void ClearMounts(Transform[] mounts)
	{
		foreach(Transform trans in mounts)
		{
			for(int i = 0; i < trans.GetChildCount(); i++)
			{
				Destroy(trans.GetChild(i).gameObject);
			}
		}
	}
}

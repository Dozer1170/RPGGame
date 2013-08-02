using UnityEngine;
using System.Collections;

[System.Serializable]
public class Encounter 
{
	[SerializeField] private GameObject[] _combatants;
	public GameObject[] Combatants
	{
		get
		{
			return _combatants;
		}
	}
}

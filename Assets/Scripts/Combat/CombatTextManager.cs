using UnityEngine;
using System.Collections;

public class CombatTextManager : MonoBehaviour 
{
	[SerializeField] private GameObject _combatText;
	[SerializeField] private GameObject _critCombatText;
	
	private float _layerOffset = -5f;
	
	private static CombatTextManager _instance;
	public static CombatTextManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
	void Awake()
	{
		_instance = this;
	}
	
	public void CreateCombatTextAtPosition(Vector3 position, string text, bool critical, bool heal)
	{
		GameObject textObject = null;
		position.z += _layerOffset;
		if(critical)
		{
			textObject = (GameObject) Instantiate(_critCombatText, position, Quaternion.identity);
		}
		else
		{
			textObject = (GameObject) Instantiate(_combatText, position, Quaternion.identity);
		}
		
		tk2dTextMesh mesh = (tk2dTextMesh) textObject.GetComponent(typeof(tk2dTextMesh));
		mesh.text = text;
		
		if(heal)
		{
			mesh.color = Color.green;
		}
		
		mesh.Commit();
	}
}

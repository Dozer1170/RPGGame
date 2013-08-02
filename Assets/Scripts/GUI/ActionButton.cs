using UnityEngine;
using System.Collections;

public class ActionButton : MonoBehaviour
{
	private static readonly Vector3 _offset = new Vector3(0,0,2);
	
	[SerializeField] private Color _unSelectedColor;
	[SerializeField] private Color _selectedColor;
	
	//private tk2dSprite _border;
	private tk2dSprite _icon;
	
	private int _spellId;
	public int SpellId
	{
		get
		{
			return _spellId;
		}
		set
		{
			_spellId = value;
		}
	}
	
	private string _spellName;
	public string SpellName
	{
		get
		{
			return _spellName;
		}
		set
		{
			_spellName = value;
		}	
	}
	
	public void CreateIcon(GameObject iconPrefab)
	{
		GameObject icon = (GameObject) Instantiate(iconPrefab);
		icon.transform.parent = transform;
		icon.transform.localPosition = _offset;
		icon.transform.localRotation = Quaternion.identity;
		
		//_border = (tk2dSprite) GetComponent(typeof(tk2dSprite));
		_icon = (tk2dSprite) icon.GetComponent(typeof(tk2dSprite));
	}
	
	public void Selected()
	{
		_icon.color = _selectedColor;
	}
	
	public void UnSelected()
	{
		_icon.color = _unSelectedColor;
	}
}


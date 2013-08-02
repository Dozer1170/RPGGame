using UnityEngine;
using System.Collections;

public class BarTextManager : MonoBehaviour 
{
	private string _currentStatText;
	public string CurrentStatText
	{
		get
		{
			return _currentStatText;
		}
		set
		{
			_currentStatText = value;
		}
	}
	private string _totalStat;
	public string TotalStat
	{
		get
		{
			return _totalStat;
		}
		set
		{
			_totalStat = value;
		}
	}
	
	private tk2dTextMesh _textMesh;
	
	void Awake()
	{
		_textMesh = (tk2dTextMesh) GetComponent(typeof(tk2dTextMesh));
	}
	
	void Update()
	{
		_textMesh.text = _currentStatText + "/" + _totalStat;
		_textMesh.Commit();
	}
}

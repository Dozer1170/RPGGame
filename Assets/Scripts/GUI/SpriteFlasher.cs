using UnityEngine;
using System.Collections;

public class SpriteFlasher : MonoBehaviour 
{
	[SerializeField] private Color _colorOne;
	[SerializeField] private Color _colorTwo;
	[SerializeField] private float _interval;
	
	private tk2dSprite _sprite;
	private float _startTime;
	
	void Awake()
	{
		_sprite = (tk2dSprite) GetComponent(typeof(tk2dSprite));
		_startTime = Time.fixedTime;
	}
	
	void Update()
	{
		float lerpValue = (Time.fixedTime - _startTime)/_interval;
		_sprite.color = Color.Lerp(_colorOne, _colorTwo, lerpValue);
		
		if(lerpValue >= 1)
		{
			Color temp = _colorOne;
			_colorOne = _colorTwo;
			_colorTwo = temp;
			_startTime = Time.fixedTime;
		}
	}
}

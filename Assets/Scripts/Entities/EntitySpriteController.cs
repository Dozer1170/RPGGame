using UnityEngine;
using System.Collections;

public class EntitySpriteController : MonoBehaviour 
{
	private tk2dSprite _sprite;
	[SerializeField] private float _deathFlickerToggleTime = 0.1f;
	[SerializeField] private float _totalDeathFlickerTime = 2f;
	[SerializeField] private float _hitFlickerToggleTime = 0.05f;
	[SerializeField] private float _totalHitFlickerTime = 0.3f;
	
	private Color _normalSpriteColor;
	
	private static readonly string _outlineObjectName = "Outline";
	private static readonly Color _outlineDisabledColor = Color.black;
	private static readonly Color _outlineTurnColor = Color.white;
	private static readonly Color _outlineEnemySelectColor = Color.red;
	private static readonly Color _outlineFriendlySelectColor = Color.green;
	private tk2dSprite _barOutline;
	
	private GameObject _statusBars;
	public GameObject StatusBars
	{
		get
		{
			return _statusBars;
		}
		set
		{
			_statusBars = value;
			_barOutline = (tk2dSprite) _statusBars.transform.FindChild(_outlineObjectName).gameObject
				.GetComponent(typeof(tk2dSprite));
			_barOutline.color = _outlineDisabledColor;
		}
	}
	
	private GameObject _turnTimer;
	public GameObject TurnTimer
	{
		get
		{
			return _turnTimer;
		}
		set
		{
			_turnTimer = value;
		}
	}
	
	void Awake()
	{
		_sprite = (tk2dSprite) GetComponent(typeof(tk2dSprite));
		_normalSpriteColor = _sprite.color;
	}
	
	public void SetNormalColor(Color color)
	{
		_normalSpriteColor = color;
	}
	
	public void BeginTurn()
	{
		_barOutline.gameObject.SetActive(true);
		_barOutline.color = _outlineTurnColor;
	}
	
	public void EndTurn()
	{
		DisableStatusOutline();
	}
	
	public void FriendlySelect()
	{
		_barOutline.gameObject.SetActive(true);
		_barOutline.color = _outlineFriendlySelectColor;
	}
	
	public void EnemySelect()
	{
		_barOutline.gameObject.SetActive(true);
		_barOutline.color = _outlineEnemySelectColor;
	}
	
	public void DisableStatusOutline()
	{
		_barOutline.color = _outlineDisabledColor;
	}
	
	public void EntityDied()
	{
		_statusBars.SetActive(false);
		StartCoroutine(FlickerSpriteOut());
		
		if(_turnTimer != null)
		{
			_turnTimer.transform.parent.gameObject.SetActive(false);
		}
	}
	
	public void EntityRevived()
	{
		StatusBars.SetActive(true);
	}
	
	public void EntityWasHit()
	{
		StartCoroutine(FlickerColor(Color.red));
	}
	
	public void EntityWasHealed()
	{
		StartCoroutine(FlickerColor(Color.green));
	}
	
	public void FlipSprite()
	{
		_sprite.FlipX = !_sprite.FlipX;
	}
	
	private IEnumerator FlickerSpriteOut()
	{
		float _startTime = Time.fixedTime;
		while(Time.fixedTime - _startTime < _totalDeathFlickerTime)
		{
			if(renderer.enabled)
			{
				renderer.enabled = false;
			}
			else
			{
				renderer.enabled = true;
			}
			
			yield return new WaitForSeconds(_deathFlickerToggleTime);
		}
		
		renderer.enabled = false;
	}
	
	private IEnumerator FlickerColor(Color flickerColor)
	{
		float _startTime = Time.fixedTime;
		while(Time.fixedTime - _startTime < _totalHitFlickerTime)
		{
			if(_sprite.color == flickerColor)
			{
				_sprite.color = _normalSpriteColor;
			}
			else
			{
				_sprite.color = flickerColor;
			}
			
			yield return new WaitForSeconds(_hitFlickerToggleTime);
		}
		
		_sprite.color = _normalSpriteColor;
	}
}

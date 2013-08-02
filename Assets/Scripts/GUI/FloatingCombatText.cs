using UnityEngine;
using System.Collections;

public class FloatingCombatText : MonoBehaviour 
{
	[SerializeField] private float _startSpeed;
	[SerializeField] private float _endSpeed;
	[SerializeField] private float _timeBetweenSpeeds;
	[SerializeField] private float _fadeOutTime;
	[SerializeField] private float _alphaFadeDelay;
	[SerializeField] private float _startScale;
	[SerializeField] private float _middleScale;
	[SerializeField] private float _endScale;
	[SerializeField] private float _scaleTime;
	
	private tk2dTextMesh _mesh;
	private float _alpha = 1.0f;
	private float _speed;
	private float _scale;
	private float _startTime;
	private float _scaleStartTime;
	private float _alphaStartTime;
	
	void Awake()
	{
		_mesh = (tk2dTextMesh) GetComponent(typeof(tk2dTextMesh));
		_speed = _startSpeed;
		_startTime = Time.fixedTime;
		_alphaStartTime = Time.fixedTime + _alphaFadeDelay;
	}
	
	void Update() 
	{
		_alpha = Mathf.Lerp(_alpha, 0f, (Time.fixedTime - _alphaStartTime)/_fadeOutTime);
		_speed = Mathf.Lerp(_startSpeed, _endSpeed, (Time.fixedTime - _startTime)/_timeBetweenSpeeds);
		
		if(Time.fixedTime - _startTime < _scaleTime)
		{
			_scale = Mathf.Lerp(_startScale, _middleScale, (Time.fixedTime - _startTime)/_scaleTime);
		}
		else
		{
			_scale = Mathf.Lerp(_middleScale, _endScale, (Time.fixedTime - (_startTime + _scaleTime))/_scaleTime);
		}
		
		Color col = _mesh.color;
		col.a = _alpha;
		_mesh.color = col;
		
		Vector3 pos = _mesh.transform.position;
		pos.y += _speed;
		_mesh.transform.position = pos;
		
		Vector3 scale = _mesh.scale;
		scale.x = _scale;
		scale.y = _scale;
		scale.z = _scale;
		_mesh.scale = scale;
		
		_mesh.Commit();
		
		if(_alpha == 0f)
		{
			Destroy(gameObject);
		}
	}
}

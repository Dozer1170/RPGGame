using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
	protected tk2dSpriteAnimator _animator;
	
	// Use this for initialization
	void Start ()
	{
		_animator = (tk2dSpriteAnimator) GetComponent(typeof(tk2dSpriteAnimator));
	}
	
}

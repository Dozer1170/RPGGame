using UnityEngine;
using System.Collections;

public class DestroyAfterAnimation : MonoBehaviour 
{
	public tk2dSpriteAnimator animation;
	
	void Update()
	{
		if(!animation.Playing)
		{
			Destroy(gameObject);
		}
	}
}

using UnityEngine;
using System.Collections;

public class RotatingObject : MonoBehaviour 
{
	[SerializeField] private float _rotationsPerSecond;
	
	void Update()
	{
		Vector3 rot = transform.localEulerAngles;
		rot.z += _rotationsPerSecond * Time.deltaTime * 360f;
		transform.localEulerAngles = rot;
	}
}

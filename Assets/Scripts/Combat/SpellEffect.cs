using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellEffect : MonoBehaviour 
{
	public EffectData[] effects;
	
	public void StartEffect(Entity target)
	{
		StartCoroutine(DoEffect(target));
	}
	
	private IEnumerator DoEffect(Entity target)
	{
		float startTime = Time.fixedTime;
		float[] effectActivateTimes = new float[effects.Length];
		int effectIndex = 0;
		for(int i = 0; i < effectActivateTimes.Length; i++)
		{
			effectActivateTimes[i] = effects[i].effectTime.time + startTime;
		}
		
		while(effectIndex < effectActivateTimes.Length)
		{
			yield return new WaitForSeconds(effectActivateTimes[effectIndex] - Time.fixedTime);
			
			Instantiate(SpellEffectManager.Instance.GetVisualEffect(effects[effectIndex++].effectName), 
				target.transform.position, Quaternion.identity);
		}
		
		Destroy(gameObject);
	}
}

[System.Serializable]
public class EffectData
{
	public string effectName;
	public EffectTime effectTime;
}
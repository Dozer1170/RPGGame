using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpellEffectManager : MonoBehaviour 
{
	private static readonly string SPELL_EFFECT_DIR = "Effects";
	
	private static SpellEffectManager _instance;
	public static SpellEffectManager Instance
	{
		get
		{
			return _instance;
		}
	}
	
	private Dictionary<string,SpellEffect> _spellEffects = new Dictionary<string, SpellEffect>();
	private Dictionary<string,GameObject> _visualEffects = new Dictionary<string, GameObject>();
	
	void Awake()
	{
		_instance = this;
		
		LoadEffects();
	}
	
	private void LoadEffects()
	{
		foreach(Object obj in Resources.LoadAll(SPELL_EFFECT_DIR))
		{
			GameObject effect = (GameObject) obj;
			SpellEffect spellEffect = (SpellEffect) effect.GetComponent(typeof(SpellEffect));
			if(spellEffect != null)
			{
				_spellEffects.Add(spellEffect.name, spellEffect);
			}
			else
			{
				//Is a visual effect
				_visualEffects.Add(effect.name, effect);
			}
		}
	}
	
	public GameObject GetVisualEffect(string name)
	{
		return _visualEffects[name];
	}
	
	public void SpawnSpellEffect(Entity target, string effectName)
	{
		if(effectName != null && _spellEffects.ContainsKey(effectName))
		{
			GameObject spellEffect = (GameObject) Instantiate(_spellEffects[effectName].gameObject);
			((SpellEffect) spellEffect.GetComponent(typeof(SpellEffect))).StartEffect(target);
		}
	}
}

using UnityEngine;
using System.Collections;

public class ManaResourceManager : SecondaryResourceManager 
{
	public override void Init (CombatEntity entity)
	{
		base.Init (entity);
		_currentSr = _entity.Stats.MaxSecondaryResource;
	}
}

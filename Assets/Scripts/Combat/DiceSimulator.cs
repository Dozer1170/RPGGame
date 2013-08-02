using UnityEngine;
using System.Collections;

public class DiceSimulator 
{
	private static DiceSimulator _instance;
	public static DiceSimulator Instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new DiceSimulator();
			}
			return _instance;
		}
	}
	
	public int RollD4(int times = 1)
	{
		return RollDie(4, times);
	}
	
	public int RollD6(int times = 1)
	{
		return RollDie(6, times);
	}
	
	public int RollD8(int times = 1)
	{
		return RollDie(8, times);
	}
	
	public int RollD10(int times = 1)
	{
		return RollDie(10, times);
	}
	
	public int RollD12(int times = 1)
	{
		return RollDie(12, times);
	}
	
	public int RollD20(int times = 1)
	{
		return RollDie(20, times);
	}
	
	public int RollD100(int times = 1)
	{
		return RollDie(100, times);
	}
	
	public int RollDie(int dieSize, int times)
	{
		int rval = 0;
		for(int i = 0; i < times; i++)
		{
			rval += Random.Range(1, dieSize + 1);
		}
		return rval;
	}
}

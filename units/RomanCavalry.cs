using UnityEngine;
using System.Collections;

public class RomanCavalry : Unit
{
	
	void Start ()
	{
		this.description = "ROMAN_CAVALRY";
		this.playerManaged = GameManager.i.romansByPlayer;
		BaseStart ();
		maxMovement = 3.0F;
	}
	

	void Update ()
	{
		UnitUpdate ();
	}

	public override int ComputeAttackForceAgainst (Unit unit)
	{
		return 1;
	}
	
	public override int ComputeDefenceForceAgainst (Unit unit)
	{
		return 1;
	}


}

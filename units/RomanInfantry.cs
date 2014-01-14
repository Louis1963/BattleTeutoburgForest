using UnityEngine;
using System.Collections;

public class RomanInfantry : Unit
{
	public bool quadratum = false;

	void Start ()
	{
		this.playerManaged = GameManager.i.romansByPlayer;
		BaseStart ();
		maxMovement = 1.0F;
		remainingAmmo = 2;
		lifePoints = 3;
		statesSupported.Add (StateReadyLaunch);
	}
 
	void Update ()
	{
		UnitUpdate ();
	}

	public override int ComputeAttackForceAgainst (Unit unit)
	{
		return 2;
	}
	
	public override int ComputeDefenceForceAgainst (Unit unit)
	{
		int baseDef = 2;
		if (quadratum)
			baseDef *= 2;
		return baseDef;
	}
}
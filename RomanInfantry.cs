using UnityEngine;
using System.Collections;

public class RomanInfantry : Unit, FiniteStateMachine
{
		public bool quadratum = false;

		void Start ()
		{
				this.playerManaged = GameManager.i.romansByPlayer;
				BaseStart ();
				maxMovement = 1.0F;
				lifePoints = 5;
		}
 
		void Update ()
		{
				UnitUpdate ();

				if (playerManaged)
						BaseByPlayerMovement ();
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
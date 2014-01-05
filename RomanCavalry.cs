using UnityEngine;
using System.Collections;

public class RomanCavalry : Unit, FiniteStateMachine
{
	
		void Start ()
		{
				this.playerManaged = GameManager.i.romansByPlayer;
				BaseStart ();
				maxMovement = 3.0F;
		}
	

		void Update ()
		{
				if (playerManaged)
						BaseByPlayerMovement ();
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

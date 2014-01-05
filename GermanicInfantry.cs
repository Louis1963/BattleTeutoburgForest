using UnityEngine;
using System.Collections;
using Holoville.HOTween; 
using Holoville.HOTween.Plugins;

public class GermanicInfantry : Unit, FiniteStateMachine
{
		float lastMovedOn;

		void Start ()
		{
				this.playerManaged = GameManager.i.germanicsByPlayer;
				this.side = Side.Germanic;
				BaseStart ();
				maxMovement = 1.0F;
		}
	
		void Update ()
		{
				UnitUpdate ();
		
				if (playerManaged)
						BaseByPlayerMovement ();
				else if (BattleSceneManager.s.germanicChaseEnabled && getState () == StateIdle) {
						setState (StateChase);
				}
 
				if (StateChase == getState ())
						Chase ();
		}
 
		public void Chase ()
		{
				if ((lastMovedOn + GameManager.i.aiTick) < Time.time) {
						lastMovedOn = Time.time;
						
						Unit unit = FindClosestEnemyAlive ();
						if (unit != null) {
								//transform.position = Vector3.Lerp (transform.position, unit.transform.position, fracJourney);
								Vector3 target = unit.transform.position;
								var dist = Vector3.Distance (transform.position, target);
								if (dist > (maxMovement / 3)) {
										Vector3 vect = transform.position - target;
										vect = vect.normalized;
										vect *= (dist - (maxMovement / 3));
										target += vect;
								}
								HOTween.To (transform, .5f, "position", target);
						}
				}
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

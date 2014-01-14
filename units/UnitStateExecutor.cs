using UnityEngine;
using System.Collections;
using Holoville.HOTween; 
using Holoville.HOTween.Plugins;

public class UnitStateExecutor
{
	public Unit u;

	public UnitStateExecutor (Unit unit)
	{
		this.u = unit;
	}

	public void EnterMelee (Unit unit)
	{
		u.inMeleeWith = unit;
		u.SetState (Unit.StateMelee);
		u.inMeleeSince = Time.time;
		unit.inMeleeWith = u;
		unit.SetState (Unit.StateMelee);
		unit.inMeleeSince = Time.time;
		
	}
	
	public void Attack (Unit unit)
	{
		u.SetState (Unit.StateIdle);
		unit.SetState (Unit.StateIdle);
		u.inMeleeWith = null;
		unit.inMeleeWith = null;
		
		int attackForce = u.ComputeAttackForceAgainst (unit);
		int defenceForce = unit.ComputeDefenceForceAgainst (u);
		
		//ground effect
		if (unit.specialGround != null) {
			defenceForce += unit.specialGround.defenceBonus;
		}
		if (u.specialGround != null) {
			attackForce += u.specialGround.attackBonus;
		}
		
		//fate effect
		attackForce += Random.Range (1, 4);
		defenceForce += Random.Range (1, 4);
		
		//Debug.Log ("att " + attackForce + " def " + defenceForce);
		
		if (attackForce > defenceForce) {
			unit.lifePoints--;				
		} else {
			u.lifePoints--;
		}
		
		//bounce
		Vector3 oppositeDirection = (2.0f * u.transform.position) - unit.transform.position;
		
		//Debug.Log ("oppositeDirection.magnitude " + oppositeDirection.magnitude);
		//gameObject.GetComponent<Rigidbody2D> ().AddForce (oppositeDirection);
		//GameUtilities.AddForce (gameObject.GetComponent<Rigidbody2D> (), new Vector2 (1, 1), ForceMode.Impulse);
		
		//transform.position = Vector3.Lerp (transform.position, oppositeDirection, 1);
		HOTween.To (u.transform, 1, "position", oppositeDirection);
		
	}

	public void Chase ()
	{
		if ((u.lastMovedOn + GameManager.i.aiTick) < Time.time) {
			u.lastMovedOn = Time.time;
			
			Unit unit = u.FindClosestEnemyAlive ();
			if (unit != null) {
				//transform.position = Vector3.Lerp (transform.position, unit.transform.position, fracJourney);
				Vector3 target = unit.transform.position;
				var dist = Vector3.Distance (u.transform.position, target);
				if (dist > (u.maxMovement / 3)) {
					Vector3 vect = u.transform.position - target;
					vect = vect.normalized;
					vect *= (dist - (u.maxMovement / 3));
					target += vect;
				}
				HOTween.To (u.transform, .5f, "position", target);
			}
		}
	}

	public void LaunchProjectile (Vector3 targetPosition)
	{
		if (u.projectilePrefab != null) {
		
			Vector3 newPos = new Vector3 (u.transform.position.x, u.transform.position.y, u.transform.position.z);
			Transform es = (Transform)MonoBehaviour.Instantiate (u.projectilePrefab, newPos, Quaternion.identity);

			float diffX = es.position.x - targetPosition.x;
			float diffY = es.position.y - targetPosition.y;
			
			float angle = Mathf.Atan2 (diffY, diffX) * Mathf.Rad2Deg;
			es.rotation = Quaternion.Euler (new Vector3 (0, 0, angle + 90));

			//todo generalize
			Javelin j = es.GetComponent<Javelin> ();
			j.LaunchByTo (u, targetPosition);
			u.remainingAmmo--;
		}
	
	}

}

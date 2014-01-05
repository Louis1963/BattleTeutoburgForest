using UnityEngine;
using System.Collections;
using Holoville.HOTween; 
using Holoville.HOTween.Plugins;

public abstract class Unit : MonoBehaviour
{
		public enum Side
		{
				Roman,
				Germanic
		}

		//state
		/*public enum State
		{
				Idle=0,
				Selected =1,
				Melee=2,
				Advancing=3,
				Chasing=4,
				Quadratum=5,
				Volley=6,
				Dead=7,
		}*/
		public static int StateIdle = 0;
		public static int StateSelected = 1;
		public static int StateAdvancing = 2;
		public static int StateMelee = 3;
		public static int StateDead = 4;
		public static int StateChase = 5;

		//public List<> bonusAttacking = new List();

		//components
		protected SpriteRenderer spriteRenderer;
		protected Animator animator;

		//basic game features
		public bool playerManaged = false;
		public Side side = Side.Roman;

		//movement & selection
		public float elapsedTime = 0;
		public Transform targetPosition;
		public float speed = 1.0F;
		public Vector3 target;
		public float smooth = 5.0F;
		public float maxMovement = 2.0F;
		bool mouseOnObject;
		public Ground specialGround;
	
		//state
		public Unit inMeleeWith;
		public float inMeleeSince;
		public int lifePoints = 3;
		public int thresholdForChase = 3;
	
		protected void BaseStart ()
		{
				spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
				animator = gameObject.GetComponent<Animator> ();

				BattleSceneManager.s.units.Add (this);
		
				setState (StateIdle);

				BattleSceneManager.s.OnUnitClick += OnUnitClick;
		}

		protected void UnitUpdate ()
		{
				var inMelee = StateMelee == getState ();
				if (inMelee) {
						if ((inMeleeSince + 5) < Time.time) {
								Attack (inMeleeWith);
						} 
				}

				if (lifePoints <= 0 && StateDead != getState ())
						Die ();
		}

		void OnMouseEnter ()
		{
				mouseOnObject = true;
		}

		void OnMouseExit ()
		{
				mouseOnObject = false;
		}

		void OnMouseOver ()
		{
				if (playerManaged && getState () == StateIdle && Input.GetMouseButtonDown (0)) {
						BattleSceneManager.s.UnitClick (transform.gameObject);
				}
		}
 
		void OnUnitClick (GameObject g)
		{
				// If g is THIS gameObject
				if (g == gameObject) {

						BattleSceneManager.s.focusedUnit = transform;
						setState (StateSelected);
						
				} else {
						if (getState () == StateSelected)
								setState (StateIdle);
				}
		}
 
		void OnCollisionEnter2D (Collision2D col)
		{
				if (getState () != StateMelee && getState () != StateDead) {
						Unit unit = col.gameObject.GetComponent<Unit> ();
						if (unit != null && !unit.side.Equals (side) && unit.getState () != StateDead) {
								//Debug.Log ("Hit an enemy!");
								EnterMelee (unit);
						} else {
								//Debug.Log ("Hit a friend!");
						}
				}
		}

		void EnterMelee (Unit unit)
		{
				this.inMeleeWith = unit;
				this.setState (StateMelee);
				this.inMeleeSince = Time.time;
				unit.inMeleeWith = this;
				unit.setState (StateMelee);
				unit.inMeleeSince = Time.time;
				
		}
			
		void Attack (Unit unit)
		{
				this.setState (StateIdle);
				unit.setState (StateIdle);
				this.inMeleeWith = null;
				unit.inMeleeWith = null;

				int attackForce = ComputeAttackForceAgainst (unit);
				int defenceForce = unit.ComputeDefenceForceAgainst (this);

				//ground effect
				if (unit.specialGround != null) {
						defenceForce += unit.specialGround.defenceBonus;
				}
				if (specialGround != null) {
						attackForce += specialGround.attackBonus;
				}

				//fate effect
				attackForce += Random.Range (1, 4);
				defenceForce += Random.Range (1, 4);

				//Debug.Log ("att " + attackForce + " def " + defenceForce);

				if (attackForce > defenceForce) {
						unit.lifePoints--;				
				} else {
						this.lifePoints--;
				}

				//bounce
				Vector3 oppositeDirection = (2.0f * transform.position) - unit.transform.position;

				//Debug.Log ("oppositeDirection.magnitude " + oppositeDirection.magnitude);
				//gameObject.GetComponent<Rigidbody2D> ().AddForce (oppositeDirection);
				//GameUtilities.AddForce (gameObject.GetComponent<Rigidbody2D> (), new Vector2 (1, 1), ForceMode.Impulse);

				//transform.position = Vector3.Lerp (transform.position, oppositeDirection, 1);
				HOTween.To (transform, 1, "position", oppositeDirection);
	
		}

		public abstract int ComputeAttackForceAgainst (Unit unit);
		public abstract int ComputeDefenceForceAgainst (Unit unit);

		public void Die ()
		{
				BattleSceneManager.s.OnUnitClick -= OnUnitClick;
				BattleSceneManager.s.units.Remove (this);
				//Debug.Log ("died");
				setState (StateDead);
		}

		protected Unit FindClosestEnemyAlive ()
		{
				Unit closestEnemy = null;
				float closestDistance = -1;
				foreach (Unit unit in BattleSceneManager.s.units) {

						if (! (unit.side.Equals (side)) && StateDead != unit.getState ()) {
								if (closestDistance == -1) {
										closestEnemy = unit;
										closestDistance = Vector3.Distance (transform.position, unit.transform.position);
								} else {		
										float dist = Vector3.Distance (transform.position, unit.transform.position);
										if (dist < closestDistance) {
												closestEnemy = unit;
												closestDistance = dist;

										}
								}
						}
				}
				return closestEnemy;
		}

		protected void BaseByPlayerMovement ()
		{
				if (StateSelected == getState () && Input.GetMouseButtonDown (0) &&
						transform.Equals (BattleSceneManager.s.focusedUnit) && !mouseOnObject) {
			
						//Debug.Log ("x y " + Input.mousePosition.x + " " + Input.mousePosition.y);
			
						Vector3 mouse = Input.mousePosition;
						Vector3 vec = Camera.main.ScreenToWorldPoint (mouse);
						//Debug.Log ("x y ScreenToWorldPoint " + vec.x + " " + vec.y);
			
						target = new Vector3 (vec.x, vec.y, 0);			
						setState (StateAdvancing);
				}
		
				if (StateAdvancing == getState ()) {
						//shorten target to max distance
						var dist = Vector3.Distance (transform.position, target);
						if (dist > maxMovement) {
							
								Vector3 vect = transform.position - target;
								vect = vect.normalized;
								vect *= (dist - maxMovement);
								target += vect;

						}
						//transform.position = Vector3.Lerp (transform.position, target, 1);
						HOTween.To (transform, .5f, "position", target);
						setState (StateIdle);
						BattleSceneManager.s.focusedUnit = null;
				}
		}	

		public int setState (int state)
		{
				this.animator.SetInteger ("curState", state);
				return state;
		}
	
		public int getState ()
		{
				int i = StateIdle;
				if (this.animator != null)
						i = this.animator.GetInteger ("curState");
				return i;
		}

}
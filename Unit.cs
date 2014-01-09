using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	public static int StateReadyLaunch = 6;




	//components
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;

	//movement & selection
	public float elapsedTime = 0;
	public Transform targetPosition;
	public float speed = 1.0F;
	public Vector3 target;
	public float smooth = 5.0F;
	public float maxMovement = 2.0F;
	bool mouseOnObject;
	public Ground specialGround;

	//unit specific configuration
	public int lifePoints = 2;
	public bool playerManaged = false;
	public Side side = Side.Roman;
	//public List<> bonusAttacking = new List();
	public List<int> statesSupported;
	public int maxLaunches = 0;

	//runtime state helpers
	public Unit inMeleeWith;
	public float inMeleeSince;
	float lastMovedOn;
		
	
	protected void BaseStart ()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		animator = gameObject.GetComponent<Animator> ();

		statesSupported = new List<int> (new int[] {
			StateIdle,
			StateSelected,
			StateAdvancing,
			StateMelee,
			StateDead
		});

		BattleSceneManager.s.units.Add (this);
		
		SetState (StateIdle);

		BattleSceneManager.s.OnUnitClick += OnUnitClick;
	}

	protected void UnitUpdate ()
	{

		//basic state management

		if (lifePoints <= 0 && StateDead != GetState ()) {

			Die ();
	
		} else if (StateChase == GetState ()) {

			Chase ();
		
		} else if (StateMelee == GetState ()) {

			if ((inMeleeSince + 5) < Time.time) {
				Attack (inMeleeWith);
			}

		} else if (StateAdvancing == GetState ()) {

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
			SetState (StateIdle);
			
		}


		//human player management
		if (playerManaged)
			BaseByPlayerMovement ();
 
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
		if (playerManaged && GetState () == StateIdle && Input.GetMouseButtonDown (0)) {
			BattleSceneManager.s.UnitClick (transform.gameObject);
		}
	}
 
	void OnUnitClick (GameObject g)
	{
		// If g is THIS gameObject
		if (g == gameObject) {
			if (GetState () == StateIdle) {
				BattleSceneManager.s.focusedUnit = transform;
				SetState (StateSelected);
			} else if (GetState () == StateSelected) {
			}
						
		} else {
			if (GetState () == StateSelected)
				SetState (StateIdle);
		}
	}
 
	void OnCollisionEnter2D (Collision2D col)
	{
		if (GetState () != StateMelee && GetState () != StateDead) {
			Unit unit = col.gameObject.GetComponent<Unit> ();
			if (unit != null && !unit.side.Equals (side) && unit.GetState () != StateDead) {
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
		this.SetState (StateMelee);
		this.inMeleeSince = Time.time;
		unit.inMeleeWith = this;
		unit.SetState (StateMelee);
		unit.inMeleeSince = Time.time;
				
	}
			
	void Attack (Unit unit)
	{
		this.SetState (StateIdle);
		unit.SetState (StateIdle);
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
		SetState (StateDead);
	}

	protected Unit FindClosestEnemyAlive ()
	{
		Unit closestEnemy = null;
		float closestDistance = -1;
		foreach (Unit unit in BattleSceneManager.s.units) {

			if (! (unit.side.Equals (side)) && StateDead != unit.GetState ()) {
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
		if (StateSelected == GetState () && Input.GetMouseButtonDown (0) &&
			transform.Equals (BattleSceneManager.s.focusedUnit) && !mouseOnObject) {
			
			//Debug.Log ("x y " + Input.mousePosition.x + " " + Input.mousePosition.y);
			
			Vector3 mouse = Input.mousePosition;
			Vector3 vec = Camera.main.ScreenToWorldPoint (mouse);
			//Debug.Log ("x y ScreenToWorldPoint " + vec.x + " " + vec.y);
			
			target = new Vector3 (vec.x, vec.y, 0);			
			SetState (StateAdvancing);
			BattleSceneManager.s.focusedUnit = null;
		}

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

	public int SetState (int state)
	{
		if (statesSupported.Contains (state)) {
			this.animator.SetInteger ("curState", state);
		} else {
			Debug.Log ("trying to set state " + state + " failed");
		}
		return GetState ();
	}
	
	public int GetState ()
	{
		int i = StateIdle;
		if (this.animator != null)
			i = this.animator.GetInteger ("curState");
		return i;
	}



}
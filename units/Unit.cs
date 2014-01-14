using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Holoville.HOTween; 

public abstract class Unit : MonoBehaviour
{
	public enum Side
	{
		Roman,
		Germanic
	}

	//state

	public static int StateIdle = 0;
	public static int StateSelected = 1;
	public static int StateAdvancing = 2;
	public static int StateMelee = 3;
	public static int StateDead = 4;
	public static int StateChase = 5;
	public static int StateReadyLaunch = 6;

	//audio for state
	public AudioClip Audio0;
	public AudioClip Audio1;
	public AudioClip Audio2;
	public AudioClip Audio3;
	public AudioClip Audio4;
	public AudioClip Audio5;
	public AudioClip Audio6;

	//model
	public UnitStateExecutor use;

	//components
	protected SpriteRenderer spriteRenderer;
	protected Animator animator;
	protected AudioSource audioSource;


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
	public int remainingAmmo = 0;
	public Transform projectilePrefab;

	//runtime state helpers
	public Unit inMeleeWith;
	public float inMeleeSince;
	public float lastMovedOn;
		
	
	protected void BaseStart ()
	{
		//this may eventually be extended by specific unit extensions
		use = new UnitStateExecutor (this);

		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		animator = gameObject.GetComponent<Animator> ();
		audioSource = gameObject.GetComponent<AudioSource> ();

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

			use.Chase ();
		
		} else if (StateMelee == GetState ()) {

			if ((inMeleeSince + 5) < Time.time) {
				use.Attack (inMeleeWith);
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
		if (playerManaged && Input.GetMouseButtonDown (0)) {
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
			
				if (statesSupported.Contains (StateReadyLaunch) && remainingAmmo > 0)
					SetState (StateReadyLaunch);
				else
					SetState (StateIdle);
			
			} else if (GetState () == StateReadyLaunch) {
				SetState (StateIdle);
			}
						
		} else {

			if (GetState () == StateSelected) {
 
				SetState (StateIdle);
			}
				
		}
	}
 
	void OnCollisionEnter2D (Collision2D col)
	{
		if (GetState () != StateMelee && GetState () != StateDead) {
			Unit unit = col.gameObject.GetComponent<Unit> ();
			if (unit != null && !unit.side.Equals (side) && unit.GetState () != StateDead) {
				//Debug.Log ("Hit an enemy!");
				use.EnterMelee (unit);
			} else {
				//Debug.Log ("Hit a friend!");
			}
		}
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

	public Unit FindClosestEnemyAlive ()
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
		if (Input.GetMouseButtonDown (0) &&
			transform.Equals (BattleSceneManager.s.focusedUnit) && !mouseOnObject) {

			if (StateSelected == GetState () || StateReadyLaunch == GetState ()) {
				//Debug.Log ("x y " + Input.mousePosition.x + " " + Input.mousePosition.y);
			
				Vector3 mouse = Input.mousePosition;
				Vector3 vec = Camera.main.ScreenToWorldPoint (mouse);
				//Debug.Log ("x y ScreenToWorldPoint " + vec.x + " " + vec.y);
			
				target = new Vector3 (vec.x, vec.y, 0);	

				if (StateSelected == GetState ()) {
					SetState (StateAdvancing);
				} else if (StateReadyLaunch == GetState ()) {
				
					use.LaunchProjectile (target);
					SetState (StateIdle);
				}
				BattleSceneManager.s.focusedUnit = null;			
			} 
		}
	}
		
	
	public int SetState (int state)
	{
		if (statesSupported.Contains (state)) {

			this.animator.SetInteger ("curState", state);

			//stop sound for old state
			if (audioSource != null) {

				//each state change should stop audio from preceding one
				//audio.Stop ();

				//start sound for new state
				FieldInfo field = this.GetType ().GetField ("Audio" + state); 
				if (field != null) {

					AudioClip audioClip = (AudioClip)field.GetValue (this);
					if (audioClip != null) {

						//but we stop only when there is a new one
						audioSource.Stop ();

						audioSource.clip = audioClip;
						if (audioSource.clip != null)
							audioSource.Play ();
					} 
				}
			}


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
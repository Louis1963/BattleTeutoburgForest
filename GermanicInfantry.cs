using UnityEngine;
using System.Collections;
using Holoville.HOTween; 
using Holoville.HOTween.Plugins;

public class GermanicInfantry : Unit, FiniteStateMachine
{
		

	void Awake ()
	{

	}

	void Start ()
	{
		this.playerManaged = GameManager.i.germanicsByPlayer;
		this.side = Side.Germanic;
		BaseStart ();
		statesSupported.Add (StateChase);
		statesSupported.Add (StateReadyLaunch);
		maxMovement = 1.0F;
	}
	
	void Update ()
	{
		UnitUpdate ();	

		if (StateIdle == GetState ()) {
			
			if (BattleSceneManager.s.germanicChaseEnabled) {
				SetState (StateChase);
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

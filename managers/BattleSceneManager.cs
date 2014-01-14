using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleSceneManager : MonoBehaviour
{
	public List<Unit> units = new List<Unit> ();

	public bool germanicChaseEnabled = false;
	
	public Transform focusedUnit;
	
	public delegate void OnUnitClickEvent (GameObject g);
	public event OnUnitClickEvent OnUnitClick;

	public void UnitClick (GameObject g)
	{
		OnUnitClick (g);
	}
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
 
	void Awake ()
	{
		_instance = this;
	}

	// Usual horrific singleton
	static BattleSceneManager _instance;
	
	public static BattleSceneManager  s {
		get { return _instance; }
		private set { _instance = value; }
	}
	
	private BattleSceneManager ()
	{
	}
}

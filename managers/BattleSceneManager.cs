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
	public GameManager gameManager ;

	public void UnitClick (GameObject g)
	{
		OnUnitClick (g);
	}
	
	// Use this for initialization
	void Start ()
	{
		gameManager = GameManager.i;
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckWinningConditions ();
	}
 
	void Awake ()
	{
		_instance = this;
	}

	public void UpdateGUIScores ()
	{
		dfLabel rs = GameObject.FindGameObjectWithTag ("RomanScore").GetComponent<dfLabel> ();
		rs.Text = I18n.T ("ROMAN_SCORE_%%", GameManager.i.romanPoints);
		dfLabel gs = GameObject.FindGameObjectWithTag ("GermanicScore").GetComponent<dfLabel> ();
		gs.Text = I18n.T ("GERMANIC_SCORE_%%", GameManager.i.germanicPoints);
		
	}

	private void CheckWinningConditions ()
	{

		if (GameManager.i.allGermanicDead || GameManager.i.allGermanicDead) {

			StopAllActivity ();

			dfLabel go = GameObject.FindGameObjectWithTag ("GameOver").GetComponent<dfLabel> ();
			go.Text = I18n.T ("GAME_OVER_%%_WON", GameManager.i.allGermanicDead ? "Romans" : "Germanics");

			dfLabel rs = GameObject.FindGameObjectWithTag ("RomansResult").GetComponent<dfLabel> ();
			rs.Text = I18n.T ("ROMAN_SCORE_%%", GameManager.i.romanPoints);
			dfLabel gs = GameObject.FindGameObjectWithTag ("GermanicResult").GetComponent<dfLabel> ();
			gs.Text = I18n.T ("GERMANIC_SCORE_%%", GameManager.i.germanicPoints);

			dfPanel panel = GameObject.FindGameObjectWithTag ("Results").GetComponent<dfPanel> ();
			panel.IsVisible = true;

		} 
	}

	private void StopAllActivity ()
	{
	
		foreach (Unit u in units) {
			if (Unit.StateDead != u.GetState ()) {
				u.SetState (Unit.StateIdle);
			}
		}
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

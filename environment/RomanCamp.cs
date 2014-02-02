using UnityEngine;
using System.Collections;

public class RomanCamp : MonoBehaviour
{

	public static bool romanReachedItAlive = false;

	void OnTriggerEnter2D (Collider2D col)
	{
		Unit unit = col.gameObject.GetComponent<Unit> ();
		if (unit != null && unit.side.Equals (Unit.Side.Roman) && !romanReachedItAlive) {
			romanReachedItAlive = true;
			GameManager.i.romanPoints += 3;
			BattleSceneManager.s.UpdateGUIScores ();
		}
	}
}

using UnityEngine;
using System.Collections;

public class Swamp : MonoBehaviour
{

	void OnTriggerEnter2D (Collider2D col)
	{
		Unit unit = col.gameObject.GetComponent<Unit> ();
		if (unit != null && unit.side.Equals (Unit.Side.Roman)) {
			unit.ChangeLifePoints (-unit.GetLifePoints ());
		}
	}
}

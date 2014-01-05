using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour
{

		void OnTriggerEnter2D (Collider2D col)
		{
				Unit unit = col.gameObject.GetComponent<Unit> ();
				if (unit != null && unit.side.Equals (Unit.Side.Roman)) {
						BattleSceneManager.s.germanicChaseEnabled = true;				
				}
		}

}

using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
	public int defenceBonus;
	public int attackBonus;
	public GroundType groundType;

	public enum GroundType
	{
		WOOD,
		HILLTOP,
		ROMANCAMP
	}

	// Use this for initialization
	void Start ()
	{
		if (groundType == GroundType.WOOD) {
			defenceBonus = 1;
			attackBonus = 0;
		} else if (groundType == GroundType.HILLTOP) {
			defenceBonus = 1;
			attackBonus = 1;

		}	
	}

		

	void OnTriggerEnter2D (Collider2D col)
	{
		Unit unit = col.gameObject.GetComponent<Unit> ();
		if (unit != null)
			unit.specialGround = this;
	}

	void OnTriggerExit2D (Collider2D col)
	{
		Unit unit = col.gameObject.GetComponent<Unit> ();
		if (unit != null)
			unit.specialGround = null;
	}


		
				
		
}

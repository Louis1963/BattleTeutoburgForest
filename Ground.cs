using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
		public int defenceBonus;
		public int attackBonus;
		public int groundType;

		public static int Wood = 0;
		public static int HillTop = 1;

		// Use this for initialization
		void Start ()
		{
				if (groundType == Wood) {
						defenceBonus = 1;
						attackBonus = 0;
				} else if (groundType == HillTop) {
						defenceBonus = 1;
						attackBonus = 1;

				}	
		}

		

		void OnTriggerEnter2D (Collider2D col)
		{
				Unit unit = col.gameObject.GetComponent<Unit> ();
				unit.specialGround = this;
		}

		void OnTriggerExit2D (Collider2D col)
		{
				Unit unit = col.gameObject.GetComponent<Unit> ();
				unit.specialGround = null;
		}


		
				
		
}

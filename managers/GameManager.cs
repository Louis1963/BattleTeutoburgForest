using System.Collections.Generic;

public class GameManager
{
		public bool romansByPlayer = true;
		public bool germanicsByPlayer = false;
		public int romanPoints = 0;
		public int germanicPoints = 0;
		public float aiTick = 2f;

		// Usual horrific singleton
		static GameManager _instance = new GameManager ();

		public static GameManager  i {
				get { return _instance; }
				private set { _instance = value; }
		}

		private GameManager ()
		{
		}

	
}

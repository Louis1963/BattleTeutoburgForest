using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class U
{
	public static bool ex (object o)
	{
		bool result = false;
		if (o != null) {
			if (o.GetType () == typeof(string)) {
				string s = (string)o;
				result = s.Trim ().Length > 0;
			}
		}
		return result;
	}
	
}


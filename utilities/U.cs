using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//A collection of simple utility methods for writing code
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

	public static string L (string message, object o)
	{
		var str = message + " ---> " + o;
		Debug.Log (str);
		return str;
	}
  
	public static string L (object o)
	{
		var str = o.GetType () + "" + " ---> " + o;
		Debug.Log (str);
		return str;
	}
	
}


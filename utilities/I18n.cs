using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class I18n
{
	public static Dictionary<string,Dictionary<string,string>> translations = new Dictionary<string,Dictionary<string,string>> ();

	public static string T (string key)
	{
		return T (key, null);
	}

	public static string T (string key, params string[] args)
	{
		string translation;
		(I18n.translations ["en"]).TryGetValue (key, out translation);
		Debug.Log ("1 " + translation);
		if (U.ex (translation)) {
			foreach (string arg in args) {
				//todo replace first
				translation = translation.Replace ("%%", arg);

			}
			Debug.Log ("2 " + translation);
		} else {
			Debug.Log ("3 " + translation);
			translation = key.Replace ("_", " ").ToLower ();
		}
		Debug.Log ("4 " + translation);
		return translation;
	}

	public static void setup ()
	{
		var dictionaryEN = new Dictionary<string, string> ();
		dictionaryEN.Add ("UNIT_DEFENSOR_%%_HIT", "%% was hit in defence");
		dictionaryEN.Add ("UNIT_ATTACKER_%%_HIT", "%% was hit in attack");

		translations.Add ("en", dictionaryEN);
	}

}

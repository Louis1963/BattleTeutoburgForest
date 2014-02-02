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

	public static string T (string key, int arg)
	{
		return I18n.T (key, arg + "");
	}

	public static string T (string key, params string[] args)
	{
		string translation;
		(I18n.translations ["en"]).TryGetValue (key, out translation);
		if (U.ex (translation)) {
			foreach (string arg in args) {
				translation = translation.Replace ("%%", arg);
			}
		} else {
			translation = key.Replace ("_", " ").ToLower ();
		}
		return translation;
	}

	public static void setup ()
	{
		var dictionaryEN = new Dictionary<string, string> ();
		dictionaryEN.Add ("UNIT_DEFENSOR_%%_HIT", "%% was hit in defence");
		dictionaryEN.Add ("UNIT_ATTACKER_%%_HIT", "%% was hit in attack");

		dictionaryEN.Add ("ROMAN_SCORE_%%", "Roman Score: %%");
		dictionaryEN.Add ("GERMANIC_SCORE_%%", "Germanic Score: %%");

		dictionaryEN.Add ("GAME_OVER_%%_WON", "GAME OVER - %% WON!");



		translations.Add ("en", dictionaryEN);
	}

}

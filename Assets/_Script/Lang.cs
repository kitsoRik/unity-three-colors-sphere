using UnityEngine;
using System.Collections;
using System.IO;

public class Lang : MonoBehaviour {

	private static int NumberOfLanguage;
	private static string FileName = "Language";
	private static string[] Words;
	public static void Starting () 
	{
#if UNITY_EDITOR
        if (File.Exists (Application.dataPath + "/Resources/" + FileName + ".csv")) {
		GetLanguageString ();
		TextAsset TA = Resources.Load<TextAsset> (FileName);
		Words = TA.text.Split ( '\n' );
		} else 
		{
			File.Create(Application.dataPath + "/Resources/" + FileName + ".csv").Dispose();
			File.WriteAllText(Application.dataPath + "/Resources/" + FileName + ".csv", "English;Russian;Ukrainian\n");
			Starting();
		}
#elif !UNITY_EDITOR
		GetLanguageString();
		TextAsset TA = Resources.Load<TextAsset> (FileName);
		Words = TA.text.Split ( '\n' );
#endif
	}


	public static string Phrase(string EnglishPhrase)
	{
		bool HaveEnglish = false;
		for(int i = 1; i < Words.Length; i++)
		{
			string[] temp = Words[i].Split(';');
				if(temp[0] == EnglishPhrase)
			{
				if(temp.Length > NumberOfLanguage)
					return temp[NumberOfLanguage];
				else 
					HaveEnglish = true;
			}
		}
		if (!HaveEnglish)
            File.WriteAllText(Application.dataPath + "/Resources/" + FileName + ".csv", string.Join("\n", Resources.Load<TextAsset>(FileName).text.Split('\n')) + EnglishPhrase + "\n");
        return EnglishPhrase;
	}

		
		
	#region Dont touch

	public static void GetLanguageString()
	{
		FindLanguage (Application.systemLanguage.ToString());
	}
	
	static void FindLanguage(string FindLanguage)
	{
		TextAsset TA = Resources.Load<TextAsset> (FileName);
		string [] data1 = TA.text.Split ( '\n' )[0].Split(';');

		for(int i = 0; i < data1.Length-1; i++)
		{
			if(data1[i] == FindLanguage)
			{
				NumberOfLanguage = i;
				return;
			}
		}
		if (NumberOfLanguage == 0 && data1 [data1.Length - 1] == FindLanguage + '\r') 
		{
			NumberOfLanguage = data1.Length-1;
			return;
		}
	}

	#endregion
}

using UnityEngine;
using System.Collections;

public class Save : MonoBehaviour {

    static public int BestScore
    {
        get { return PlayerPrefs.GetInt("BestScore", 0); }
        set { PlayerPrefs.SetInt("BestScore", value); }
    }

    static public int CountGames
    {
        get { return PlayerPrefs.GetInt("CountGames", 0); }
        set { PlayerPrefs.SetInt("CountGames", value); }
    }

    static public float AverageScore
    {
        get { return PlayerPrefs.GetFloat("AverageScore", 0); }
        set { PlayerPrefs.SetFloat("AverageScore", value); }
    }

    static public int PlusToHasCubes
    {
        get { return PlayerPrefs.GetInt("PlusToHasCubes", 0); }
        set { PlayerPrefs.SetInt("PlusToHasCubes", value); }
    }

    static public int NowQuest
    {
        get { return PlayerPrefs.GetInt("NowQuest", 1); }
        set { PlayerPrefs.SetInt("NowQuest", value); }
    }
}

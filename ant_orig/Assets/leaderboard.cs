﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

// Leaderboard script, based on the following source: https://www.grimoirehex.com/unity-3d-local-leaderboard/

public class PlayerInfo
{
    public string name;
    public int score;

    public PlayerInfo(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}

public class leaderboard : MonoBehaviour
{

    //Use TextMeshPro To Populate The List
    public TextMeshPro name;
    public TextMeshPro score;
    //public GameObject PlayerText;


    //List To Hold "PlayerInfo" Objects
    List<PlayerInfo> collectedStats;

    public MeshRenderer[] rs;
    public bool shown, prevShown = true;


    // Use this for initialization
    public void Start()
    {
        collectedStats = new List<PlayerInfo>();
        LoadLeaderBoard();

        rs = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in rs)
            r.enabled = false;
        print("shown = " + shown + ", prevShown = " + prevShown);
    }

    // Shows the leaderboard (enables it) on-screen for the player
    public void Show()
    {
        shown = true;
        print("shown = " + shown + ", prevShown = " + prevShown);
    }

    // Saves a new entry (result) to the csv file
    public void SaveResult(string name, int time, float degrees)
    {
        string csvLine = name + "," + time.ToString() + "," + degrees.ToString();
        System.IO.File.AppendAllText("leaderboardData.csv", csvLine + Environment.NewLine);
    }

    // Update is called once per frame
    void Update()
    {
        if (shown && !prevShown)
        {
            foreach (MeshRenderer r in rs)
                r.enabled = true;
            prevShown = true;
            print("Leaderboard enabled");
        } else if (prevShown && !shown)
        {
            foreach (MeshRenderer r in rs)
                r.enabled = false;
            prevShown = false;
            print("Leaderboard disabled");
        }
    }

    public void SubmitButton()
    {
        //Create Object Using Values From InputFields, This Is Done So That A Name And Score Can Easily Be Moved/Sorted At The Same Time
        PlayerInfo stats = new PlayerInfo(name.text, int.Parse(score.text));//Depending On How You Obtain The Score, It May Be Necessary To Parse To Integer

        //Add The New Player Info To The List
        collectedStats.Add(stats);

        //Clear InputFields Now That The Object Has Been Created
        name.text = "";
        score.text = "";

        //Start Sorting Method To Place Object In Correct Index Of List
        SortStats();
    }

    public void SortStats()
    {
        //Start At The End Of The List And Compare The Score To The Number Above It
        for (int i = collectedStats.Count - 1; i > 0; i--)
        {
            //If The Current Score Is Higher Than The Score Above It , Swap
            if (collectedStats[i].score > collectedStats[i - 1].score)
            {
                //Temporary variable to hold small score
                PlayerInfo tempInfo = collectedStats[i - 1];

                // Replace small score with big score
                collectedStats[i - 1] = collectedStats[i];
                //Set small score closer to the end of the list by placing it at "i" rather than "i-1" 
                collectedStats[i] = tempInfo;
            }
        }

        //Update PlayerPref That Stores Leaderboard Values
        UpdatePlayerPrefsString();
    }

    public void UpdatePlayerPrefsString()
    {
        //Start With A Blank String
        string stats = "";

        //Add Each Name And Score From The Collection To The String
        for (int i = 0; i < collectedStats.Count; i++)
        {
            //Be Sure To Add A Comma To Both The Name And Score, It Will Be Used To Separate The String Later
            stats += collectedStats[i].name + ",";
            stats += collectedStats[i].score + ",";
        }

        //Add The String To The PlayerPrefs, This Allows The Information To Be Saved Even When The Game Is Turned Off
        PlayerPrefs.SetString("LeaderBoards", stats);

        //Now Update The On Screen LeaderBoard
        UpdateLeaderBoardVisual();
    }

    public void UpdateLeaderBoardVisual()
    {
        //Clear Current Displayed LeaderBoard
        //display.text = "";

        //Simply Loop Through The List And Add The Name And Score To The Display Text
        for (int i = 0; i <= collectedStats.Count - 1; i++)
        {
            //display.text += collectedStats[i].name + " : " + collectedStats[i].score + "\n";
        }
    }

    public void LoadLeaderBoard()
    {
        //Load The String Of The Leaderboard That Was Saved In The "UpdatePlayerPrefsString" Method
        string stats = PlayerPrefs.GetString("LeaderBoards", "");

        //Assign The String To An Array And Split Using The Comma Character
        //This Will Remove The Comma From The String, And Leave Behind The Separated Name And Score
        string[] stats2 = stats.Split(',');

        //Loop Through The Array 2 At A Time Collecting Both The Name And Score
        for (int i = 0; i < stats2.Length - 2; i += 2)
        {
            //Use The Collected Information To Create An Object
            PlayerInfo loadedInfo = new PlayerInfo(stats2[i], int.Parse(stats2[i + 1]));

            //Add The Object To The List
            collectedStats.Add(loadedInfo);

            //Update On Screen LeaderBoard
            UpdateLeaderBoardVisual();
        }
    }

    public void ClearPrefs()
    {
        //Use This To Delete All Names And Scores From The LeaderBoard
        PlayerPrefs.DeleteAll();

        //Clear Current Displayed LeaderBoard
        //display.text = "";
        name.text = "";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float timeTaken;
    public int pelletScore = 1;
    public int energizerScore = 10;
    public int currentScore; // total score would be 269 + 40 (4 energizers)
    private int ghostTotal = 4;

    // private int lives = 3;

    // private GameObject[] pellets;
    public int pelletCount;

    // private GameObject[] energizers;
    public int energizerCount;

    public enum GameStates
    {
        win,
        gameOver,
        respawn,
        inGame
    }

    public GameStates state;

    void Awake() 
    {
        state = GameStates.respawn;
        currentScore = 0;
        pelletCount = GetPelletCount();
        energizerCount = GetEnergizerCount();
    }

    void Start()
    {
        // start timer?
        state = GameStates.inGame;
    }

    // Update is called once per frame
    void Update()
    {
        currentScore = UpdateScore();

        pelletCount = GetPelletCount();
        energizerCount = GetEnergizerCount();
        
        if (pelletCount == 0 && energizerCount == 0)
        {
            state = GameStates.win;
        }

        /*if (GhostCollidePlayer())
        {
            state = GameStates.gameOver;
        }*/

        
    }

    public int GetPelletCount()
    {
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        int count = pellets.Length;
        return count;
    }

    public int GetEnergizerCount()
    {
        GameObject[] energizers = GameObject.FindGameObjectsWithTag("Energizer");
        int count = energizers.Length;
        return count;
    }

    public int UpdateScore()
    {
        int pCount = GetPelletCount();
        int eCount = GetEnergizerCount();
        int pDifference = Math.Abs(pelletCount - pCount);
        int eDifference = Math.Abs(energizerCount - eCount);
        
        //int score = currentScore + pDifference + (eDifference * 10);
        int score = currentScore + pDifference + (eDifference * 10);
        return score;
    }
}

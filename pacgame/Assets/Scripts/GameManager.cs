using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private float startTime;
    public float endTime;
    public int pelletScore = 1;
    public int energizerScore = 10;
    public int currentScore; // total score would be 269 + 40 (4 energizers)
    public int ghostTotal = 4;
    // private int lives = 3;
    // private GameObject[] pellets;
    public int pelletCount;
    // private GameObject[] energizers;
    public int energizerCount;

    // public int collectRate = 0; // collection rate per 10 sec
    // private int secRate = 10;

    public GameObject ghosts; // given in inspector
    public GameObject player;

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
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        endTime = Time.time - startTime;
        // CheckPlayerCaught();
        currentScore = UpdateScore();

        pelletCount = GetPelletCount();
        energizerCount = GetEnergizerCount();
        
        if (pelletCount == 0 && energizerCount == 0)
        {
            state = GameStates.win;
        }

        // if (state == GameStates.inGame)
        // {
            // start timer
        // }

        /*if (state == GameStates.inGame) {
            endTime = t;
            if (endTime - startTime >= secRate) { // after 10 seconds interval passed
                // calculate pellet collection rate         
            }
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
        int score = currentScore;
        if (state == GameStates.inGame) {
            int pCount = GetPelletCount();
            int eCount = GetEnergizerCount();
            int pDifference = Math.Abs(pelletCount - pCount);
            int eDifference = Math.Abs(energizerCount - eCount);
        
            //int score = currentScore + pDifference + (eDifference * 10);
            score = currentScore + pDifference + (eDifference * 10);
        }
        return score;
    }

    /*public void CheckPlayerCaught()
    {

        for (int i = 0; i < 4; i++)
        {
            GameObject ghost = ghosts.transform.GetChild(i).gameObject;
            if (ghost.layer == 7)
            {
                if (ghost.GetComponent<EnemyController>().playerCaught)
                {
                    state = GameStates.gameOver;
                }
            }
        }
    }*/


    public void ResetScene()
    {
        // SceneManager.LoadScene("Game");
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * GameManager class oversees game state
*/
public class GameManager : MonoBehaviour
{
    private float startTime;
    public float endTime;
    public int pelletScore = 1; // Score for one pellet
    public int energizerScore = 10; // Score for one energizer 
    public int currentScore; // Total score would be 269 + 40 (4 energizers)
    public int ghostTotal = 4; 
    public int pelletCount; // Variable to keep track of pellet count in game
    public int energizerCount; // Variable to keep track of energizer count

    // Collection of game states 
    public enum GameStates
    {
        win,
        gameOver,
        respawn,
        inGame
    }

    // Current game state 
    public GameStates state;

    /** 
     * Awake is the first method called when a Scene is loaded.
     * This method runs once per scene, and lasts until a Scene is unloaded. 
     * For initializing variables or states before the game starts.
    */
    void Awake() 
    {
        state = GameStates.respawn;
        currentScore = 0;
        pelletCount = GetPelletCount();
        energizerCount = GetEnergizerCount();
    }

    /**
     * Start is called before the first call to Update. 
     * This method runs once per scene.
     * Occurs at the very start of the game, the first frame.  
    */
    void Start()
    {
        // Start timer
        state = GameStates.inGame;
        startTime = Time.time;
    }

    /** 
     * Update is called once per frame.
     * Continually updates end time, score, pellet/energizer count, and game state.
    */
    void Update()
    {
        endTime = Time.time - startTime;
        currentScore = UpdateScore();

        pelletCount = GetPelletCount();
        energizerCount = GetEnergizerCount();
        
        // Win if pellet and energizer counts are both 0
        if (pelletCount == 0 && energizerCount == 0)
        {
            state = GameStates.win;
        }
    }
    
    /**
     * Retrieves pellet count.
     * @return The amount of pellets currently in game. 
    */
    public int GetPelletCount()
    {
        GameObject[] pellets = GameObject.FindGameObjectsWithTag("Pellet");
        int count = pellets.Length;
        return count;
    }


    /**
     * Retrieves energizer count.
     * @return The amount of energizers currently in game. 
    */
    public int GetEnergizerCount()
    {
        GameObject[] energizers = GameObject.FindGameObjectsWithTag("Energizer");
        int count = energizers.Length;
        return count;
    }

    /**
     * Updates score.
     * @return The current score. 
    */
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
}

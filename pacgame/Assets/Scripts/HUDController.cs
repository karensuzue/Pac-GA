using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 * The HUDController class is in charge of the game's HUD.
 * Additionally, it generates game-over or win pop-ups at the end of each run.
*/
public class HUDController : MonoBehaviour
{
    public Text scoreText; // The Text object containing the score in string format
    public Text timerText; // The Text object containing the play time in string format

    public GameObject endPopup; // The GameObject object that houses the end game pop-up

    public int currentScore; // Integer variable holding the current score

    public GameObject gameManagerObj; // Reference to the GameManager GameObject instance in Hierarchy
    public GameManager gameManager; // GameManager class obtained from the GameManager GameObject object

    public GameManager.GameStates state; // The current state of the game (win, lose, etc.)

    /** 
     * Awake is called at the very start when a Scene is loaded, before all other methods.
     * This method runs once per scene, and lasts until a Scene is unloaded. 
     * For initializing variables or states before the game starts.
    */
    void Awake()
    {
        gameManager = gameManagerObj.GetComponent<GameManager>(); // Obtain GameManager class from GameManager object
        currentScore = gameManager.currentScore; // Obtain current score from GameManager class
        scoreText.text = "Score: " + currentScore.ToString(); // Set text for score in HUD
        state = gameManager.state; // Obtain current game state from GameManager class
        endPopup.SetActive(false); // Pop-up inactive
    }

    /** 
     * Update is called once per frame. Constantly reupdates information for HUD.
    */
    void Update()
    {
        currentScore = gameManager.currentScore; // Obtain current score from GameManager class
        scoreText.text = "Score: " + currentScore.ToString(); // Set text for score in HUD

        state = gameManager.state;
        if (state == GameManager.GameStates.win) // If current game state is 'win' (player won)
        {
            endPopup.SetActive(true); // Pop-up active, appears on screen
            endPopup.transform.GetChild(0).gameObject.SetActive(true); // 'Win' Text active
            endPopup.transform.GetChild(1).gameObject.SetActive(false); // 'Loss' Text inactive
        }
        
        else if (state == GameManager.GameStates.gameOver) // If current game state is 'gameOver' (player lost)
        {
            endPopup.SetActive(true); // Pop-up active
            endPopup.transform.GetChild(0).gameObject.SetActive(false); // 'Win' Text inactive
            endPopup.transform.GetChild(1).gameObject.SetActive(true); // 'Loss' text active
        }
        else // If game is still ongoing, has not ended 
        {
            timerText.text = "Time: " + gameManager.endTime.ToString(); // Update text for time 
        }
    }

    /**
     * Method for reloading scenes. Called when clicking 'Play Again' button in pop-up.
     * Uncomment depending on scene/game mode chosen.
     * Two game modes are available: "Game" and "Game GA".
    */
    public void RestartButton()
    {
        // For scene "Game"
        // SceneManager.LoadScene("Game");

        // For scene "Game GA"
        SceneManager.LoadScene("Game GA");
    }
}

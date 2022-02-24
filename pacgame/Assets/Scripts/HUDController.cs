using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text scoreText;
    public Text timerText;

    public GameObject endPopup;

    public int currentScore;
    // public float startTime;
    // public float endTime;

    public GameObject gameManagerObj;
    public GameManager gameManager;

    public GameManager.GameStates state;

    void Awake()
    {
        gameManager = gameManagerObj.GetComponent<GameManager>();
        currentScore = gameManager.currentScore;
        scoreText.text = "Score: " + currentScore.ToString();
        state = gameManager.state;
        endPopup.SetActive(false);
    }

    void Start()
    {
        // startTime = Time.time;
    }


    // Update is called once per frame
    void Update()
    {
        currentScore = gameManager.currentScore;
        scoreText.text = "Score: " + currentScore.ToString();

        // float t = Time.time - startTime;

        state = gameManager.state;
        if (state == GameManager.GameStates.win)
        {
            endPopup.SetActive(true);
            endPopup.transform.GetChild(0).gameObject.SetActive(true);
            endPopup.transform.GetChild(1).gameObject.SetActive(false);
        }
        
        else if (state == GameManager.GameStates.gameOver)
        {
            endPopup.SetActive(true);
            endPopup.transform.GetChild(0).gameObject.SetActive(false);
            endPopup.transform.GetChild(1).gameObject.SetActive(true);
        }
        else 
        {
            // endTime = t;
            timerText.text = "Time: " + gameManager.endTime.ToString();
        }
    }

    public void RestartButton()
    {
        gameManager.ResetScene();
    }
}

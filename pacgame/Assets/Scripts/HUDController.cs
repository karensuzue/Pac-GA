using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    // Start is called before the first frame update
    public Text scoreText;
    public GameObject endPopup;

    public int currentScore;
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

    // Update is called once per frame
    void Update()
    {
        currentScore = gameManager.currentScore;
        scoreText.text = "Score: " + currentScore.ToString();
        state = gameManager.state;

        if (state == GameManager.GameStates.win)
        {
            endPopup.SetActive(true);
        }
    }
}

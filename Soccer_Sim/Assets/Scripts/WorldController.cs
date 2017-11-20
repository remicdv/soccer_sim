using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour {

    GameObject canvas;
    Transform child;
    Text scoreText;

	// Use this for initialization
	void Start () {

        canvas = GameObject.Find("Canvas");
        child = canvas.transform.Find("ScoreLabel");
        scoreText = child.GetComponent<Text>();
		
	}

    void GoalScored(string team)
    {
        if (team == "Red")
        {
            ++GameConstants.RedScore;
            scoreText.text = "Score : " + GameConstants.BlueScore + " - " + GameConstants.RedScore;
        }
        else if (team == "Blue")
        {
            ++GameConstants.BlueScore;
            scoreText.text = "Score : " + GameConstants.BlueScore + " - " + GameConstants.RedScore;

        }
    }
	
	// Update is called once per frame
	void Update () {

		
	}
}

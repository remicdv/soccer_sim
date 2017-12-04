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
            scoreText.text = "Score : " + GameConstants.RedScore + " - " + GameConstants.BlueScore;
        }
        else if (team == "Blue")
        {
            ++GameConstants.BlueScore;
            scoreText.text = "Score : " + GameConstants.RedScore + " - " + GameConstants.BlueScore;

        }
        GameObject[] player = GameObject.FindGameObjectsWithTag("RedTeam");
        GameObject[] player1 = GameObject.FindGameObjectsWithTag("BlueTeam");
        foreach (GameObject p in player)
        {
            AI_Player a = p.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                p.transform.position = a.startPos;
                a.stateMachine.ChangeState(FirstState.Instance);
            }
            p.GetComponent<Rigidbody>().velocity *= 0;

        }

        foreach (GameObject p in player1)
        {
            AI_Player a = p.GetComponent("AI_Player") as AI_Player;
            if (a != null)
            {
                p.transform.position = a.startPos;
                a.stateMachine.ChangeState(FirstState.Instance);
            }
            p.GetComponent<Rigidbody>().velocity *= 0;

        }
    }
	
	// Update is called once per frame
	void Update () {

		
	}
}

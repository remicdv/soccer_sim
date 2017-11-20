using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour {

    GameObject world;
    GameObject ball;

    // Use this for initialization
    void Start () {

        world = GameObject.Find("World");
        ball = GameObject.Find("Ball");

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Ball" && name == "BlueGoal")
        {
            Debug.Log("Blue goal");
            world.SendMessage("GoalScored", "Blue");
            ball.transform.position = new Vector3(0, 1.5f, 0);
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
        else if (other.name == "Ball" && name == "RedGoal")
        {
            Debug.Log("Red goal");
            world.SendMessage("GoalScored", "Red");
            ball.transform.position = new Vector3(0, 1.5f, 0);
            ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        }
    }
}

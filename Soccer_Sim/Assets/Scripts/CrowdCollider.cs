using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdCollider : MonoBehaviour {
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
        if (other.name == "Ball" && name == "Crowd_blue" || other.name == "Ball" && name == "Crowd_red")
        {
            world.SendMessage("CrowdCollide", "Crazy");
        }
        else if(other.name == "Ball" && name == "Crowd_blue_mid" || other.name == "Ball" && name == "Crowd_red_mid")
        {
            world.SendMessage("CrowdCollide", "Mid");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Ball" && name == "Crowd_blue" || other.name == "Ball" && name == "Crowd_red")
        {
            world.SendMessage("CrowdExit", "Crazy");
        }
        else if (other.name == "Ball" && name == "Crowd_blue_mid" || other.name == "Ball" && name == "Crowd_red_mid")
        {
            world.SendMessage("CrowdExit", "Mid");
        }
    }

}

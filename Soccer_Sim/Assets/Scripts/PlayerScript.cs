using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    float speed = 30f;
    bool test = false;
	// Use this for initialization
	void Start () {

	}
    
    // Update is called once per frame
    void Update () {

        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        GameObject ball = GameObject.Find("Ball");

        if(!test)
            transform.position = Vector3.MoveTowards(transform.position, ball.transform.position, 1f);
    }
}

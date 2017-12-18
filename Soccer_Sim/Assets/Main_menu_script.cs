using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Main_menu_script : MonoBehaviour {

	public Texture[] image;

	public float image_speed = 0.5f;
	private float timer = 0f;
	private float timer_text = 0f;
	private int current_image = 0;

	public GameObject text;
	private bool display_text = true;
	public float text_speed = 0.25f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		timer_text += Time.deltaTime;
		if (timer > image_speed) {
			current_image++;
			current_image = current_image % 2;
			GetComponent<RawImage> ().texture = image [current_image];
			timer = 0;
		}

		if (timer_text > text_speed) {
			timer_text = 0;
			display_text = !display_text;
			switch (display_text) {
			case true:
				text.GetComponent<Text> ().text = "Press Start";
				break;
			case false:
				text.GetComponent<Text> ().text = "";
				break;
			}
		}

		if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.KeypadEnter) || Input.GetKeyDown (KeyCode.Space)) {
			SceneManager.LoadScene ("sim");
		}
	}
}

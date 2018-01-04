using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu_Button : MonoBehaviour {


    public Button yourButton;
    // Use this for initialization
    void Start () {

        Button btn = yourButton.GetComponent<Button>();
        btn.onClick.AddListener(clickMenu);

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void clickMenu()
    {
        Debug.Log("click _button");
        SceneManager.LoadScene("Main_menu");
    }
}

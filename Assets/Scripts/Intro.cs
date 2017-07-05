using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class Intro : MonoBehaviour {

    private static int screenheight;
    private static int baseFontSize;

    // Use this for initialization
    void Start() {
        ResizeGUI();
    }

    // Update is called once per frame
    void FixedUpdate() {

        if (Input.GetButton("Fire1")) {
            SceneManager.LoadScene("Scene1");
        }
        if (Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene("Scene1");
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene("Credits");
        }

        if (screenheight != Screen.height) {
            ResizeGUI();
        }
    }


    void ResizeGUI() {
        screenheight = Screen.height;
        baseFontSize = screenheight / 28;

        GameObject.Find("Canvas/Title").GetComponent<Text>().fontSize = baseFontSize * 3/2;
        GameObject.Find("Canvas/Go").GetComponent<Text>().fontSize = baseFontSize;
    }




}

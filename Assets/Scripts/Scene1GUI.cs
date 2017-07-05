using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Scene1GUI : MonoBehaviour {

    private static int screenheight;
    private static int baseFontSize;

    // Use this for initialization
    void Start() {
        ResizeGUI();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (screenheight != Screen.height) {
            ResizeGUI();
        }
    }


    void ResizeGUI() {
        screenheight = Screen.height;
        baseFontSize = screenheight / 28;

        GameObject.Find("Canvas/ScoreText").GetComponent<Text>().fontSize = baseFontSize;
        GameObject.Find("Canvas/ReplayText").GetComponent<Text>().fontSize = baseFontSize;
        GameObject.Find("Canvas/GameoverText").GetComponent<Text>().fontSize = baseFontSize*3/2;
    }


}


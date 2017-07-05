using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class Credits : MonoBehaviour {

    private static int screenheight;
    private static int baseFontSize;


    // for this text to work it all needs to be space 
    // padded properly                    to here ->|
    // in VStudio use Ctrl-R,Ctrl-W 
    // to enable seeing all spaces

    static string message = @"
                      W   - Thrust                    
                     A,D  - Turn                      
                    mouse - Fire                      
                    space - Hyperjump                 
                      M   - Toggle Music              
                      P   - Pause                     
                                                      
                Big Ass Rocks -  10 pts               
                Regular Rocks -  20 pts               
                  Smithereens -  50 pts               
               Largish Saucer - 200 pts               
                  Li'l Saucer - 500 pts               
                                                      
     Finish in less than 3:00 -  10 pts/sec           
                                                      
Pilot's Notes:                                        
   Don't blow all rocks to smithereens too soon, they 
will be hard to avoid. Li'l saucers are most annoying.
An asteroid field is a really bad place to hyperjump. 
                                                      
                                                      
                                                      
                   -- CREDITS --                      
                                                      
       most game assets:  Unity Technologies          
     saucer model asset:  GamePoly, Sickleadz         
                                                      
       background music:  Fast & Furious              
                          by Kabbalistic Village      
                                                      
            inspired by Asteroids, Atari Inc.         
       some original soundfx from the Atari game      
                                                      
            produced by the logicalOctopus            
            directed by Ridley Scott (lol, jk)        
";

    // Use this for initialization
    void Start () {
        ResizeGUI();
        GameObject.Find("Canvas/Credits").GetComponent<Text>().text = message;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            SceneManager.LoadScene("Scene1");
        }
        if (Input.GetButton("Fire1")) {
            SceneManager.LoadScene("Scene1");
        }

        if (screenheight != Screen.height) {
            ResizeGUI();
        }

    }

    void ResizeGUI() {
        screenheight = Screen.height;
        baseFontSize = screenheight / 28;

        GameObject.Find("Canvas/Title").GetComponent<Text>().fontSize = baseFontSize;
        GameObject.Find("Canvas/Go").GetComponent<Text>().fontSize = baseFontSize;
        GameObject.Find("Canvas/Credits").GetComponent<Text>().fontSize = baseFontSize*2/3;
    }


}

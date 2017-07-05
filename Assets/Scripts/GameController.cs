using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ExplosionInfo {
    public Vector3 origin;
    public float strength;
}

public class GameController : MonoBehaviour {

    // SCORING SYSTEM
    static int pts_AsteroidLarge  = 10;
    static int pts_AsteroidMedium = 20;
    static int pts_AsteroidSmall  = 50;
    static int pts_SaucerLarge = 200;
    static int pts_SaucerSmall = 500;

    static int pts_TimeBonusBeforeSeconds = 180;
    static int pts_TimeBonusPerSecond = 10;

    static int init_playerLives = 3;
    static int max_playerLives = 10;

    static int bonus_every = 2000;

    static int[] level_numAsteroids = new int[] { 6, 6, 6, 6, 8, 8, 8, 8, 10 }; // ... ad infinitum
    static int[][] level_saucerAppearanceOdds = new int[][] {
        // large, small
        //new int[] { 0, 0 },
        new int[] { 1, 0 },
        new int[] { 1, 0 },
        new int[] { 3, 1 },
        new int[] { 3, 1 },
        new int[] { 1, 3 },
        new int[] { 1, 3 },
        new int[] { 0, 1 }  // ... ad infinitum
    };

    //static int bonus_every = 2000; // 2500;

    public static Bounds gameBoundary;
    public static bool init;

    public static int currentLevel;
    public static long score;


    //public int initialObstacles;
    public GameObject[] obstacles;
    public GameObject[] powerups;

    public GameObject saucerLarge;
    public GameObject saucerSmall;

    private Text scoreText;
    private Text replayText;
    private Text gameoverText;

    private int numAsteroids;
    private int playerLives;
    private float levelStartTime;
    private bool gameover;
    private float gameoverTime;
    private bool showCredits;

    private bool newSaucerAllowed;



    // Use this for initialization
    void Start() {

        // only one should exist
        //if (instance != null && instance != this) {
        if (init) { 
            Destroy(this.gameObject);
            return;
        } 


        gameBoundary = GameObject.Find("GameBoundary/Inner").GetComponent<BoxCollider>().bounds;
        Debug.Log(gameBoundary);

#if UNITY_EDITOR
        // don't hide the cursor 
        if ( !init ) {
            OnLevelWasLoaded();
        }
#else
        Cursor.visible = false;
#endif

        // This GameController persists between scenes
        DontDestroyOnLoad(gameObject);
        init = true;
    }

    void OnLevelWasLoaded() {
        Debug.Log("**LEVEL LOADED**");
        gameover = false;
        showCredits = false;

        playerLives = init_playerLives;
        currentLevel = 0;
        score = 0;

        // destroy all asteroids
        GameObject[] aster = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject ast in aster) {
            Destroy(ast);
        }

        GrabGUI();
        HyperspaceIn(true); // in the center
        ReinitLevel();
    }

    void ReinitLevel() {

        numAsteroids = 0;
        newSaucerAllowed = true;
        GameObject player = GameObject.Find("PlayerShip");

        currentLevel += 1;
        Debug.Log("Current Level "+currentLevel);

        int initialObstacles = getInitNumAsteroidsByLevel(currentLevel);
        Debug.Log("Initializing "+initialObstacles+" asteroids.");
        for (int i = 0; i < initialObstacles; i++) {

            // placed anywhere within edges but outside a specified radius from where the player is
            //GameObject.Find("edges");
            // how can I get the params from inside "Edges"?
            float x, y;
            Vector3 delta;
            do {
                x = Random.Range(-16, 16);
                y = Random.Range(-9, 9);
                delta = player.transform.position - new Vector3(x, 0.0f, y);
            } while ( delta.magnitude < 5);

            AsterInfo info = new AsterInfo();
            info.level = 1;
            info.position = new Vector3(x, 0.0f, y);
            NewAsteroid(info);
        }

        levelStartTime = Time.time;
        ClearGameover();
        UpdateScore(0);
        SpawnSaucerAfterRandomDelay();
    }

    int getInitNumAsteroidsByLevel(int level) {
        int idx = level - 1;
        int len = level_numAsteroids.Length;
        if (idx >= len) return level_numAsteroids[len - 1];
        return level_numAsteroids[idx];
    }

    int[] getSaucerOddsByLevel(int level) {
        int idx = level - 1;
        int len = level_saucerAppearanceOdds.Length;
        if (idx >= len) return level_saucerAppearanceOdds[len - 1];
        return level_saucerAppearanceOdds[idx];
    }

    void GrabGUI() {
        //Debug.Log(Screen.height);

        int fontsize = Screen.height / 30;

        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        replayText = GameObject.Find("ReplayText").GetComponent<Text>();
        gameoverText = GameObject.Find("GameoverText").GetComponent<Text>();

        scoreText.fontSize = fontsize;
        replayText.fontSize = fontsize;
        gameoverText.fontSize = fontsize*3/2;
    }

    void Update() {

        if ( gameover ) {
            if (Input.GetKeyDown(KeyCode.Return)) {
                SceneManager.LoadScene("Scene1");
            }

            if (Time.time > gameoverTime + 10 && showCredits==false) {   // ********************
                SceneManager.LoadScene("Credits");
                showCredits = true;
            }

        }
        if (Input.GetKeyDown(KeyCode.P)) {
            if (Time.timeScale == 0.0f ) {
                Time.timeScale = 1.0f;
                AudioListener.pause = false;
                gameoverText.enabled = gameover;
            }
            else {
                Time.timeScale = 0.0f;
                AudioListener.pause = true;
                gameoverText.text = "Paused";
                gameoverText.enabled = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.M)) {
            // toggle Music
            GetComponent<AudioSource>().mute = !GetComponent<AudioSource>().mute;
        }
    }


    void SetGameover() {
        gameover = true;
        gameoverTime = Time.time;
        gameoverText.text = "Game Over!";
        gameoverText.enabled = true;
        replayText.text = "'Enter' to restart";
        replayText.enabled = true;
        newSaucerAllowed = false;
    }

    void ClearGameover() {
        gameover = false;
        gameoverText.enabled = false;
        replayText.enabled = false;
    }


    void NewAsteroid(AsterInfo info) {
        numAsteroids += 1;

        // random motion is imparted when the new obstacle calls Start()
        GameObject obstacle = Instantiate(
            obstacles[Random.Range(0, obstacles.Length)],
            info.position, Random.rotation) as GameObject;

        obstacle.SendMessage("SetLevel", info.level);
    }

    void NewPowerup(AsterInfo info) {
        numAsteroids += 1; // its fine to count this as an asteroid

        // random motion is imparted when the new obstacle calls Start()
        Instantiate(
            powerups[Random.Range(0, powerups.Length)],
            info.position, Random.rotation);
    }

    void KillPlayer() {
        playerLives -= 1;
        UpdateScore(0);

        if (playerLives > 0) {
            Invoke("Respawn", 2.0f);
        }
        else {
            SetGameover();
        }
    }

    void Respawn() {
        if (gameover == false) {
            HyperspaceIn(false);
        }
    }

    void HyperspaceIn(bool center) {
        // todo add cool effect
        GameObject playerShip = GameObject.Find("PlayerShip");
        playerShip.SendMessage("HyperspaceIn",center);
    }

    void ExtraLife() {
        if ( playerLives < max_playerLives ) {
            playerLives += 1;
            UpdateScore(0);
            GameObject.Find("NewLife").GetComponent<AudioSource>().Play();
        }
    }


    void MakeExplosion(ExplosionInfo info) {
        GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
        for (int i=0;i<objs.Length;i++) {
            Rigidbody rb = objs[i].GetComponent<Rigidbody>();
            if (rb) {
                rb.AddExplosionForce(info.strength, info.origin, 10f, 0f, ForceMode.Impulse);
            }
        }
    }

    void UpdateScore(int points) {

        
        long before = score / bonus_every;
        score += points;
        long after = score / bonus_every;


        string display = "Score: " + score.ToString();
        string lives = "";

#if UNITY_WEBGL
        if ( playerLives>0 ) {
            if (playerLives == 1){
                lives = "1 life";
            }
            else {
                int n = playerLives;
                lives = n.ToString() + " lives";
            }
        }
#else 
        // one icon for each REMAINING ship
        for (int i = 1; i < playerLives; i++) {
            lives += "▲";
        }
#endif
        scoreText.text = display + "\n " + lives;

        scoreText.enabled = true;
        replayText.enabled = false;

        while (after > before) {
            // we've just crossed the bonus score again
            ExtraLife();
            before += 1;
        }
    }

    void ScoreAsteroid(int astLevel) {
        switch ( astLevel ) {
            case 1:
                UpdateScore(pts_AsteroidLarge);
                break;
            case 2:
                UpdateScore(pts_AsteroidMedium);
                break;
            case 3:
                UpdateScore(pts_AsteroidSmall);
                break;
        }
    }

    void DecrementAsteroid() {
        numAsteroids -= 1;
        // if saucer is allowed that means its not here
        if (numAsteroids == 0 && newSaucerAllowed == true) LevelCleared();
    }

    void ScoreSaucer(bool isSmall) {

        newSaucerAllowed = true;

        if (isSmall) {
            UpdateScore(pts_SaucerSmall);
        }
        else {
            UpdateScore(pts_SaucerLarge);
        }

        if (numAsteroids == 0) {
            LevelCleared();
        }
        else {
            SpawnSaucerAfterRandomDelay();
        }
    }

    void SpawnSaucerAfterRandomDelay() {
        // ater a random time, spawn a saucer
        float wait = 30.0f * Random.Range(1.0f, 2.0f) / Mathf.Pow(currentLevel,0.3f);
        Invoke("SpawnSaucer", wait);
    }

    void SpawnSaucer() {
        if (newSaucerAllowed == false) return;

        // decide whether to spawn Large or Small
        GameObject saucer;
        int[] odds = getSaucerOddsByLevel(currentLevel);

        Debug.Log("choosing between " + odds[0]+","+odds[1]);

        int range = odds[0] + odds[1];
        float value = Random.value * range; 
        if (range > 0) {
            if (value < odds[0]) {
                saucer = saucerLarge;
            }
            else {
                saucer = saucerSmall;
            }
            Instantiate(saucer);
            newSaucerAllowed = false;
        }
    }

    IEnumerator CountTimeBonus(int leveltime, int convergetime) {
        string clear = "Level Cleared!";
        string message = clear;

        while (leveltime < convergetime) {
            string min = (convergetime / 60).ToString();
            string sec = (convergetime % 60).ToString();
            if (sec.Length < 2) sec = "0" + sec;
            convergetime -= 1;
            message = clear + "\nTime bonus " + min + ":" + sec;
            gameoverText.text = message;
            UpdateScore(pts_TimeBonusPerSecond);
            yield return new WaitForSeconds(0.05f);
        }
        gameoverText.text = message;
        Invoke("ReinitLevel", 3);
    }

    void LevelCleared() {
        newSaucerAllowed = false;

        int leveltime = (int)(Time.time - levelStartTime);
        gameoverText.enabled = true;
        StartCoroutine( CountTimeBonus(leveltime,pts_TimeBonusBeforeSeconds) );
    }


}

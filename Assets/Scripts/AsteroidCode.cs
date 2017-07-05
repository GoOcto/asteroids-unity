using UnityEngine;
using System.Collections;

public class AsterInfo {
    public int level;
    public Vector3 position;
}

public class AsteroidCode : MonoBehaviour {

    public GameObject finalExplosion;

    public AudioSource sndLevel1;
    public AudioSource sndLevel2;
    public AudioSource sndLevel3;

    private int level;
    private Rigidbody rb;
    private Collider cc;
    private float outsideCount;

	// Use this for initialization
	void Start () {
        cc = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = Random.insideUnitSphere * Random.Range(.5f,4.0f);
        rb.velocity = new Vector3(
            Random.Range(0.0f, 1.0f),
            0.0f,
            Random.Range(0.0f, 1.0f)
            );
    }
	
	void FixedUpdate () {
        // limit the velocity
        float speed = Mathf.Clamp(rb.velocity.magnitude, 0.0f, 3.0f);
        rb.velocity = rb.velocity.normalized * speed;

        // count how long we are outside the gameBoundary for
        if ( cc.bounds.Intersects(GameController.gameBoundary) ) {
            outsideCount = 0.0f;
        }
        // and direct back inwards if too long
        else {
            outsideCount += Time.fixedDeltaTime;
            if ( outsideCount > 5.0f ) {
                RedirectInwards();
            }
        }
    }

    void RedirectInwards() {
        Debug.Log("GET THERE!");

        float dx = transform.position.x;
        float dy = transform.position.z;

        // direct bearing to center of screen (in radians)
        float angle = Mathf.Atan2(-dx, -dy);
        float speed = Random.Range(1.0f, 3.0f);

        // apply variance to angle
        angle += Random.Range(-45 * Mathf.Deg2Rad, 45 * Mathf.Deg2Rad);

        float x = Mathf.Sin(angle) * speed;
        float y = Mathf.Cos(angle) * speed;

        rb.velocity = new Vector3(x, 0.0f, y);
        outsideCount = 0.0f;
    }

    void SetLevel(int n) {
        level = n;
        float mult = 1 / Mathf.Pow(2, n - 1);
        // resize according to level
        transform.localScale = transform.localScale * mult;

        rb = GetComponent<Rigidbody>();
        rb.mass = rb.mass * mult*mult*mult; 
    }

    void Break(bool scorePoints) {

        if ( level<3 ) {
            AsterInfo info = new AsterInfo();
            info.level = level + 1;
            info.position = transform.position;

            // break into a number of smaller units (level=level+1)
            //if (level == 2 && Random.Range(0, 3) < 1) {
            //    GameObject.Find("GameController").SendMessage("NewAsteroid", info);
            //    GameObject.Find("GameController").SendMessage("NewAsteroid", info);
            //    GameObject.Find("GameController").SendMessage("NewPowerup", info);
            //}
            //else {
                GameObject.Find("GameController").SendMessage("NewAsteroid", info);
                GameObject.Find("GameController").SendMessage("NewAsteroid", info);
                GameObject.Find("GameController").SendMessage("NewAsteroid", info);
            //}

        }
        else {
            Instantiate(finalExplosion, transform.position, Quaternion.identity);
        }

        if (scorePoints) GameObject.Find("GameController").SendMessage("ScoreAsteroid", level);
        Explode();
    }

    void BreakByPlayer() {
        Break(true);
    }

    void BreakByEnemy() {
        Break(false);
    }

    void Explode() {
        // play the correct soundFx
        ExplosionInfo info = new ExplosionInfo();
        info.origin = transform.position;

        switch (level) {
            case 1:
                Instantiate(sndLevel1);
                info.strength = 0.1f;
                break;
            case 2:
                info.strength = 0.1f;
                Instantiate(sndLevel2);
                break;
            case 3:
                info.strength = 0.1f;
                Instantiate(sndLevel3);
                break;
        }
        GameObject.Find("GameController").SendMessage("MakeExplosion",info);
        GameObject.Find("GameController").SendMessage("DecrementAsteroid");
        Destroy(gameObject);
    }


}

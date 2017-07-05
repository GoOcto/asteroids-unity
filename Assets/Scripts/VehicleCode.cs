using UnityEngine;
using System.Collections;


public class VehicleCode : MonoBehaviour {

    public float turnForce;
    public float moveForce;
    public float moveMax;

    public GameObject[] blaster;
    public GameObject shields;

    public GameObject blast;
    public GameObject engine;
    public GameObject explosion;

    private Rigidbody rb;
    private float heading;

    private float turn;
    private float move;

    private double nextShotTime;
    private bool isAlive;
    private int hasTrishots;
    private int hasShields;

    void Start() {
        rb = GetComponent<Rigidbody>();
        heading = 0;
        Show();
        nextShotTime = Time.time;
    }

    void Fire() {
        if (isAlive) {
            double now = Time.time;
            if (now >= nextShotTime) {
                if ( hasTrishots>0 ) {
                    hasTrishots -= 1;
                    Instantiate(blast, blaster[1].transform.position, blaster[1].transform.rotation);
                    Instantiate(blast, blaster[2].transform.position, blaster[2].transform.rotation);
                }
                Instantiate(blast, blaster[0].transform.position, blaster[0].transform.rotation);
                GetComponent<AudioSource>().Play();
                nextShotTime = now + 0.2;
            }
        }
    }

    void Update() {
        if (isAlive) {
            turn = Input.GetAxis("Horizontal");
            move = Input.GetAxis("Vertical");

            if (Input.GetButton("Fire1")) {
                if (Time.timeScale != 0.0f) {
                    // if the game isn't paused
                    Fire();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                HyperspaceJump();
            }

            if (move > 0) {
                if (engine) engine.SetActive(true);
            }
            else {
                move = 0;
                if (engine) engine.SetActive(false);
            }

            // apply forces
            heading += turnForce * turn;
            rb.velocity += transform.forward * moveForce * transform.lossyScale.y * move;
            rb.rotation = Quaternion.Euler(0.0f, heading, turn * -30);

            // clamp max velocity
            float velocity = Mathf.Clamp(rb.velocity.magnitude, 0, moveMax * transform.lossyScale.y);
            rb.velocity = rb.velocity.normalized * velocity;
        }
        else {
            if (engine) engine.SetActive(false);
        }

    }

    void OnTriggerEnter(Collider other) {
        if (isAlive) {
            // I've crashed into something
            if (other.gameObject.CompareTag("Asteroid") ) { 
                Explode();
            }

            if (other.gameObject.CompareTag("Saucer")) {
                other.gameObject.SendMessage("PlayerBlast");
                Explode();
            }

            if (other.gameObject.CompareTag("Powerup")) {
                ApplyPowerup(other.gameObject);
            }
        }
    }

    void ApplyPowerup(GameObject powerup) {
        GameObject.Find("GameController").SendMessage("DecrementAsteroid");

        PowerupCode code = powerup.GetComponent<PowerupCode>();

        if ( code.type=="Life" ) {
            GameObject.Find("GameController").SendMessage("ExtraLife");
        }
        if ( code.type=="Trishot" ) {
            hasTrishots = 12;
        }
        if ( code.type=="Shield" ) {
            hasShields = 3;
            shields.SetActive(true);
        }
        Destroy(powerup);
    }

    void Explode() {
        if (hasShields > 0) {
            hasShields -= 1;
            if (hasShields<1) {
                shields.SetActive(false);
            }
        }
        else {
            hasTrishots = 0;
            hasShields = 0;

            Instantiate(explosion, transform.position, Quaternion.identity);
            Hide();
            GameObject.Find("GameController").SendMessage("KillPlayer");

            ExplosionInfo info = new ExplosionInfo();
            info.origin = transform.position;
            info.strength = 1.0f;
            GameObject.Find("GameController").SendMessage("MakeExplosion", info);
        }
    }

    void HyperspaceJump() {
        Hide();
        HyperspaceIn();
    }

    void HyperspaceIn(bool center=false) {

        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.rotation = Quaternion.identity;

        if (center) {
            // center
            transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            heading = 0.0f;
        }
        else {
            // random
            float x, y, a;
            x = Random.Range(-15, 15);
            y = Random.Range(-8, 8);
            a = Random.Range(0.0f, 360.0f);

            transform.position = new Vector3(x, 0.0f, y);
            heading = a;
        }

        ExplosionInfo info = new ExplosionInfo();
        info.origin = transform.position;
        info.strength = 0.1f;
        GameObject.Find("GameController").SendMessage("MakeExplosion",info);
        Show();
    }

    void Hide() {
        GetComponent<Renderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        isAlive = false;
    }

    void Show() {
        GetComponent<Renderer>().enabled = true;
        GetComponent<MeshCollider>().enabled = true;
        isAlive = true;
    }


}

using UnityEngine;
using System.Collections;

public class PowerupCode : MonoBehaviour {

    public string type;

    private Rigidbody rb;
    private Collider cc;
    private AudioSource sound;
    private float outsideCount;

	// Use this for initialization
	void Start () {
        cc = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        sound = GetComponent<AudioSource>();

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

    void Clear() {
        // TODO make a sound
        sound.Play();

        // this counts as an asteroid for purposes on ClearLevel
        GameObject.Find("GameController").SendMessage("DecrementAsteroid");
        Destroy(gameObject);
    }

}

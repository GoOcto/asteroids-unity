using UnityEngine;
using System.Collections;

public class SaucerCode : MonoBehaviour {

    public GameObject torpedo;
    public GameObject explosion;

    public float spin;
    public float speed;
    public bool isSmall;

    private Rigidbody rb;
    private Vector3 vector;
    private float turn;


    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = new Vector3(0.0f, 0.0f, 6.0f);
        rb.transform.rotation = Quaternion.Euler(spin, 0.0f, 0.0f);

        // decide where to start
        StartAtRandomEdge();
        Invoke("Shoot", 1.5f);
    }

    void FixedUpdate() {
        turn += 3.0f*360.0f * Time.fixedDeltaTime;
        float wobble = 0.01f * turn;
        float wobx = Mathf.Cos(wobble);
        float woby = Mathf.Sin(wobble);

        transform.position = transform.position + vector*Time.fixedDeltaTime;
        transform.rotation = Quaternion.Euler(15.0f*wobx, 15.0f*woby, turn);
    }

    void Shoot() {
        GameObject player = GameObject.Find("PlayerShip");
        Vector3 direction = player.transform.position - transform.position;
        direction = direction.normalized;

        // direct bearing to playerShip (in dgrees)
        float angle = Mathf.Atan2(direction.x, direction.z) * 180.0f/Mathf.PI;

        // random variance (in degrees)
        float vary = Random.Range(-30.0f, 30.0f) + Random.Range(-45.0f, 45.0f);

        // even narrower for higher levels
        if ( isSmall ) {
            vary /= ( GameController.currentLevel - 1 );
        }

        Instantiate(torpedo, transform.position, Quaternion.Euler(0.0f,angle+vary,0.0f));

        // shoot again after a delay
        Invoke("Shoot", Random.Range(1.5f,2.5f));
    }

    void StartAtRandomEdge() {
        float size = 3.0f * transform.localScale.y;

        float x, y;
        // whats the best way to get the game rectangle?
        int xe = Random.Range(0.0f, 1.0f) < 0.5f ? -1 : +1;
        int ye = Random.Range(0.0f, 1.0f) < 0.5f ? -1 : +1;
        x = xe * (16 + size);
        y = ye * (9 + size);

        transform.position = new Vector3(x, 0.0f, y);
        ChangeVector(); // straight for the center
    }

    void ChangeVector() {
        float dx = transform.position.x;
        float dy = transform.position.z;

        // direct bearing to center of screen (in radians)
        float angle = Mathf.Atan2(-dx,-dy);
        float distance = Mathf.Sqrt(dx*dx + dy*dy);

        // variance (in degrees) depends on distance from center
        float vary = Mathf.Lerp(180, 60, distance / 9.0f); 

        // apply variance to angle
        angle += Random.Range(-vary*Mathf.Deg2Rad, vary*Mathf.Deg2Rad);

        float x = Mathf.Sin(angle) * speed;
        float y = Mathf.Cos(angle) * speed;

        vector = new Vector3(x, 0.0f, y);

        // change again shortly
        Invoke("ChangeVector", Random.Range(0.8f, 3.0f));
    }

    void PlayerBlast() {
        // player has hit this saucer
        Instantiate(explosion,transform.position,Quaternion.identity);
        Destroy(gameObject);

        ExplosionInfo info = new ExplosionInfo();
        info.origin = transform.position;
        info.strength = 2.0f;
        GameObject.Find("GameController").SendMessage("MakeExplosion", info);

        GameObject.Find("GameController").SendMessage("ScoreSaucer",isSmall);
    }


}

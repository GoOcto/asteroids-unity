using UnityEngine;
using System.Collections;

public class TorpedoCode : MonoBehaviour {

    private float speed;
    private float spin;
    private Vector3 vector;

    // Use this for initialization
    void Start() {
        spin = 0.0f;
        speed = 20.0f * transform.lossyScale.y;
        vector = transform.forward;
    }

    void Update() {
        // randomly enable 2 of the 4 quads
        MeshRenderer[] quads = GetComponentsInChildren<MeshRenderer>();
        int n = Random.Range(0, 6);
        switch (n) {
            case 0:
                quads[0].enabled = true;
                quads[1].enabled = true;
                quads[2].enabled = false;
                quads[3].enabled = false;
                break;
            case 1:
                quads[0].enabled = true;
                quads[1].enabled = false;
                quads[2].enabled = true;
                quads[3].enabled = false;
                break;
            case 2:
                quads[0].enabled = true;
                quads[1].enabled = false;
                quads[2].enabled = false;
                quads[3].enabled = true;
                break;
            case 3:
                quads[0].enabled = false;
                quads[1].enabled = true;
                quads[2].enabled = true;
                quads[3].enabled = false;
                break;
            case 4:
                quads[0].enabled = false;
                quads[1].enabled = true;
                quads[2].enabled = false;
                quads[3].enabled = true;
                break;
            case 5:
                quads[0].enabled = false;
                quads[1].enabled = false;
                quads[2].enabled = true;
                quads[3].enabled = true;
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 position = transform.position;
        transform.position = position + vector * speed * Time.fixedDeltaTime;

        spin += 17.0f;
        transform.rotation = Quaternion.Euler(0.0f, spin, 0.0f);
    }

    void OnTriggerEnter(Collider other) {

        if (other.gameObject.CompareTag("Asteroid")) {
            other.gameObject.SendMessage("BreakByEnemy");
            //remove this bullet
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.CompareTag("Player")) {
            other.gameObject.SendMessage("Explode");
            //remove this bullet
            Destroy(gameObject);
            return;
        }

    }


}

using UnityEngine;
using System.Collections;

public class ExplosionDie : MonoBehaviour {

    private float lifetime = 2.0f;
    private float dieWhen;

	// Use this for initialization
	void Start () {
        dieWhen = Time.time + lifetime;
	}
	
	// Update is called once per frame
	void Update () {
	    if ( Time.time > dieWhen ) {
            Destroy(this.gameObject);
        }
	}
}

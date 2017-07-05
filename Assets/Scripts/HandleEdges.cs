using UnityEngine;
using System.Collections;

public class HandleEdges : MonoBehaviour {

    public float left;
    public float right;
    public float top;
    public float bottom;


    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnTriggerExit(Collider other) {

        if (name == "Inner") {

            if (other.gameObject.CompareTag("Weapon")) {
                Destroy(other.gameObject);
                return;
            }

            // wrap it back to the opposite edge
            Vector3 position = other.transform.position;
            float extra = 0.0f;

            if (other.gameObject.CompareTag("Asteroid")) {
                extra = CalcSize(other.gameObject); // what is the size of this asteroid?
            }


            if (position.x < left) {
                position.x += (right - left) + extra;
            }
            if (position.x > right) {
                position.x -= (right - left) + extra;
            }
            if (position.z < bottom) {
                position.z += (top - bottom) + extra;
            }
            if (position.z > top) {
                position.z -= (top - bottom) + extra;
            }

            other.gameObject.transform.position = position;
            return;
        }

        // safety net for stray asteroids (only happens to asteroids, because they might have peculiar vectors)
        if (name == "Outer") {



            // wrap it back to the opposite edge
            Vector3 position = other.transform.position;
            float extra = 0.0f;
            float margin = 3.0f;

            if (other.gameObject.CompareTag("Asteroid")) {
                extra = CalcSize(other.gameObject); // what is the size of this asteroid?
            }

            if (position.x < left-margin) {
                position.x += (right - left) + extra+margin;
            }
            if (position.x > right+margin) {
                position.x -= (right - left) + extra+margin;
            }
            if (position.z < bottom-margin) {
                position.z += (top - bottom) + extra+margin;
            }
            if (position.z > top+margin) {
                position.z -= (top - bottom) + extra+margin;
            }

            // it's probably a good idea to deflect its trajectory by 45 degrees if this happened
            Vector3 velocity = other.gameObject.GetComponent<Rigidbody>().velocity;
            velocity = Quaternion.Euler(0,45,0) * velocity;
            float speed = Mathf.Clamp(velocity.magnitude, 2.0f, 3.0f);

            other.gameObject.transform.position = position;
            other.gameObject.GetComponent<Rigidbody>().velocity = velocity*speed;
            return;
        }

    }

    float CalcSize(GameObject obj) {
        //return obj.GetComponent<Collider>().height * obj.transform.localScale.y;
        return 3f;
    }

}

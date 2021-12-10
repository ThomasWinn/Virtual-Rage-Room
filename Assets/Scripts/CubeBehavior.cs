using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehavior : MonoBehaviour
{

    public float speed = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        //this.GetComponent<Rigidbody>().velocity = new Vector3(0,0,-speed);
    }

    // Update is called once per frame
    void Update()
    {
        
        Vector3 position = this.transform.localPosition;
        position.z -= speed * Time.deltaTime;
        this.transform.localPosition = position;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Wall (3)")
        {
            
            Destroy(this.gameObject);
            // collision.gameObject.GetComponent<Renderer>().enabled = false;
        }
        // Debug.Log(collision.gameObject.name);
    }
}

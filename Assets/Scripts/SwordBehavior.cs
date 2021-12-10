using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwordBehavior : MonoBehaviour

{
    private bool destroyed = false;
    Vector3 lastPosition;
    float lastSwingTime;
    float swingAngle;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = this.transform.position;
        lastSwingTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed)
        {
            AudioSource audio = GetComponent<AudioSource>();
            destroyed = false;
            audio.Play();
            
        }
        Vector3 velocity = (this.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = this.transform.position;

        if (velocity.magnitude > 5 && (Time.time - lastSwingTime) > 1)
        {
            velocity.z = 0;
            velocity.Normalize();

            float dotProduct = Vector3.Dot(Vector3.up, velocity);
            swingAngle = Mathf.Acos(dotProduct);

            if (velocity.x < 0)
            {
                swingAngle *= -1.0f;
            }

            swingAngle = swingAngle * 180 / Mathf.PI;
            //Debug.Log(swingAngle);
            //ti.text = "" + swingAngle;
            lastSwingTime = Time.time;
           
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // TOLERANCE 15
        // EULER ANGLE ROTATION
        // DOWN = 0 
        // RIGHT = 90
        // LEFT = 270
        // UP = 180

        // DEFINE CUBE
        // 0 = down
        // 1 = right
        // 2 = left
        // 3 = up
        var cube_z = collision.gameObject.transform.eulerAngles.z;

        int cube_type = 100;

        if ((cube_z <= 15 && cube_z >= 0) || (cube_z <= 360 && cube_z >= 345))
        {
            cube_type = 0;
            Debug.Log("DOWN");
        }
        else if (cube_z <= 105 && cube_z >= 75)
        {
            cube_type = 1;
            Debug.Log("RIGHT");
        }
        else if (cube_z <= 285 && cube_z >= 255)
        {
            cube_type = 2;
            Debug.Log("LEFT");
        }
        else if (cube_z <= 195 && cube_z >= 165)
        {
            cube_type = 3;
            Debug.Log("UP");
        }
        else
        {
            cube_type = 100;
            Debug.Log("Unknown Cube Rot: " + collision.gameObject.transform.eulerAngles.z);
        }

        Debug.Log(swingAngle);

        if (collision.gameObject.name == "cubey_blue" || collision.gameObject.name == "cubey_blue(Clone)")
        {
            switch (cube_type)
            {
                case 0:
                    if ((swingAngle >= -180 && swingAngle <= -126) || (swingAngle >= 126 && swingAngle <= 180))
                    {
                        Debug.Log(cube_type);
                        Debug.Log("DOWN DESTROY");
                        destroyed = true;
                        Destroy(collision.gameObject);
                        //break;
                    }
                    break;
                case 1:
                    if (swingAngle >= 35 && swingAngle <= 125)
                    {
                        Debug.Log(cube_type);
                        Debug.Log("RIGHT DESTROY");
                        destroyed = true;
                        Destroy(collision.gameObject);
                        //break;
                    }
                    break;
                case 2:
                    if (swingAngle >= -125 && swingAngle <= -35)
                    {
                        Debug.Log(cube_type);
                        Debug.Log("LEFT DESTROY");
                        destroyed = true;
                        Destroy(collision.gameObject);
                        //break;
                    }
                    break;
                case 3:
                    if (swingAngle >= -34 && swingAngle <= 34)
                    {
                        Debug.Log(cube_type);
                        Debug.Log("UP DESTROY");
                        destroyed = true;
                        Destroy(collision.gameObject);
                        //break;
                    }
                    break;
                default:
                    Debug.Log("FLDJKSJFLKDSJFDSLKFJDSLKFJSDLKFJDLSKFJDLSKFJDSLKFJDSLKFJDSLK");
                    break;
            }
        }


        // SWINGS 
        // UP = -34 -> 34
        // LEFT = -35 -> -125
        // RIGHT = 35 -> 125
        // DOWN = +/- -126 -> -180
        ///Debug.Log(cube_type);
        //Debug.Log(swingAngle);

        /*
        // DOWN
        if ((collision.gameObject.name == "cubey_blue" || collision.gameObject.name == "cubey_blue(Clone)") && (cube_type == 0 && (swingAngle >= -180 && swingAngle <= -126) || (swingAngle >= 126 && swingAngle <= 180))) 
        {
            // Debug.Log(collision.gameObject.name);
            destroyed = true;
            Destroy(collision.gameObject);
            // collision.gameObject.GetComponent<Renderer>().enabled = false;
        }
        // RIGHT
        else if ((collision.gameObject.name == "cubey_blue" || collision.gameObject.name == "cubey_blue(Clone)") && (cube_type == 1 && (swingAngle >= 35 && swingAngle <= 125)))
        {
            destroyed = true;
            Destroy(collision.gameObject);
        }
        // LEFT
        else if ((collision.gameObject.name == "cubey_blue" || collision.gameObject.name == "cubey_blue(Clone)") && (cube_type == 2 && (swingAngle >= -125 && swingAngle <= -35)))
        {
            destroyed = true;
            Destroy(collision.gameObject);
        }
        // UP
        else if ((collision.gameObject.name == "cubey_blue" || collision.gameObject.name == "cubey_blue(Clone)") && (cube_type == 3 && (swingAngle >= -34 && swingAngle <= 34)))
        {
            destroyed = true;
            Destroy(collision.gameObject);
        }
        //Debug.Log(collision.gameObject.name);
        */
    }
}

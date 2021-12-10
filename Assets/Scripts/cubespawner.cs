using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cubespawner : MonoBehaviour
{
    public GameObject go;
    public GameObject red;
    public float currentTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > currentTime)
        {
            //Random random = new Random();
            var newIncrement = Random.Range(0.5f, 2.0f);
            currentTime += newIncrement;
            BlockSpawn();
            
        }
    }

    void BlockSpawn()
    {
        // random spawn x and y between two values
        float[] z_val = new float[4] { 0f, 90f, 180f, 270f };
        var rx = Random.Range(-0.5f, 0.5f);
        var ry = Random.Range(0.1f, 1.0f);
        var rz = 12.0f;
        var rando = Random.Range(0, 2); // 0, 1
        
        if (rando == 0)
        {
            Instantiate(go, new Vector3(rx, ry, rz), Quaternion.Euler(0f, 0f, z_val[Random.Range(0,4)]));
        }
        else
        {
            Instantiate(red, new Vector3(rx, ry, rz), Quaternion.Euler(0f, 0f, z_val[Random.Range(0, 4)]));
        }
    }

}

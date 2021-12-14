using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GraspGrabber : Grabber
{
    public InputActionProperty grabAction;
    public InputActionProperty XButton;
    public InputActionProperty AButton;

    public GameObject player_headset;

    public GameObject other_controller;

    public GameObject mp_cube;

    Grabbable currentObject;
    Grabbable grabbedObject;

    float parentz;

    int buttonPress;

    Transform grabPoint;

    float previous_spindle_length;

    private Quaternion LastSpindleRotation;
    int this_index = -1;
    int other_index = -1;

    Mesh deformingMesh;
    Vector3[] originalVertices, newVertices, worldSpaceVertices;
    int[] thisVerticesList, otherVerticesList;
    // List<int> thisVerticesList, otherVerticesList;


    // Start is called before the first frame update
    void Start()
    {
        // grab point = child of controller. use local position to change everything?
        grabPoint = new GameObject().transform;
        grabPoint.name = "Grab Point";
        grabPoint.parent = this.transform;

        grabbedObject = null;
        currentObject = null;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;

        XButton.action.performed += XAButtonPress;
        AButton.action.performed += XAButtonPress;

        parentz = this.transform.parent.position.z;

        buttonPress = 0; // DEFULAT GOGO IS OFF
        Debug.Log("SPINDLE - ON");

    }

    private void XAButtonPress(InputAction.CallbackContext obj)
    {
        /*buttonPress += 1;
        // if odd number of presses, it's on. DEFAULT it is off.
        if (buttonPress % 2 == 1)
        {
            Debug.Log("SPINDLE - OFF");
            Debug.Log("GOGO - ON");
        }
        else
        {
            Debug.Log("GOGO - OFF");
            Debug.Log("SPINDLE - ON");
        }
        */
        grabbedObject.transform.rotation *= Quaternion.Euler(0, 0, 90);
    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;

        XButton.action.performed -= XAButtonPress;
        AButton.action.performed -= XAButtonPress;
    }

    // Update is called once per frame
    void Update()
    {

        // make mp cube visible
        mp_cube.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

        float mid_point_x = (other_controller.transform.position.x + this.transform.parent.position.x) / 2;
        float mid_point_y = (other_controller.transform.position.y + this.transform.parent.position.y) / 2;
        float mid_point_z = (other_controller.transform.position.z + this.transform.parent.position.z) / 2;


        mp_cube.transform.position = new Vector3(mid_point_x, mid_point_y, mid_point_z);
        
        if (grabbedObject != null)
        {
            // use line renderer to see spindle from user

            // set update recalculate spindle every frame and move object at that position = translation.
            grabbedObject.transform.position = new Vector3(mid_point_x, mid_point_y, mid_point_z);
            // grabbedObject.transform.position = new Vector3(mid_point_x, 0, 0);

            /*
            // WILD GROWTH

            // spindle_legnth = distance between the controllers
            // want total distance rather than x distance
            float current_spindle_length = Vector3.Distance(other_controller.transform.position, this.transform.parent.position);
            // float current_spindle_length = other_controller.transform.position.x - this.transform.parent.position.x;

            // spindle_length and previous spindle length ... current length / previous spindle length... % change
            float change_ratio = current_spindle_length / previous_spindle_length; 

            // . Scale object by % change (obj.x * % change, obj.y * % change, obj.z * % change) . Then manipulate object scale transform
            grabbedObject.transform.localScale = new Vector3(grabbedObject.transform.localScale.x * change_ratio, grabbedObject.transform.localScale.y * change_ratio, grabbedObject.transform.localScale.z * change_ratio);

            // update previous
            previous_spindle_length = current_spindle_length;
            */

            // PROF CODE

            Quaternion spindleRotation = Quaternion.LookRotation(other_controller.transform.position - this.transform.parent.position);

            Quaternion rotationChange = spindleRotation * Quaternion.Inverse(LastSpindleRotation);

            grabbedObject.transform.rotation = rotationChange * grabbedObject.transform.rotation;

            LastSpindleRotation = spindleRotation;
            float current_spindle_length = Vector3.Distance(other_controller.transform.position, this.transform.parent.position);
            float change_ratio = current_spindle_length / previous_spindle_length;
            float reverse_change_ratio = (previous_spindle_length / current_spindle_length);

            previous_spindle_length = current_spindle_length;

            for (int i = 0; i < thisVerticesList.Length; i++)
            {
                // Debug.Log(thisVerticesList[i]);
                newVertices[thisVerticesList[i]] = Vector3.Scale(new Vector3(change_ratio, 0, 0), newVertices[thisVerticesList[i]]);
            }
            for (int i = 0; i < otherVerticesList.Length; i++)
            {
                // Debug.Log(thisVerticesList[i]);
                newVertices[otherVerticesList[i]] = Vector3.Scale(new Vector3(change_ratio, 0, 0), newVertices[otherVerticesList[i]]);
            }
            // newVertices[this_index] = Vector3.Scale(new Vector3(change_ratio, change_ratio, change_ratio), newVertices[this_index]);
            // newVertices[other_index] = Vector3.Scale(new Vector3(change_ratio, change_ratio, change_ratio), newVertices[other_index]);
            // grabbedObject.transform.localScale = new Vector3(grabbedObject.transform.localScale.x * reverse_change_ratio, grabbedObject.transform.localScale.y * reverse_change_ratio, grabbedObject.transform.localScale.z * reverse_change_ratio);

            grabbedObject.GetComponent<MeshFilter>().mesh.SetVertices(newVertices);

            //Debug.Log("Current Spindle Length: " + current_spindle_length);
            if (current_spindle_length >= 0.8 && grabbedObject.GetComponent<MeshRenderer>().enabled)
            {
                grabbedObject.GetComponent<MeshRenderer>().enabled = false;
                //grabbedObject.transform.GetComponentInChildren<MeshRenderer>().enabled = true;
                
                if (grabbedObject.transform.childCount > 0)
                {
                    //Debug.Log(grabbedObject.transform.childCount);
                    for (int i = 0; i < grabbedObject.transform.childCount; i++)
                    {
                        Debug.Log(grabbedObject.transform.GetChild(i).gameObject.name);
                        grabbedObject.transform.GetChild(i).gameObject.SetActive(true);
                        // grabbedObject.transform.GetChild(i).gameObject.GetComponent<MeshRenderer>().enabled = true;
                        // grabbedObject.transform.GetComponentInChildren<MeshRenderer>().enabled = true;
                    }
                }
                
            } 

            // first time flag just_grabbed = 1
            // int this_index = -1;
            // int other_index = -1;
            // float this_min_distance = 100000;
            // float other_min_distance = 100000;
            // for (int i = 0; i < newVertices.Length; i++) {
            //     float this_distance = Vector3.Distance(this.transform.parent.position, newVertices[i]);
            //     float other_distance = Vector3.Distance(this.transform.parent.position, newVertices[i]);
            //     if (this_distance < this_min_distance)
            //     {
            //         this_min_distance = this_distance;
            //         this_index = i;
            //     }
            //     if (other_distance < other_min_distance)
            //     {
            //         other_min_distance = other_distance;
            //         other_index = i;
            //     }
            // }
            // newVertices[this_index] = new Vector3(5,5,5);
            // newVertices[other_index] = new Vector3(5,5,5);

            // grabbedObject.GetComponent<MeshFilter>().mesh.SetVertices(newVertices);
        }
    }

    public override void Grab(InputAction.CallbackContext context)
    {
        if (currentObject && grabbedObject == null)
        {
            if (currentObject.GetCurrentGrabber() != null)
            {
                currentObject.GetCurrentGrabber().Release(new InputAction.CallbackContext());
            }
            Debug.Log(originalVertices);

            grabbedObject = currentObject;
            grabbedObject.SetCurrentGrabber(this);

            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }

            if (buttonPress % 2 == 1)
            {
                grabbedObject.transform.parent = this.transform;
            }

            float mid_point_x = (other_controller.transform.position.x + this.transform.parent.position.x) / 2;
            float mid_point_y = (other_controller.transform.position.y + this.transform.parent.position.y) / 2;
            float mid_point_z = (other_controller.transform.position.z + this.transform.parent.position.z) / 2;

            grabbedObject.transform.position = new Vector3(mid_point_x, mid_point_y, mid_point_z);

            //Debug.Log(grabbedObject.transform.position);
            //Debug.Log(this.transform.position);
            //Debug.Log(other_controller.transform.position);


            //Debug.Log(grabbedObject);
            deformingMesh = grabbedObject.GetComponent<MeshFilter>().mesh;
            originalVertices = deformingMesh.vertices;
            newVertices = new Vector3[originalVertices.Length];
            worldSpaceVertices = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                worldSpaceVertices[i] = grabbedObject.transform.TransformPoint(originalVertices[i]);
                newVertices[i] = originalVertices[i];
            }
            // newVertices[0] = new Vector3(5,5,5);
            // grabbedObject.GetComponent<MeshFilter>().mesh.SetVertices(newVertices);

            float this_min_distance = 100000;
            float other_min_distance = 100000;
            for (int i = 0; i < worldSpaceVertices.Length; i++)
            {
                // Debug.Log(this.transform.position);
                // Debug.Log(other_controller.transform.position);
                float this_distance = Vector3.Distance(this.transform.position, worldSpaceVertices[i]);
                float other_distance = Vector3.Distance(other_controller.transform.position, worldSpaceVertices[i]);
                if (this_distance < this_min_distance)
                {
                    this_min_distance = this_distance;
                    this_index = i;
                }
                if (other_distance < other_min_distance)
                {
                    other_min_distance = other_distance;
                    other_index = i;
                }
            }
            Debug.Log(newVertices[this_index]);
            Debug.Log(newVertices[other_index]);

            List<int> thisIndexList = new List<int>();
            List<int> otherIndexList = new List<int>();
            int counter = 0;
            for (int i = 0; i < worldSpaceVertices.Length; i++)
            {
                // Debug.Log(this.transform.position);
                // Debug.Log(other_controller.transform.position);
                float this_distance = Vector3.Distance(worldSpaceVertices[this_index], worldSpaceVertices[i]);
                float other_distance = Vector3.Distance(worldSpaceVertices[other_index], worldSpaceVertices[i]);
                // Debug.Log(this_distance);
                if (this_distance < 0.07)
                {
                    // Debug.Log("Test");
                    thisIndexList.Add(i);
                }
                if (other_distance < 0.07)
                {
                    // Debug.Log("Test");
                    otherIndexList.Add(i);
                }
            }
            thisVerticesList = thisIndexList.ToArray();
            otherVerticesList = otherIndexList.ToArray();

            // for (int i = 0; i < thisVerticesList.Length; i++) {
            //     Debug.Log(thisVerticesList[i]);
            // }
            Debug.Log(counter);


            // newVertices[this_index] = Vector3.Scale(new Vector3(2, 2, 2), newVertices[this_index]);
            // newVertices[other_index] = Vector3.Scale(new Vector3(2, 2, 2), newVertices[other_index]);

            // grabbedObject.GetComponent<MeshFilter>().mesh.SetVertices(newVertices);
            Debug.Log(this_index);
            Debug.Log(other_index);
            Debug.Log(originalVertices.Length);

            // COMMENTED - want the controller independent from the grabbed object, so if I rotate my hand with the hand that grabbed the object, object won't rotate with controller rotation.
            // HE explained in class that the object shouldn't rotate if we rotate wrists.
            // grabbedObject.transform.parent = this.transform;
        }
        /// want total distance rather than x distance
        previous_spindle_length = Vector3.Distance(other_controller.transform.position, this.transform.parent.position);
        //previous_spindle_length = other_controller.transform.position.x - this.transform.parent.position.x;

        LastSpindleRotation = Quaternion.LookRotation(other_controller.transform.position - this.transform.parent.position);
    }

    public override void Release(InputAction.CallbackContext context)
    {
        if (grabbedObject)
        {
            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            }
            grabbedObject.GetComponent<MeshFilter>().mesh.SetVertices(originalVertices);
            this_index = -1;
            other_index = -1;
            grabbedObject.SetCurrentGrabber(null);
            grabbedObject.transform.parent = null;
            grabbedObject = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentObject == null && other.GetComponent<Grabbable>())
        {
            currentObject = other.gameObject.GetComponent<Grabbable>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentObject)
        {
            if (other.GetComponent<Grabbable>() && currentObject.GetInstanceID() == other.GetComponent<Grabbable>().GetInstanceID())
            {
                currentObject = null;
            }
        }
    }
}

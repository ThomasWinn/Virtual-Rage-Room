using System;
using UnityEngine;
using UnityEngine.InputSystem;

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
        buttonPress += 1;
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
        // GOGO
        if (buttonPress % 2 == 1)
        {

            // make mp cube go invisible
            mp_cube.transform.localScale = new Vector3(0, 0, 0);


            // calculate distance between controller and headset
            // IN LECTURE HE SAID WE COULD SET THE Y VALUE TO 0 AND CALL THE BUILT IN DISTANCE FUNCTION


            Vector3 controller_parent = new Vector3(this.transform.parent.position.x, 0, this.transform.parent.position.z);
            Vector3 headset = new Vector3(player_headset.transform.position.x, 0, player_headset.transform.position.z);
            float distance = Vector3.Distance(controller_parent, headset);
            // Debug.Log("Distance: " + distance); // 0.32 - 0.7

            float d_threshold = 0.50f;

            if (distance > d_threshold)
            {

                // IN LECTURE PROF SAID WE COULD DO DISTANCE - D / D * (SCALE_VAL), but I encapsulate in a power function because the graph of gogo slides said reach needs to be exponential
                float scale = 13f;
                float new_z = Mathf.Pow((distance - d_threshold) / d_threshold * scale, 2); // u want the distance - d_threshold to be as close to 0 as possible
                this.transform.localPosition = new Vector3(0, 0, new_z); 
            }
            else
            {
                this.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
        // Spindle
        else
        {
            if (grabbedObject != null)
            {
                // use line renderer to see spindle from user

                // set update recalculate spindle every frame and move object at that position = translation.
                grabbedObject.transform.position = new Vector3(mid_point_x, mid_point_y, mid_point_z);
                // grabbedObject.transform.position = new Vector3(mid_point_x, 0, 0);


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
                
                // PROF CODE

                Quaternion spindleRotation = Quaternion.LookRotation(other_controller.transform.position - this.transform.parent.position);

                Quaternion rotationChange = spindleRotation * Quaternion.Inverse(LastSpindleRotation);

                grabbedObject.transform.rotation = rotationChange * grabbedObject.transform.rotation;

                LastSpindleRotation = spindleRotation;
            }
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

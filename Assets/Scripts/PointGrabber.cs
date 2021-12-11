using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointGrabber : Grabber
{
    public LineRenderer laserPointer;
    public Material grabbablePointerMaterial;

    public InputActionProperty touchAction;
    public InputActionProperty scrollAction;
    public InputActionProperty grabAction;

    public float deadZone = 0.25f;
    public float maxVelocity = 4.0f;

    Material lineRendererMaterial;
    Transform grabPoint;
    Grabbable grabbedObject;
    Transform initialParent;

    // Start is called before the first frame update
    void Start()
    {
        laserPointer.enabled = false;
        lineRendererMaterial = laserPointer.material;

        grabPoint = new GameObject().transform;
        grabPoint.name = "Grab Point";
        grabPoint.parent = this.transform;
        grabbedObject = null;
        initialParent = null;

        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;

        touchAction.action.performed += TouchDown;
        touchAction.action.canceled += TouchUp;

        //scrollAction.action.performed += ScrollDown;
        scrollAction.action.performed += ScrollUp;
    }

    private void ScrollUp(InputAction.CallbackContext obj)
    {
        // Debug.Log("IN");
        // get up and down motion of joystick
        Vector2 inputAxes = obj.action.ReadValue<Vector2>();
        if (grabbedObject != null)
        {
            // Debug.Log(inputAxes);
            if (inputAxes.y >= deadZone || inputAxes.y <= -deadZone)
            {
                float velocity = maxVelocity * inputAxes.y * Time.deltaTime;
                // Debug.Log(inputAxes.y);
                if (inputAxes.y > 0)
                {
                    // Debug.Log(inputAxes.y);
                    //Debug.Log(grabPoint.parent.localPosition);
                    grabPoint.localPosition = new Vector3(0, 0, grabPoint.localPosition.z + velocity);
                }
                else
                {
                    // Debug.Log(inputAxes.y);
                    //Debug.Log(grabPoint.parent.localPosition);
                    if (grabPoint.localPosition.z + velocity > grabPoint.parent.localPosition.z)
                    {
                        grabPoint.localPosition = new Vector3(0, 0, grabPoint.localPosition.z + velocity);
                    }
                }
            }
        }
    }

    private void ScrollDown(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;

        touchAction.action.performed -= TouchDown;
        touchAction.action.canceled -= TouchUp;

        //scrollAction.action.performed -= ScrollDown;
        scrollAction.action.performed -= ScrollUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (laserPointer.enabled)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, hit.distance));
                // if it hits a grabbable object, change material to green else keep it blue
                if (hit.collider.GetComponent<Grabbable>())
                {
                    laserPointer.material = grabbablePointerMaterial;
                    /*
                    if (grabbedObject != null)
                    {
                        // THIS AINT IT bc aint going closer to user.It's just going back at whatever position it's at
                        // QUESTIONS:

                        // How to make object go towards camera(position) instead of arbitrary direction and incrementation
                        // How to check for joystick command or button commands
                        // 
                        // if flick up, 
                        // if flick down

                        grabbedObject.transform.localPosition = new Vector3(grabbedObject.transform.position.x, grabbedObject.transform.position.y, grabbedObject.transform.position.z - 0.1f);
                        // manipulate object using joystick
                        // CHeck what direction joystick flicked
                        //if (user flicked up)
                        // grabbedObject.transform.position = new Vector3(grabbedObject.transform.position.x, grabbedObject.transform.position.y, grabbedObject.transform.position.z + 1)
                        Debug.Log(grabbedObject); // name of object - SM_Toy_01 (5)
                    }
                    */
                }
                else
                {
                    laserPointer.material = lineRendererMaterial;
                }
            }
            else
            {
                laserPointer.SetPosition(1, new Vector3(0, 0, 100));
                laserPointer.material = lineRendererMaterial;
            }
        }
    }

    public override void Grab(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            // if ray hits a grabbable object
            if (hit.collider.GetComponent<Grabbable>())
            {
                grabPoint.localPosition = new Vector3(0, 0, hit.distance);

                // if soemthing else is grabbing  that object, force it to release it.
                if (hit.collider.GetComponent<Grabbable>().GetCurrentGrabber() != null)
                {
                    hit.collider.GetComponent<Grabbable>().GetCurrentGrabber().Release(new InputAction.CallbackContext());
                }

                grabbedObject = hit.collider.GetComponent<Grabbable>();
                grabbedObject.SetCurrentGrabber(this);

                // disregard physics when item is grabbed.
                if (grabbedObject.GetComponent<Rigidbody>())
                {
                    grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                    grabbedObject.GetComponent<Rigidbody>().useGravity = false;
                }

                initialParent = grabbedObject.transform.parent;
                grabbedObject.transform.parent = grabPoint;
                // Debug.Log("Object Held");
            }
        }
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

            grabbedObject.transform.parent = initialParent;
            grabbedObject = null;
        }
    }


    // if you touch the up or down trigger button...
    void TouchDown(InputAction.CallbackContext context)
    {
        laserPointer.enabled = true;
        /*
        if (grabbedObject != null)
        {
            Debug.Log("Transform");
            grabbedObject.transform.localPosition = new Vector3(grabbedObject.transform.localPosition.x, grabbedObject.transform.localPosition.y, grabbedObject.transform.localPosition.z - 0.5f);
        }
        */
        //Debug.Log("TOUCHDOWN");
    }

    void TouchUp(InputAction.CallbackContext context)
    {
        laserPointer.enabled = false;
        /*
        if (grabbedObject != null)
        {
            grabbedObject.transform.localPosition = new Vector3(grabbedObject.transform.position.x, grabbedObject.transform.position.y, grabbedObject.transform.position.z + 0.5f);
        }
        */
        //Debug.Log("TOUCHUP");
    }
}

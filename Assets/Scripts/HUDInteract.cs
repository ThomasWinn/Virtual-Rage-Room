using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDInteract : MonoBehaviour
{
    public InputActionProperty touchAction;
    public InputActionProperty pushAction;
    private bool hit_grabbable;

    public GameObject panel;
    //public GameObject panel_text;
    public Text panel_text;
    public int strength = 5;

    public LineRenderer laserPointer;
    public Material stat_boost_material;
    Material LineRedererMaterial;
    public Text strength_text;

    // Start is called before the first frame update
    void Start()
    {
        LineRedererMaterial = laserPointer.material;

        hit_grabbable = false;
        touchAction.action.performed += TouchTrigger;
        touchAction.action.canceled += UnTouchTrigger;

        pushAction.action.performed += push;
    }

    private void push(InputAction.CallbackContext obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit.collider.name == "Increase_Strength")
            {
                if (strength < 10)
                {
                    strength += 1;
                    Debug.Log("INCREASE");
                }
                
            }
            else if (hit.collider.name == "Decrease_Strength")
            {
                if (strength > 1)
                {
                    strength -= 1;
                    Debug.Log("Decrease");
                }
            }
        }
    }

    void UnTouchTrigger(InputAction.CallbackContext obj)
    {
        hit_grabbable = false;
    }

    void TouchTrigger(InputAction.CallbackContext obj)
    {
        hit_grabbable = true;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit_grabbable)
            {
                if (hit.collider.GetComponent<Grabbable>()) // check what it is, show HUD with appropriate stats
                {
                    panel.SetActive(true);
                    //panel_text.SetActive(true);

                    if (hit.collider.name.Contains("Gum"))
                    {
                        panel_text.text = "Item: Gum\nElasticity: 10";
                    }
                    else if (hit.collider.name.Contains("Rock"))
                    {
                        panel_text.text = "Item: Rock\nElasticity: 0";
                    }
                    else if (hit.collider.name.Contains("Plant"))
                    {
                        panel_text.text = "Item: Plant\nElasticity: 5";
                    }
                }
                else // if it doesn't hit anything hide the HUD stats
                {
                    panel.SetActive(false);
                    // panel_text.SetActive(false);
                }
            }
            /*
            if (hit.collider.name == "Increase_Strength_Button" || hit.collider.name == "Decrease_Strength_Button")
            {
                Debug.Log
                laserPointer.material = stat_boost_material;
            }
            else
            {
                laserPointer.material = LineRedererMaterial;
            }
            */
        }
        strength_text.text = "STRENGTH: " + strength.ToString();
    }
}

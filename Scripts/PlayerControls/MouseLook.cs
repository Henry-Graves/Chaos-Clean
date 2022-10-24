using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
//Note: we disable this so it doesn't yell at use for using serialized fields 
 //because we can leave these blank and assign them in the editor 
#pragma warning disable 649 
    [SerializeField] float sensitivityX = 15f;
    [SerializeField] float sensitivityY = 0.1f;
    [SerializeField] Transform playerCamera;
    //We have this clamp to ensure that the roation never goes to high or to low 
    [SerializeField] float xClamp = 85f;

    private bool disableCameraRoation = false;


    float mouseX, mouseY;
    float xRotation = 0;

    public void Start()
    {
        //Lets load our saved sensitivites 
        sensitivityX = PlayerPrefs.GetFloat("xSens", 15f);
        sensitivityY = PlayerPrefs.GetFloat("ySens", .1f);
    }


    public void RecieveInput(Vector2 mouseInput)
    {
        mouseX = mouseInput.x * sensitivityX;
        mouseY = mouseInput.y * sensitivityY;
    }


    // Update is called once per frame
    void Update()
    {
        if (!disableCameraRoation)
        {
            transform.Rotate(Vector3.up, mouseX * Time.deltaTime);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);

            Vector3 targetRoation = transform.eulerAngles;
            targetRoation.x = xRotation;

            playerCamera.eulerAngles = targetRoation;
        }

    }

    public void disableCamera()
    {
        disableCameraRoation = true;
    }

    public void enableCamera()
    {
        disableCameraRoation = false;
    }


    public float getXSens()
    {
        return sensitivityX; 
    }

    public float getYSens()
    {
        return sensitivityY;
    }


    public void setXSens(float newValue)
    {
        sensitivityX = newValue;
    }

    public void setYSens(float newValue)
    {
        sensitivityY = newValue;
    }
}

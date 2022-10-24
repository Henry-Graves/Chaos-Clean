using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractor : MonoBehaviour
{
    public LayerMask buttonMask;
    // Update is called once per frame
    public void SendRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2f, buttonMask))
        {
            ButtonController bs = hit.collider.GetComponent<ButtonController>();
            bs.ButtonPressed();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBobbing : MonoBehaviour
{
    [Header("Transform References")]
    public Transform headTransform;
    public Transform cameraTransform;

    [Header("Head Bobbing")]
    public float bobFrequency = 5f;
    public float bobHorizontalAmplitude = 0.1f;
    public float bobVerticalAmplitude = 0.1f;
    [Range(0, 1)] public float headBobSmoothing = 0.1f;

    // State
    private bool isWalking;
    private float walkingTime;
    private Vector3 targetCameraPosition;
    private Vector3 lastPosition;

    private void Update()
    {
        // check if player is walking
        if (headTransform.transform.position != lastPosition)
            isWalking = true;
        else
            isWalking = false;

        // set time and offset to 0
        if (!isWalking)
            walkingTime = 0;
        else
            walkingTime += Time.deltaTime;

        // calculate camera's target position
        targetCameraPosition = headTransform.position + CalculateHeadBobOffset(walkingTime);

        // interpolate position
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetCameraPosition, headBobSmoothing);

        // snap to position if it's already close enough
        if ((cameraTransform.position - targetCameraPosition).magnitude <= 0.001)
            cameraTransform.position = targetCameraPosition;

        lastPosition = headTransform.transform.position;
    }

    private Vector3 CalculateHeadBobOffset(float t)
    {
        float horizontalOffset = 0;
        float verticalOffset = 0;
        Vector3 offset = Vector3.zero;

        if (t > 0)
        {
            // calculate offsets
            horizontalOffset = Mathf.Cos(t * bobFrequency) * bobHorizontalAmplitude;
            verticalOffset = Mathf.Sin(t * bobFrequency * 2) * bobVerticalAmplitude;

            // combine offsets relative to the head's position and calculate the camera's target position
            offset = headTransform.right * horizontalOffset + headTransform.up * verticalOffset;
        }

        return offset;
    }
}

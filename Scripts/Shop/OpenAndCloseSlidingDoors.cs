using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAndCloseSlidingDoors : ButtonController
{
    public Transform leftDoor;
    public Transform rightDoor;
    public float animationTime = 1;
    public float sizeOfDoorInX = 1;
    public float amountOfDoorInWall = 0.9f;
    public bool startOpen = true;
    public bool doorsAreOpen = false;

    public AudioClip doorSoundClip;
    [Range(0, 1)]
    public float doorSoundVolume;

    private AudioSource doorSoundSource;

    private Vector3 leftDoorCloseTarget;
    private Vector3 rightDoorCloseTarget;
    private Vector3 leftDoorOpenTarget;
    private Vector3 rightDoorOpenTarget;
    private float totalDistanceToCover;
    private bool animating = false;

    // Start is called before the first frame update
    void Start()
    {
        doorSoundSource = gameObject.AddComponent<AudioSource>();
        doorSoundSource.volume = doorSoundVolume;

        SetInitialReferences();
        if (startOpen) OpenDoors();
    }

    void SetInitialReferences()
    {
        leftDoorCloseTarget = leftDoor.localPosition;
        rightDoorCloseTarget = rightDoor.localPosition;

        leftDoorOpenTarget = new Vector3(
            leftDoor.localPosition.x - (sizeOfDoorInX * amountOfDoorInWall),
            leftDoor.localPosition.y,
            leftDoor.localPosition.z);

        rightDoorOpenTarget = new Vector3(
            rightDoor.localPosition.x + (sizeOfDoorInX * amountOfDoorInWall),
            rightDoor.localPosition.y,
            rightDoor.localPosition.z);

        totalDistanceToCover = Vector3.Distance(leftDoorCloseTarget, leftDoorOpenTarget);
    }

    public void OpenDoors()
    {
        doorSoundSource.PlayOneShot(doorSoundClip);
        StartCoroutine(OpenDoorsCo());
    }
    IEnumerator OpenDoorsCo()
    {
        animating = true;
        float startTime = Time.time;
        while (Time.time - startTime < animationTime)
        {
            float distanceCovered = (Time.time - startTime) / animationTime;
            float fractionOfJourney = distanceCovered / totalDistanceToCover;
            leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftDoorOpenTarget, fractionOfJourney);
            rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightDoorOpenTarget, fractionOfJourney);

            yield return null;
        }

        leftDoor.localPosition = leftDoorOpenTarget;
        rightDoor.localPosition = rightDoorOpenTarget;

        doorsAreOpen = true;
        animating = false;
    }

    public void CloseDoors()
    {
        doorSoundSource.PlayOneShot(doorSoundClip);
        StartCoroutine(CloseDoorsCo());
    }
    IEnumerator CloseDoorsCo()
    {
        animating = true;
        float startTime = Time.time;
        while (Time.time - startTime < animationTime)
        {
            float distanceCovered = (Time.time - startTime) / animationTime;
            float fractionOfJourney = distanceCovered / totalDistanceToCover;
            leftDoor.localPosition = Vector3.Lerp(leftDoor.localPosition, leftDoorCloseTarget, fractionOfJourney);
            rightDoor.localPosition = Vector3.Lerp(rightDoor.localPosition, rightDoorCloseTarget, fractionOfJourney);

            yield return null;
        }

        leftDoor.localPosition = leftDoorCloseTarget;
        rightDoor.localPosition = rightDoorCloseTarget;

        doorsAreOpen = false;
        animating = false;
    }

    // Door is only used to open the door
    public override void ButtonPressed()
    {
        if (animating) return;
        if (!doorsAreOpen) OpenDoors();
    }
}
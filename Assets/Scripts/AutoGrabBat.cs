using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Collections.Generic;

public class AutoGrabBat : MonoBehaviour
{
    public HandGrabInteractor rightHandInteractor; // assign RightHand
    public HandGrabInteractable batInteractable;

    void Start()
    {
        StartCoroutine(GrabAfterDelay());
    }
    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            StartCoroutine(GrabAfterDelay());
        }
    }
    System.Collections.IEnumerator GrabAfterDelay()
    {
        yield return new WaitForSeconds(1f); // wait for system init

        if (rightHandInteractor != null && batInteractable != null)
        {
            rightHandInteractor.ForceSelect(batInteractable);
        }
    }
}
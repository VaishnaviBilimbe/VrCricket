using UnityEngine;

public class SetVRHeight : MonoBehaviour
{
    public Transform trackingSpace;
    public float height = 1.6f;

    void Start()
    {
        trackingSpace.localPosition = new Vector3(0, height, 0);
    }
}
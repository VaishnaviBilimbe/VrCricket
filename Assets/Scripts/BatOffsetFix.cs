using UnityEngine;

public class BatOffsetFix : MonoBehaviour
{
    public Transform batHolder;
    public Vector3 defaultPos;
    public Vector3 defaultRot;

    void Start()
    {
        ApplyOffset();
    }

    void Update()
    {
        // Detect reset view (Start/Menu button)
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            ApplyOffset();
        }
    }

    void ApplyOffset()
    {
        batHolder.localPosition = defaultPos;
        batHolder.localEulerAngles = defaultRot;
    }
}
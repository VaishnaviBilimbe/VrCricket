using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnCollide : MonoBehaviour
{
    public AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private bool _playOnCollision = true;
    [SerializeField] private string[] _ignoreLayer = {"XR"};

    private void OnCollisionEnter(Collision collision)
    {
        // Get the layer name of the colliding object
        string collisionLayerName = LayerMask.LayerToName(collision.gameObject.layer);
    
        // Check if the layer should be ignored
        bool shouldIgnore = false;
        foreach (string ignoreLayerName in _ignoreLayer)
        {
            if (collisionLayerName == ignoreLayerName)
            {
                shouldIgnore = true;
                break;
            }
        }
    
        // Play sound if not ignored and audio components are available
        if (!shouldIgnore && _playOnCollision && _audioSource != null && _clip != null)
        {
            _audioSource.PlayOneShot(_clip);
        }
    }
}

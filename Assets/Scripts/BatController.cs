using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BatController : MonoBehaviour
{
    [SerializeField] private float hitForce = 20f;
    [SerializeField] private AudioSource hitSound;
    [SerializeField] private AudioClip batHitClip;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CricketBall>() != null)
        {
            // Calculate direction away from bat
            Vector3 hitDirection = (collision.transform.position - transform.position).normalized;
            
            // Apply strong impulse force
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.velocity = Vector3.zero; // Reset existing velocity
                ballRb.AddForce(hitDirection * hitForce, ForceMode.Impulse);
            }
            
            // Play hit sound
            if (hitSound != null && batHitClip != null)
                hitSound.PlayOneShot(batHitClip);
        }
        else
        {
            
        }
    }

   
}
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CricketBallThrower : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    
    [Header("Throw Settings")]
    public float throwForce = 12f;
    public float upwardAngle = 10f;
    
    [Header("Variation")]
    public bool addRandomness = true;
    public float forceVariation = 1.5f;
    public float directionVariation = 5f;
    public float spinIntensity = 0.5f;

    [Header("Audio")]
    public AudioSource _audioSource;
    [SerializeField] private AudioClip _throwSound;

    public UnityEvent<int> OnBoundaryHit;
    public void ThrowBall()
    {
        // Create ball at spawn position
        if (spawnPoint == null)
            spawnPoint = transform;
            
        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        
        if (ballRb == null)
        {
            ballRb = ball.AddComponent<Rigidbody>();
            ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
        
        // Calculate throw direction (forward with upward angle)
        Vector3 throwDirection = spawnPoint.forward;
        float _upwardAngle = Random.Range(1f, 3f);
        throwDirection = Quaternion.AngleAxis(_upwardAngle, -spawnPoint.right) * throwDirection;  // to change the spawn point to right pr left randomize
        
        // Add randomness if enabled
        if (addRandomness)
        {
            // Randomize force
            float actualForce = /*throwForce +*/ Random.Range(0.15F, .25F);
            
            // Randomize direction slightly
            float randomYaw = Random.Range(-2,2);
            float randomPitch = Random.Range(-2, 2); // To change teh randdom value manually for direct variation
            throwDirection = Quaternion.Euler(randomPitch, randomYaw, 0) * throwDirection;
            
            // Apply force and spin
            ballRb.AddForce(throwDirection * actualForce, ForceMode.Impulse);
            ballRb.AddTorque(Random.insideUnitSphere * spinIntensity * actualForce, ForceMode.Impulse);
        }
        else
        {
            float _throwForce=Random.Range(.25f,0.40f);
            // Apply consistent force and spin
            ballRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            ballRb.AddTorque(Vector3.left * spinIntensity * throwForce, ForceMode.Impulse);
        }
    }
    
    // For testing in editor
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowBall();
        }
        if (Input.GetKeyDown(KeyCode.JoystickButton1))
        {
            OnBoundaryHit?.Invoke(0);
        }

    }

    public void PlaySound()
    {
        _audioSource.PlayOneShot(_throwSound);
    }
}
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CricketBallThrower : MonoBehaviour
{
    [Header("Ball Settings")]
    public GameObject ballPrefab;
    public Transform spawnPoint;
    
    [Header("Pitch Target")]
    public Transform pitchTarget;
    public float timeToTarget = 0.5f; // Adjust this to make the throw faster or slower

    [Header("Variation")]
    public bool addRandomness = true;
    public float pitchForwardBackVariation = 2.0f; // Randomize forward/backward pitch distance
    public float pitchLeftRightVariation = 0.5f; // Randomize left/right pitch distance
    public float timeVariation = 0.1f; // How much the speed can vary
    public float spinIntensity = 0.5f;

    [Header("Audio")]
    public AudioSource _audioSource;
    [SerializeField] private AudioClip _throwSound;

    public UnityEvent<int> OnBoundaryHit;

    [Header("Debug")]
    public float actualForce;
    public void ThrowBall()
    {
        // Create ball at spawn position
        if (spawnPoint == null)
            spawnPoint = transform;

        if (pitchTarget == null)
        {
            Debug.LogError("Pitch Target not assigned! Please assign a target on the ground.");
            return;
        }

        GameObject ball = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();

        if (ballRb == null)
        {
            ballRb = ball.AddComponent<Rigidbody>();
            ballRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        Vector3 targetPosition = pitchTarget.position;

        // Apply variations if enabled
        if (addRandomness)
        {
            // Calculate a variation based on the spawnPoint's rotation so forward/back is relative to the thrower
            float zOffset = Random.Range(-pitchForwardBackVariation, pitchForwardBackVariation);
            float xOffset = Random.Range(0, pitchLeftRightVariation);
            
            // Apply this offset relative to the spawnPoint's forward and right vectors
            targetPosition += spawnPoint.forward * zOffset;
            targetPosition += spawnPoint.right * xOffset;
            
            // Randomize time slightly to create slight flight variations
            float randomizedTime = timeToTarget + Random.Range(-timeVariation, timeVariation);
            // Ensure time doesn't go negative or too small
            randomizedTime = Mathf.Max(0.2f, randomizedTime); 
            
            Vector3 velocity = CalculateVelocity(spawnPoint.position, targetPosition, randomizedTime);
            
            actualForce = velocity.magnitude; // Just for display
            
            ballRb.velocity = velocity;
            ballRb.AddTorque(Random.insideUnitSphere * spinIntensity * velocity.magnitude, ForceMode.Impulse);
        }
        else
        {
            Vector3 velocity = CalculateVelocity(spawnPoint.position, targetPosition, timeToTarget);
            
            actualForce = velocity.magnitude; // Just for display
            
            ballRb.velocity = velocity;
            ballRb.AddTorque(Vector3.right * spinIntensity * velocity.magnitude, ForceMode.Impulse);
        }
    }

    private Vector3 CalculateVelocity(Vector3 origin, Vector3 target, float time)
    {
        // Calculate the vector from origin to target
        Vector3 displacement = target - origin;
        
        // Calculate horizontal velocity needed to reach target in given time
        Vector3 displacementXZ = new Vector3(displacement.x, 0, displacement.z);
        Vector3 velocityXZ = displacementXZ / time;
        
        // Calculate vertical velocity needed to reach target in given time, accounting for gravity
        float gravity = Mathf.Abs(Physics.gravity.y);
        float velocityY = (displacement.y + 0.5f * gravity * time * time) / time;
        
        // Combine horizontal and vertical velocities
        return new Vector3(velocityXZ.x, velocityY, velocityXZ.z);
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
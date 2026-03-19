using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CricketBall : MonoBehaviour
{
    private bool hasBounced = false;
    private bool hasHitWicket = false;
    private bool hasBeenScored = false;
    public bool hasBeenHitByBat = false;
    public bool crossedBoundary = false;
    [SerializeField] private float _lifeTime = 10f;
    public float timeSinceSpawned = 0f;

    private bool firstGroundContact = false;

    public int groundBounceCount = 0;
    ScoringSystem scoringSystem;
    private void Start()
    {
        scoringSystem = FindObjectOfType<ScoringSystem>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bat"))
        {
            Debug.Log("Ball hit with bat");
            hasBeenHitByBat = true;
            groundBounceCount = 0;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Ground");
            groundBounceCount++;
            // First bounce = pitch → ignore
          /*  if (groundBounceCount == 1)
            {
                Debug.Log("Pitch bounce");
                return;
            }

            // Second bounce → check if bat missed
            if (groundBounceCount >= 3)
            {
                if (!hasBeenHitByBat)
                {
                    Debug.Log("Bat missed - next ball");
                    StartCoroutine(ExecuteNextBall());
                }
            }*/
            
            // Boundary logic bounce tracking
            if (!crossedBoundary)
            {
                hasBounced = true;
            }
        }

        if (collision.gameObject.CompareTag("Wickets") )
        {
            if((groundBounceCount == 0))
            {
                hasHitWicket = true;
                Debug.Log("OUT");
                if (scoringSystem != null && !hasBeenScored)
                {
                    hasBeenScored = true;
                    ScoringSystem.instance.OnBoundaryHit.Invoke(0);
                    scoringSystem.RegisterWicketHit(this);
                }
            }
            else
            {
                ScoringSystem.instance.OnBoundaryHit.Invoke(0);
            }
        }
        if (collision.gameObject.CompareTag("Stemp"))
        {
            if (scoringSystem != null && !hasBeenScored)
            {
                hasBeenScored = true;
                Debug.Log("STEMP");
                scoringSystem.RegisterWicketHit(this);
                ScoringSystem.instance.OnBoundaryHit.Invoke(0);
            }
        }
    }
    IEnumerator ExecuteNextBall()
    {
        yield return new WaitForSeconds(1f);
        ScoringSystem.instance.OnBoundaryHit?.Invoke(0);
    }
    public void MarkBoundaryCrossed()
    {
        crossedBoundary = true;
    }
    public bool HasBouncedOnGround()
    {
        return hasBounced;
    }

    public bool HasHitWicket()
    {
        return hasHitWicket;
    }

    public bool HasBeenHitByBat()
    {
        return hasBeenHitByBat;
    }

    public void MarkAsScored()
    {
        hasBeenScored = true;
    }

    public bool HasBeenScored()
    {
        return hasBeenScored;
    }

    private void Update()
    {
        timeSinceSpawned += Time.deltaTime;
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0 && !hasBeenScored)
        {
            MarkAsScored();

            ScoringSystem scoringSystem = FindObjectOfType<ScoringSystem>();
            if (scoringSystem != null)
            {
                scoringSystem.OnBoundaryHit?.Invoke(0);
            }
            Destroy(gameObject);
        }
    }

}
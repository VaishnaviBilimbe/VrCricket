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

    private bool firstGroundContact = false;

    public int groundBounceCount = 0;

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
            groundBounceCount++;

            // First bounce = pitch → ignore
            if (groundBounceCount == 1)
            {
                Debug.Log("Pitch bounce");
                return;
            }

            // Second bounce → check if bat missed
            if (groundBounceCount == 3)
            {
                if (!hasBeenHitByBat)
                {
                    Debug.Log("Bat missed - next ball");
                    StartCoroutine(ExecuteNextBall());
                }
            }

            // Boundary logic bounce tracking
            if (!crossedBoundary)
            {
                hasBounced = true;
            }
        }

        if (collision.gameObject.CompareTag("Wickets") && (groundBounceCount==0))
        {
            hasHitWicket = true;

            ScoringSystem scoringSystem = FindObjectOfType<ScoringSystem>();

            if (scoringSystem != null && !hasBeenScored)
            {
                hasBeenScored = true;
                scoringSystem.RegisterWicketHit(this);
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
using UnityEngine;
using System.Collections.Generic;

public class FielderManager : MonoBehaviour
{
    public static FielderManager instance;

    [SerializeField] private List<Fielder> fielders = new List<Fielder>();
    private GameObject currentBall;
    private Fielder activeFielder;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        
        // Auto-collect fielders if list is empty
        if (fielders.Count == 0)
        {
            fielders.AddRange(FindObjectsOfType<Fielder>());
        }
    }

    private void Update()
    {
        // Find current ball in scene if not already tracked
        if (currentBall == null)
        {
            CricketBall ball = FindObjectOfType<CricketBall>();
            if (ball != null)
            {
                currentBall = ball.gameObject;
            }
        }

        // Logic to assign nearest fielder only if ball has been hit by the bat
        if (currentBall != null)
        {
            CricketBall ballScript = currentBall.GetComponent<CricketBall>();
            if (ballScript != null && ballScript.HasBeenHitByBat() && !ballScript.HasBeenScored())
            {
                AssignNearestFielder();
            }
        }
        else
        {
            if (activeFielder != null)
            {
                activeFielder.StopChasing();
                activeFielder = null;
            }
        }
    }

    private void AssignNearestFielder()
    {
        if (fielders.Count == 0 || currentBall == null) return;

        Fielder nearest = null;
        float minDistance = float.MaxValue;

        foreach (Fielder fielder in fielders)
        {
            float distance = Vector3.Distance(fielder.transform.position, currentBall.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = fielder;
            }
        }

        if (nearest != activeFielder)
        {
            if (activeFielder != null) activeFielder.StopChasing();
            activeFielder = nearest;
            activeFielder.StartChasing(currentBall.transform);
            Debug.Log("chasing");
        }
    }
    public void ResetFielderPositions()
    {
        foreach(Fielder fielder in fielders)
        {
            fielder.ResetPosotion();
        }
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Collections.Generic;
using System;

[Serializable]
public class SixUI
{
    public GameObject sixUI;
    public GameObject dotUI;
}
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Game References")]
    [SerializeField] private Animator bowlerAnimator;
    [SerializeField] private ScoringSystem scoringSystem;
    [SerializeField] private AnimationsController animationsController;

    [Header("UI Elements")]
    [SerializeField] private GameObject fourUI;
    [SerializeField] private GameObject sixUI;
    [SerializeField] private GameObject outUI;
    [SerializeField] private GameObject fourUI_Center;
    [SerializeField] private GameObject sixUI_Center;
    [SerializeField] private GameObject outUI_Center;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI runsWicketsText;
    [SerializeField] private TextMeshProUGUI oversText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI gameResultText;

    [Header("Particle")]
    [SerializeField] private GameObject confetti_1;
    [SerializeField] private GameObject confetti_2;

    [Header("Game Settings")]
    [SerializeField] private float initialDelay = 2f;
    [SerializeField] private float playResultDisplayTime = 2f;
    [SerializeField] private int maxWickets = 10;
    [SerializeField] private int maxOvers = 5;
    [SerializeField] private int ballsPerOver = 6;
    [SerializeField] private int scoretoWin = 50;

    [Header("Events")]
    public UnityEvent OnGameStarted;
    public UnityEvent OnBallThrown;
    public UnityEvent OnPlayCompleted;
    public UnityEvent<bool> OnGameOver; // true = win, false = lose

    [SerializeField] private GameObject[] _wickets;

    private Transform[] _wicketTransforms;
    private bool gameActive = false;
    [SerializeField] private bool playInProgress = false;
    private CricketBall currentBall;
    
    // Match state tracking
    private int currentOver = 0;
    private int ballsInOver = 0;
    public SixUI[] ballUI; // size = 6
    private int currentBallIndex = 0;

    bool isResultUIShown = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        HideAllUI();
        HideParticle();
        StoreWicketTransforms();
        UpdateMatchDisplay();

        if (scoringSystem != null)
        {
            Debug.Log("Subscribing to scoring events");
            scoringSystem.OnBoundaryHit.AddListener(HandleBoundaryHit);
            scoringSystem.OnWicketHit.AddListener(HandleWicketHit);
            scoringSystem.OnScoreChanged.AddListener(_ => UpdateMatchDisplay());
        }
        else
        {
            Debug.LogError("ScoringSystem reference not set in GameManager!");
        }
    }

    public void StartGame()
    {
        if (gameActive) return;

        gameActive = true;
        scoringSystem?.ResetScore();
        currentOver = 0;
        ballsInOver = 0;
        OnGameStarted?.Invoke();
        animationsController?.gameStart();
        Debug.Log("Cricket game started!");
        UpdateMatchDisplay();

        StartCoroutine(StartNextBowl());
    }

    public void EndGame(bool isWin)
    {
        if (!gameActive) return;

        gameActive = false;
        scoringSystem?.EndGame();

        HideAllUI();
        StopAllCoroutines();
        
        // Show game over UI with appropriate message
        if (gameResultText != null)
        {
            if (isWin)
            {
                gameResultText.text = "You Win!\nScore: " + scoringSystem.GetScore();
                animationsController?.gameWin();
            }

            else
            {
                gameResultText.text = "Game Over!\nWickets: " + scoringSystem.GetWickets() + "/" + maxWickets;
                animationsController?.gameLose();
            }
        }
        
        ShowUI(gameOverUI);
        PlayParticle();
        OnGameOver?.Invoke(isWin);
        Debug.Log("Cricket game ended! Win: " + isWin);
    }

    private IEnumerator StartNextBowl()
    {
        Debug.Log("StartNextBowl called - gameActive: " + gameActive);
        while (!gameActive)
        {
            Debug.LogWarning("StartNextBowl exiting early because gameActive is false");
            yield break;
        }
        
        // Check if match is over (overs completed)
        if (currentOver >= maxOvers)
        {
            EndGame(false);
            yield break;
        }

        UpdateMatchDisplay();
        FielderManager.instance.ResetFielderPositions();
        yield return new WaitForSeconds(initialDelay);

        if (bowlerAnimator != null)
        {
            ResetWicketTransforms();
            ThrowBall();
            Debug.Log("Bowler started running");
        }
        else
        {
            Debug.LogWarning("Bowler animator not assigned!");
        }
    }

    public void ThrowBall()
    {
        if (!gameActive || playInProgress) return;
        playInProgress = true;

        if (bowlerAnimator != null)
        {
            bowlerAnimator.SetTrigger("isRunning");
            OnBallThrown.Invoke();
            
            Debug.Log("Ball thrown");
        }
        else
        {
            Debug.LogError("Bowler Animator not assigned!");
            playInProgress = false;
        }
    }


    private void HandleBoundaryHit(int runs)
    {
        Debug.Log("player runs: " + runs);

        isResultUIShown = false; // reset

        if (runs == 4)
        {
            ShowUI(fourUI);
            ShowCenterUI(fourUI_Center);
            PlayParticle();
            animationsController?.four();

            ballUI[currentBallIndex].dotUI.SetActive(true);

            isResultUIShown = true;
        }
        else if (runs == 6)
        {
            ShowUI(sixUI);
            ShowCenterUI(sixUI_Center);
            PlayParticle();
            animationsController?.six();

            ballUI[currentBallIndex].sixUI.SetActive(true);

            isResultUIShown = true;
        }
        else
        {
            Debug.Log("Ball missed, no runs awarded");
            ballUI[currentBallIndex].dotUI.SetActive(true);
        }

        currentBallIndex++;
        CompletePlay();
    }

    private void HandleWicketHit()
    {
        isResultUIShown = true;

        ShowUI(outUI);
        ShowCenterUI(outUI_Center);
        animationsController?.wicket();

        CompletePlay();
    }

    private void CompletePlay()
    {
        if (!playInProgress) return;
        Debug.Log("Completing play, ready for next bowl");

        CricketBall[] balls = FindObjectsOfType<CricketBall>();
        foreach(CricketBall ball in balls)
        {
            Destroy(ball.gameObject);
        }
        playInProgress = false;
        
        // Increment ball count
        ballsInOver++;
        if (ballsInOver >= ballsPerOver)
        {
            ballsInOver = 0;
            currentOver++;
        }
        
        UpdateMatchDisplay();
        
        // Check win/lose conditions
        if (scoringSystem.GetScore() >= scoretoWin)
        {
            EndGame(true); // Win
            return;
        }
        
        if (scoringSystem.GetWickets() >= maxWickets)
        {
            EndGame(false); // Lose
            return;
        }
        
        OnPlayCompleted?.Invoke();
        StartCoroutine(PrepareForNextBowl());
    }

    private IEnumerator PrepareForNextBowl()
    {
        Debug.Log("Preparing for next bowl");

        float delay = isResultUIShown ? playResultDisplayTime : 2f;

        yield return new WaitForSeconds(delay);

        HideAllUI();
        HideParticle();

        Debug.Log("UI hidden, about to start next bowl");
        StartCoroutine(StartNextBowl());
    }
    
    private void UpdateMatchDisplay()
    {
        // Update runs and wickets display
        if (runsWicketsText != null && scoringSystem != null)
        {
            runsWicketsText.text = $"{scoringSystem.GetScore()}-{scoringSystem.GetWickets()}";
        }
        
        // Update overs display
        if (oversText != null)
        {
            oversText.text = $"{currentOver}.{ballsInOver}";
        }
        
        // Update target score display
        if (targetText != null && scoringSystem != null)
        {
            int runsNeeded = scoretoWin - scoringSystem.GetScore();
            targetText.text = $"{runsNeeded}";
        }
    }
    private void ShowCenterUI(GameObject uiElement)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }
    private void ShowUI(GameObject uiElement)
    {
        HideAllUI();
        if (uiElement != null)
        {
            uiElement.SetActive(true);
        }
    }
    private void PlayParticle()
    {
        confetti_1.SetActive(true);
        confetti_2.SetActive(true);
    }
    private void HideParticle()
    {
        confetti_1.SetActive(false);
        confetti_2.SetActive(false);
    }
    private void HideAllUI()
    {
        if (fourUI != null) fourUI.SetActive(false);
        if (sixUI != null) sixUI.SetActive(false);
        if (outUI != null) outUI.SetActive(false);

        if (fourUI_Center != null) fourUI_Center.SetActive(false);
        if (sixUI_Center != null) sixUI_Center.SetActive(false);
        if (outUI_Center != null) outUI_Center.SetActive(false);
    }

    private void StoreWicketTransforms()
    {
        if (_wickets == null || _wickets.Length == 0)
        {
            Debug.LogError("No wickets assigned in GameManager!");
            return;
        }

        _wicketTransforms = new Transform[_wickets.Length];
        for (int i = 0; i < _wickets.Length; i++)
        {
            if (_wickets[i] != null)
            {
                _wicketTransforms[i] = _wickets[i].transform;
            }
            else
            {
                Debug.LogWarning($"Wicket at index {i} is not assigned!");
            }
        }
    }

    private void ResetWicketTransforms()
    {
        for (int i = 0; i < _wickets.Length; i++)
        {
            _wickets[i].transform.position = _wicketTransforms[i].position;
            _wickets[i].transform.rotation = _wicketTransforms[i].rotation;
        }
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
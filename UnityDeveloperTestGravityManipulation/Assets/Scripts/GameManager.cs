using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float timeLimit = 120f; // 2 minutes
    private float timeRemaining;

    public int totalCollectibles = 5;
    private int collectedCount = 0;

    public TMP_Text timerText;
    public TMP_Text statusText;

    private bool gameOver = false;
    public PlayerGravityController player;
    private float airTime = 0f;

    void Start()
    {
        player = FindAnyObjectByType<PlayerGravityController>();
        timeRemaining = timeLimit;
        UpdateUI();
    }

    void Update()
    {
        if (gameOver) return;

        timeRemaining -= Time.deltaTime;
        UpdateUI();

        if (timeRemaining <= 0f)
        {
            LoseGame();
        }

        if (player != null && !player.IsGrounded())
        {
            airTime += Time.deltaTime;
            if (airTime >= 5f)
            {
                statusText.text = "You fell for too long! You Lose!";
                LoseGame();
            }
        }
        else
        {
            airTime = 0f;
        }
    }


    public void Collect()
    {
        collectedCount++;
        UpdateUI();

        if (collectedCount >= totalCollectibles)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        gameOver = true;
        statusText.text = "You Win!";
        Invoke("RestartGame", 5f);
    }

    void LoseGame()
    {
        gameOver = true;
        statusText.text = "You Lose!";
        Invoke("RestartGame", 5f);
    }

    void UpdateUI()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining);
        statusText.text = "Collected: " + collectedCount + "/" + totalCollectibles;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

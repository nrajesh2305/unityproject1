using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int playerScore;
    private Text scoreDisplay;

    private int enemy1KillCount;
    private int enemy2KillCount;
    private int enemy3KillCount;
    private int buildingDestroyCount;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerScore = 0;
        scoreDisplay.text = "Total Score: " + playerScore;

        enemy1KillCount = 0;
        enemy2KillCount = 0;
        enemy3KillCount = 0;
        buildingDestroyCount = 0;
    }

    public void Enemy1Killed()
    {
        enemy1KillCount++;
    }

    public void Enemy2Killed()
    {
        enemy2KillCount++;
    }

    public void Enemy3Killed()
    {
        enemy3KillCount++;
    }

    public void BuildingDestroyed()
    {
        buildingDestroyCount++;
    }

    public void CalculateScore()
    {
        playerScore = enemy1KillCount * 50 + enemy2KillCount * 100 + enemy3KillCount * 150;
    }

    public int GetTotalScore()
    {
        return playerScore;
    }

    public Text GetScoreDisplay()
    {
        return scoreDisplay;
    }

    public int GetBuildingDestroyCount()
    {
        return buildingDestroyCount;
    }

    void Update()
    {
        CalculateScore();
        scoreDisplay.text = "Total Score: " + playerScore;
    }
}

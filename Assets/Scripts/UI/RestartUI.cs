using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // for Text


public class RestartUI : MonoBehaviour
{

    public static RestartUI Instance;
    public Text DefectEnemyTipText;
    public Text RoomCleanText;
    public Text SpendTimeText;
    public Text ScoreText;
    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        // Subscribe to the player death event
        
    }

    void OnDestroy()
    {
        // Clean up to prevent memory leaks
    }

    public void HandleData()
    {
        var record = GameManager.Instance.recordCenter;

        DefectEnemyTipText.text = $"Defeated Enemies: {record.Kill_enemies}";
        RoomCleanText.text = $"Rooms Cleaned: {record.CleanRoom}";
        SpendTimeText.text = $"Time Spent: {record.SpendTime} sec";
        ScoreText.text = $"Score: {record.Score:F1}";
    }

    public void RestartGame()
    {
        GameManager.Instance.state = GameManager.GameState.PREGAME;
        gameObject.SetActive(false);
        GameManager.Instance.RestartGame();
    }
}
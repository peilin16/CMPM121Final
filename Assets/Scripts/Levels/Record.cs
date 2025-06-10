using UnityEngine;

public class Record
{
    public int Kill_enemies { get; private set; }
    public int SpendTime => Mathf.FloorToInt(_timeAccumulated); // Rounded display in seconds
    public int CleanRoom { get; private set; }
    private float _timeAccumulated;
    public float Score;


    public Record()
    {
        Init();
        // Subscribe to the monster death event
        EventBus.Instance.OnMonsterDeath += OnEnemyKilled;
        EventBus.Instance.CleanRoomAction += roomRecord;


    }

    public void Init()
    {
        Kill_enemies = 0;
        Score = 0f;
        CleanRoom = 0;
        _timeAccumulated = 0f;
    }

    private void roomRecord(Controller c) {
        this.CleanRoom += 1;
    }


    private void OnEnemyKilled(GameObject enemy)
    {
        Kill_enemies++;
        // Optionally update score here
        // Score += ...;
    }

    public void Update()
    {
        if (GameManager.Instance.state == GameManager.GameState.INWAVE ||
            GameManager.Instance.state == GameManager.GameState.EXPEDITION)
        {
            _timeAccumulated += Time.deltaTime;
        }
    }


}

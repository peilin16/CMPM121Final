using UnityEngine;

public class LevelController : MonoBehaviour
{
    public PlayerController player;
    public EnemySpawner enemySpawner;
    public GameObject gate; // 拖进来的大门对象


    private Room lastRoom;
    private Room currentRoom;

    private Level currentLevel;
    



    void Start()
    {
        currentLevel = GameManager.Instance.levelManager.GetLevel();

        lastRoom = currentLevel.GetRoomFromPosition(player.transform.position);
        if (lastRoom != null)
            CloseRoomGates(lastRoom);
    }

    void Update()
    {

        Room room = currentLevel.GetRoomFromPosition(player.transform.position);

        if (room != null && GameManager.Instance.state == GameManager.GameState.EXPEDITION) {
            if(!room.isExplored)
            {
                currentRoom = room;
                currentRoom.isActive = true;
                GameManager.Instance.state = GameManager.GameState.INWAVE;
                CloseRoomGates(currentRoom);
                enemySpawner.StartWave(currentRoom);
            }


        }






    }

    void CloseRoomGates(Room room)
    {
        gate.SetActive(true); // 激活大门对象
        Debug.Log($"Gates closed for room: {room.name}");
    }
}

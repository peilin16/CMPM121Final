using UnityEngine;

public class LevelController : MonoBehaviour
{
    public PlayerController player;
    public EnemySpawner enemySpawner;
    public GameObject gate; //


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
        
        currentRoom = currentLevel.GetRoomFromPosition(player.transform.position);

        if (currentRoom != null && GameManager.Instance.state == GameManager.GameState.EXPEDITION) {
            if( !currentRoom.isActive && !currentRoom.isCleared)
            {
                currentRoom.isActive = true;
                GameManager.Instance.state = GameManager.GameState.INWAVE;
                CloseRoomGates(currentRoom);
                enemySpawner.StartWave(currentRoom);
            }
            
        }
        else if(currentRoom != null && GameManager.Instance.state == GameManager.GameState.WAVEEND)
        {
            GameManager.Instance.state = GameManager.GameState.EXPEDITION;
            currentRoom.isActive = false;
            OpenRoomGates(currentRoom);
            //currentRoom = null;
        }
        

    }

    private void CloseRoomGates(Room room)
    {
        gate.SetActive(true); // Open Gates
        Debug.Log($"Gates closed for room: {room.name}");
    }
    private void OpenRoomGates(Room room)
    {
        gate.SetActive(false); // Close
        Debug.Log($"Gates closed for room: {room.name}");
    }
}

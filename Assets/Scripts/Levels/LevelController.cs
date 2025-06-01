using UnityEngine;

public class LevelController : MonoBehaviour
{
    public PlayerController player;
    private Room lastRoom;
    public GameObject gate; // �Ͻ�ȥ

    void Start()
    {
        lastRoom = GameManager.Instance.roomManager.GetRoomFromPosition(player.transform.position);
        if (lastRoom != null)
            CloseRoomGates(lastRoom);
    }
    void Update()
    {
        Room currentRoom = GameManager.Instance.roomManager.GetRoomFromPosition(player.transform.position);

        if (currentRoom != null && currentRoom != lastRoom)
        {
            lastRoom = currentRoom;
            CloseRoomGates(currentRoom);
        }
    }

    void CloseRoomGates(Room room)
    {
        GameObject roomGO = GameObject.Find(room.name);
        if (roomGO == null)
        {
            Debug.LogWarning($"Room GameObject '{room.name}' not found in scene.");
            return;
        }

        gate.SetActive(true); // �������

        Debug.Log($"Gates closed for room: {room.name}");
    }




}

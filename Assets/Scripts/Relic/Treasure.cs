using UnityEngine;
using UnityEngine.Tilemaps;
public class Treasure : MonoBehaviour
{
    public string treasureID;
    private bool opened = false;
    public TileBase closedTile;   // ��ʼtile���ر�״̬��
    public TileBase openedTile;   // ����tile����״̬��
    public Vector3Int tilePosition; // tile �ľֲ���������

    private Tilemap tilemap;
    public void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("No Tilemap found on Treasure object.");
            return;
        }

        if (tilePosition == Vector3Int.zero)
            tilePosition = tilemap.WorldToCell(transform.position); // �Զ�����ǰtileλ��

        tilemap.SetTile(tilePosition, closedTile);


        treasureID = gameObject.name;
        GameManager.Instance.treasureManager.RegisterTreasure(this);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Player touched the treasure!");
        if (opened) return;
        
        if (other.CompareTag("Player"))
        {
            tilemap.SetTile(tilePosition, openedTile);
            opened = true;
            EventCenter.Broadcast(EventDefine.ShowRelicSelectorPanel);
        }
    }

    public bool IsOpened() => opened;

    public Vector3 GetPosition() => transform.position;
}
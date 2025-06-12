using UnityEngine;
using UnityEngine.Tilemaps;
public class Treasure : MonoBehaviour
{
    public string treasureID;
    private bool opened = false;
    public TileBase closedTile;   // 初始tile（关闭状态）
    public TileBase openedTile;   // 开启tile（打开状态）
    public Vector3Int tilePosition; // tile 的局部格子坐标

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
            tilePosition = tilemap.WorldToCell(transform.position); // 自动捕获当前tile位置

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
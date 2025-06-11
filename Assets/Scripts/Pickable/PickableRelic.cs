using UnityEngine;

public class PickableRelic : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (triggered) return;

    if (collision.CompareTag("Player"))
    {
        Debug.Log("[PickableRelic] Player triggered relic.");
        triggered = true;
        GameManager.Instance.currentPickableRelic = this;
        EventCenter.Broadcast(EventDefine.ShowRelicSelectorPanel);
        gameObject.SetActive(false);
    }
}


    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
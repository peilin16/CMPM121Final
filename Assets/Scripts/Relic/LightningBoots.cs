using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections;
public class LightningBoots : Relic
{
    private PlayerController boundPC;
    private bool isBoosting = false;
    private float originalSpeed;

    public override void JsonInit(JObject jsonObj)
    {
        base.JsonInit(jsonObj);
    }

    public override void Application(PlayerController pc)
    {
        boundPC = pc;
        EventBus.Instance.OnPlayerDamaged += OnPlayerDamaged;
    }

    private void OnPlayerDamaged(Damage dmg)
    {
        if (isBoosting || boundPC == null) return;

        isBoosting = true;
        originalSpeed = boundPC.player.speed;

        float boostMultiplier = amount > 0 ? amount : 1.5f;
        boundPC.player.speed = Mathf.RoundToInt(originalSpeed * boostMultiplier);

        Debug.Log($"Lightning Boots activated: Speed x{boostMultiplier}");

        CoroutineManager.Instance.StartManagedCoroutine(
            "LightningBoots",
            boundPC.Controller_ID.ToString(),
            ResetSpeedAfterDuration(3f)
        );
    }

    private IEnumerator ResetSpeedAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (boundPC?.player != null)
        {
            boundPC.player.speed = Mathf.RoundToInt(originalSpeed);
            Debug.Log("Lightning Boots expired: Speed reset.");
        }

        isBoosting = false;
    }

    public override void OnRemoved()
    {
        EventBus.Instance.OnPlayerDamaged -= OnPlayerDamaged;
        isBoosting = false;
    }
}

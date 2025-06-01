using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Unit : MonoBehaviour
{
    public Vector2 movement;
    public float distance;
    public event System.Action<float> OnMove;

    Rigidbody2D rb;
    BoxCollider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        Vector2 ds = movement * Time.fixedDeltaTime;

        if (ds != Vector2.zero)
        {
            if (!IsBlocked(ds))
            {
                rb.MovePosition(rb.position + ds);

                distance += ds.magnitude;
                if (distance > 0.5f)
                {
                    OnMove?.Invoke(distance);
                    distance = 0;
                }
            }
        }
    }

    bool IsBlocked(Vector2 moveDelta)
    {
        ContactFilter2D filter = new ContactFilter2D
        {
            useLayerMask = true,
            useTriggers = false
        };

        // Ö»¼ì²â Wall ²ã
        filter.SetLayerMask(LayerMask.GetMask("Wall"));

        RaycastHit2D[] results = new RaycastHit2D[1];
        int count = col.Cast(moveDelta.normalized, filter, results, moveDelta.magnitude);

        return count > 0;
    }
}

using UnityEngine;

public interface Controller
{
    long Controller_ID { get; set => _id = GameManager.Instance.GenerateID(); }
    Character character { get; set; }  
    HealthBar HealthUI { get; set; }
    bool IsDead { get; set; }
    void Die(); 
}
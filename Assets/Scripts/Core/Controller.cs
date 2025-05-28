using UnityEngine;

public interface Controller
{
    long Controller_ID { get; }
    Character character { get; set; }  
    HealthBar HealthUI { get; set; }
    bool IsDead { get; set; }
    void Die(); 
}
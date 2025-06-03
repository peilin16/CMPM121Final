using UnityEngine;
using Newtonsoft.Json.Linq;



public class Character
{
    public int speed;
    public Hittable hp;
    public int iconIndex;
    public GameObject gameObject;
    public float health;
    //protected Controller controller;
    //level start
    public virtual void StartLevel()
    {
        //this.StartWave();
    }
    
    protected virtual void JsonLoad(JObject obj)
    {
    }
    //Init controller start
    /*public virtual void InitController(Controller c)
    {
        controller = c;
    }*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity
{
    public int id;
    public List<EntityComponent> components;
    public GameObject gameObject;
    public Entity(int id, GameObject gameObject)
    {
        this.id = id;
        this.gameObject = gameObject;
        components = new List<EntityComponent>();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECSSystem
{
    public static class EntityComponentSystem
    {


        public static void AddComponent(Entity entity, EntityComponent component)
        {
            entity.components.Add(component);

            //Logic for updating entity Type map
        }

        public static void NewEntity()
        {
            int id = ECSComponent.EntityComponentMap.entities.Count;
            GameObject gameObject = GameObject.Instantiate(ECSComponent.PrefabMap.prefabs[ECSComponent.PrefabMap.PrefabType.Test]);
            ECSComponent.EntityComponentMap.entities.Add(id, new Entity(id, gameObject));
        }

        public static void FixedUpdate()
        {

        }
    }
}

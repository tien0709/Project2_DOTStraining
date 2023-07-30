using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Components
{
    public class TankAuthoring : MonoBehaviour
    {
        public class TankAuthoringBaker : Baker<TankAuthoring>
        {
            public override void Bake(TankAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TankComponent 
                {
                }) ;
            }
        }
    }
}
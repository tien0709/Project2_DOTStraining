using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Components
{
    public class PointAuthoring : MonoBehaviour
    {

        public class PointAuthoringBaker : Baker<PointAuthoring>
        {
            public override void Bake(PointAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PointComponent 
                {
                   RedPoint = 0,
                   BluePoint = 0
                }) ;
            }
        }
    }
}

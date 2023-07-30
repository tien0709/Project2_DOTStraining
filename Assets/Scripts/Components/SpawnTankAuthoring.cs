using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Components
{
    public class SpawnTankAuthoring : MonoBehaviour
    {
        public GameObject PrefabRed;
        public GameObject PrefabBlue;
        public Material RedMaterial;
        public Material BlueMaterial;
        public class SpawnTankAuthoringBaker : Baker<SpawnTankAuthoring>
        {
            public override void Bake(SpawnTankAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SpawnTankComponent 
                {
                    PrefabRed = GetEntity(authoring.PrefabRed, TransformUsageFlags.Dynamic),
                    PrefabBlue = GetEntity(authoring.PrefabBlue, TransformUsageFlags.Dynamic),
                    RedMaterial = hybridRenderer.RegisterMaterial(authoring.RedMaterial),
                    BlueMaterial = hybridRenderer.RegisterMaterial(authoring.BlueMaterial)
                }) ;
            }
        }
    }
}

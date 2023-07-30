using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Components
{
    public class SpawnCellAuthoring : MonoBehaviour
    {
        public GameObject Prefab;
        public Material RedMaterial;
        public Material BlueMaterial;
        public Material WallMaterial;
        public Material EmptyMaterial;

        public class SpawnCellAuthoringBaker : Baker<SpawnCellAuthoring>
        {
            public override void Bake(SpawnCellAuthoring authoring)
            {
                var hybridRenderer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SpawnCellComponent 
                {
                    Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic),
                    RedMaterial = hybridRenderer.RegisterMaterial(authoring.RedMaterial),
                    BlueMaterial = hybridRenderer.RegisterMaterial(authoring.BlueMaterial),               
                    EmptyMaterial = hybridRenderer.RegisterMaterial(authoring.EmptyMaterial),
                    WallMaterial = hybridRenderer.RegisterMaterial(authoring.WallMaterial)
                }) ;
            }
        }
    }
}

using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;


namespace Components
{
    
    public struct SpawnCellComponent : IComponentData
    {
            public Entity Prefab;
            public BatchMaterialID EmptyMaterial;
            public BatchMaterialID WallMaterial;
            public BatchMaterialID BlueMaterial;
            public BatchMaterialID RedMaterial;
    }


}


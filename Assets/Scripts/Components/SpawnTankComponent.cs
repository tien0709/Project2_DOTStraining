using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace Components
{
    
    public struct SpawnTankComponent : IComponentData
    {
        public Entity PrefabRed;
        public Entity PrefabBlue;
        public BatchMaterialID RedMaterial;
        public BatchMaterialID BlueMaterial;
    }


}
using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;


namespace Components
{
    
    public struct CellComponent : IComponentData
    {
            public CellType CurrentType;
            public Vector2 Position;// Array, not Oxy
    }


}

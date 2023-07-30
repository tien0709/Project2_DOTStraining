using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using UnityEngine.Rendering;


namespace Components
{
    
    public struct TankComponent : IComponentData
    {
        public TankType CurrentTank ;
        public Vector2 CurrentCell;
    }


}

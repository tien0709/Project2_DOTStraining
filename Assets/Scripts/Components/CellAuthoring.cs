using Unity.Entities;
using UnityEngine;


namespace Components
{
    public class CellAuthoring : MonoBehaviour
    {

        public class CellAuthoringBaker : Baker<CellAuthoring>
        {           
            public override void Bake(CellAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CellComponent {
                    CurrentType = CellType.EMPTY,
                });
            }
        }
    }
}
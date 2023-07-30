using System.Runtime.CompilerServices;
using Components;
using Unity.Entities;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine.Rendering;
using Unity.Jobs;

namespace System
{

    public partial struct PointSystem : ISystem
    {

        
        public void OnCreate(ref SystemState state){
                state.RequireForUpdate<CellComponent>();
                state.RequireForUpdate<TankComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            if(!state.EntityManager.CreateEntityQuery(typeof(PointComponent)).IsEmpty){
                Entity entity = state.EntityManager.CreateEntityQuery(typeof(PointComponent)).GetSingletonEntity();
                float redPoint = 0;
                float bluePoint = 0;
                foreach( var cell in SystemAPI.Query< RefRW<CellComponent>>()){
                    if(cell.ValueRO.CurrentType == CellType.RED) 
                        redPoint ++;
                    else if(cell.ValueRO.CurrentType == CellType.BLUE) 
                        bluePoint++ ;    
            }
            
            state.EntityManager.SetComponentData(entity, new PointComponent
            {
                RedPoint = redPoint,
                BluePoint = bluePoint
            });
            }
        }
    }
}

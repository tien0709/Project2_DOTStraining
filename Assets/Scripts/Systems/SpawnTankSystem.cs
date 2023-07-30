using Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace System
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SimulationSystemGroup))]
    public partial struct TankSpawnSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
          var entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            Entity entity = state.EntityManager.CreateEntityQuery(typeof(SpawnTankComponent)).GetSingletonEntity();
            var SpawnTankComponent = state.EntityManager.GetComponentData<SpawnTankComponent>(entity);

            if(state.EntityManager.CreateEntityQuery(typeof(TankComponent)).IsEmpty){   //check have spawn tank yet?   if no=>>spawn     
                NativeArray<Entity> SpawnArray = new NativeArray<Entity>(2, Allocator.TempJob);
                SpawnArray[0] = SpawnTankComponent.PrefabRed;
                SpawnArray[1] = SpawnTankComponent.PrefabBlue;

                // Create a new SpawnCell job and schedule it to be executed.
                state.Dependency = new SpawnTank
                {
                    ecb = entityCommandBuffer,
                    spawnArray = SpawnArray,
                }.Schedule(state.Dependency);

                // Complete the dependency.
                state.Dependency.Complete();

                // Playback the entity command buffer and dispose of the native arrays.
                entityCommandBuffer.Playback(state.EntityManager);
                entityCommandBuffer.Dispose();
                SpawnArray.Dispose();
            }


        }

        public partial struct SpawnTank : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public NativeArray<Entity> spawnArray ;
            
            private void Execute(RefRW<LocalTransform> tf, RefRO<SpawnTankComponent> spawntank)
            {

                var newRedTank = ecb.Instantiate(spawnArray[0]);
                var newBlueTank = ecb.Instantiate(spawnArray[1]);
                ecb.SetComponent(newRedTank, new LocalTransform
                {
                        Position = new Vector3(250 , -250 , tf.ValueRO.Position.z),
                        Scale = 20f,
                        Rotation = Quaternion.identity * Quaternion.Euler(new Vector3(0, 0, 0))
                });

                ecb.SetComponent(newBlueTank, new LocalTransform
                {
                        Position = new Vector3(-250 , 250 , tf.ValueRO.Position.z),
                        Scale = 20f,
                        Rotation = Quaternion.identity * Quaternion.Euler(new Vector3(0, 0, 0))
                });

                ecb.SetComponent(newBlueTank, new TankComponent
                {
                        CurrentTank = TankType.BLUE,
                        CurrentCell = new Vector2(0,0)
                });

                ecb.SetComponent(newRedTank, new TankComponent
                {
                        CurrentTank = TankType.RED,
                        CurrentCell = new Vector2(9,9)
                });



            }
                    
        }
    
    }
}
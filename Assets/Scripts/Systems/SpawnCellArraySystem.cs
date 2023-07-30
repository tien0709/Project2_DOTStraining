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

    public partial struct CellSpawnSystem : ISystem
    {

        public NativeArray<CellType> LogicMap ;
        
        public void OnCreate(ref SystemState state){
                //state.RequireForUpdate<CellComponent>();
                //state.RequireForUpdate<SimulationSingleton>();
                LogicMap = new NativeArray<CellType>(100, Allocator.TempJob);
                RandomWall( ref LogicMap);

        }

        public void OnDestroy(ref SystemState state){
                 LogicMap.Dispose();
        }
        public void OnUpdate(ref SystemState state)
        {
           var entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            Entity entity = state.EntityManager.CreateEntityQuery(typeof(SpawnCellComponent)).GetSingletonEntity();
            var SpawnCellComponent = state.EntityManager.GetComponentData<SpawnCellComponent>(entity);       
            var tf = state.EntityManager.GetComponentData<LocalTransform>(entity);
                       
            if(state.EntityManager.CreateEntityQuery(typeof(CellComponent)).IsEmpty){//check have spawn map yet?   if no=>>spawn
                NativeArray<Entity> SpawnArray = new NativeArray<Entity>(1, Allocator.TempJob);
                SpawnArray[0] = SpawnCellComponent.Prefab;

                // Create a new SpawnCell job and schedule it to be executed.
                state.Dependency = new SpawnCell
                {
                    ecb = entityCommandBuffer,
                    spawnArray = SpawnArray,
                    LogicMap = this.LogicMap
                }.Schedule(state.Dependency);

                // Complete the dependency.
                state.Dependency.Complete();

                // Playback the entity command buffer and dispose of the native arrays.
                entityCommandBuffer.Playback(state.EntityManager);
                entityCommandBuffer.Dispose();
                SpawnArray.Dispose();
            }

            //change Color of cells          
            // Create a new ChangeCellDisplay job with multithread
            new ChangeCellDisplay  {
                red = SpawnCellComponent.RedMaterial,
                blue = SpawnCellComponent.BlueMaterial,
                wall = SpawnCellComponent.WallMaterial,
                empty = SpawnCellComponent.EmptyMaterial
            }.ScheduleParallel();

        }

        public partial struct SpawnCell : IJobEntity
        {
            public EntityCommandBuffer ecb;
            public NativeArray<CellType> LogicMap ;
            public NativeArray<Entity> spawnArray ;
            
            private void Execute(RefRW<LocalTransform> tf, RefRO<SpawnCellComponent> spawncell)
            {
                            for(int i = 0; i < 10; i++ ){
                                for(int j = 0; j < 10  ; j++ ){

                                    var newCell = ecb.Instantiate(spawnArray[0]);
                                    ecb.SetComponent(newCell, new LocalTransform
                                    {
                                          Position = new Vector3( -250 + j*55, 250 - i*55, tf.ValueRO.Position.z),
                                          Scale = 5f,
                                          Rotation = Quaternion.identity * Quaternion.Euler(new Vector3(0, 270, 90))
                                   });

                                   if(LogicMap[i*10 + j] == CellType.WALL)                                     
                                        ecb.SetComponent(newCell, new CellComponent
                                        {
                                           CurrentType = CellType.WALL,
                                           Position = new Vector2(i,j)
                                        });
                                    else 
                                        ecb.SetComponent(newCell, new CellComponent
                                        {
                                           CurrentType = CellType.EMPTY,
                                           Position = new Vector2(i,j)
                                        });
                                }
                            }
            }
        
        }

        public void RandomWall(ref NativeArray<CellType> LogicMap){
                NativeArray<int> RandomX = new NativeArray<int>(5, Allocator.TempJob);
                NativeArray<int> RandomY = new NativeArray<int>(5, Allocator.TempJob);

                for(int i=0;i<5;i++){
                        RandomX[i] = -1;
                        RandomY[i] = -1;
                }

                for(int i=0;i<5;i++){
                    while(true){
                        Random random = new Random();
                        int tempX = random.Next(0,10);//0->9
                        int tempY = random.Next(0,10);//0->9
                        bool checkExist = false;
                        for(int j=0;j<5;j++){
                            if(RandomX[j]==tempX&&RandomY[j]==tempY) checkExist = true;
                        }
                        if( !checkExist && ((tempX != 0 && tempX != 9)||((tempY != 0 && tempY != 9))) ) {//wall khong bi trung va khong nam o cell[0,0] va cell[9,9]
                            RandomX[i] = tempX;
                            RandomY[i] = tempY;
                            break;
                        }
                    }
                }
                
                for(int i=0;i<5;i++){
                    int x = RandomX[i];
                    int y = RandomY[i];

                    int x1 = 9-RandomX[i];
                    int y1 = 9-RandomY[i];

                    LogicMap[x*10 + y] = CellType.WALL;
                    LogicMap[x1*10 + y1] = CellType.WALL;
                }

                RandomX.Dispose();
                RandomY.Dispose();

        }

        public partial struct ChangeCellDisplay : IJobEntity
        {
            //public SpawnCellComponent spawncell;
            public BatchMaterialID red;
            public BatchMaterialID blue;
            public BatchMaterialID empty;
            public BatchMaterialID wall;
            void Execute(RefRW<MaterialMeshInfo> mmi, RefRO<CellComponent> cell)
            {
                 if(cell.ValueRO.CurrentType == CellType.WALL) 
                    mmi.ValueRW.MaterialID = wall;
                 else if(cell.ValueRO.CurrentType == CellType.EMPTY) 
                    mmi.ValueRW.MaterialID = empty;
                 else if(cell.ValueRO.CurrentType == CellType.RED) 
                    mmi.ValueRW.MaterialID = red;
                 else if(cell.ValueRO.CurrentType == CellType.BLUE) 
                    mmi.ValueRW.MaterialID = blue;
            }
        }
    }
}
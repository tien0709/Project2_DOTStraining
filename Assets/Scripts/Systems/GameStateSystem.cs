using Components;
using Unity.Entities;
using UnityEngine;
using Unity.Burst;
using Unity.Transforms;
using Unity.Collections;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace System
{
    public partial struct GameStateSystem : ISystem
    {

        public int PlayerID;//turn play
        public NativeArray<CellType> LogicMap;
        public bool check;// check instantiated cell or not
        public bool playerStop;// check instantiated cell or not
        public bool machineStop;// check instantiated cell or not

        public void OnCreate(ref SystemState state){
            state.RequireForUpdate<CellComponent>();
            state.RequireForUpdate<TankComponent>();
            PlayerID = 1;//player play first, then Machine play
            LogicMap = new NativeArray<CellType>(100, Allocator.TempJob);
            check = true;
            playerStop = false;
            machineStop = false;
        }

        public void OnDestroy(ref SystemState state){
            LogicMap.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            

            if(check){ // assure this system run after completely instantiate cell 
               int k=0;
                foreach( var cell in SystemAPI.Query< RefRW<CellComponent>>()){k++;break;}
                if(k!=0){
                        foreach( var cell in SystemAPI.Query< RefRW<CellComponent>>()){
                            if(cell.ValueRO.Position == new Vector2(0,0)) {cell.ValueRW.CurrentType = CellType.BLUE;}
                            else if(cell.ValueRO.Position == new Vector2(9,9)) {cell.ValueRW.CurrentType = CellType.RED;}
                            LogicMap[(int)(cell.ValueRO.Position.x*10 + cell.ValueRO.Position.y)] = cell.ValueRO.CurrentType ;
                            check = false;
                        }
                    return;
                }
            }

            if( PlayerID == 1){//Player turn
                foreach( var (tank, tf) in SystemAPI.Query< RefRW<TankComponent>, RefRW<LocalTransform>>()){
                    if(tank.ValueRO.CurrentTank == TankType.BLUE){//
                        NativeArray<Vector2> MoveArray= new NativeArray<Vector2>(4, Allocator.TempJob);
                        for(int i=0;i<4;i++){
                            MoveArray[i] = new Vector2(-1,-1);
                        } 
                        CellCanMove(tank.ValueRO.CurrentCell, ref MoveArray);

                        //if it has cell to move
                        if(MoveArray[0]!= new Vector2(-1,-1)||MoveArray[1]!= new Vector2(-1,-1)||MoveArray[2]!= new Vector2(-1,-1)||MoveArray[3]!= new Vector2(-1,-1)) {
                            if(Input.GetKeyDown(KeyCode.LeftArrow) && MoveArray[0]!= new Vector2(-1,-1)){
                                tank.ValueRW.CurrentCell = MoveArray[0];
                                tf.ValueRW.Position = new Vector3( - 250 + MoveArray[0].y*55, 250 - MoveArray[0].x*55, tf.ValueRO.Position.z);
                                PlayerID = 0;
                                LogicMap[(int)(MoveArray[0].x * 10 + MoveArray[0].y)]  = CellType.BLUE;
                            }
                            else if(Input.GetKeyDown(KeyCode.RightArrow) && MoveArray[1]!= new Vector2(-1,-1)){
                                tank.ValueRW.CurrentCell = MoveArray[1];
                                tf.ValueRW.Position = new Vector3( - 250 + MoveArray[1].y*55, 250 - MoveArray[1].x*55, tf.ValueRO.Position.z);
                                PlayerID = 0;  
                                LogicMap[(int)(MoveArray[1].x * 10 + MoveArray[1].y)]  = CellType.BLUE;
                            }
                            else if(Input.GetKeyDown(KeyCode.UpArrow) && MoveArray[2]!= new Vector2(-1,-1)){
                                tank.ValueRW.CurrentCell = MoveArray[2];
                                tf.ValueRW.Position = new Vector3( - 250 + MoveArray[2].y*55, 250 - MoveArray[2].x*55, tf.ValueRO.Position.z);
                                PlayerID = 0;  
                                LogicMap[(int)(MoveArray[2].x * 10 + MoveArray[2].y)]  = CellType.BLUE;
                            }
                            else if(Input.GetKeyDown(KeyCode.DownArrow) && MoveArray[3]!= new Vector2(-1,-1)){
                                tank.ValueRW.CurrentCell = MoveArray[3];
                                tf.ValueRW.Position = new Vector3( - 250 + MoveArray[3].y*55, 250 - MoveArray[3].x*55, tf.ValueRO.Position.z);
                                PlayerID = 0; 
                                LogicMap[(int)(MoveArray[3].x * 10 + MoveArray[3].y)]  = CellType.BLUE;

                            }
                        }
                        else {
                            // to assure game dont stopped as soon as one object dont have any cell to move, that check other object have any cell to move? 
                                if(playerStop){
                                     checkWin();
                                     state.Enabled = false;
                                }
                                else {
                                    playerStop = true;
                                    PlayerID = 0; 
                                }
                                
                        }

                        MoveArray.Dispose();
                    }
                }
            }


            else if(PlayerID == 0) {// Machine turn            
                foreach( var (tank, tf) in SystemAPI.Query< RefRW<TankComponent>, RefRW<LocalTransform>>()){
                    if(tank.ValueRO.CurrentTank == TankType.RED){//
                        NativeArray<Vector2> MoveArray= new NativeArray<Vector2>(4, Allocator.TempJob);
                        for(int i=0;i<4;i++){
                            MoveArray[i] = new Vector2(-1,-1);
                        } 
                        CellCanMove(tank.ValueRO.CurrentCell, ref MoveArray);
                        if(MoveArray[0]!= new Vector2(-1,-1)||MoveArray[1]!= new Vector2(-1,-1)||MoveArray[2]!= new Vector2(-1,-1)||MoveArray[3]!= new Vector2(-1,-1)) {
                            //change Position logic of Machine
                            int leftScore =-1;
                            int rightScore = -1;
                            int upScore = -1;
                            int downScore = -1;
                        // different from variable, array pass for function by(only) reference =>> to dont change LogicMap then pass function by copy of LogicMap
                        NativeArray<CellType> copy= new NativeArray<CellType>(100, Allocator.TempJob);
                        for(int i=0;i<100;i++) copy[i] = LogicMap[i];
                            if(MoveArray[0]!= new Vector2(-1,-1)) {
                                leftScore = AIMiniMax(MoveArray[0], copy);
                            }
                            if(MoveArray[1]!= new Vector2(-1,-1)) {
                                rightScore = AIMiniMax(MoveArray[1], copy);
                            }
                            if(MoveArray[2]!= new Vector2(-1,-1)) {
                                upScore = AIMiniMax(MoveArray[2], copy);
                            }
                            if(MoveArray[3]!= new Vector2(-1,-1)) {
                                downScore = AIMiniMax(MoveArray[3], copy);
                            }
                        copy.Dispose();

                            int temp = FindMax(leftScore, rightScore, upScore, downScore);
                            // have more or 2 score same=>> priority: Up->left->right->bot=>> to accqauire land for advantage
                            if(temp == upScore ) temp = 2;
                            else if(temp == leftScore ) temp = 0;
                            else if(temp == downScore ) temp = 3;
                            else if(temp == rightScore ) temp = 1;
                            tank.ValueRW.CurrentCell = MoveArray[temp];

                            //change Position display of Machine
                            tf.ValueRW.Position = new Vector3( - 250 + MoveArray[temp].y*55, 250 - MoveArray[temp].x*55, tf.ValueRO.Position.z);
                            PlayerID = 1;  
                            LogicMap[(int)(MoveArray[temp].x * 10 + MoveArray[temp].y)]  = CellType.RED;
                        }
                        else { 
                            // to assure game dont stopped as soon as one object dont have any cell to move, that check other object have any cell to move? 
                                if(machineStop ){
                                     checkWin();
                                     state.Enabled = false;
                                }
                                else {
                                    machineStop = true;
                                    PlayerID = 1; 
                                }
                        }

                        MoveArray.Dispose();
                    }
                }

            }

        // update type of cells
            new UpdateTypeCell  {
                Map = LogicMap
            }.ScheduleParallel();

        }

        private int FindMax(int a, int b, int c, int d)
        {
            int max = a;
            if (b > max) max = b;
            if (c > max) max = c;
            if (d > max) max = d;
            return max;
        }

        private int AIMiniMax(Vector3 Cell, NativeArray<CellType> contemporaryMap){
            if(contemporaryMap[(int)(Cell.x * 10 + Cell.y)] != CellType.EMPTY) return 0;
            contemporaryMap[(int)(Cell.x* 10 + Cell.y)] = CellType.RED;
            if(Cell.x == 0 || Cell.y == 0 || Cell.x == 9 || Cell.y == 9) return 1;

            return  AIMiniMax(new Vector3(Cell.x + 1, Cell.y), contemporaryMap) + AIMiniMax(new Vector3(Cell.x - 1, Cell.y), contemporaryMap) +
            AIMiniMax(new Vector3(Cell.x , Cell.y + 1), contemporaryMap) + AIMiniMax(new Vector3(Cell.x, Cell.y - 1), contemporaryMap);
        }


        private void checkWin ()
        {   
            int red = 0;
            int blue = 0;
            for(int i=0;i<100;i++){
                if( LogicMap[i]== CellType.RED) red++; 
                else if ( LogicMap[i] == CellType.BLUE) blue++; 
            }
            if(red>blue) UnityEngine.Debug.Log("You Lose");
            else if(red<blue) UnityEngine.Debug.Log("You Win");
            else if(red==blue) UnityEngine.Debug.Log("Draw");
        }

        
        private void CellCanMove(Vector2 CurrentCell, ref NativeArray<Vector2> arr ){
            if(CurrentCell.y!=0)//go Left
                if(LogicMap[(int)(CurrentCell.x * 10 + CurrentCell.y-1)] == CellType.EMPTY){
                   arr[0] = new Vector2(CurrentCell.x  , CurrentCell.y -1);  
                }
            if(CurrentCell.y!=9)//go Right
                if(LogicMap[(int)(CurrentCell.x * 10 + CurrentCell.y+1)] == CellType.EMPTY){
                   arr[1] = new Vector2(CurrentCell.x , CurrentCell.y+1);  
                }
            if(CurrentCell.x!=0)//go Up
                if(LogicMap[(int)((CurrentCell.x-1) * 10 + CurrentCell.y)] == CellType.EMPTY){
                   arr[2] = new Vector2(CurrentCell.x - 1, CurrentCell.y);  
                }
            if(CurrentCell.x!=9)//go Down
                if(LogicMap[(int)((CurrentCell.x + 1) * 10 + CurrentCell.y)] == CellType.EMPTY){
                   arr[3] = new Vector2(CurrentCell.x + 1 , CurrentCell.y); 
                }
/* NOTE:           CurrentCell.x , CurrentCell.y la index cua array chu khong phai toa do trong truc Oxy, nên x=>i ~ Oy còn y=>j ~ Ox   
nen khi lam viec voi Position nho dao nguoc*/
        }


        //  paralel jobs use same NativeArray<CellType> Map so to avoid race condition=>> i have to check index of job

        public partial struct UpdateTypeCell : IJobEntity
        {

            public NativeArray<CellType> Map ;

            
            private void Execute(RefRW<CellComponent> cell, [EntityIndexInQuery] int index)
            {                                   

                int mapIndex = (int)(cell.ValueRO.Position.x * 10 + cell.ValueRO.Position.y);

                // Check the job index before accessing the Map buffer.
                if (index == mapIndex)
                {
                    cell.ValueRW.CurrentType = Map[mapIndex];
                }
            }
        }

    }
}
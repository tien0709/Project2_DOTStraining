using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{


        public enum Direction
        {
            UP,
            LEFT,
            DOWN,
            RIGHT
        }

        public enum CellType
        {
            EMPTY,
            WALL,
            RED,
            BLUE,
        } 
        
        public enum GameResult
        {
            PLAYING,
            DRAW,
            PLAYER1_WIN,
            PLAYER2_WIN,
        }
        
        public enum TankType
        {
            RED,
            BLUE
        }
}


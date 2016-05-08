using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    public class Move
    {
        public enum Direction
        {
            NONE,
            UP,
            UP_LEFT,
            UP_RIGHT,
            RIGHT,
            DOWN,
            DOWN_LEFT,
            DOWN_RIGHT,
            LEFT
        }

        public enum RelativeDirection
        {
            NONE,
            AWAY,
            TOWARDS
        }

        public Vector3 FromPos;
        public Vector3 Pos;
        public Structs.Coordinate FromCoord;
        public Structs.Coordinate Coord;

        public Direction FromDir;
        public Direction Dir;
        public GameObject Owner = null;

        private Piece attacker;
        public Piece Attacker { get; set; }
    }
}

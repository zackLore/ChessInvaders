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
            UP_RIGHT,
            RIGHT,
            DOWN_RIGHT,
            DOWN,
            DOWN_LEFT,
            LEFT,
            UP_LEFT
        }
        public enum RelativeDirection
        {
            NONE,
            AWAY,
            TOWARDS
        }

        readonly public static Structs.Coordinate[] Offsets =
        {
            new Structs.Coordinate(0, 0),   // Direction.NONE
            new Structs.Coordinate(-1, 0),  // Direction.UP
            new Structs.Coordinate(-1, 1),  // Direction.UP_RIGHT
            new Structs.Coordinate(0, 1),   // Direction.RIGHT
            new Structs.Coordinate(1, 1),   // Direction.DOWN_RIGHT
            new Structs.Coordinate(1, 0),   // Direction.DOWN
            new Structs.Coordinate(1, -1),  // Direction.DOWN_LEFT
            new Structs.Coordinate(0, -1),  // Direction.LEFT
            new Structs.Coordinate(-1, -1), // Direction.UP_LEFT
        };

        readonly public static Direction[] Directions_All =
        {
            Direction.UP,
            Direction.UP_RIGHT,
            Direction.RIGHT,
            Direction.DOWN_RIGHT,
            Direction.DOWN,
            Direction.DOWN_LEFT,
            Direction.LEFT,
            Direction.UP_LEFT,
        };

        readonly public static Direction[] Directions_NoDiagonals =
        {
            Direction.UP,
            Direction.RIGHT,
            Direction.DOWN,
            Direction.LEFT,
        };

        public Vector3 Pos;
        public Structs.Coordinate Coord;

        public Direction Dir;
        public GameObject Owner = null;

        private Piece _pieceAtPosition;
        public Piece PieceAtPosition
        {
            get { return _pieceAtPosition; }
            set { _pieceAtPosition = value; }
        }

        public Move(Vector3 pos, Structs.Coordinate coord, Direction dir, Piece pieceAtPosition)
        {
            Pos = pos;
            Coord = coord;
            Dir = dir;
            PieceAtPosition = pieceAtPosition;
        }

        public Move(Vector3 pos, Structs.Coordinate coord, Piece pieceAtPosition)
        {
            Pos = pos;
            Coord = coord;
            Dir = Move.Direction.NONE;
            PieceAtPosition = pieceAtPosition;
        }
    }
}

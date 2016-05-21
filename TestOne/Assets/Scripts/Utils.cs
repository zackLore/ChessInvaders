using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    public static class Utils
    {
        public static Move CheckMoveDown(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.row + 1 > 7) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col;
            move.Coord.row = coordinate.row + 1;
            move.Dir = Move.Direction.DOWN;
            move.FromDir = Move.Direction.UP;

            //var square = game.Squares[coordinate.y + 1][coordinate.x];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        public static Move CheckMoveDownLeft(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.row + 1 > 7 || coordinate.col - 1 < 0) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col - 1;
            move.Coord.row = coordinate.row + 1;
            move.Dir = Move.Direction.DOWN_LEFT;
            move.FromDir = Move.Direction.UP_RIGHT;

            //var square = game.Squares[coordinate.y + 1][coordinate.x - 1];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        public static Move CheckMoveDownRight(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.row + 1 > 7 || coordinate.col + 1 > 7) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col + 1;
            move.Coord.row = coordinate.row + 1;
            move.Dir = Move.Direction.DOWN_RIGHT;
            move.FromDir = Move.Direction.UP_LEFT;

            //var square = game.Squares[coordinate.y + 1][coordinate.x + 1];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        public static Move CheckMoveLeft(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.col - 1 < 0) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col - 1;
            move.Coord.row = coordinate.row;
            move.Dir = Move.Direction.LEFT;
            move.FromDir = Move.Direction.RIGHT;

            //var square = game.Squares[coordinate.y][coordinate.x - 1];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        public static Move CheckMoveRight(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.col + 1 > 7) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col + 1;
            move.Coord.row = coordinate.row;
            move.Dir = Move.Direction.RIGHT;
            move.FromDir = Move.Direction.LEFT;

            //var square = game.Squares[coordinate.y][coordinate.x + 1];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        public static Move CheckMoveUp(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.row - 1 < 0) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col;
            move.Coord.row = coordinate.row - 1;
            move.Dir = Move.Direction.UP;
            move.FromDir = Move.Direction.DOWN;

            //var square = game.Squares[coordinate.y - 1][coordinate.x];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        public static Move CheckMoveUpLeft(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.row - 1 < 0 || coordinate.col - 1 < 0) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col - 1;
            move.Coord.row = coordinate.row - 1;
            move.Dir = Move.Direction.UP_LEFT;
            move.FromDir = Move.Direction.DOWN_RIGHT;

            //var square = game.Squares[coordinate.y - 1][coordinate.x - 1];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        public static Move CheckMoveUpRight(Game game, Structs.Coordinate coordinate)
        {
            if (coordinate.row - 1 < 0 || coordinate.col + 1 > 7) { return null; }

            Move move = new Move();
            move.Coord.col = coordinate.col + 1;
            move.Coord.row = coordinate.row - 1;
            move.Dir = Move.Direction.UP_RIGHT;
            move.FromDir = Move.Direction.DOWN_LEFT;

            //var square = game.Squares[coordinate.y - 1][coordinate.x + 1];
            var square = game.Squares[move.Coord.row][move.Coord.col];
            move.Pos = square.gameObject.transform.position;

            if (square.transform.childCount > 0)
            {
                move.Attacker = square.transform.GetChild(0).GetComponent<Piece>();
            }
            return move;
        }

        /// <summary>
        /// Takes multidimensional array of GameObjects and returns the index in an n:n format.
        /// </summary
        /// <param name="arr">2 dimensional array of GameObjects</param>
        /// <param name="obj">GameObject to find</param>
        /// <returns>String of index in n:n format.  Empty string if not found.</returns>
        public static string FindIndex(GameObject[][] arr, GameObject obj)
        {
            for (int row = 0; row < arr.Length; row++)
            {
                for (int col = 0; col < arr[row].Length; col++)
                {
                    if (arr[row][col].transform.position == obj.transform.position)
                    {
                        return row + ":" + col;                        
                    }
                }
            }
            return "";
        }

        public static bool IsDot(GameObject obj)
        {
            if (obj.transform.name.ToLower().Contains("move"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Converts string version of Coordinate to Scructs.Coordinate
        /// </summary>
        /// <param name="coordindate">Coordinate of piece as a string</param>
        /// <returns>Scruts.Coordinate version of coordinate</returns>
        public static Structs.Coordinate GetCoordinate(string coordindate)
        {
            Structs.Coordinate co = new Structs.Coordinate();

            try
            {
                if (coordindate != "")
                {
                    string[] vals = coordindate.Split(':');
                    int x = int.Parse(vals[0]);
                    int y = int.Parse(vals[1]);
                    co.col = x;
                    co.row = y;
                }
            }
            catch (Exception)
            {
                //Not numeric
            }

            return co;
        }

        public static GameObject FindGameObject(Vector3 pos)
        {
            GameObject temp = null;
            Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(pos));
            RaycastHit hit;

            Physics.Raycast(ray, out hit);

            return temp;
        }

        // ****************************************************
        // Degug Helpers
        // ****************************************************
        public static void ShowMoves(List<Move> moves)
        {
            foreach (Move move in moves)
            {
                Debug.Log(move.Coord.row + "|" + move.Coord.col);
            }
        }

        public static void ShowMoves(Stack<Move> moves)
        {
            foreach (Move move in moves)
            {
                Debug.Log(move.Coord.row + "|" + move.Coord.col);
            }
        }

    }
}

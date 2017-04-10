using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    public static class Utils
    {        
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
        /// <param name="coordinate">Coordinate of piece as a string</param>
        /// <returns>Scruts.Coordinate version of coordinate</returns>
        public static Structs.Coordinate GetCoordinate(string coordinate)
        {
            Structs.Coordinate co = new Structs.Coordinate();

            try
            {
                if (coordinate != "")
                {
                    string[] vals = coordinate.Split(':');
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
        public static void LogMoves(List<Move> moves)
        {
            foreach (Move move in moves)
            {
                Debug.Log(move.Coord.row + "|" + move.Coord.col);
            }
        }

        public static void LogMoves(Stack<Move> moves)
        {
            foreach (Move move in moves)
            {
                Debug.Log(move.Coord.row + "|" + move.Coord.col);
            }
        }

    }
}

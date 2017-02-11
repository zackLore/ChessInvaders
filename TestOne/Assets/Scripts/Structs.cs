using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public static class Structs
    {
        public struct Coordinate
        {
            public int row;
            public int col;
            
            public Coordinate(int r, int c)
            {
                this.row = r;
                this.col = c;
            }

            public static Coordinate operator +(Coordinate c1, Coordinate c2)
            {
                return new Coordinate(c1.row + c2.row, c1.col + c2.col);
            }

            public Boolean IsValid()
            {
                return  (this.row >= Consts.minRow) && 
                        (this.row <= Consts.maxRow) && 
                        (this.col >= Consts.minCol) && 
                        (this.col <= Consts.maxCol);
            }
        }

        public struct Position
        {
            public double x;
            public double y;
        }
    }
}

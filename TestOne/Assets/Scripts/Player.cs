using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        // ****************************************************
        // Properties
        // ****************************************************
        public Piece[][] Pieces;
        public int PlayerNumber;
        public string Name;

        //represents numbers of specific pieces
        public int FighterCount = 6;
        public int DefenderCount = 4;
        public int DroneCount = 4;
        public int BombCount = 0;
        public int QueenCount = 1;
        public int KingCount = 1;

        public Stack<Move> MoveHistory;

        // ****************************************************
        // Constructors
        // ****************************************************
        #region Player()
        public Player(int number)//TODO: Add more constructors to handle name/number of player
        {
            //InitializePieces();
            //PlayerNumber = number;
            //LoadPieces();
        }
        #endregion

        // ****************************************************
        // MonoBehaviour Events
        // ****************************************************

        // ****************************************************
        // Public Mehtods
        // ****************************************************
        #region InitPlayer(int number)

        public void InitPlayer(int number)
        {
            Pieces = new Piece[2][];
            PlayerNumber = number;
            InitializePieces();
            LoadPieces();
        }

        #endregion

        #region LoadPieces()

        public void LoadPieces()
        {
            //Load Default Piece Loadout
            //0 - f, f, dr, k, q, dr, f, f
            //1 - dr, f, d, d, d, d, f, dr
            if (PlayerNumber == 1)
            {
                //row 0
                Pieces[0][0] = InstantiatePiece(@"Prefabs/Fighter1a");
                Pieces[0][1] = InstantiatePiece(@"Prefabs/Fighter1a");
                Pieces[0][2] = InstantiatePiece(@"Prefabs/Drone1a");
                Pieces[0][3] = InstantiatePiece(@"Prefabs/King1a");
                Pieces[0][4] = InstantiatePiece(@"Prefabs/Queen1a");
                Pieces[0][5] = InstantiatePiece(@"Prefabs/Drone1a");
                Pieces[0][6] = InstantiatePiece(@"Prefabs/Fighter1a");
                Pieces[0][7] = InstantiatePiece(@"Prefabs/Fighter1a");

                //row 1
                Pieces[1][0] = InstantiatePiece(@"Prefabs/Drone1a");
                Pieces[1][1] = InstantiatePiece(@"Prefabs/Fighter1a");
                Pieces[1][2] = InstantiatePiece(@"Prefabs/Defender1a");
                Pieces[1][3] = InstantiatePiece(@"Prefabs/Defender1a");
                Pieces[1][4] = InstantiatePiece(@"Prefabs/Defender1a");
                Pieces[1][5] = InstantiatePiece(@"Prefabs/Defender1a");
                Pieces[1][6] = InstantiatePiece(@"Prefabs/Fighter1a");
                Pieces[1][7] = InstantiatePiece(@"Prefabs/Drone1a");
            }
            else if (PlayerNumber == 2)
            {
                //row 6
                Pieces[0][0] = InstantiatePiece(@"Prefabs/Drone2a");
                Pieces[0][1] = InstantiatePiece(@"Prefabs/Fighter2a");
                Pieces[0][2] = InstantiatePiece(@"Prefabs/Defender2a");
                Pieces[0][3] = InstantiatePiece(@"Prefabs/Defender2a");
                Pieces[0][4] = InstantiatePiece(@"Prefabs/Defender2a");
                Pieces[0][5] = InstantiatePiece(@"Prefabs/Defender2a");
                Pieces[0][6] = InstantiatePiece(@"Prefabs/Fighter2a");
                Pieces[0][7] = InstantiatePiece(@"Prefabs/Drone2a");

                //row 7
                Pieces[1][0] = InstantiatePiece(@"Prefabs/Fighter2a");
                Pieces[1][1] = InstantiatePiece(@"Prefabs/Fighter2a");
                Pieces[1][2] = InstantiatePiece(@"Prefabs/Drone2a");
                Pieces[1][3] = InstantiatePiece(@"Prefabs/King2a");
                Pieces[1][4] = InstantiatePiece(@"Prefabs/Queen2a");
                Pieces[1][5] = InstantiatePiece(@"Prefabs/Drone2a");
                Pieces[1][6] = InstantiatePiece(@"Prefabs/Fighter2a");
                Pieces[1][7] = InstantiatePiece(@"Prefabs/Fighter2a");
            }
        }

        public void LoadPieces(GameObject[][] pieces_)
        {
            //TODO: finish code
        }

        #endregion

        // ****************************************************
        // Private Methods
        // ****************************************************
        private void InitializePieces()
        {
            Pieces[0] = new Piece[Consts.colCount];
            Pieces[1] = new Piece[Consts.colCount];
        }

        private Piece InstantiatePiece(string prefabPath)
        {
            return ((GameObject)Instantiate(Resources.Load(prefabPath), Vector2.zero, Quaternion.identity)).GetComponent<Piece>();
        }
    }
}

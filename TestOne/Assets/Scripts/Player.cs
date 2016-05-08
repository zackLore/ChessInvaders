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
        public GameObject[][] Pieces;
        //public List<Piece> Pieces = new List<Piece>();
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
            Pieces = new GameObject[2][];
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
                Pieces[0][0] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter1a"), Vector2.zero, Quaternion.identity);
                Pieces[0][1] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter1a"), Vector2.zero, Quaternion.identity);
                Pieces[0][2] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone1a"), Vector2.zero, Quaternion.identity);
                Pieces[0][3] = (GameObject)Instantiate(Resources.Load(@"Prefabs/King1a"), Vector2.zero, Quaternion.identity);
                Pieces[0][4] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Queen1a"), Vector2.zero, Quaternion.identity);
                Pieces[0][5] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone1a"), Vector2.zero, Quaternion.identity);
                Pieces[0][6] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter1a"), Vector2.zero, Quaternion.identity);
                Pieces[0][7] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter1a"), Vector2.zero, Quaternion.identity);

                //row 1
                Pieces[1][0] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone1a"), Vector2.zero, Quaternion.identity);
                Pieces[1][1] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter1a"), Vector2.zero, Quaternion.identity);
                Pieces[1][2] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender1a"), Vector2.zero, Quaternion.identity);
                Pieces[1][3] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender1a"), Vector2.zero, Quaternion.identity);
                Pieces[1][4] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender1a"), Vector2.zero, Quaternion.identity);
                Pieces[1][5] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender1a"), Vector2.zero, Quaternion.identity);
                Pieces[1][6] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter1a"), Vector2.zero, Quaternion.identity);
                Pieces[1][7] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone1a"), Vector2.zero, Quaternion.identity);
            }
            else if (PlayerNumber == 2)
            {
                //row 6
                Pieces[0][0] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter2a"), Vector2.zero, Quaternion.identity);
                Pieces[0][1] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter2a"), Vector2.zero, Quaternion.identity);
                Pieces[0][2] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone2a"), Vector2.zero, Quaternion.identity);
                Pieces[0][3] = (GameObject)Instantiate(Resources.Load(@"Prefabs/King2a"), Vector2.zero, Quaternion.identity);
                Pieces[0][4] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Queen2a"), Vector2.zero, Quaternion.identity);
                Pieces[0][5] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone2a"), Vector2.zero, Quaternion.identity);
                Pieces[0][6] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter2a"), Vector2.zero, Quaternion.identity);
                Pieces[0][7] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter2a"), Vector2.zero, Quaternion.identity);

                //row 7
                Pieces[1][0] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone2a"), Vector2.zero, Quaternion.identity);
                Pieces[1][1] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter2a"), Vector2.zero, Quaternion.identity);
                Pieces[1][2] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender2a"), Vector2.zero, Quaternion.identity);
                Pieces[1][3] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender2a"), Vector2.zero, Quaternion.identity);
                Pieces[1][4] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender2a"), Vector2.zero, Quaternion.identity);
                Pieces[1][5] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Defender2a"), Vector2.zero, Quaternion.identity);
                Pieces[1][6] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Fighter2a"), Vector2.zero, Quaternion.identity);
                Pieces[1][7] = (GameObject)Instantiate(Resources.Load(@"Prefabs/Drone2a"), Vector2.zero, Quaternion.identity);
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
            Pieces[0] = new GameObject[8];
            Pieces[1] = new GameObject[8];            
        }
    }
}

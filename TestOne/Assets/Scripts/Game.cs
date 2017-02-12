using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public enum PlayerActionMode
    {
        kSelect,
        kMove,
        kAttack,
    };

    [Serializable]
    public class Game : MonoBehaviour//TODO: Remove duplicate square code, finish implementation of Behaviors
    {
        public enum ScreenPosition
        {
            NONE,
            CENTER,
            TOP_LEFT,
            TOP_RIGHT,
            BOTTOM_LEFT,
            BOTTOM_RIGHT
        }

        public Vector3 LastMousePos;
        Move.RelativeDirection RelativeDir = Move.RelativeDirection.NONE;

        public Player Player1;
        public Player Player2;
        public GameObject CurrentSquare;
        public Player CurrentTurn;
        public Move CurrentMove = null;
        public Piece SelectedPiece;
        public Piece BattleLoser;

        public GameObject Board;
        public GameObject Highlight;
        public GameObject PlaceHolder;
        public GameObject MovePiece;
        public PlayerUI Player_UI;

        //** References to allow buttons to trigger movement and attacks
        public GameObject CurrentMovePiece;
        public GameObject CurrentAttackPiece;

        private float SquareHeight;
        private float SquareWidth;
        private float HalfWidth;
        private float HalfHeight;

        public float MoveStartTime;
        public float MoveSpeed = 1.5f;
        private bool MoveComplete = false;
        
        public GameSquare[][] GameSquares = new GameSquare[Consts.rowCount][];
        public BackgroundSquare[][] BackgroundSquares = new BackgroundSquare[Consts.rowCount][];

        public Move LastMove;

        public Canvas RollMenu;
        public Text rollMenuLabel;
        public Text MoveCountLabel;
        public Text DirectionLabel;
        public Image rollMenuBackground;

        public Canvas AttackMenu;

        //** Preview Menu References
        public Canvas PreviewMenu;
        public Button AttackButton;
        public Button MoveButton;
        public Button RollButton;
        public ScreenPosition PreviewMenuScreenPosition = ScreenPosition.NONE;

        //** Win Menu
        public Canvas WinMenu;

        public SoundManager soundManager;
        
        public bool Dragging = false;
        public bool ActionPieceWasPlaced = false;
        public PlayerActionMode CurrentPlayerActionMode = PlayerActionMode.kSelect;

        public string defaultBGColor = "DDE2E2FF";
        public string p1BGColor = "22FF36FF";
        public string p2BGColor = "EE55B1FF";

        // ****************************************************
        // Events
        // ****************************************************
        void Awake()
        {
            soundManager = GameObject.FindObjectOfType<SoundManager>();
            
            Player1 = (Player)((GameObject)Instantiate(Resources.Load(@"Prefabs/Player"))).GetComponent<Player>();
            Player1.InitPlayer(1);        
            Player1.Name = "Player 1";
            Player2 = (Player)((GameObject)Instantiate(Resources.Load(@"Prefabs/Player"))).GetComponent<Player>();
            Player2.InitPlayer(2);
            Player2.Name = "Player 2";
            CurrentTurn = Player1;

            CurrentPlayerActionMode = PlayerActionMode.kSelect;

            GameSquare testSquare = InstantiateGameSquare(new Vector3(0,0,0), -1, -1);
            SquareHeight = testSquare.spriteRenderer.sprite.rect.height;
            SquareWidth = testSquare.spriteRenderer.sprite.rect.width;
            HalfHeight = SquareHeight / 2;
            HalfWidth = SquareWidth / 2;

            PlaceHolder = (GameObject)Instantiate(Resources.Load(@"Prefabs/PreviewPiece"), Vector3.zero, Quaternion.identity);
            PlaceHolder.SetActive(false);

            MovePiece = (GameObject)Instantiate(Resources.Load(@"Prefabs/MovePiece"), Vector3.zero, Quaternion.identity);
            MovePiece.SetActive(false);

            var texts = RollMenu.GetComponentsInChildren<Text>();
            rollMenuLabel = texts.Where(x => x.name == "RollMenuLabel").FirstOrDefault();
            //MoveCountLabel = texts.Where(x => x.name == "MoveCountLabel").FirstOrDefault();
            DirectionLabel = texts.Where(x => x.name == "DirectionLabel").FirstOrDefault();
            //RollMenu.gameObject.SetActive(true);
            //MoveCountLabel = PreviewMenu.transform.Find("MoveCountLabel").GetComponent<Text>();
            
            Vector3 startPos = new Vector3();
            startPos.x = -SquareWidth * 4;  //  half of the total width (4 squares)
            startPos.y = SquareHeight * 4;
            startPos.z = Consts.zPos_Piece;
                        
            for (int row = 0; row < Consts.rowCount; row++)
            {
                GameSquares[row] = new GameSquare[Consts.colCount];
                BackgroundSquares[row] = new BackgroundSquare[Consts.colCount];

                for (int col = 0; col < Consts.colCount; col++)
                {
                    GameSquares[row][col] = InstantiateGameSquare(startPos, row, col);
                    BackgroundSquares[row][col] = InstantiateBackgroundGameSquare(startPos, row, col);

                    switch (row)
                    {
                        case 0:
                        case 1:
                            InitializePiece(Player1, GameSquares[row][col], row, col, startPos);
                            break;
                        case 6:
                        case 7:
                            InitializePiece(Player2, GameSquares[row][col], row, col, startPos);
                            break;
                    }
                    
                    startPos.x = startPos.x + SquareWidth;
                }

                startPos.x = -SquareWidth * 4;
                startPos.y = startPos.y - SquareHeight;
            }
            
            DestroyImmediate(testSquare.gameObject);
            RollMenu.transform.Find("RollMenuBackground").GetComponent<Image>().color = CurrentTurn.PlayerNumber == 1 ? UnityEngine.Color.green : UnityEngine.Color.magenta;

            var buttons = PreviewMenu.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if (button.name == "AttackButton")
                {
                    AttackButton = button;
                    AttackButton.gameObject.SetActive(false);
                }
                else if(button.name == "MoveButton")
                {
                    MoveButton = button;
                    MoveButton.gameObject.SetActive(false);
                }
                else if (button.name == "RollButton")
                {
                    RollButton = button;
                    RollButton.gameObject.SetActive(true);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (SelectedPiece != null && SelectedPiece.Moving) { return; }

            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                Dragging = true;
            }
            else if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended)
            {
                Dragging = false;
            }

            GetDragDirectionFromSelectedPiece();
            LastMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // Turned off preview menu 9-13-16
            //if (SelectedPiece != null && !SelectedPiece.Moving && (CurrentMode != Mode.kAttack))
            //{
            //    if (!PreviewMenu.gameObject.activeInHierarchy)// Show the Preview Menu
            //    {
            //        ShowPreviewMenu(SelectedPiece, GetSelectedPieceScreenPosition());
            //    }
            //    else
            //    {
            //        if (SelectedPiece.PreviewMoves.Count > 0 && (Dragging))
            //        {
            //            ScreenPosition PreviewPosition = GetScreenPosition(SelectedPiece.PreviewMoves.Peek().Pos);
            //            if (PreviewPosition == PreviewMenuScreenPosition)
            //            {
            //                MovePreviewWindowKiddieCorner(PreviewPosition);
            //            }
            //        }
            //    }
            //}
            //else if (SelectedPiece == null)
            //{
            //    PreviewMenu.gameObject.SetActive(false);
            //}

            if (Dragging && (CurrentPlayerActionMode != PlayerActionMode.kMove))
            {
                HandleMove();
            }
        }

        // ****************************************************
        // Public Methods
        // ****************************************************
        public GameSquare GetGameSquare(Structs.Coordinate coord)
        {
            return GameSquares[coord.row][coord.col];
        }

        public BackgroundSquare GetBackgroundSquare(Structs.Coordinate coord)
        {
            return BackgroundSquares[coord.row][coord.col];
        }

        public void BeginAttack()
        {
            CurrentAttackPiece.GetComponent<AttackSquare>().DoubleClick();
        }

        public void BeginMove()
        {
            CurrentMovePiece.GetComponent<MovePiece>().DoubleClick();
        }

        public bool CheckForAttackSquare(GameObject square)
        {
            bool found = false;
            for (int i = 0; i < square.transform.childCount; i++)
            {
                var child = square.transform.GetChild(i);
                if (child.name.Contains("AttackSquare"))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public void ClearAllHighlights()
        {
            foreach (BackgroundSquare[] rowOfSquares in BackgroundSquares)
            {
                foreach (BackgroundSquare backgroundSquare in rowOfSquares)
                {
                    backgroundSquare.spriteRenderer.color = Color.clear;
                }
            }
        }

        public void ClearAllMovePieces()
        {
            var previews = Board.GetComponentsInChildren<PreviewPiece>();
            var movePieces = Board.GetComponentsInChildren<MovePiece>();
            var attackPieces = Board.GetComponentsInChildren<AttackSquare>();

            if (previews != null)
            {
                foreach (var p in previews)
                {
                    GameObject.Destroy(p.gameObject);
                }
            }

            if (movePieces != null)
            {
                foreach (var mp in movePieces)
                {
                    GameObject.Destroy(mp.gameObject);
                }
            }

            if (attackPieces != null)
            {
                foreach (var ap in attackPieces)
                {
                    GameObject.Destroy(ap.gameObject);
                }
            }
        }

        public void CompleteAttack()
        {
            //** Check for Win/Loss
            if (BattleLoser.PieceType == Piece.TypeOfPiece.King)
            {
                //Show Win Screen
                WinMenu.transform.Find("PlayerLabel").GetComponent<Text>().text = "Player " + CurrentTurn.PlayerNumber;
                WinMenu.gameObject.SetActive(true);
            }

            var p = SelectedPiece;
            p.Moving = false;
            SwapTurns();
            BattleLoser.Active = false;
            BattleLoser.gameObject.SetActive(false);
            if (p.PieceType == Piece.TypeOfPiece.Bomb)
            {
                p.Active = false;
                p.gameObject.SetActive(false);
            }           
            AttackMenu.gameObject.SetActive(false);

            (AttackMenu.transform.Find("AttackPlayerOneValue").gameObject).GetComponent<Text>().text = "0";
            (AttackMenu.transform.Find("AttackPlayerTwoValue").gameObject).GetComponent<Text>().text = "0";
        }

        public void CompleteBombAttack()
        {
            BattleLoser = SelectedPiece.CurrentMove.PieceAtPosition;
            //** Check for Win/Loss
            if (BattleLoser.PieceType == Piece.TypeOfPiece.King)
            {
                //Show Win Screen
                WinMenu.transform.Find("PlayerLabel").GetComponent<Text>().text = "Player " + CurrentTurn.PlayerNumber;
                WinMenu.gameObject.SetActive(true);
            }

            var p = SelectedPiece;
            p.Moving = false;
            SwapTurns();
            BattleLoser.Active = false;
            BattleLoser.gameObject.SetActive(false);
            p.Active = false;
            p.gameObject.SetActive(false);
            AttackMenu.gameObject.SetActive(false);

            (AttackMenu.transform.Find("AttackPlayerOneValue").gameObject).GetComponent<Text>().text = "0";
            (AttackMenu.transform.Find("AttackPlayerTwoValue").gameObject).GetComponent<Text>().text = "0";
        }

        /// <summary>
        /// Displays the Attack Button on the preview menu and hides the others
        /// </summary>
        public void DisplayAttackButton()
        {
            RollButton.gameObject.SetActive(false);
            MoveButton.gameObject.SetActive(false);
            AttackButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// Displays the Attack Button on the preview menu and hides the others
        /// </summary>
        public void DisplayMoveButton()
        {
            RollButton.gameObject.SetActive(false);
            MoveButton.gameObject.SetActive(true);
            AttackButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Displays the Attack Button on the preview menu and hides the others
        /// </summary>
        public void DisplayRollButton()
        {
            RollButton.gameObject.SetActive(true);
            MoveButton.gameObject.SetActive(false);
            AttackButton.gameObject.SetActive(false);
        }

        public void GetDragDirectionFromLastPreviewMove()
        {
            if (SelectedPiece == null){ return; }
            if (SelectedPiece.PreviewMoves.Count <= 0) { return; }

            //Move lastMove = SelectedPiece.PreviewMoves.Peek();
            Move lastMove = SelectedPiece.PreviewMoves.ElementAt(SelectedPiece.PreviewMoves.Count - 2);

            Vector3 mousePos = GetGameSquare(lastMove.Coord).gameObject.transform.position;
            Vector3 last = LastMousePos - lastMove.Pos;
            Vector3 curr = mousePos - lastMove.Pos;

            //Debug.Log("mousePos: " + mousePos + " | lastMove: " + Squares[lastMove.Coord.row][lastMove.Coord.col].transform.position);

            float lastDif = Mathf.Abs(last.x) + Mathf.Abs(last.y);
            float currDif = Mathf.Abs(curr.x) + Mathf.Abs(curr.y);

            if (lastDif < currDif)
            {
                RelativeDir = Move.RelativeDirection.AWAY;
            }
            else if (lastDif > currDif)
            {
                RelativeDir = Move.RelativeDirection.TOWARDS;
            }
            else
            {
                RelativeDir = Move.RelativeDirection.NONE;
            }
        }

        public void GetDragDirectionFromSelectedPiece()
        {
            if (SelectedPiece == null)
            {
                return;
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 last = LastMousePos - SelectedPiece.transform.position;
            Vector3 curr = mousePos - SelectedPiece.transform.position;

            float lastDif = Mathf.Abs(last.x) + Mathf.Abs(last.y);
            float currDif = Mathf.Abs(curr.x) + Mathf.Abs(curr.y);

            if (lastDif < currDif)
            {
                RelativeDir = Move.RelativeDirection.AWAY;
            }
            else if (lastDif > currDif)
            {
                RelativeDir = Move.RelativeDirection.TOWARDS;
            }
            else
            {
                RelativeDir = Move.RelativeDirection.NONE;
            }
        }

        public ScreenPosition GetScreenPosition(Vector3 pos)
        {
            bool isTop = false;
            bool isLeft = false;

            ScreenPosition sp = ScreenPosition.NONE;

            if (pos.x == 0 && pos.y == 0) { return ScreenPosition.CENTER; }

            isTop = pos.y > 0 ? true : false;
            isLeft = pos.x > 0 ? false : true;

            if (isTop && isLeft)
            {
                sp = ScreenPosition.TOP_LEFT;
            }
            else if (isTop && !isLeft)
            {
                sp = ScreenPosition.TOP_RIGHT;
            }
            else if (!isTop && isLeft)
            {
                sp = ScreenPosition.BOTTOM_LEFT;
            }
            else
            {
                sp = ScreenPosition.BOTTOM_RIGHT;
            }

            return sp;
        }

        public ScreenPosition GetSelectedPieceScreenPosition()
        {
            if (SelectedPiece == null) { return ScreenPosition.NONE; }
            bool isTop = false;
            bool isLeft = false;

            ScreenPosition sp = ScreenPosition.NONE;
            Vector3 pos = SelectedPiece.transform.position;

            if (pos.x == 0 && pos.y == 0) { return ScreenPosition.CENTER; }

            isTop = pos.y > 0 ? true : false;
            isLeft = pos.x > 0 ? false : true;

            if (isTop && isLeft)
            {
                sp = ScreenPosition.TOP_LEFT;
            }
            else if (isTop && !isLeft)
            {
                sp = ScreenPosition.TOP_RIGHT;
            }
            else if (!isTop && isLeft)
            {
                sp = ScreenPosition.BOTTOM_LEFT;
            }
            else
            {
                sp = ScreenPosition.BOTTOM_RIGHT;
            }
               
            return sp;
        }

        public Move GetValidMove(Move move)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Move validMove = null;
            GameSquare gameSquare = GetGameSquare(move.Coord);
            if (!gameSquare.IsActive)
            {
                return null;
            }

            float xHigh = gameSquare.gameObject.transform.position.x + HalfWidth;
            float xLow = gameSquare.gameObject.transform.position.x - HalfWidth;
            float yHigh = gameSquare.gameObject.transform.position.y + HalfHeight;
            float yLow = gameSquare.gameObject.transform.position.y - HalfHeight;

            if (mousePos.x <= xHigh && mousePos.x >= xLow &&
                mousePos.y <= yHigh && mousePos.y >= yLow)
            {
                validMove = move;
            }
            return validMove;
        }

        public void HandleMove()
        {
            if (SelectedPiece != null)
            {
                if (SelectedPiece.Dragging)
                {
                    GameSquare gameSquare = null;
                    //Check Direction - if away, add, if towards, remove
                    if (RelativeDir == Move.RelativeDirection.AWAY && (!ActionPieceWasPlaced || SelectedPiece.PieceType == Piece.TypeOfPiece.Queen))
                    {                        
                        SetMovePiece();                        
                    }
                    else if (RelativeDir == Move.RelativeDirection.TOWARDS)
                    {
                        if (SelectedPiece.PreviewMoves.Count > 0)
                        {
                            Move testMove = GetValidMove(SelectedPiece.PreviewMoves.Peek());//Check to see if the last preview move is the current move                        
                            if (testMove != null)
                            {
                                gameSquare = GetGameSquare(testMove.Coord);
                                if (!gameSquare.CanRemove)
                                {
                                    return;
                                }

                                //Limit movement to middle of spot
                                Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameSquare.gameObject.transform.position;
                                float centerSize = HalfWidth / 2;

                                if (Mathf.Abs(diff.x) < centerSize &&
                                    Mathf.Abs(diff.y) < centerSize ) { return; }

                                try
                                {
                                    Debug.Log("MoveBack: Check Squares.");
                                    if (gameSquare.ContainsPreviewPiece())
                                    {
                                        gameSquare.gameObject.transform.GetComponentInChildren<PreviewPiece>().RemoveMove();
                                        MoveBackOne(gameSquare);
                                    }
                                    else if (gameSquare.ContainsEnemyPiece(this))
                                    {
                                        ActionPieceWasPlaced = false;
                                        gameSquare.gameObject.transform.GetComponentInChildren<AttackSquare>().RemoveMove();
                                        MoveBackOne(gameSquare);
                                    }
                                    else if (SquareContainsMovePiece(gameSquare.gameObject))
                                    {
                                        ActionPieceWasPlaced = false;
                                        gameSquare.gameObject.transform.GetComponentInChildren<MovePiece>().RemoveMove();
                                        MoveBackOne(gameSquare);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.Log(gameSquare);
                                    Debug.Log(ex);
                                }

                                //Need to re-do this portion?
                                //SelectedPiece.GetAvailableMoves();
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Handles single touch event
        /// </summary>
        public void HandleSingleTouch()
        {
            ////Debug.Log(CurrentTouch.position);
        }

        public void HidePreviewMenu()
        {
            PreviewMenu.gameObject.SetActive(false);
        }

        public IEnumerator LerpPiece(Piece piece)
        {
            yield return null;

            float length = Vector3.Distance(piece.transform.position, piece.NextMove.Pos);
            float distanceTraveled = (Time.time - MoveStartTime) * MoveSpeed;
            float portionOfMove = distanceTraveled / length;

            if (!piece.FinishedMoving && piece.NextMove != null)
            {
                piece.CurrentMove = piece.NextMove;
                piece.transform.position = Vector3.Lerp(piece.transform.position, piece.NextMove.Pos, portionOfMove);
                if (piece.transform.position.x == piece.NextMove.Pos.x && piece.transform.position.y == piece.NextMove.Pos.y)
                {
                    ////Debug.Log("Piece finished moving.");
                    piece.FinishedMoving = true;
                    piece.HasMoved = true;
                    GameSquare currSquare = GetGameSquare(piece.NextMove.Coord);
                    if (currSquare != null)
                    {
                        if (currSquare.gameObject.transform.childCount > 0)
                        {
                            currSquare.ClickCount = 0;   //  Reset click count for square

                            for (int i = 0; i < currSquare.gameObject.transform.childCount; i++)
                            {
                                var dot = currSquare.gameObject.transform.GetChild(i);
                                if (dot != null)
                                {
                                    if (dot.GetComponent<Piece>() == null)
                                    {
                                        GameObject.Destroy(dot.gameObject);
                                    }
                                }
                            }

                            if ((SelectedPiece.PreviewMoves.Count == 0) && (CurrentPlayerActionMode == PlayerActionMode.kAttack))//Start Attack Sequence
                            {
                                //Debug.Log("Start Attack Sequence...");
                                ShowAttackMenu();
                            }
                        }
                    }
                }
            }
        }

        public void MakePieceMove(Piece piece)
        {
            StartCoroutine(LerpPiece(piece));            
        }

        public void MoveBackOne(GameSquare square)
        {
            square.CanMoveTo = true;//reset the square
            square.CanRemove = false;
            Debug.Log("MoveBackOne...");
            //** Remove the Current Move or Attack if they exist
            CurrentAttackPiece = null;//There is only ever one attack piece placed to remove it if it exists
            CurrentMovePiece = null;//Remove move piece and re-set if appropriate
            if (SelectedPiece.PieceType == Piece.TypeOfPiece.Queen)
            {
                int previewMoveCount = SelectedPiece.PreviewMoves.Count;
                if (previewMoveCount > 0)
                {
                    Structs.Coordinate coord = SelectedPiece.PreviewMoves.ElementAt(0).Coord;
                    Debug.Log("preview count: " + previewMoveCount + "Coordinate: " + coord.row + " | " + coord.col);
                    //var test = square.GetComponentInChildren<MovePiece>();
                    var test = GetGameSquare(coord).gameObject.GetComponentInChildren<MovePiece>();
                    Debug.Log("Test: " + test);
                    //CurrentMovePiece = square.GetComponentInChildren<MovePiece>().gameObject;
                    CurrentMovePiece = test.gameObject;
                    Debug.Log("CurrentMovePiece: " + CurrentAttackPiece );//TODO: NOT DONE! Not setting Current Move Piece
                }
            }            
        }
       
        /// <summary>
        /// Moves preview window out of the way of the player to the left or right
        /// </summary>
        /// <param name="position">position of selected piece</param>
        public void MovePreviewWindow(ScreenPosition position)
        {
            float sw = Screen.width;
            float sh = Screen.height;
            float halfW = sw / 2;
            float halfH = sh / 2;
            float qtrW = halfW / 2;
            float qtrH = halfH / 2;
            
            switch (position)
            {
                case ScreenPosition.TOP_LEFT:
                    PreviewMenuScreenPosition = ScreenPosition.TOP_RIGHT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint( new Vector3(halfW + qtrW, halfH + qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.TOP_RIGHT:
                    PreviewMenuScreenPosition = ScreenPosition.TOP_LEFT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(halfW - qtrW, halfH + qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.BOTTOM_LEFT:
                case ScreenPosition.CENTER:
                    PreviewMenuScreenPosition = ScreenPosition.BOTTOM_RIGHT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(halfW + qtrW, halfH - qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.BOTTOM_RIGHT:
                    PreviewMenuScreenPosition = ScreenPosition.BOTTOM_LEFT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(halfW - qtrW, halfH - qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.NONE:
                    PreviewMenu.transform.position = Vector3.zero;
                    break;
            }
        }

        /// <summary>
        /// Moves preview window out of the way of the player to the left or right
        /// </summary>
        /// <param name="position">position of selected piece</param>
        public void MovePreviewWindowKiddieCorner(ScreenPosition position)
        {
            float sw = Screen.width;
            float sh = Screen.height;
            float halfW = sw / 2;
            float halfH = sh / 2;
            float qtrW = halfW / 2;
            float qtrH = halfH / 2;

            switch (position)
            {
                case ScreenPosition.TOP_LEFT:
                    PreviewMenuScreenPosition = ScreenPosition.BOTTOM_RIGHT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(halfW + qtrW, halfH - qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.TOP_RIGHT:
                    PreviewMenuScreenPosition = ScreenPosition.BOTTOM_LEFT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(halfW - qtrW, halfH - qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.BOTTOM_LEFT:
                case ScreenPosition.CENTER:
                    PreviewMenuScreenPosition = ScreenPosition.TOP_RIGHT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(halfW + qtrW, halfH + qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.BOTTOM_RIGHT:
                    PreviewMenuScreenPosition = ScreenPosition.TOP_LEFT;
                    PreviewMenu.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(halfW - qtrW, halfH + qtrH, 0));
                    PreviewMenu.transform.position += new Vector3(0, 0, 15);
                    break;
                case ScreenPosition.NONE:
                    PreviewMenu.transform.position = Vector3.zero;
                    break;
            }
        }

        public void MoveToSquare(Piece piece, Move move)
        {
            GameSquare square1 = GetGameSquare(piece.StartSpot.Coord);
            square1.gameObject.transform.DetachChildren();
            GameSquare square2 = GetGameSquare(move.Coord);
            //May need to nullify children before detach ?         
            piece.transform.parent = square2.gameObject.transform;
        }

        public void PlaceAttackPiece(GameSquare gameSquare, Move move)
        {
            if (Board.transform.Find("AttackSquare") || Board.transform.FindChild("AttackSquare"))  //  Only place one attack square
            {
                return;
            }

            if (SelectedPiece.PieceType != Piece.TypeOfPiece.Queen)
                SelectedPiece.MovesRemaining--;

            gameSquare.CanMoveTo = false;

            GameObject attackSquare = (GameObject)Instantiate(Resources.Load(Consts.prefabPath_attackSquare), Vector3.zero, Quaternion.identity);
            attackSquare.transform.parent = gameSquare.gameObject.transform;
            attackSquare.transform.position = new Vector3(gameSquare.gameObject.transform.position.x, gameSquare.gameObject.transform.position.y, -2);
            attackSquare.transform.localScale = new Vector3(1, 1, 1);
            attackSquare.GetComponent<AttackSquare>().CurrentMove = move;

            move.Owner = attackSquare;
            gameSquare.CanAttack = true;
            LastMove = move;
            ActionPieceWasPlaced = true;

            CurrentAttackPiece = attackSquare;
            DisplayAttackButton();
        }

        public void PlaceMovePiece(GameSquare gameSquare, Move move)
        {
            if (gameSquare.gameObject.GetComponent<Piece>() != null)
            {
                return;
            }

            if (SelectedPiece.PieceType != Piece.TypeOfPiece.Queen)
                SelectedPiece.MovesRemaining--;

            gameSquare.CanMoveTo = false;

            GameObject moveHolder = (GameObject)Instantiate(MovePiece, Vector3.zero, Quaternion.identity);
            moveHolder.transform.parent = gameSquare.gameObject.transform;
            moveHolder.transform.position = move.Pos;
            moveHolder.SetActive(true);
            moveHolder.GetComponent<MovePiece>().CurrentMove = move;

            move.Owner = moveHolder;
            LastMove = move;
            ActionPieceWasPlaced = true;

            CurrentMovePiece = moveHolder;
            DisplayMoveButton();
        }

        public void PlacePreviewPiece(GameSquare gameSquare, Move move)
        {
            if (gameSquare.gameObject.GetComponent<Piece>() != null)
            {
                return;
            }

            SelectedPiece.MovesRemaining--;

            gameSquare.CanMoveTo = false;

            float pieceSizeDivider = 1f / SelectedPiece.CurrentMoveCount;
            float relativeSize = Mathf.Abs(SelectedPiece.CurrentMoveCount - SelectedPiece.MovesRemaining + 1) * pieceSizeDivider;

            GameObject moveHolder = (GameObject)Instantiate(PlaceHolder, Vector3.zero, Quaternion.identity);
            moveHolder.transform.parent = gameSquare.gameObject.transform;
            moveHolder.transform.position = move.Pos;
            moveHolder.SetActive(true);
            moveHolder.GetComponent<PreviewPiece>().CurrentMove = move;

            move.Owner = moveHolder;

            var img = moveHolder.transform.GetChild(0);
            Vector3 newScale = new Vector3(relativeSize, relativeSize);
            img.transform.localScale = newScale;

            LastMove = move;
        }

        public void RollAttackDice()
        {
            float p1Attack = 0f;
            float p2Defend = 0f;

            while (p1Attack == p2Defend)
            {
                p1Attack = SelectedPiece.AttackDice.RollDice();
                p2Defend = SelectedPiece.CurrentMove.PieceAtPosition.DefendDice.RollDice();
            }

            GameObject p1RollLabel = AttackMenu.transform.Find("AttackPlayerOneValue").gameObject;
            p1RollLabel.GetComponent<Text>().text = p1Attack.ToString();

            GameObject p2RollLabel = AttackMenu.transform.Find("AttackPlayerTwoValue").gameObject;
            p2RollLabel.GetComponent<Text>().text = p2Defend.ToString();

            //Set Loser Piece
            if (p1Attack > p2Defend)
            {
                BattleLoser = SelectedPiece.CurrentMove.PieceAtPosition;
            }
            else
            {
                BattleLoser = SelectedPiece;
            }

            Invoke("CompleteAttack", 1);
        }
        
        public void SetMovePiece()
        {
            GameSquare currSquare = null;

            if (SelectedPiece.MovesRemaining > 0 || SelectedPiece.PieceType == Piece.TypeOfPiece.Queen)
            {                
                //Reset direction
                if (SelectedPiece.MovesRemaining == SelectedPiece.CurrentMoveCount)
                {
                    SelectedPiece.CurrentDirection = Move.Direction.NONE;
                }

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Move validMove = null;

                //Check to see if square is valid move
                foreach (Move move in SelectedPiece.AvailableMoves.Where(x => ((x.PieceAtPosition == null) || (x.PieceAtPosition.IsOwnedByCurrentTurnPlayer() == false)) ))
                {
                    validMove = GetValidMove(move);
                    if (validMove != null)
                    {
                        break;
                    }
                }

                //If a valid move was found
                if (validMove != null)
                {
                    currSquare = GetGameSquare(validMove.Coord);
                    if (!currSquare.CanMoveTo)
                    {
                        return;
                    }

                    //Limit movement to middle of spot
                    Vector3 diff = mousePos - currSquare.gameObject.transform.position;
                    float centerSize = HalfWidth / 2f;

                    if (Mathf.Abs(diff.x) > centerSize || Mathf.Abs(diff.y) > centerSize)
                    {
                        return;
                    }
                    
                    SelectedPiece.CurrentMove = validMove;

                    //Set Move Dot or Attack Dot
                    if (currSquare.ContainsEnemyPiece(this))
                    {
                        PlaceAttackPiece(currSquare, validMove);
                    }
                    else if (   !currSquare.ContainsPreviewPiece() && 
                                (SelectedPiece.MovesRemaining == 1 || SelectedPiece.PieceType == Piece.TypeOfPiece.Queen) )
                    {
                        PlaceMovePiece(currSquare, validMove);
                    }
                    else
                    {
                        PlacePreviewPiece(currSquare, validMove);
                    }

                    //Set Changed Direction Flag
                    if (SelectedPiece.CurrentDirection != Move.Direction.NONE && SelectedPiece.CurrentDirection != validMove.Dir)
                    {
                        SelectedPiece.HasChangedDirection = true;
                    }

                    SelectedPiece.CurrentDirection = validMove.Dir;
                    currSquare.CanMoveTo = false;
                    
                    //MoveCountLabel.text = SelectedPiece.MovesRemaining.ToString();
                    
                    if (SelectedPiece.MovesRemaining > 0)
                    {
                        //Sets preview mode to get accurate moves
                        SelectedPiece.PreviewMode = true;
                        SelectedPiece.PreviewCoord = validMove.Coord;

                        SelectedPiece.PreviewMoves.Push(validMove);
                        if(!ActionPieceWasPlaced || SelectedPiece.PieceType == Piece.TypeOfPiece.Queen)
                            SelectedPiece.GetAvailableMoves();
                    }
                    else if (SelectedPiece.MovesRemaining == 0)
                    {
                        SelectedPiece.PreviewMode = false;
                        SelectedPiece.PreviewMoves.Push(validMove);
                        SelectedPiece.ClearHighlights();
                    }
                    else
                    {
                        SelectedPiece.ClearHighlights();
                    }
                }
            }
        }

        public void ShowAttackMenu()
        {
            if (AttackMenu != null)
            {
                try
                {
                    if (SelectedPiece == null || SelectedPiece.CurrentMove == null || SelectedPiece.CurrentMove.PieceAtPosition == null) { return; }

                    PreviewMenu.gameObject.SetActive(false);

                    //Clear out remaining Attack Squares
                    var attackSquares = this.transform.GetComponentsInChildren<AttackSquare>();
                    if (attackSquares != null)
                    {
                        foreach (AttackSquare attsqr in attackSquares)
                        {
                            GameObject.Destroy(attsqr);
                        }
                    }

                    GameObject pOneLabel = AttackMenu.transform.Find("AttackPlayerOneLabel").gameObject;
                    if (pOneLabel != null)
                    {
                        pOneLabel.GetComponent<Text>().text = (SelectedPiece.GetComponent<Piece>()).PieceType.ToString();
                    }
                    GameObject pTwoLabel = AttackMenu.transform.Find("AttackPlayerTwoLabel").gameObject;
                    if (pTwoLabel != null)
                    {
                        pTwoLabel.GetComponent<Text>().text = (SelectedPiece.GetComponent<Piece>()).CurrentMove.PieceAtPosition.PieceType.ToString();
                    }

                    var p1Image = AttackMenu.transform.Find("AttackPlayerOneImage");
                    p1Image.GetComponent<SpriteRenderer>().sprite = SelectedPiece.GetComponent<SpriteRenderer>().sprite;

                    var p2Image = AttackMenu.transform.Find("AttackPlayerTwoImage");
                    p2Image.GetComponent<SpriteRenderer>().sprite = SelectedPiece.CurrentMove.PieceAtPosition.gameObject.GetComponent<SpriteRenderer>().sprite;

                    AttackMenu.gameObject.SetActive(true);
                }
                catch (Exception)
                {
                    Debug.Log("AttackMenu component was null");
                    //throw;
                }
            }
        }

        public void ShowPreviewMenu(Piece piece, ScreenPosition position)
        {
            PreviewMenu.gameObject.SetActive(true);

            var txtPieceType = PreviewMenu.transform.Find("txtPieceType");
            txtPieceType.GetComponent<Text>().text = piece.PieceType.ToString();

            var txtAttackDice = PreviewMenu.transform.Find("txtAttackDiceMax");
            txtAttackDice.GetComponent<Text>().text = piece.AttackDice.DiceCollection.ElementAt(0).UpperLimit.ToString();

            var txtAttackCount = PreviewMenu.transform.Find("txtAttackDiceCount");
            txtAttackCount.GetComponent<Text>().text = piece.AttackDice.DiceCollection.Count.ToString();

            var txtDefendDice = PreviewMenu.transform.Find("txtDefendDiceMax");
            txtDefendDice.GetComponent<Text>().text = piece.DefendDice.DiceCollection.ElementAt(0).UpperLimit.ToString();

            var txtDefendCount = PreviewMenu.transform.Find("txtDefendDiceCount");
            txtDefendCount.GetComponent<Text>().text = piece.DefendDice.DiceCollection.Count.ToString();

            var txtMoveDice = PreviewMenu.transform.Find("txtMoveDiceMax");
            txtMoveDice.GetComponent<Text>().text = piece.MoveDice.DiceCollection.ElementAt(0).UpperLimit.ToString();

            var txtMoveCount = PreviewMenu.transform.Find("txtMoveDiceCount");
            txtMoveCount.GetComponent<Text>().text = piece.MoveDice.DiceCollection.Count.ToString();

            var playerSpriteRenderer = PreviewMenu.transform.Find("CurrentPlayerImage").GetComponent<SpriteRenderer>();
            playerSpriteRenderer.sprite = SelectedPiece.GetComponent<SpriteRenderer>().sprite;

            var bombButton = PreviewMenu.transform.Find("BombButton");
            if (SelectedPiece.PieceType == Piece.TypeOfPiece.Drone)
            {
                playerSpriteRenderer.gameObject.SetActive(false);
                bombButton.gameObject.SetActive(true);
            }
            else
            {
                playerSpriteRenderer.gameObject.SetActive(true);
                bombButton.gameObject.SetActive(false);
            }

            MovePreviewWindow(position);

            //var rect = PreviewMenu.GetComponent<RectTransform>();
            //Vector3 newPos = piece.transform.position + new Vector3((rect.rect.width / 2) + 40, -((rect.rect.height / 2) + 40), 0);

            //PreviewMenu.transform.position = newPos;

        }

        public void SwapTurns()
        {
            SelectedPiece.Selected = false;
            SelectedPiece = null;
            ClearAllHighlights();
            ClearAllMovePieces();
            //Clear all rolls from all pieces

            Highlight.SetActive(false);
            CurrentPlayerActionMode = PlayerActionMode.kSelect;

            if (CurrentTurn == Player1)
            {
                CurrentTurn = Player2;
            }
            else
            {
                CurrentTurn = Player1;
            }

            RollMenu.transform.Find("RollMenuBackground").GetComponent<Image>().color = CurrentTurn.PlayerNumber == 1 ? UnityEngine.Color.green : UnityEngine.Color.magenta;
            DisplayRollButton();
        }
        
        public Stack<Move> SwapMoves(Stack<Move> moves)
        {
            Stack<Move> newStack = new Stack<Move>();
            int count = moves.Count;
            for (int i = 0; i < count; i++)
            {
                newStack.Push(moves.Pop());
            }
            return newStack;
        }

        /// <summary>
        /// Turns the current selected piece into a bomb if it is a drone
        /// </summary>
        public void TransformIntoBomb()
        {
            //Transform Piece
            SelectedPiece.TransformIntoBomb();
            //Switch turns
            SwapTurns();
        }

        public void UpdateMoveLabel()
        {
            if (SelectedPiece == null) { return; }
            //MoveCountLabel.text = SelectedPiece.MovesRemaining < 0 ? "0" : SelectedPiece.MovesRemaining.ToString();
        }


        // ****************************************************
        // Select Piece Methods
        // ****************************************************
        public void SelectPiece(Piece pieceToSelect)
        {
            if (pieceToSelect != null)
            {
                pieceToSelect.Select();
            }

            SelectedPiece = pieceToSelect;
            Player_UI.UpdateUI(pieceToSelect);
        }

        public void DeselectPiece(Piece pieceToDeselect)
        {
            if (pieceToDeselect != null)
            {
                pieceToDeselect.Deselect();
            }

            ClearAllHighlights();
            ClearAllMovePieces();

            if (soundManager != null)
            {
                soundManager.StopSound();
            }

            pieceToDeselect.ClearHighlights();

            SelectedPiece = null;
            Player_UI.UpdateUI(null);
        }


        // ****************************************************
        // Private Methods
        // ****************************************************
        private void InitializePiece(Player currPlayer, GameSquare currSquare, int row, int col, Vector3 startPos)
        {
            // Initialize piece game object properties
            int playerRowIndex = row % 2;
            Piece currPiece = currPlayer.Pieces[playerRowIndex][col];
            currPiece.gameObject.transform.parent = currSquare.gameObject.transform;
            currPiece.gameObject.transform.position = startPos;
            if (currPlayer.PlayerNumber == 2)
            {
                currPiece.gameObject.transform.Rotate(new Vector3(0, 0, 180));
            }
            // Initialize Piece properties
            currPiece.gameRef = this;
            currPiece.Owner = currPlayer;
            currPiece.Coord = new Structs.Coordinate(row, col);
            currPiece.StartSpot = new Move(startPos, currPiece.Coord, currPiece);
        }

        private Piece FindPiece(Vector3 pos)
        {
            Piece temp = null;
            Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(pos));
            RaycastHit hit;

            Physics.Raycast(ray, out hit);

            return temp;
        }

        private BaseSquare InstantiateSquare(string prefabPathname, string squareName, Vector3 startPos, float zPos, int row, int col)
        {
            BaseSquare newSquare = ((GameObject)Instantiate(Resources.Load(prefabPathname), Vector3.zero, Quaternion.identity)).GetComponent<BaseSquare>();
            newSquare.gameObject.transform.parent = Board.transform;
            newSquare.gameObject.transform.position = new Vector3(startPos.x, startPos.y, zPos);
            newSquare.gameObject.name = squareName + "[" + row + "," + col + "]";
            newSquare.Coord = new Structs.Coordinate(row, col);
            return newSquare;
        }

        private GameSquare InstantiateGameSquare(Vector3 startPos, int row, int col)
        {
            return (GameSquare)InstantiateSquare(Consts.prefabPath_gameSquare, "Square", startPos, Consts.zPos_GameSquare, row, col);
        }

        private BackgroundSquare InstantiateBackgroundGameSquare(Vector3 startPos, int row, int col)
        {
            return (BackgroundSquare)InstantiateSquare(Consts.prefabPath_backgroundSquare, "BG_Square", startPos, Consts.zPos_BackgroundSquare, row, col);
        }

        private bool SquareContainsAttackSquare(GameObject square)
        {
            if (square.transform.childCount <= 0)
            {
                return false;
            }

            for (int i = 0; i < square.transform.childCount; i++)
            {
                //Debug.Log("Child " + i + ": " + square.transform.GetChild(i));
                try
                {
                    AttackSquare temp = square.transform.GetChild(i).GetComponent<AttackSquare>();
                    if (temp != null)
                    {
                        //Debug.Log("MovePiece temp: " + temp);
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        
        private bool SquareContainsMovePiece(GameObject square)
        {
            if (square.transform.childCount <= 0)
            {
                return false;
            }

            for (int i = 0; i < square.transform.childCount; i++)
            {
                //Debug.Log("Child " + i + ": " + square.transform.GetChild(i));
                try
                {
                    MovePiece temp = square.transform.GetChild(i).GetComponent<MovePiece>();
                    if (temp != null)
                    {
                        //Debug.Log("MovePiece temp: " + temp);
                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }


        // ****************************************************
        // End Game Actions
        // ****************************************************
        public void Restart()
        {
            GameObject.FindObjectOfType<LevelManager>().LoadLevel("Level 01");
        }

        public void Quit()
        {
            GameObject.FindObjectOfType<LevelManager>().LoadLevel("MainMenu");
        }
    }
}

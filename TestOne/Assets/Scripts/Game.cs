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
        public GameObject Square;
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

        public List<GameObject> Pieces = new List<GameObject>();//TODO: switch to multidimensional array?
        public GameObject[][] Squares = new GameObject[8][];
        public GameObject[][] BackgroundSquares = new GameObject[8][];

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
            
            Square = (GameObject)Instantiate(Resources.Load(@"Prefabs/GameSquare"), Vector3.zero, Quaternion.identity);
            SquareHeight = Square.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height;
            SquareWidth = Square.gameObject.GetComponent<SpriteRenderer>().sprite.rect.width;
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

            this.Pieces.Clear();
            float height = Square.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height;
            float width = Square.gameObject.GetComponent<SpriteRenderer>().sprite.rect.width;
            
            Vector3 startPos = new Vector3();
            startPos.x = -width * 4;//half of the total width (4 squares)
            startPos.y = height * 4;
            startPos.z = Consts.zPos_Piece;
                        
            for (int row = 0; row < 8; row++)
            {
                Squares[row] = new GameObject[8];
                BackgroundSquares[row] = new GameObject[8];
                for (int col = 0; col < 8; col++)
                {
                    GameObject newSquare = (GameObject)Instantiate(Resources.Load(@"Prefabs/GameSquare"), Vector3.zero, Quaternion.identity);
                    newSquare.transform.parent = Board.transform;
                    newSquare.transform.position = new Vector3(startPos.x, startPos.y, Consts.zPos_GameSquare);
                    newSquare.name = "Square[" + row + "," + col + "]";
                    Squares[row][col] = newSquare;

                    GameObject newBackgroundSquare = (GameObject)Instantiate(Resources.Load(@"Prefabs/BackgroundSquare"), Vector3.zero, Quaternion.identity);
                    newBackgroundSquare.transform.parent = Board.transform;
                    newBackgroundSquare.transform.position = new Vector3(startPos.x, startPos.y, Consts.zPos_BackgroundSquare);
                    newBackgroundSquare.name = "BG_Square[" + row + "," + col + "]";
                    BackgroundSquares[row][col] = newBackgroundSquare;

                    GameObject p = null;
                    Piece temp = null;

                    switch (row)
                    {
                        case 7:
                            p = Player2.Pieces[0][col];
                            p.transform.Rotate(new Vector3(0, 0, 180));
                            temp = p.GetComponent<Piece>();
                            temp.gameRef = this;
                            temp.Owner = Player2;
                            temp.SetPieceType(temp.PieceType);
                            temp.Coord = new Structs.Coordinate { col = col, row = row };
                            temp.StartSpot = new Move() { Pos = startPos, Coord = temp.Coord, Attacker = temp };
                            Pieces.Add(p);
                            break;
                        case 6:
                            p = Player2.Pieces[1][col];
                            p.transform.Rotate(new Vector3(0, 0, 180));
                            temp = p.GetComponent<Piece>();
                            temp.gameRef = this;
                            temp.Owner = Player2;
                            temp.Coord = new Structs.Coordinate { col = col, row = row };
                            temp.StartSpot = new Move() { Pos = startPos, Coord = temp.Coord, Attacker = temp };
                            Pieces.Add(p);
                            break;
                        case 1:
                            p = Player1.Pieces[1][col];
                            temp = p.GetComponent<Piece>();
                            temp.gameRef = this;
                            temp.Owner = Player1;
                            temp.Coord = new Structs.Coordinate { col = col, row = row };
                            temp.StartSpot = new Move() { Pos = startPos, Coord = temp.Coord, Attacker = temp };
                            Pieces.Add(p);
                            break;
                        case 0:
                            p = Player1.Pieces[0][col];
                            temp = p.GetComponent<Piece>();
                            temp.gameRef = this;
                            temp.Owner = Player1;
                            temp.Coord = new Structs.Coordinate { col = col, row = row };
                            temp.StartSpot = new Move() { Pos = startPos, Coord = temp.Coord, Attacker = temp };
                            Pieces.Add(p);
                            break;
                    }

                    if (p != null)
                    {
                        p.transform.parent = newSquare.transform;
                        p.transform.position = startPos;
                    }
                    startPos.x = startPos.x + width;
                }
                startPos.x = -width * 4;
                startPos.y = startPos.y - height;
            }
            
            Square.SetActive(false);//TODO: Use prefab instead of set game objects
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
            foreach (var s in BackgroundSquares)
            {
                foreach (var sq in s)
                {
                    sq.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
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
            BattleLoser = SelectedPiece.CurrentMove.Attacker;
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

            Vector3 mousePos = Squares[lastMove.Coord.row][lastMove.Coord.col].transform.position;
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
            GameObject s = Squares[move.Coord.row][move.Coord.col];
            if (s == null) { return null; }
            if (!s.GetComponent<GameSquare>().IsActive) { return null; }

            float xHigh = s.transform.position.x + HalfWidth;
            float xLow = s.transform.position.x - HalfWidth;
            float yHigh = s.transform.position.y + HalfHeight;
            float yLow = s.transform.position.y - HalfHeight;

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
                    GameObject square = null;
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
                                square = Squares[testMove.Coord.row][testMove.Coord.col];
                                if (!square.GetComponent<GameSquare>().CanRemove) { return; }
                                //Limit movement to middle of spot
                                Vector3 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition) - square.transform.position;
                                float centerSize = HalfWidth / 2;

                                if (Mathf.Abs(diff.x) < centerSize &&
                                    Mathf.Abs(diff.y) < centerSize ) { return; }

                                try
                                {
                                    Debug.Log("MoveBack: Check Squares.");
                                    if (SquareContainsPreviewPiece(square))
                                    {
                                        square.transform.GetComponentInChildren<PreviewPiece>().RemoveMove();
                                        MoveBackOne(square);
                                    }
                                    else if (SquareContainsEnemyPiece(square))
                                    {
                                        ActionPieceWasPlaced = false;
                                        square.transform.GetComponentInChildren<AttackSquare>().RemoveMove();
                                        MoveBackOne(square);
                                    }
                                    else if (SquareContainsMovePiece(square))
                                    {
                                        ActionPieceWasPlaced = false;
                                        square.transform.GetComponentInChildren<MovePiece>().RemoveMove();
                                        MoveBackOne(square);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.Log(square);
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
                    GameObject currSquare = Squares[piece.NextMove.Coord.row][piece.NextMove.Coord.col];
                    if (currSquare != null)
                    {
                        if (currSquare.transform.childCount > 0)
                        {
                            currSquare.GetComponent<GameSquare>().ClickCount = 0;//Reset click count for square
                            for (int i = 0; i < currSquare.transform.childCount; i++)
                            {
                                var dot = currSquare.transform.GetChild(i);
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

        public void MoveBackOne(GameObject square)
        {
            square.GetComponent<GameSquare>().CanMoveTo = true;//reset the square
            square.GetComponent<GameSquare>().CanRemove = false;
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
                    var test = Squares[coord.row][coord.col].GetComponentInChildren<MovePiece>();
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
            var square1 = Squares[piece.StartSpot.Coord.row][piece.StartSpot.Coord.col];
            square1.transform.DetachChildren();
            var square2 = Squares[move.Coord.row][move.Coord.col];
            //May need to nullify children before detach ?         
            piece.transform.parent = square2.transform;
        }

        public void PlaceAttackPiece(GameObject square, Move move)
        {
            if (Board.transform.Find("AttackSquare") || Board.transform.FindChild("AttackSquare"))//Only place one attack square
            {
                return;
            }
            if (SelectedPiece.PieceType != Piece.TypeOfPiece.Queen)
                SelectedPiece.MovesRemaining--;
            square.GetComponent<GameSquare>().CanMoveTo = false;
            GameObject attackSquare = (GameObject)Instantiate(Resources.Load(@"Prefabs/AttackSquare"),
                                                                                        Vector3.zero,
                                                                                        Quaternion.identity);
            attackSquare.transform.parent = square.transform;
            attackSquare.transform.position = new Vector3(square.transform.position.x, square.transform.position.y, -2);
            attackSquare.transform.localScale = new Vector3(1, 1, 1);
            move.Owner = attackSquare;
            square.GetComponent<GameSquare>().CanAttack = true;
            attackSquare.GetComponent<AttackSquare>().CurrentMove = move;
            LastMove = move;
            ActionPieceWasPlaced = true;

            CurrentAttackPiece = attackSquare;
            DisplayAttackButton();
        }

        public void PlaceMovePiece(GameObject square, Move move)
        {
            if (square.GetComponent<Piece>() != null) { return; }
            if (SelectedPiece.PieceType != Piece.TypeOfPiece.Queen)
                SelectedPiece.MovesRemaining--;
            square.GetComponent<GameSquare>().CanMoveTo = false;
            GameObject moveHolder = (GameObject)Instantiate(MovePiece, Vector3.zero, Quaternion.identity);
            moveHolder.transform.parent = square.transform;
            moveHolder.transform.position = move.Pos;

            move.Owner = moveHolder;
            moveHolder.SetActive(true);
            moveHolder.GetComponent<MovePiece>().CurrentMove = move;
            LastMove = move;
            ActionPieceWasPlaced = true;

            CurrentMovePiece = moveHolder;
            DisplayMoveButton();
        }

        public void PlacePreviewPiece(GameObject square, Move move)
        {
            if (square.GetComponent<Piece>() != null) { return; }
            SelectedPiece.MovesRemaining--;
            square.GetComponent<GameSquare>().CanMoveTo = false;
            float pieceSizeDivider = 1f / SelectedPiece.CurrentMoveCount;
            float relativeSize = Mathf.Abs(SelectedPiece.CurrentMoveCount - SelectedPiece.MovesRemaining + 1) * pieceSizeDivider;

            GameObject moveHolder = (GameObject)Instantiate(PlaceHolder, Vector3.zero, Quaternion.identity);
            moveHolder.transform.parent = square.transform;
            moveHolder.transform.position = move.Pos;

            move.Owner = moveHolder;
            moveHolder.SetActive(true);

            var img = moveHolder.transform.GetChild(0);
            Vector3 newScale = new Vector3(relativeSize, relativeSize);
            img.transform.localScale = newScale;

            moveHolder.GetComponent<PreviewPiece>().CurrentMove = move;
            LastMove = move;
        }

        public void RollAttackDice()
        {
            float p1Attack = 0f;
            float p2Defend = 0f;

            while (p1Attack == p2Defend)
            {
                p1Attack = SelectedPiece.AttackDice.RollDice();
                p2Defend = SelectedPiece.CurrentMove.Attacker.DefendDice.RollDice();
            }

            GameObject p1RollLabel = AttackMenu.transform.Find("AttackPlayerOneValue").gameObject;
            p1RollLabel.GetComponent<Text>().text = p1Attack.ToString();

            GameObject p2RollLabel = AttackMenu.transform.Find("AttackPlayerTwoValue").gameObject;
            p2RollLabel.GetComponent<Text>().text = p2Defend.ToString();

            //Set Loser Piece
            if (p1Attack > p2Defend)
            {
                BattleLoser = SelectedPiece.CurrentMove.Attacker;
            }
            else
            {
                BattleLoser = SelectedPiece;
            }

            Invoke("CompleteAttack", 1);
        }
        
        public void SetMovePiece()
        {
            GameObject square = null;
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
                foreach (Move move in SelectedPiece.AvailableMoves.Where(x => x.Attacker == null || x.Attacker.Owner != CurrentTurn))
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
                    square = Squares[validMove.Coord.row][validMove.Coord.col];
                    if (!square.GetComponent<GameSquare>().CanMoveTo) { return; }

                    //Limit movement to middle of spot
                    Vector3 diff = mousePos - square.transform.position;
                    float centerSize = HalfWidth / 2f;

                    if (Mathf.Abs(diff.x) > centerSize || Mathf.Abs(diff.y) > centerSize) { return; }

                    //Set the from
                    validMove.FromCoord = SelectedPiece.PreviewMoves.Count > 0 ?
                                            SelectedPiece.PreviewMoves.Peek().Coord :
                                            SelectedPiece.Coord;
                    validMove.FromPos = SelectedPiece.PreviewMoves.Count > 0 ?
                                            SelectedPiece.PreviewMoves.Peek().Pos :
                                            SelectedPiece.CurrentPosition;
                    SelectedPiece.CurrentMove = validMove;

                    //Set Move Dot or Attack Dot
                    if (SquareContainsEnemyPiece(square))
                    {
                        PlaceAttackPiece(square, validMove);
                    }
                    else if (!SquareContainsPreviewPiece(square) && (SelectedPiece.MovesRemaining == 1 || SelectedPiece.PieceType == Piece.TypeOfPiece.Queen))
                    {
                        PlaceMovePiece(square, validMove);
                    }
                    else
                    {
                        PlacePreviewPiece(square, validMove);
                    }

                    //Set Changed Direction Flag
                    if (SelectedPiece.CurrentDirection != Move.Direction.NONE && SelectedPiece.CurrentDirection != validMove.Dir)
                    {
                        SelectedPiece.HasChangedDirection = true;
                    }

                    SelectedPiece.CurrentDirection = validMove.Dir;
                    square.GetComponent<GameSquare>().CanMoveTo = false;
                    
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
                    if (SelectedPiece == null || SelectedPiece.CurrentMove == null || SelectedPiece.CurrentMove.Attacker == null) { return; }

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
                        pTwoLabel.GetComponent<Text>().text = (SelectedPiece.GetComponent<Piece>()).CurrentMove.Attacker.PieceType.ToString();
                    }

                    var p1Image = AttackMenu.transform.Find("AttackPlayerOneImage");
                    p1Image.GetComponent<SpriteRenderer>().sprite = SelectedPiece.GetComponent<SpriteRenderer>().sprite;

                    var p2Image = AttackMenu.transform.Find("AttackPlayerTwoImage");
                    p2Image.GetComponent<SpriteRenderer>().sprite = SelectedPiece.CurrentMove.Attacker.gameObject.GetComponent<SpriteRenderer>().sprite;

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
        private Piece FindPiece(Vector3 pos)
        {
            Piece temp = null;
            Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(pos));
            RaycastHit hit;

            Physics.Raycast(ray, out hit);

            return temp;
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

        private bool SquareContainsEnemyPiece(GameObject square)
        {
            if (square.transform.childCount <= 0)
            {
                return false;
            }

            for (int i = 0; i < square.transform.childCount; i++)
            {
                //Debug.Log(square.transform.GetChild(i));
                try
                {
                    Piece temp = square.transform.GetChild(i).GetComponent<Piece>();
                    if (temp != null)
                    {
                        if (temp.Owner != CurrentTurn)
                        {
                            //Debug.Log("EnemyPiece temp: " + temp);
                            return true;
                        }
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

        private bool SquareContainsPreviewPiece(GameObject square)
        {
            if (square.transform.childCount <= 0)
            {
                return false;
            }

            for (int i = 0; i < square.transform.childCount; i++)
            {
                //Debug.Log(square.transform.GetChild(i));
                try
                {
                    PreviewPiece temp = square.transform.GetChild(i).GetComponent<PreviewPiece>();                
                    if (temp != null)
                    {
                        //Debug.Log("PreviewPiece temp: " + temp);
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

            // ****************************************************
            // Behavior Implementation
            // ****************************************************
            //public override void LeftClickDown()
            //{
            //    ////Debug.Log(this + " Left Click Down");
            //    Dragging = true;
            //}

            //public override void LeftClickUp()
            //{
            //    ////Debug.Log(this + " Left Click Up : " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //    ClickCount++;
            //    Dragging = false;
            //}

            //public override void LongPressUp()
            //{
            //    ////Debug.Log(this + " Long Press Up");
            //    ClickCount++;
            //    Dragging = false;
            //}

            //public override void DoubleClick()
            //{
            //    //Disabled
            //}

            //

        }
}

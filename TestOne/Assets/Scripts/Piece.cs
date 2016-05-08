using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;

namespace Assets.Scripts
{
    public class Piece : BaseBehavior//TODO: Remove un-needed code, finish implementation of Behaviors
    {
        // ****************************************************
        // Properties
        // ****************************************************
        public enum TypeOfPiece
        {
            None,
            Fighter,
            Defender,
            King,
            Queen,
            Drone,
            Bomb
        }

        public TypeOfPiece PieceType = TypeOfPiece.None;

        public bool Active = true;
        public bool Dragging = false;
        public bool HasMoved = false;
        public bool Moving = false;
        public bool FinishedMoving = true;
        public bool Selected = false;
        public bool HasChangedDirection = false;

        private bool _previewMode;
        public bool PreviewMode
        {
            get
            {
                if (PreviewMoves.Count > 0)
                {
                    _previewMode = true;
                }
                else
                {
                    _previewMode = false;
                }
                return _previewMode;
            }
            set
            {
                _previewMode = value;
            }
        }

        public int AttackLimit = 0;
        public int CurrentAttack = 0;
        public int CurrentDefence = 0;
        public int CurrentMoveCount = 0;
        public int DefendLimit = 0;
        public int MoveLimit = 0;
        public int MovesRemaining = 0;

        public float Height
        {
            get
            {
                Sprite temp = this.gameObject.GetComponent<SpriteRenderer>().sprite;
                if(temp != null)
                {
                    return temp.rect.height;
                }
                else
                {
                    return 0;
                }
            }
        }

        public float Width
        {
            get
            {
                Sprite temp = this.gameObject.GetComponent<SpriteRenderer>().sprite;
                if (temp != null)
                {
                    return temp.rect.width;
                }
                else
                {
                    return 0;
                }
            }
        }

        public Player Owner;//Owner of the piece

        public Structs.Coordinate Coord;
        public Structs.Coordinate PreviewCoord;

        private Vector3 _currentPosition;
        public Vector3 CurrentPosition
        {
            get
            {                
                return _currentPosition;
            }
            set
            {
                _currentPosition = value;
                this.gameObject.transform.position = value;
            }
        }
        
        public List<Move> AvailableMoves = null;
        public Stack<Move> PreviewMoves = null;

        //public Move CurrentMove = null;
        public Move LastMove = null;
        public Move StartSpot = null;

        public Dice AttackDice  = null;
        public Dice DefendDice  = null;
        public Dice MoveDice    = null;

        public Game gameRef = null;//reference to main game script
        public Move NextMove = null;
        public Move.Direction CurrentDirection = Move.Direction.NONE;

        public TimeSpan HoverTime;
        public float HoverStart = 1.5f;

        void Awake()
        {
            GameRef = GameObject.Find("Game").GetComponent<Game>();
            InitClickTimes();
        }

        void Start()
        {
            AvailableMoves = new List<Move>();
            PreviewMoves = new Stack<Move>();
            _currentPosition = this.gameObject.transform.position;
            AttackDice = (Dice)((GameObject)Instantiate(Resources.Load(@"Prefabs/Dice"))).GetComponent<Dice>();
            AttackDice.transform.parent = this.transform;
            DefendDice = (Dice)((GameObject)Instantiate(Resources.Load(@"Prefabs/Dice"))).GetComponent<Dice>();
            DefendDice.transform.parent = this.transform;
            MoveDice = (Dice)((GameObject)Instantiate(Resources.Load(@"Prefabs/Dice"))).GetComponent<Dice>();
            MoveDice.transform.parent = this.transform;
            SetDice();
        }

        void OnMouseEnter()
        {
            HoverTime = DateTime.Now.TimeOfDay;
        }

        void OnMouseOver()
        {
            DetectClicks();
            if (DateTime.Now.TimeOfDay - HoverTime > TimeSpan.FromSeconds(HoverStart))
            {
                GameRef.ShowPreviewMenu(this);
            }
            else
            {
                GameRef.HidePreviewMenu();
            }
            //if ((DateTime.Now.TimeOfDay - HoverTime).Milliseconds > (100 * DoubleClickTime))
            //{
            //    ClickCount = 0;
            //}
        }

        void OnMouseExit()
        {
            GameRef.HidePreviewMenu();
            HoverTime = TimeSpan.FromSeconds(0);
            ClickCount = 0;
        }

        void FixedUpdate()
        {
            if (Moving)
            {
                if (PreviewMoves.Count >= 0)
                {
                    //Debug.Log("Preview Moves: " + PreviewMoves.Count);
                    if (!FinishedMoving)
                    {
                        //Debug.Log("MakePieceMove: " + NextMove.Coord.row + "|" + NextMove.Coord.col);
                        if (NextMove != null)
                        {
                            this.LastMove = this.CurrentMove;//TODO: Needs to be current location.  Current Move is incorrect for this spot
                            gameRef.MakePieceMove(this);
                            gameRef.SelectedPiece.Coord = NextMove.Coord;                                        gameRef.CurrentMove = this.CurrentMove;                
                        }
                    }
                    else
                    {
                        //Debug.Log("Finished Moving, NextMove = null");
                        if (PreviewMoves.Count == 0)//Move is complete, clear out values and make next game iteration
                        {
                            //Debug.Log("PreviewMoves = " + PreviewMoves.Count());
                            gameRef.MoveToSquare(this, this.CurrentMove);
                            this.StartSpot = this.CurrentMove;
                            Moving = false;

                            gameRef.MoveMode = false;
                            ResetValues();
                            gameRef.MoveCountLabel.text = gameRef.SelectedPiece.CurrentMoveCount.ToString();
                            if (gameRef.SelectedPiece.CurrentMove != null && gameRef.SelectedPiece.CurrentMove.Attacker != null)
                            {
                                //Debug.Log("Attacker = " + gameRef.SelectedPiece.CurrentMove.Attacker);
                                if (gameRef.AttackMenu == null)
                                {
                                    try
                                    {
                                        gameRef.AttackMenu = (Canvas)Instantiate(Resources.Load(@"Menus/AttackMenu"));
                                    }
                                    catch (Exception)
                                    {
                                        //Could not load attack menu
                                        throw;
                                    }
                                }
                                gameRef.AttackMode = true;
                                gameRef.ShowAttackMenu();
                            }
                            else
                            {
                                gameRef.SwapTurns();
                            }
                        }
                        else
                        {
                            NextMove = PreviewMoves.Pop();
                            FinishedMoving = false;
                        }
                    }
                }
                else
                {
                    NextMove = null;
                    Moving = false;
                }
            }
        }

        // ****************************************************
        // Public Methods
        // ****************************************************
        public void ClearHighlight(Move move)
        {
            var square = gameRef.Squares[move.Coord.row][move.Coord.col];
            if (square != null)
            {
                //reset color
                square.GetComponent<SpriteRenderer>().color = Color.white;
            }                
        }

        public void ClearHighlights()
        {
            if (AvailableMoves.Count > 0)
            {
                foreach (Move move in AvailableMoves)
                {
                    var square = gameRef.BackgroundSquares[move.Coord.row][move.Coord.col];
                    if (square != null)
                    {
                        //reset color
                        square.GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 0f, 0f);
                    }
                }
            }
        }

        public void ClearValues()
        {
            HasChangedDirection = false;
            PreviewMoves.Clear();
            CurrentDirection = Move.Direction.NONE;
            //gameRef.DirectionLabel.text = CurrentDirection.ToString();
            MovesRemaining = CurrentMoveCount;
            LastMove = null;
        }

        public void GetAvailableMoves()
        {
            ClearHighlights();
            AvailableMoves.Clear();
            //Debug.Log("Get Available Moves - " + PieceType + " - Coord " + Coord.col + "|" + Coord.row + ": ");
            switch(PieceType)
            {
                case TypeOfPiece.Fighter:
                    try
                    {
                        if (!HasChangedDirection || CurrentDirection == Move.Direction.NONE)
                        {
                            AvailableMoves = GetMovesAllDirections();
                        }
                        else
                        {
                            AvailableMoves = GetMovesAllDirections().Where(x => x.Dir == CurrentDirection).ToList();
                        }
                        //Utils.ShowMoves(AvailableMoves);
                    }
                    catch (Exception)
                    {
                        throw ;
                    }
                    break;
                case TypeOfPiece.Defender:
                    try
                    {
                        if (CurrentDirection == Move.Direction.NONE)
                        {
                            AvailableMoves = GetMovesAllDirections();
                        }
                        else
                        {
                            AvailableMoves = GetMovesAllDirections().Where(x => x.Dir == CurrentDirection).ToList();
                        }
                        //Utils.ShowMoves(AvailableMoves);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    break;
                case TypeOfPiece.Drone:
                    try
                    {
                        if (CurrentDirection == Move.Direction.NONE)
                        {
                            AvailableMoves = GetMovesNoDiagonals();
                        }
                        else
                        {
                            AvailableMoves = GetMovesNoDiagonals().Where(x => x.Dir == CurrentDirection).ToList();
                        }
                        //Utils.ShowMoves(AvailableMoves);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    break;
                case TypeOfPiece.Bomb:
                    try
                    {
                        if (!HasChangedDirection || CurrentDirection == Move.Direction.NONE)
                        {
                            AvailableMoves = GetMovesAllDirections();
                        }
                        else
                        {
                            AvailableMoves = GetMovesAllDirections().Where(x => x.Dir == CurrentDirection).ToList();
                        }
                        //Utils.ShowMoves(AvailableMoves);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    break;
                case TypeOfPiece.Queen:
                    try
                    {
                        if (CurrentDirection == Move.Direction.NONE)
                        {
                            AvailableMoves = GetMovesAllDirections();
                        }
                        else
                        {
                            AvailableMoves = GetMovesAllDirections().Where(x => x.Dir == CurrentDirection).ToList();
                        }
                        //Utils.ShowMoves(AvailableMoves);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    break;
                case TypeOfPiece.King:
                    try
                    {
                        AvailableMoves = GetMovesAllDirections();
                        //Utils.ShowMoves(AvailableMoves);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    break;
            }
            SetHighlights();
        }

        public bool PreviewMoveExists(Move move)
        {
            //Debug.Log("Preview Moves: " + PreviewMoves.Count());
            if (move == null) { Debug.Log("Null Move"); }
            //Debug.Log("Move: " + move.Coord.row + " | " + move.Coord.col);
            foreach (Move m in PreviewMoves)
            {
                //Debug.Log(move.Coord.row + " | " + move.Coord.col);
                if (m.Coord.row == move.Coord.row &&
                    m.Coord.col == move.Coord.col)
                {
                    return true;
                }
            }
            return false;
        }

        public void ResetValues()
        {
            Coord = CurrentMove != null ? CurrentMove.Coord : Coord;
            CurrentMoveCount = 0;
            HasChangedDirection = false;
            CurrentDirection = Move.Direction.NONE;
            //gameRef.DirectionLabel.text = CurrentDirection.ToString();
            MovesRemaining = CurrentMoveCount;
        }

        public void SetAttackHighlight(Move move)
        {
            var square = gameRef.BackgroundSquares[move.Coord.row][move.Coord.col];
            if (square != null)
            {
                //reset color
                square.GetComponent<SpriteRenderer>().color = new Color(200f, 0f, 0f, .5f);
            }
        }

        public void SetDice()
        {
            switch (PieceType)
            {
                case TypeOfPiece.Fighter:
                    AttackLimit = 20;
                    DefendLimit = 6;
                    MoveLimit = 6;
                    AttackDice.InitDice(1, 20);
                    DefendDice.InitDice(1, 6);
                    MoveDice.InitDice(1, 6);
                    break;
                case TypeOfPiece.Defender:
                    AttackLimit = 6;
                    DefendLimit = 20;
                    MoveLimit = 4;
                    AttackDice.InitDice(1, 6);
                    DefendDice.InitDice(1, 20);
                    MoveDice.InitDice(1, 4);
                    break;
                case TypeOfPiece.Drone:
                    AttackLimit = 10;
                    DefendLimit = 10;
                    MoveLimit = 6;
                    AttackDice.InitDice(1, 10);
                    DefendDice.InitDice(1, 10);
                    MoveDice.InitDice(1, 6);
                    break;
                case TypeOfPiece.Bomb:
                    AttackLimit = 20;
                    DefendLimit = 6;
                    MoveLimit = 6;
                    MoveDice.InitDice(1, 6);
                    break;
                case TypeOfPiece.Queen:
                    AttackLimit = 20;
                    DefendLimit = 6;
                    MoveLimit = 6;
                    MoveDice.InitDice(3, 6);
                    AttackDice.InitDice(3, 6);
                    DefendDice.InitDice(3, 6);
                    break;
                case TypeOfPiece.King:
                    AttackLimit = 100;
                    DefendLimit = 0;
                    MoveLimit = 1;
                    MoveDice.InitDice(1, 1);
                    AttackDice.InitDice(1, 100, 99);
                    DefendDice.InitDice(1, 0, 0);
                    break;
            }
        }

        public void SetFriendlyHighlight(Move move)
        {
            var square = gameRef.BackgroundSquares[move.Coord.row][move.Coord.col];
            if (square != null)
            {
                //reset color
                square.GetComponent<SpriteRenderer>().color = new Color(10f, 10f, 10f, .1f);
            }
        }

        public void SetHighlights()
        {
            if (AvailableMoves.Count > 0)
            {
                foreach (Move move in AvailableMoves)
                {
                    var square = gameRef.Squares[move.Coord.row][move.Coord.col];
                    if (square != null)
                    {
                        if (move.Attacker != null)
                        {
                            if (move.Attacker.Owner.PlayerNumber != gameRef.CurrentTurn.PlayerNumber)
                            {
                                SetAttackHighlight(move);
                            }
                            else if (move.Attacker.Owner.PlayerNumber == gameRef.CurrentTurn.PlayerNumber)
                            {
                                SetFriendlyHighlight(move);
                            }
                        }
                        else
                        {
                            SetMoveHighlight(move);
                        }
                    }
                }
            }
        }

        public void SetMoveHighlight(Move move)
        {
            var square = gameRef.BackgroundSquares[move.Coord.row][move.Coord.col];
            if (square != null)
            {
                //reset color
                square.GetComponent<SpriteRenderer>().color = new Color(0f, 200f, 0f, .5f);
            }
            else
            {
                //Debug.Log("Move null: " + move);
            }
        }

        public void SetPieceType(TypeOfPiece type)//TODO: Update these values with accurate dice 
        {
            PieceType = type;
            if (AttackDice == null || DefendDice == null || MoveDice == null)
            {
                return;
            }
            switch (PieceType)
            {
                case TypeOfPiece.Fighter:
                    AttackLimit = 20;
                    DefendLimit = 6;
                    MoveLimit   = 6;
                    AttackDice.InitDice(1, 20);
                    DefendDice.InitDice(1, 6);
                    MoveDice.InitDice(1, 6);
                    break;
                case TypeOfPiece.Defender:
                    AttackLimit = 6;
                    DefendLimit = 20;
                    MoveLimit   = 4;
                    AttackDice.InitDice(1, 6);
                    DefendDice.InitDice(1, 20);
                    MoveDice.InitDice(1, 4);
                    break;
                case TypeOfPiece.Drone:
                    AttackLimit = 10;
                    DefendLimit = 10;
                    MoveLimit   = 6;
                    AttackDice.InitDice(1, 10);
                    DefendDice.InitDice(1, 10);
                    MoveDice.InitDice(1, 6);
                    break;
                case TypeOfPiece.Bomb:
                    AttackLimit = 20;
                    DefendLimit = 6;
                    MoveLimit   = 6;
                    MoveDice.InitDice(1, 6);
                    break;
                case TypeOfPiece.Queen:
                    AttackLimit = 20;
                    DefendLimit = 6;
                    MoveLimit   = 6;
                    MoveDice.InitDice(3, 6);
                    AttackDice.InitDice(3, 6);
                    DefendDice.InitDice(3, 6);
                    break;
                case TypeOfPiece.King:
                    AttackLimit = 20;
                    DefendLimit = 6;
                    MoveLimit   = 6;
                    MoveDice.InitDice(1, 1);
                    AttackDice.InitDice(1, 100, 99);
                    DefendDice.InitDice(1, 0, 0);
                    break;
            }        
        }

        // ****************************************************
        // Private Methods
        // ****************************************************
        private List<Move> GetMovesAllDirections()
        {
            List<Move> moves = new List<Move>();
            //Debug.Log("Move: " + Coord.row + " | " + Coord.col);
            
            Move moveDown = PreviewMode == true ? Utils.CheckMoveDown(gameRef, PreviewCoord) : Utils.CheckMoveDown(gameRef, Coord);
            if (moveDown != null)
            {
                moves.Add(moveDown);
            }
            
            Move moveDownLeft = PreviewMode == true ? Utils.CheckMoveDownLeft(gameRef, PreviewCoord) : Utils.CheckMoveDownLeft(gameRef, Coord); 
            if (moveDownLeft != null)
            {
                moves.Add(moveDownLeft);
            }
            
            Move moveLeft = PreviewMode == true ? Utils.CheckMoveLeft(gameRef, PreviewCoord) : Utils.CheckMoveLeft(gameRef, Coord);
            if (moveLeft != null)
            {
                moves.Add(moveLeft);
            }
            
            Move moveUpLeft = PreviewMode == true ? Utils.CheckMoveUpLeft(gameRef, PreviewCoord) : Utils.CheckMoveUpLeft(gameRef, Coord);
            if (moveUpLeft != null)
            {
                moves.Add(moveUpLeft);
            }
            
            Move moveUp = PreviewMode == true ? Utils.CheckMoveUp(gameRef, PreviewCoord) : Utils.CheckMoveUp(gameRef, Coord);
            if (moveUp != null)
            {
                moves.Add(moveUp);
            }
            
            Move moveUpRight = PreviewMode == true ? Utils.CheckMoveUpRight(gameRef, PreviewCoord) : Utils.CheckMoveUpRight(gameRef, Coord);
            if (moveUpRight != null)
            {
                moves.Add(moveUpRight);
            }
            
            Move moveRight = PreviewMode == true ? Utils.CheckMoveRight(gameRef, PreviewCoord) : Utils.CheckMoveRight(gameRef, Coord);
            if (moveRight != null)
            {
                moves.Add(moveRight);
            }
            
            Move moveDownRight = PreviewMode == true ? Utils.CheckMoveDownRight(gameRef, PreviewCoord) : Utils.CheckMoveDownRight(gameRef, Coord);
            if (moveDownRight != null)
            {
                moves.Add(moveDownRight);
            }
            return moves;
        }

        private List<Move> GetMovesNoDiagonals()
        {
            List<Move> moves = new List<Move>();

            Move moveDown = PreviewMode == true ? Utils.CheckMoveDown(gameRef, PreviewCoord) : Utils.CheckMoveDown(gameRef, Coord);
            if (moveDown != null)
            {
                moves.Add(moveDown);
            }
            
            Move moveLeft = PreviewMode == true ? Utils.CheckMoveLeft(gameRef, PreviewCoord) : Utils.CheckMoveLeft(gameRef, Coord);
            if (moveLeft != null)
            {
                moves.Add(moveLeft);
            }

            Move moveUp = PreviewMode == true ? Utils.CheckMoveUp(gameRef, PreviewCoord) : Utils.CheckMoveUp(gameRef, Coord);
            if (moveUp != null)
            {
                moves.Add(moveUp);
            }
            
            Move moveRight = PreviewMode == true ? Utils.CheckMoveRight(gameRef, PreviewCoord) : Utils.CheckMoveRight(gameRef, Coord);
            if (moveRight != null)
            {
                moves.Add(moveRight);
            }
            
            return moves;
        }

        // ****************************************************
        // Behavior Implementation
        // ****************************************************
        public override void LeftClickDown()
        {
            //Debug.Log(this + " Left Click Down");
            //ClickCount++;
            Dragging = true;
        }

        public override void LeftClickUp()
        {
            //Debug.Log(this + " Left Click Up");
            Dragging = false;
            if (GameRef.SelectedPiece != null && GameRef.SelectedPiece.Moving)
            {
                return;
            }

            if (GameRef.SelectMode)//when not moving - select piece or deselect piece
            {
                if (GameRef.SelectedPiece == null)//piece selected
                {
                    if (this.Owner == GameRef.CurrentTurn)
                    {
                        GameRef.SelectedPiece = this;
                        this.Selected = true;
                        Debug.Log("CurrentMoveCount: " + CurrentMoveCount);
                        if (CurrentMoveCount > 0)
                        {
                            ClearValues();
                            GetAvailableMoves();
                            Debug.Log("Moves Gotten");
                        }
                    }
                }
                else
                {
                    if (this.Owner == GameRef.CurrentTurn)
                    {
                        if (this != GameRef.SelectedPiece)//Selecting a new piece
                        {
                            GameRef.SelectedPiece.Selected = false;
                            GameRef.ClearAllHighlights();
                            GameRef.ClearAllMovePieces();
                            ClearValues();
                            //Check to see if piece can be selected

                            //Assign piece as selected piece
                            GameRef.SelectedPiece = this;
                            Selected = true;
                            Debug.Log("CurrentMoveCount: " + CurrentMoveCount);
                            if (CurrentMoveCount > 0)
                            {
                                GetAvailableMoves();
                                Debug.Log("Moves Gotten");
                            }
                        }
                        else
                        {
                            Selected = false;
                            GameRef.SelectedPiece = null;
                            GameRef.ClearAllHighlights();
                            GameRef.ClearAllMovePieces();
                            ClearValues();
                        }
                    }
                    GameRef.UpdateMoveLabel();
                }

                if (GameRef.SelectedPiece != null && this.Owner == GameRef.CurrentTurn)
                {
                    GameRef.Highlight.transform.position = GameRef.SelectedPiece.transform.position;
                    GameRef.Highlight.SetActive(true);

                    GameRef.RollMenu.gameObject.SetActive(true);
                    GameRef.rollMenuLabel.text = GameRef.SelectedPiece.PieceType.ToString();
                    GameRef.rollMenuLabel.text += "\n" + GameRef.SelectedPiece.Coord.row + " | " + GameRef.SelectedPiece.Coord.col;
                }
                else
                {
                    GameRef.Highlight.SetActive(false);
                }
            }
        }

        public override void LongPressUp()
        {
            //Debug.Log(this + " Long Press Up");
            Dragging = false;
        }

        public override void DoubleClick()
        {
            //Debug.Log(this + " Double Click");
            Dragging = false;
            if (this == GameRef.SelectedPiece)
            {
                GameRef.RollMoveDice();
            }
        }

    }
}

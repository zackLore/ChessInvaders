using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [Serializable]
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

            InitializeDice();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            HoverTime = DateTime.Now.TimeOfDay;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            DetectClicks(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            //DetectClicks(false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            GameRef.HidePreviewMenu();
            HoverTime = TimeSpan.FromSeconds(0);
            ClickCount = 0;
        }

        void Update()
        {
            //if (PieceType == TypeOfPiece.Bomb)
            //{
            //    if (colorSwitchCounter < colorSwitchMax)
            //    {
            //        colorSwitchCounter++;
            //    }
            //    else
            //    {
            //        colorSwitchCounter = 0;

            //        colorIndex = colorIndex < bombColors.Length - 1 ? colorIndex + 1 : 0;
            //        GetComponent<SpriteRenderer>().color = bombColors[colorIndex];
            //    }
            //}
        }

        void FixedUpdate()
        {
            if (Moving)
            {
                //if (PieceType == TypeOfPiece.King) { GetComponent<Animator>().SetBool("Moving", true); }
                if (PreviewMoves.Count >= 0)
                {
                    if (!FinishedMoving)
                    {
                        if (NextMove != null)
                        {
                            this.LastMove = this.CurrentMove;//TODO: Needs to be current location.  Current Move is incorrect for this spot
                            gameRef.MakePieceMove(this);
                            gameRef.SelectedPiece.Coord = NextMove.Coord;
                            gameRef.CurrentMove = this.CurrentMove;
                        }
                    }
                    else
                    {
                        if (PreviewMoves.Count == 0)//Move is complete, clear out values and make next game iteration
                        {
                            gameRef.MoveToSquare(this, this.CurrentMove);
                            this.StartSpot = this.CurrentMove;
                            Moving = false;
                            
                            ResetValues();
                            //gameRef.MoveCountLabel.text = gameRef.SelectedPiece.CurrentMoveCount.ToString();
                            if (gameRef.SelectedPiece.CurrentMove != null && gameRef.SelectedPiece.CurrentMove.PieceAtPosition != null)
                            {
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
                                gameRef.CurrentPlayerActionMode = PlayerActionMode.kAttack;

                                if (PieceType == TypeOfPiece.Bomb || 
                                    CurrentMove != null && CurrentMove.PieceAtPosition != null && CurrentMove.PieceAtPosition.PieceType == TypeOfPiece.Bomb)
                                {
                                    //Bomb Blows up!
                                    GameRef.CompleteBombAttack();
                                }
                                else
                                {
                                    gameRef.ShowAttackMenu();
                                }
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
            else
            {
                //if (PieceType == TypeOfPiece.King) { GetComponent<Animator>().SetBool("Moving", false); }
            }
        }

        // ****************************************************
        // Public Methods
        // ****************************************************
        #region Piece Highlight Methods
        
        public void SetHighlight(Move move, Color color)
        {
            var backgroundSquare = gameRef.GetBackgroundSquare(move.Coord);
            if (backgroundSquare != null)
            {
                backgroundSquare.SetColor(color);
            }
        }

        public void ClearHighlight(Move move)
        {
            SetHighlight(move, Color.clear);
        }

        public void ClearHighlights()
        {
            if (AvailableMoves.Count > 0)
            {
                foreach (Move move in AvailableMoves)
                {
                    ClearHighlight(move);
                }
            }
        }
        
        public void SetHighlights()
        {
            if (AvailableMoves.Count > 0)
            {
                foreach (Move move in AvailableMoves)
                {
                    Color highlightColor = Consts.highlightColor_move;
                    if (move.PieceAtPosition != null)
                    {
                        highlightColor = move.PieceAtPosition.IsOwnedByCurrentTurnPlayer() ? Consts.highlightColor_friendly : Consts.highlightColor_attack;
                    }
                    SetHighlight(move, highlightColor);
                }
            }
        }

        #endregion //   Piece Highlight Methods

        public bool IsOwnedByCurrentTurnPlayer()
        {
            return (Owner.PlayerNumber == gameRef.CurrentTurn.PlayerNumber);
        }

        public void ClearValues()
        {
            HasChangedDirection = false;
            PreviewMoves.Clear();
            CurrentDirection = Move.Direction.NONE;
            MovesRemaining = CurrentMoveCount;
            LastMove = null;
            GameRef.UpdateMoveLabel();
        }

        public Move GetAvailableMoveAtCoordinate(Structs.Coordinate currCoord)
        {
            return AvailableMoves.Find(x => (x.Coord.Equals(currCoord) == true) && ((x.PieceAtPosition == null) || (x.PieceAtPosition.IsOwnedByCurrentTurnPlayer() == false)));
        }

        public void UpdateAvailableMoves()
        {
            ClearHighlights();
            AvailableMoves.Clear();
            AvailableMoves = GetAvailableMoves();
            SetHighlights();
        }
        
        public void InitializeDice()
        {
            AttackDice = (Dice)((GameObject)Instantiate(Resources.Load(@"Prefabs/Dice"))).GetComponent<Dice>();
            AttackDice.transform.parent = this.transform;
            DefendDice = (Dice)((GameObject)Instantiate(Resources.Load(@"Prefabs/Dice"))).GetComponent<Dice>();
            DefendDice.transform.parent = this.transform;
            MoveDice = (Dice)((GameObject)Instantiate(Resources.Load(@"Prefabs/Dice"))).GetComponent<Dice>();
            MoveDice.transform.parent = this.transform;

            //SetDice();
        }
        
        public bool PreviewMoveExists(Move move)
        {
            if (move == null)
            {
                Debug.Log("Null Move");
            }

            foreach (Move m in PreviewMoves)
            {
                if (m.Coord.Equals(move.Coord))
                {
                    return true;
                }
            }
            return false;
        }
        
        public void ResetValues()
        {
            Coord = (CurrentMove != null) ? CurrentMove.Coord : Coord;
            CurrentMoveCount = 0;
            HasChangedDirection = false;
            CurrentDirection = Move.Direction.NONE;
            MovesRemaining = CurrentMoveCount;
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

        public void SetPieceType(TypeOfPiece type)
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
                    AttackDice.InitDice(1, 100, 99);
                    DefendDice.InitDice(1, 100, 99);
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
        
        public void TransformIntoBomb()
        {
            if (PieceType == TypeOfPiece.Drone)
            {
                SetPieceType(TypeOfPiece.Bomb);
                //Change Image
            }
        }

        public void Select()
        {
            Selected = true;
            HandlePieceSelectionSound();
            
            // Update move count
            if (CurrentMoveCount > 0)
            {
                ClearValues();
                UpdateAvailableMoves();
            }
        }

        public void Deselect()
        {
            Debug.Log("Deselected " + this);

            Selected = false;
            ClearValues();
        }

        public int RollMoveDice()
        {
            if (CurrentMoveCount == 0)
            {
                CurrentMoveCount = MoveDice.RollDice();
                MovesRemaining = CurrentMoveCount;
                Debug.Log("Move Dice Rolled");
            }

            return CurrentMoveCount;
        }


        // ****************************************************
        // Private Methods
        // ****************************************************
        private Move GetAvailableMove(Structs.Coordinate currCoordinate, Move.Direction direction)
        {
            Structs.Coordinate newCoordinate = currCoordinate + Move.Offsets[(int)direction];

            if (!newCoordinate.IsValid())
            {
                return null;
            }
            
            return gameRef.GetGameSquare(newCoordinate).GetMoveToHere(direction);
        }

        private List<Move> GetAvailableMovesByDirectionArray(Move.Direction[] directionArray)
        {
            Structs.Coordinate testCoord = (PreviewMoves.Count() > 0) ? PreviewMoves.Peek().Coord : Coord;
            List<Move> moves = new List<Move>();

            for (int dirIndex = 0; dirIndex < directionArray.Length; ++dirIndex)
            {
                Move newMove = GetAvailableMove(testCoord, directionArray[dirIndex]);
                if (newMove != null)
                {
                    moves.Add(newMove);
                }
            }

            return moves;
        }
        
        private List<Move> GetAvailableMoves()
        {
            List<Move> moves = new List<Move>();

            switch (PieceType)
            {
                case TypeOfPiece.Fighter:
                    if (!HasChangedDirection || CurrentDirection == Move.Direction.NONE)
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All);
                    }
                    else
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All).Where(x => x.Dir == CurrentDirection).ToList();
                    }
                    break;
                case TypeOfPiece.Defender:
                    if (CurrentDirection == Move.Direction.NONE)
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All);
                    }
                    else
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All).Where(x => x.Dir == CurrentDirection).ToList();
                    }
                    break;
                case TypeOfPiece.Drone:
                    if (CurrentDirection == Move.Direction.NONE)
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_NoDiagonals);
                    }
                    else
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_NoDiagonals).Where(x => x.Dir == CurrentDirection).ToList();
                    }
                    break;
                case TypeOfPiece.Bomb:
                    if (!HasChangedDirection || CurrentDirection == Move.Direction.NONE)
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All);
                    }
                    else
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All).Where(x => x.Dir == CurrentDirection).ToList();
                    }
                    break;
                case TypeOfPiece.Queen:
                    if (CurrentDirection == Move.Direction.NONE)
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All);
                    }
                    else
                    {
                        moves = GetAvailableMovesByDirectionArray(Move.Directions_All).Where(x => x.Dir == CurrentDirection).ToList();
                    }
                    break;
                case TypeOfPiece.King:
                    moves = GetAvailableMovesByDirectionArray(Move.Directions_All);
                    break;
            }
            //Utils.LogMoves(moves);
            return moves;
        }

        private void HandlePieceSelectionSound()
        {
            if (PieceType == TypeOfPiece.Bomb)
            {
                //Play bomb sound
                GameRef.soundManager.PlaySound(this.gameObject, "8bit bomb beep", true);
            }
            else
            {
                if (GameRef.soundManager != null)
                {
                    GameRef.soundManager.StopSound();
                }
            }
        }

        private void HandleSelect()
        {
            //  if this piece is yours
            if (Owner == GameRef.CurrentTurn)
            {
                //  if no piece is selected
                if (GameRef.SelectedPiece == null)
                {
                    GameRef.SelectPiece(this);
                }
                //  if a piece is selected
                else
                {
                    //  if selected piece is yours
                    if (GameRef.SelectedPiece.Owner == GameRef.CurrentTurn)
                    {
                        //  if this piece is the currently selected piece
                        if (this == GameRef.SelectedPiece)
                        {
                            GameRef.DeselectPiece(GameRef.SelectedPiece);
                        }
                        //  if this piece is a different piece
                        else
                        {
                            GameRef.DeselectPiece(GameRef.SelectedPiece);
                            GameRef.SelectPiece(this);
                        }
                    }
                    //  if selected piece is opponent's
                    else
                    {
                        GameRef.DeselectPiece(GameRef.SelectedPiece);
                        // TODO: Do we show info on a selected enemy piece?
                    }
                }
            }
            //  if this piece is opponent's
            else
            {
                //  if no piece is selected
                if (GameRef.SelectedPiece == null)
                {
                    //  TODO: Do we show info on selected enemy piece?
                }
                //  if a piece is selected
                else
                {
                    //  if selected piece is yours
                    if (GameRef.SelectedPiece.Owner == GameRef.CurrentTurn)
                    {
                        GameRef.DeselectPiece(GameRef.SelectedPiece);
                        //  TODO: Do we show info on a selected enemy piece?
                    }
                    //  if selected piece is opponent's
                    else
                    {
                        //  if this piece is the currently selected piece
                        if (this == GameRef.SelectedPiece)
                        {
                            GameRef.DeselectPiece(GameRef.SelectedPiece);
                        }
                        //  if this piece is a different piece
                        else
                        {
                            GameRef.DeselectPiece(GameRef.SelectedPiece);
                            GameRef.SelectPiece(this);
                        }
                    }
                }
            }
        }

        private void HandleMove()
        {
            if (gameRef.SelectedPiece == this)
            {
                GameRef.DeselectPiece(GameRef.SelectedPiece);
                GameRef.CurrentPlayerActionMode = PlayerActionMode.kSelect;
            }
            else
            {
                GameRef.DeselectPiece(GameRef.SelectedPiece);
                GameRef.CurrentPlayerActionMode = PlayerActionMode.kSelect;
                HandleSelect();
            }
        }

        private void HandleAttack()
        {
            //  if piece is yours
            //      can you attack your own piece?
            //  if piece is opponents
            //      attack opponent piece
        }

        // ****************************************************
        // Behavior Implementation
        // ****************************************************
        public override void LeftClickDown()
        {
            Debug.Log(this + " Left Click Down");
            //ClickCount++;
            Dragging = true;
        }

        public override void LeftClickUp()
        {
            Debug.Log(this + " Left Click Up");
            Dragging = false;

            if (GameRef.SelectedPiece != null && GameRef.SelectedPiece.Moving)
            {
                Debug.Log(this + " " + GameRef.SelectedPiece.Moving);
                return;
            }
            
            switch(GameRef.CurrentPlayerActionMode)
            {
                case PlayerActionMode.kSelect:
                    HandleSelect();
                    break;
                case PlayerActionMode.kMove:
                    HandleMove();
                    break;
                case PlayerActionMode.kAttack:
                    HandleAttack();
                    break;
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
                RollMoveDice();
            }
        }
    }
}

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
        //public enum TypeOfPiece
        //{
        //    None,
        //    Fighter,
        //    Defender,
        //    King,
        //    Queen,
        //    Drone,
        //    Bomb
        //}
        
        //public TypeOfPiece PieceType = TypeOfPiece.None;

        public bool Active = true;
        public bool CanChangeDirection = false;
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
        public List<Move> CurrentAvailableMoves = null;
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
        public List<Move.Direction> AvailableDirections;

        public TimeSpan HoverTime;
        public float HoverStart = 1.5f;
                
        void Awake()
        {
            GameRef = GameObject.Find("Game").GetComponent<Game>();
            InitClickTimes();
        }

        public void Start()
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
            Dragging = false;
            DetectClicks(false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            Dragging = true;
            //DetectClicks(false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            GameRef.HidePreviewMenu();
            HoverTime = TimeSpan.FromSeconds(0);
            Dragging = false;
            ClickCount = 0;
        }

        void Update()
        {
            
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
                                InitAttack();
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

        /// <summary>
        /// Initializes the Attack.  If piece to attack is a bomb, call that piece's init method
        /// </summary>
        protected virtual void InitAttack()
        {
            if (CurrentMove.PieceAtPosition != null && CurrentMove.PieceAtPosition.GetType() == typeof(Bomb))
            {
                //Bomb Blows up!
                CurrentMove.PieceAtPosition.InitAttack();
            }
            else
            {
                gameRef.ShowAttackMenu();
            }
        }

        /// <summary>
        /// Populates the available direction array that determines how the piece can move.
        /// </summary>
        protected virtual void InitAvailableDirections()
        {
            AvailableDirections = new List<Move.Direction>();
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

        #endregion Piece Highlight Methods

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

        /// <summary>
        /// Returns the type the Piece can turn into.  Defaults to null -zl
        /// </summary>
        /// <returns>Type to turn into</returns>
        public virtual Type GetTransformType()
        {
            return null;
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

        //public void SetDice()
        //{
        //    switch (PieceType)
        //    {
        //        case TypeOfPiece.Fighter:
        //            AttackLimit = 20;
        //            DefendLimit = 6;
        //            MoveLimit = 6;
        //            AttackDice.InitDice(1, 20);
        //            DefendDice.InitDice(1, 6);
        //            MoveDice.InitDice(1, 6);
        //            break;
        //        case TypeOfPiece.Defender:
        //            AttackLimit = 6;
        //            DefendLimit = 20;
        //            MoveLimit = 4;
        //            AttackDice.InitDice(1, 6);
        //            DefendDice.InitDice(1, 20);
        //            MoveDice.InitDice(1, 4);
        //            break;
        //        case TypeOfPiece.Drone:
        //            AttackLimit = 10;
        //            DefendLimit = 10;
        //            MoveLimit = 6;
        //            AttackDice.InitDice(1, 10);
        //            DefendDice.InitDice(1, 10);
        //            MoveDice.InitDice(1, 6);
        //            break;
        //        case TypeOfPiece.Bomb:
        //            AttackLimit = 20;
        //            DefendLimit = 6;
        //            MoveLimit = 6;
        //            MoveDice.InitDice(1, 6);
        //            break;
        //        case TypeOfPiece.Queen:
        //            AttackLimit = 20;
        //            DefendLimit = 6;
        //            MoveLimit = 6;
        //            MoveDice.InitDice(3, 6);
        //            AttackDice.InitDice(3, 6);
        //            DefendDice.InitDice(3, 6);
        //            break;
        //        case TypeOfPiece.King:
        //            AttackLimit = 100;
        //            DefendLimit = 0;
        //            MoveLimit = 1;
        //            MoveDice.InitDice(1, 1);
        //            AttackDice.InitDice(1, 100, 99);
        //            DefendDice.InitDice(1, 0, 0);
        //            break;
        //    }
        //}

        //public void SetPieceType(TypeOfPiece type)
        //{
        //    PieceType = type;
        //    if (AttackDice == null || DefendDice == null || MoveDice == null)
        //    {
        //        return;
        //    }
        //    switch (PieceType)
        //    {
        //        case TypeOfPiece.Fighter:
        //            AttackLimit = 20;
        //            DefendLimit = 6;
        //            MoveLimit   = 6;
        //            AttackDice.InitDice(1, 20);
        //            DefendDice.InitDice(1, 6);
        //            MoveDice.InitDice(1, 6);
        //            break;
        //        case TypeOfPiece.Defender:
        //            AttackLimit = 6;
        //            DefendLimit = 20;
        //            MoveLimit   = 4;
        //            AttackDice.InitDice(1, 6);
        //            DefendDice.InitDice(1, 20);
        //            MoveDice.InitDice(1, 4);
        //            break;
        //        case TypeOfPiece.Drone:
        //            AttackLimit = 10;
        //            DefendLimit = 10;
        //            MoveLimit   = 6;
        //            AttackDice.InitDice(1, 10);
        //            DefendDice.InitDice(1, 10);
        //            MoveDice.InitDice(1, 6);
        //            break;
        //        case TypeOfPiece.Bomb:
        //            AttackLimit = 20;
        //            DefendLimit = 6;
        //            MoveLimit   = 6;
        //            MoveDice.InitDice(1, 6);
        //            AttackDice.InitDice(1, 100, 99);
        //            DefendDice.InitDice(1, 100, 99);
        //            break;
        //        case TypeOfPiece.Queen:
        //            AttackLimit = 20;
        //            DefendLimit = 6;
        //            MoveLimit   = 6;
        //            MoveDice.InitDice(3, 6);
        //            AttackDice.InitDice(3, 6);
        //            DefendDice.InitDice(3, 6);
        //            break;
        //        case TypeOfPiece.King:
        //            AttackLimit = 20;
        //            DefendLimit = 6;
        //            MoveLimit   = 6;
        //            MoveDice.InitDice(1, 1);
        //            AttackDice.InitDice(1, 100, 99);
        //            DefendDice.InitDice(1, 0, 0);
        //            break;
        //    }        
        //}
        
        //Needs to be moved - not sure how to handle with new Class set up.  Hard to think in the car...

        //public void TransformIntoBomb()
        //{
        //    if (PieceType == TypeOfPiece.Drone)
        //    {
        //        //SetPieceType(TypeOfPiece.Bomb);
        //        //Change Image
        //    }
        //}

        public virtual void Select()
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
        // Protected Methods
        // ****************************************************
        protected Move GetAvailableMove(Structs.Coordinate currCoordinate, Move.Direction direction)
        {
            Structs.Coordinate newCoordinate = currCoordinate + Move.Offsets[(int)direction];

            if (!newCoordinate.IsValid()) 
            {
                return null;
            }
            
            GameSquare square = GameRef.GetGameSquare(newCoordinate);
            if (square.ContainsFriendlyPiece())
            {
                return null;
            }

            return gameRef.GetGameSquare(newCoordinate).GetMoveToHere(direction);
        }

        /// <summary>
        /// Gets all move combinations based on the direction array provided. -zl
        /// </summary>
        /// <param name="directionArray">array of available directions</param>
        /// <returns>list of moves</returns>
        protected List<Move> GetAllAvailableMovesByDirectionArray(Move.Direction[] directionArray)
        {
            List<Move> moves = new List<Move>();
            //TODO: Continue-> get all moves possible from the start point to the end based on the current roll
            return moves;
        }

        protected List<Move> GetAvailableMovesByDirectionArray(Move.Direction[] directionArray)
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

        protected virtual List<Move> GetAvailableMoves()
        {
            List<Move> moves = GetAvailableMovesByDirectionArray(AvailableDirections.ToArray());

            return moves;
        }

        protected virtual List<Move> GetCurrentAvailableMoves()
        {
            List<Move> moves = new List<Move>();

            if (!HasChangedDirection || CurrentDirection == Move.Direction.NONE)
            {
                moves = GetAvailableMovesByDirectionArray(AvailableDirections.ToArray());
            }
            else
            {
                moves = GetAvailableMovesByDirectionArray(AvailableDirections.ToArray()).Where(x => x.Dir == CurrentDirection).ToList();
            }

            return moves;
        }

        protected void HandlePieceSelectionSound()
        {
            //if (PieceType == TypeOfPiece.Bomb)
            //{
            //    //Play bomb sound
            //    GameRef.soundManager.PlaySound(this.gameObject, "8bit bomb beep", true);
            //}
            //else
            //{
            //    if (GameRef.soundManager != null)
            //    {
            //        GameRef.soundManager.StopSound();
            //    }
            //}

            if (GameRef.soundManager != null)
            {
                GameRef.soundManager.StopSound();
            }
        }

        protected void HandleSelect()
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

        protected void HandleMove()
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

        protected void HandleAttack()
        {
            //  if piece is yours
            //      can you attack your own piece? -- No, you cannot.  
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


        public Piece Clone(Piece p)
        {
            p = (Piece) this.MemberwiseClone();
            return p;
        }
    }
}

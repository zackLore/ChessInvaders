using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class GameSquare : BaseSquare
    {
        public bool IsActive = true;
        public bool CanMoveTo = false;
        public bool CanRemove = true;
        public bool CanAttack = false;

        public TimeSpan? StartTime = null;

        void Start()
        {
            GameRef = GameObject.Find("Game").GetComponent<Game>();
            InitClickTimes();
        }

        // ****************************************************
        // Public Methods
        // ****************************************************
        public List<T> GetChildrenComponentsOfType<T>()
        {
            List<T> components = new List<T>();

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                T temp = gameObject.transform.GetChild(i).GetComponent<T>();
                if (temp != null)
                {
                    components.Add(temp);
                }
            }

            return components;
        }

        public bool ContainsEnemyPiece()
        {
            List<Piece> pieces = GetChildrenComponentsOfType<Piece>();

            foreach (Piece piece in pieces)
            {
                if (piece.Owner != GameRef.CurrentTurn)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsFriendlyPiece()
        {
            List<Piece> pieces = GetChildrenComponentsOfType<Piece>();

            foreach (Piece piece in pieces)
            {
                if (piece.Owner == GameRef.CurrentTurn)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsPreviewPiece()
        {
            List<PreviewPiece> previewPieces = GetChildrenComponentsOfType<PreviewPiece>();
            return (previewPieces.Count() > 0);
        }

        public Move GetMoveToHere(Move.Direction direction)
        {
            Piece piece = null;
            List<Piece> piecesInThisSquare = GetChildrenComponentsOfType<Piece>();

            if (piecesInThisSquare.Count() > 0)
            {
                piece = piecesInThisSquare[0];
            }

            return new Move(    gameObject.transform.position, 
                                Coord, 
                                direction,
                                piece);
        }
        

        // ****************************************************
        // Private Methods
        // ****************************************************

        private void HandleSelect()
        {
            if (GameRef.SelectedPiece != null)
            {
                GameRef.DeselectPiece(GameRef.SelectedPiece);
            }
        }

        private void HandleMove()
        {
            if (ContainsPreviewPiece())
            {
                // back out the preview moves so this is the last one
            }
            else if (ContainsEnemyPiece())
            {
                // TODO: figure out how to handle move onto enemy piece
            }
            else if (ContainsFriendlyPiece())
            {
                // Do nothing. The piece will handle it itself
            }
            else if (IsValidPreviewMove())
            {
                // is this square a valid preview square for the currently selected piece?
                GameRef.SetMovePiece(this);
            }
            else
            {
                // deselect the currently selected piece
            }
        }

        private void HandleAttack()
        {
            //  End attack mode?
        }

        private bool IsValidPreviewMove()
        {
            return  (GameRef != null) &&
                    (GameRef.SelectedPiece != null) &&
                    GameRef.SelectedPiece.GetAvailableMoveAtCoordinate(Coord) != null;
        }

        // ****************************************************
        // Behavior Implementation
        // ****************************************************

        public override void OnPointerEnter(PointerEventData eventData)
        {
            CanMoveTo = true;//allow move to this square
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            GameRef.CurrentSquare = this.gameObject;
            DetectClicks();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            //DetectClicks(false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            CanRemove = true;
        }

        public override void LeftClickDown()
        {
            //Debug.Log(this + " Left Click Down");
        }

        public override void LeftClickUp()
        {
            //Debug.Log(this + " Left Click Up : " + Camera.main.ScreenToWorldPoint(Input.mousePosition));
            switch (GameRef.CurrentPlayerActionMode)
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
        }

        public override void DoubleClick()
        {
            //Debug.Log(this + " Double Click");
        }
    }
}

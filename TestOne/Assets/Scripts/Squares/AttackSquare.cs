using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    class AttackSquare : BaseSquare
    {
        void Awake()
        {
            GameRef = GameObject.Find("Game").GetComponent<Game>();
            InitClickTimes();
        }
        
        public override void OnPointerEnter(PointerEventData eventData)
        {
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            DetectClicks();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            //DetectClicks(false);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
        }

        // ****************************************************
        // Behavior Implementation
        // ****************************************************
        public override void LeftClickDown()
        {
            ////Debug.Log(this + " Left Click Down");
        }

        public override void LeftClickUp()
        {
            ////Debug.Log(this + " Left Click Up");
            RemoveMove();
            GameRef.ActionPieceWasPlaced = false;
        }

        public override void LongPressUp()
        {
            ////Debug.Log(this + " Long Press Up");
            if (!GameRef.ActionPieceWasPlaced)
            {
                GameRef.ActionPieceWasPlaced = true;
                return;
            }
            //Start Move process
            if (ClickCount >= 1 && !GameRef.SelectedPiece.Moving)
            {
                GameRef.Highlight.gameObject.SetActive(false);
                GameRef.SelectedPiece.PreviewMoves = GameRef.SwapMoves(GameRef.SelectedPiece.PreviewMoves);
                GameRef.SelectedPiece.NextMove = GameRef.SelectedPiece.PreviewMoves.Pop();
                GameRef.SelectedPiece.Moving = true;
                GameRef.SelectedPiece.FinishedMoving = false;
                GameRef.MoveStartTime = Time.time;
                GameRef.ActionPieceWasPlaced = false;
            }
        }

        public override void DoubleClick()
        {
            ////Debug.Log(this + " Double Click");
            //Start Move process
            if (!GameRef.SelectedPiece.Moving)
            {
                GameRef.Highlight.gameObject.SetActive(false);
                GameRef.SelectedPiece.PreviewMoves = GameRef.SwapMoves(GameRef.SelectedPiece.PreviewMoves);
                GameRef.SelectedPiece.NextMove = GameRef.SelectedPiece.PreviewMoves.Pop();
                GameRef.SelectedPiece.Moving = true;
                GameRef.SelectedPiece.FinishedMoving = false;
                GameRef.MoveStartTime = Time.time;
                GameRef.ActionPieceWasPlaced = false;
            }
        }
    }
}
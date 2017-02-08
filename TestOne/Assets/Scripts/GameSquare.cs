using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    class GameSquare : BaseBehavior
    {
        public bool IsActive = true;
        public bool CanMoveTo = false;
        public bool CanRemove = true;
        public bool CanAttack = false;

        public TimeSpan? StartTime = null;
        //private bool CanMove = false;

        void Start()
        {
            GameRef = GameObject.Find("Game").GetComponent<Game>();
            InitClickTimes();
        }
        
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
            //if (GameRef.SelectedPiece.Moving == true)
            {
                GameRef.SetMovePiece();
            }
        }

        private void HandleAttack()
        {
            //  End attack mode?
        }

        // ****************************************************
        // Behavior Implementation
        // ****************************************************
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

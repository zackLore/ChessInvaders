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

        public bool ContainsEnemyPiece(Game gameRef)
        {
            if (gameObject.transform.childCount <= 0)
            {
                return false;
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                //Debug.Log(square.transform.GetChild(i));
                try
                {
                    Piece temp = gameObject.transform.GetChild(i).GetComponent<Piece>();
                    if (temp != null)
                    {
                        if (temp.Owner != gameRef.CurrentTurn)
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

        public bool ContainsPreviewPiece()
        {
            if (gameObject.transform.childCount <= 0)
            {
                return false;
            }

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                //Debug.Log(square.transform.GetChild(i));
                try
                {
                    PreviewPiece temp = gameObject.transform.GetChild(i).GetComponent<PreviewPiece>();
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
            GameRef.SetMovePiece();
        }

        private void HandleAttack()
        {
            //  End attack mode?
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

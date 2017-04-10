using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public class MoveButton : PlayerUIButton
    {
        void Awake()
        {
            Initialize();
        }

        void Update()
        {

        }

        public override void Initialize()
        {
            currentPiece = null;
            button.interactable = false;
            title.color = Consts.moveButtonColor_active;
            label.text = "";
            label.color = Consts.moveButtonColor_inactive;
        }

        public override void SelectModeUpdate(Piece newPiece)
        {
            currentPiece = newPiece;
            button.interactable = true;

            if (currentPiece.AvailableMoves.Count == 0 )   // Have not rolled yet
            {
                label.text = currentPiece.MoveLimit.ToString();
                label.color = Consts.moveButtonColor_inactive;
            }
            else
            {
                title.color = Consts.moveButtonColor_active;
                label.color = Consts.moveButtonColor_active;
                label.text = newPiece.CurrentMoveCount.ToString();
            }
        }

        // ****************************************************
        // Behavior Implementation
        // ****************************************************

        public override void OnPointerDown(PointerEventData eventData)
        {

        }

        public override void OnPointerEnter(PointerEventData eventData)
        {

        }

        public override void OnPointerExit(PointerEventData eventData)
        {

        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            DetectClicks();
        }

        public override void LeftClickDown()
        {

        }

        public override void LeftClickUp()
        {
            if (currentPiece.AvailableMoves.Count == 0)   // Have not rolled yet
            {
                button.interactable = false;
                title.color = Consts.moveButtonColor_active_disabled;
                label.color = Consts.moveButtonColor_inactive_disabled;
                playerUI.ActivateRollButton(currentPiece);
            }
            else
            {
                GameRef.CurrentPlayerActionMode = PlayerActionMode.kMove;
                playerUI.UpdateUI(null);
                currentPiece.UpdateAvailableMoves();
            }
        }

        public override void LongPressUp()
        {

        }

        public override void DoubleClick()
        {

        }
    }
}
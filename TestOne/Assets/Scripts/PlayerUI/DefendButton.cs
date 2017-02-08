using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public class DefendButton : PlayerUIButton
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
            title.color = Consts.defendButtonColor_active;
            label.text = "";
            label.color = Consts.defendButtonColor_inactive_disabled;
        }

        public override void SelectModeUpdate(Piece newPiece)
        {
            currentPiece = newPiece;
            label.text = currentPiece.DefendLimit.ToString();
            title.color = Consts.defendButtonColor_active_disabled;
            label.color = Consts.defendButtonColor_inactive_disabled;
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
            // Do something?
        }

        public override void LongPressUp()
        {

        }

        public override void DoubleClick()
        {

        }
    }
}

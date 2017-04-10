using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public class RollButton : PlayerUIButton
    {
        void Awake()
        {

        }

        void Update()
        {

        }
        
        public override void Initialize()
        {
            gameObject.SetActive(false);
            currentPiece = null;
            button.interactable = false;
            label.text = "";
            label.color = Consts.moveButtonColor_inactive;
        }
        
        public override void SelectModeUpdate(Piece newPiece)
        {
            Initialize();
        }

        public void SelectModeActivate(Piece newPiece)
        {
            currentPiece = newPiece;
            gameObject.SetActive(true);
            button.interactable = true;
        }

        public IEnumerator DoMoveRoll()
        {
            if (currentPiece != null)
            {
                Die die = currentPiece.MoveDice.DiceCollection[0];

                for (int i = 1; i < currentPiece.MoveLimit * 3; ++i)
                {
                    int randomNumber = UnityEngine.Random.Range(die.UpperLimit, die.LowerLimit);
                    label.text = randomNumber.ToString();
                    yield return new WaitForSeconds(0.05f);
                }

                int roll = currentPiece.RollMoveDice();
                label.text = roll.ToString();

                for (int blinkCount = 1; blinkCount <= 5; ++blinkCount)
                {
                    if ((blinkCount % 2) == 1)
                    {
                        label.color = Consts.moveButtonColor_active;
                    }
                    else
                    {
                        label.color = Color.black;
                    }
                    yield return new WaitForSeconds(0.15f);
                }
                yield return new WaitForSeconds(0.25f);

                GameRef.CurrentPlayerActionMode = PlayerActionMode.kMove;
                gameObject.SetActive(false);
                playerUI.UpdateUI(null);
                currentPiece.UpdateAvailableMoves();
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
            StartCoroutine(DoMoveRoll());
        }

        public override void LongPressUp()
        {

        }

        public override void DoubleClick()
        {

        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    [Serializable]
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Game _gameRef;
        public Game GameRef
        {
            get { return _gameRef; }
            set { _gameRef = value; }
        }
        
        [SerializeField]
        private Piece _currentPiece;
        public Piece CurrentPiece
        {
            get { return _currentPiece; }
            set { _currentPiece = value; }
        }

        [SerializeField]
        private AttackButton _attackButton;
        public AttackButton attackButton
        {
            get { return _attackButton; }
            set { _attackButton = value; }
        }
        [SerializeField]
        private DefendButton _defendButton;
        public DefendButton defendButton
        {
            get { return _defendButton; }
            set { _defendButton = value; }
        }
        [SerializeField]
        private MoveButton _moveButton;
        public MoveButton moveButton
        {
            get { return _moveButton; }
            set { _moveButton = value; }
        }
        [SerializeField]
        private RollButton _rollButton;
        public RollButton rollButton
        {
            get { return _rollButton; }
            set { _rollButton = value; }
        }

        [SerializeField]
        private Dictionary<string, Transform> _arrows;
        public Dictionary<string, Transform> Arrows
        {
            get { return _arrows; }
            set { _arrows = value; }
        }
                
        // Use this for initialization
        void Awake()
        {
            if (Arrows == null)
            {
                Arrows = new Dictionary<string, Transform>();
                SetArrows();
            }
        }

        public void UpdateUI(Piece newPiece)
        {
            if (newPiece == null)
            {
                gameObject.SetActive(false);
            }
            else
            { 
                CurrentPiece = newPiece;
                CurrentPiece.ClearHighlights();

                gameObject.transform.position = CurrentPiece.transform.position;
                gameObject.SetActive(true);
                                
                UpdateButtons();
                HideArrows();
            }
        }
        
        public void ShowArrow(Transform t)
        {
            t.gameObject.SetActive(true);
        }

        public void HideArrow(Transform t)
        {
            t.gameObject.SetActive(false);
        }

        public void HideArrows()
        {
            foreach (var arrow in Arrows)
            {
                arrow.Value.gameObject.SetActive(false);
            }
        }

        public void SetArrows()
        {
            var arrowImages = transform.Find("MoveArrows");
            if (arrowImages != null)
            {
                Arrows.Add("up", arrowImages.transform.Find("UpArrow"));
                Arrows.Add("down", arrowImages.transform.Find("DownArrow"));
                Arrows.Add("left", arrowImages.transform.Find("LeftArrow"));
                Arrows.Add("right", arrowImages.transform.Find("RightArrow"));
                Arrows.Add("upLeft", arrowImages.transform.Find("UpLeftArrow"));
                Arrows.Add("upRight", arrowImages.transform.Find("UpRightArrow"));
                Arrows.Add("downLeft", arrowImages.transform.Find("DownLeftArrow"));
                Arrows.Add("downRight", arrowImages.transform.Find("DownRightArrow"));
            }
        }
        
        public void UpdateButtons()
        {
            switch (GameRef.CurrentPlayerActionMode)
            {
                case PlayerActionMode.kMove:
                    {
                        //AttackButton.gameObject.SetActive(true);
                        //DefendButton.gameObject.SetActive(true);
                        //rollButton.gameObject.SetActive(false);
                        break;
                    }
                case PlayerActionMode.kSelect:
                    {
                        attackButton.SelectModeUpdate(CurrentPiece);
                        defendButton.SelectModeUpdate(CurrentPiece);
                        moveButton.SelectModeUpdate(CurrentPiece);
                        rollButton.SelectModeUpdate(CurrentPiece);
                        break;
                    }
                default:
                    {
                        //AttackButton.gameObject.SetActive(false);
                        //DefendButton.gameObject.SetActive(false);
                        //rollButton.gameObject.SetActive(false);
                        break;
                    }
            }
        }

        public void ActivateRollButton(Piece currentPiece)
        {
            rollButton.SelectModeActivate(currentPiece);
        }
    }
}

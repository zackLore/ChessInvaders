using UnityEngine;
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
        private GameObject _pieceObject;
        public GameObject PieceObject
        {
            get
            {
                return _pieceObject;
            }
            set
            {
                _pieceObject = value;
                if (value != null)
                {
                    InitUI();
                }
            }
        }

        [SerializeField]
        private Piece _currentPiece;
        public Piece CurrentPiece
        {
            get { return _currentPiece; }
            set { _currentPiece = value; }
        }

        [SerializeField]
        private Button _attackButton;
        public Button AttackButton
        {
            get { return _attackButton; }
            set { _attackButton = value; }
        }
        [SerializeField]
        private Button _defendButton;
        public Button DefendButton
        {
            get { return _defendButton; }
            set { _defendButton = value; }
        }
        [SerializeField]
        private Button _moveButton;
        public Button MoveButton
        {
            get { return _moveButton; }
            set { _moveButton = value; }
        }
        [SerializeField]
        private Button _rollButton;
        public Button RollButton
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
        void Start()
        {
            //Set up UI References
            InitUI();
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

        public void HighlightButton(Button button)
        {

        }

        public void InitUI()
        {
            if (PieceObject != null)
            {
                //Set Piece Script
                var p = PieceObject.GetComponent<Piece>();
                if (p != null)
                {
                    CurrentPiece = p;
                    GameRef = p.GameRef;
                }

                //Set Button References
                AttackButton = transform.Find("AttackButton").GetComponent<Button>();
                DefendButton = transform.Find("DefendButton").GetComponent<Button>();
                MoveButton = transform.Find("MoveButton").GetComponent<Button>();
                RollButton = transform.Find("RollButton").GetComponent<Button>();

                //Set Button Labels
                SetButtonText(AttackButton, CurrentPiece.AttackLimit.ToString());
                SetButtonText(DefendButton, CurrentPiece.DefendLimit.ToString());
                SetButtonText(MoveButton, CurrentPiece.CurrentMoveCount.ToString());
                SetButtonText(RollButton, CurrentPiece.MovesRemaining.ToString());

                //Set Arrow References
                Arrows = new Dictionary<string, Transform>();
                SetArrows();

                //Hide UI Elements
                UpdateUI();
                HideArrows();
            }
        }

        public void ShowArrow(Transform t)
        {
            t.gameObject.SetActive(true);
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

        public void SetButtonText(Button button, string text)
        {
            Text lbl = button.GetComponent<Text>();
            if (lbl != null)
            {
                lbl.text = text;
            }
        }

        public void UpdateUI()
        {
            if (GameRef.MoveMode)
            {
                AttackButton.gameObject.SetActive(true);
                DefendButton.gameObject.SetActive(true);
                MoveButton.gameObject.SetActive(true);
                RollButton.gameObject.SetActive(false);
            }
            else if (GameRef.SelectMode)
            {
                AttackButton.gameObject.SetActive(true);
                DefendButton.gameObject.SetActive(true);
                MoveButton.gameObject.SetActive(false);
                RollButton.gameObject.SetActive(true);
            }
        }
    }
}

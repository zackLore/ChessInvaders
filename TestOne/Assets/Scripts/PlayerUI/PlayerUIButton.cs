using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace Assets.Scripts
{
    public abstract class PlayerUIButton : BaseBehavior
    {
        public PlayerUI playerUI;
        public Button button;
        public Text title;
        public Text label;

        protected Piece currentPiece;

        public abstract void SelectModeUpdate(Piece newPiece);
        public abstract void Initialize();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

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

        void OnMouseEnter()
        {
            CanMoveTo = true;//allow move to this square
        }

        void OnMouseOver()
        {
            GameRef.CurrentSquare = this.gameObject;
            DetectClicks();
        }

        void OnMouseExit()
        {
            CanRemove = true;
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
            if (GameRef.SelectedPiece.Moving) { return; }//ignore clicks while moving
            GameRef.SetMovePiece();
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

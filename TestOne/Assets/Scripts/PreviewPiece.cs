using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class PreviewPiece : BaseBehavior//TODO: finish implementation of Behaviors
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

        public override void LeftClickDown()
        {
            //Debug.Log(this + "Left Click Down");
        }
        
        public override void LeftClickUp()
        {
            //Debug.Log(this + "Left Click Up");
            RemoveMove();
        }

        public override void LongPressUp()
        {
            //Debug.Log(this + "Long Press Up");
        }

        public override void DoubleClick()
        {
            //Debug.Log(this + " Double Click");
            RemoveMove();
        }
    }
}

﻿using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public abstract class BaseBehavior : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public Game GameRef = null;
        public Move CurrentMove = null;
        public int ClickCount = 0;
        public float LongPressTime = 750f;
        //public float ClickTime = 100f;
        public float DoubleClickTime = .5f;
        public float LastClickTime = 0f;

        public TimeSpan ClickStartTime = TimeSpan.FromSeconds(0);
        public TimeSpan ClickEndTime = TimeSpan.FromSeconds(0);

        public Coroutine click = null;

        //public bool Dragging = false;//Current Game Object being dragged

        public void DetectClicks(bool detectDoubleClick = true)
        {
           //if (EventSystem.current.IsPointerOverGameObject() &&
            if (GameRef != null && 
                GameRef.SelectedPiece != null && 
                GameRef.SelectedPiece.Moving) { return; }//Ignore clicks when piece is moving
            
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject())//touch started Left Click Down
            {
                StartPressTime();
                ClickCount++;
                LastClickTime = Time.time;
                LeftClickDown();                
                GameRef.Dragging = true;
            }
            
            if (Input.GetMouseButtonUp(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject())//Left Mouse Click Up
            {
                //EndPressTime();                
                //double duration = (ClickEndTime - ClickStartTime).TotalMilliseconds;

                //if (duration >= LongPressTime)
                //{
                //    LongPressUp();
                //    //ClickCount = 0;
                //}
                //else
                //{
                //    if (Time.time - LastClickTime <= DoubleClickTime && ClickCount == 2 && detectDoubleClick)
                //    {
                //        if (click != null)
                //        {
                //            StopCoroutine(click);
                //            click = null;
                //        }
                //        EndPressTime();
                //        DoubleClick();
                //        ClickCount = 0;                   
                //    }
                //    else
                //    {
                //        if (click == null)
                //        {
                //            click = StartCoroutine(TriggerSingleClick());
                //            EndPressTime();
                //        }
                //    }
                //}
                ////LastClickTime = Time.time;

                //GameRef.Dragging = false;
                //ResetPressTime();
                ////ClickCount = 0;

                GameRef.Dragging = false;
                LeftClickUp();
            }            

            
        }

        private void ResetPressTime()
        {
            ClickStartTime = TimeSpan.FromSeconds(0);
            ClickEndTime = TimeSpan.FromSeconds(0);
        }

        public void InitClickTimes()
        {
            LongPressTime = 750f;
            DoubleClickTime = .2f;
            LastClickTime = Time.time;
        }

        private void StartPressTime()
        {
            ClickStartTime = DateTime.Now.TimeOfDay;
        }

        private void EndPressTime()
        {
            ClickEndTime = DateTime.Now.TimeOfDay;
        }

        public void RemoveMove()
        {
            if (GameRef.SelectedPiece.PreviewMoveExists(CurrentMove))
            {
                //Remove moves up to CurrentMove
                while (GameRef.SelectedPiece.PreviewMoves.Count > 0)
                {
                    Move temp = GameRef.SelectedPiece.PreviewMoves.Pop();
                    GameRef.SelectedPiece.MovesRemaining++;
                    //Debug.Log("selectedPiece: " + GameRef.SelectedPiece.CurrentDirection + " | current: " + CurrentMove.Dir);
                    if (GameRef.SelectedPiece.MovesRemaining == GameRef.SelectedPiece.CurrentMoveCount ||
                        GameRef.SelectedPiece.CurrentDirection != CurrentMove.Dir)
                    {
                        GameRef.SelectedPiece.HasChangedDirection = false;
                    }

                    if (temp.Coord.row == CurrentMove.Coord.row &&
                        temp.Coord.col == CurrentMove.Coord.col)//Destry Self
                    {                       
                        GameRef.Squares[temp.Coord.row][temp.Coord.col].GetComponent<GameSquare>().CanRemove = false;
                        GameRef.SelectedPiece.PreviewCoord = CurrentMove.FromCoord;
                        GameRef.SelectedPiece.CurrentDirection = CurrentMove.Dir;
                        GameRef.SelectedPiece.GetAvailableMoves();
                        GameRef.UpdateMoveLabel();
                        GameObject.Destroy(this.gameObject);
                        return;
                    }
                    if (temp.Owner != null)//Destroy Preview Object
                    {
                        GameObject.Destroy(temp.Owner.gameObject);
                    }                    
                }
            }
            else
            {
                Debug.Log("CurrentMove was not found: " + CurrentMove);
            }
        }

        public IEnumerator TriggerSingleClick()
        {
            yield return new WaitForSeconds(DoubleClickTime);
            ClickCount = 0;
            LeftClickUp();
            click = null;
        }

        //Abstract Behaviors
        public abstract void OnPointerDown(PointerEventData eventData);
        public abstract void OnPointerEnter(PointerEventData eventData);
        public abstract void OnPointerExit(PointerEventData eventData);
        public abstract void OnPointerUp(PointerEventData eventData);
        public abstract void DoubleClick();
        public abstract void LeftClickDown();
        public abstract void LeftClickUp();
        public abstract void LongPressUp();
    }
}

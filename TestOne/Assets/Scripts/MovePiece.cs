using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

using Assets;
using Assets.Scripts;
using System.Collections.Generic;
using System;

public class MovePiece : BaseBehavior//TODO: Remove un-needed code, finish implementation of Behaviors
{
    GameSquare square = null;

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

    // ****************************************************
    // Behavior Implementation
    // ****************************************************
    public override void LeftClickDown()
    {
        //Debug.Log(this + " Left Click Down");
    }

    public override void LeftClickUp()
    {
        //Debug.Log(this + " Left Click Up| LastClickTime: " + (Time.time - LastClickTime));
        RemoveMove();
        GameRef.ActionPieceWasPlaced = false;
    }

    public override void LongPressUp()
    {
        //Debug.Log(this + " Long Press Up");
        if (!GameRef.ActionPieceWasPlaced)
        {
            GameRef.ActionPieceWasPlaced = true;
            return;
        }
        //Start Move process
        if (ClickCount >= 1 && !GameRef.SelectedPiece.Moving)
        {
            GameRef.Highlight.gameObject.SetActive(false);
            GameRef.SelectedPiece.PreviewMoves = GameRef.SwapMoves(GameRef.SelectedPiece.PreviewMoves);
            GameRef.SelectedPiece.NextMove = GameRef.SelectedPiece.PreviewMoves.Pop();
            GameRef.SelectedPiece.Moving = true;
            GameRef.SelectedPiece.FinishedMoving = false;
            GameRef.MoveStartTime = Time.time;
            ClickCount = 0;
            GameRef.ActionPieceWasPlaced = false;
        }
    }

    public override void DoubleClick()
    {
        //Debug.Log(this + " Double Click | LastClickTime: " + (Time.time - LastClickTime));
        //Start Move process
        if (!GameRef.SelectedPiece.Moving)
        {
            GameRef.Highlight.gameObject.SetActive(false);
            GameRef.SelectedPiece.PreviewMoves = GameRef.SwapMoves(GameRef.SelectedPiece.PreviewMoves);
            GameRef.SelectedPiece.NextMove = GameRef.SelectedPiece.PreviewMoves.Pop();
            GameRef.SelectedPiece.Moving = true;
            GameRef.SelectedPiece.FinishedMoving = false;
            GameRef.MoveStartTime = Time.time;
            ClickCount = 0;
            GameRef.ActionPieceWasPlaced = false;
        }
    }
}

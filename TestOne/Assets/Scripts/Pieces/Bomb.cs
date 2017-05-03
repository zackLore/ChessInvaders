using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;

public class Bomb : Piece {

    //***** Colors for Bomb
    private Color[] bombColors = { Color.cyan, Color.gray, Color.magenta, Color.red, Color.yellow, Color.white };
    private int colorIndex = 0;
    private int colorSwitchCounter = 0;
    private int colorSwitchMax = 15;

    public new void Start()
    {
        base.Start();
        InitializePiece();
    }

    public void InitializePiece()
    {
        AttackLimit = 20;
        DefendLimit = 6;
        MoveLimit = 6;
        MoveDice.InitDice(1, 6);
    }

    public void Update()
    {
        if (colorSwitchCounter < colorSwitchMax)
        {
            colorSwitchCounter++;
        }
        else
        {
            colorSwitchCounter = 0;

            colorIndex = colorIndex < bombColors.Length - 1 ? colorIndex + 1 : 0;
            GetComponent<SpriteRenderer>().color = bombColors[colorIndex];
        }
    }
}

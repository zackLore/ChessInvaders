using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;

public class Defender : Piece {

    public new void Start()
    {
        base.Start();
        InitializePiece();
    }

    public void InitializePiece()
    {
        AttackLimit = 6;
        DefendLimit = 20;
        MoveLimit = 4;
        AttackDice.InitDice(1, 6);
        DefendDice.InitDice(1, 20);
        MoveDice.InitDice(1, 4);
    }
}

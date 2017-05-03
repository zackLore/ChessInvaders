using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;

public class Drone : Piece {

    public new void Start()
    {
        base.Start();
        InitializePiece();
    }

    public void InitializePiece()
    {
        AttackLimit = 10;
        DefendLimit = 10;
        MoveLimit = 6;
        AttackDice.InitDice(1, 10);
        DefendDice.InitDice(1, 10);
        MoveDice.InitDice(1, 6);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;

public class King : Piece {

    public void Awake()
    {
        InitializePiece();
    }

    public void InitializePiece()
    {
        AttackLimit = 100;
        DefendLimit = 0;
        MoveLimit = 1;
        MoveDice.InitDice(1, 1);
        AttackDice.InitDice(1, 100, 99);
        DefendDice.InitDice(1, 0, 0);
    }
}

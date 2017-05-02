using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;

public class Queen : Piece {

    public void Awake()
    {
        InitializePiece();
    }

    public void InitializePiece()
    {
        AttackLimit = 20;
        DefendLimit = 6;
        MoveLimit = 6;
        MoveDice.InitDice(3, 6);
        AttackDice.InitDice(3, 6);
        DefendDice.InitDice(3, 6);
    }
}

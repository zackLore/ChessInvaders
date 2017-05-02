using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;

public class Fighter : Piece {

    public void Awake()
    {
        InitializePiece();
    }

    public void InitializePiece()
    {
        AttackLimit = 20;
        DefendLimit = 6;
        MoveLimit = 6;
        AttackDice.InitDice(1, 20);
        DefendDice.InitDice(1, 6);
        MoveDice.InitDice(1, 6);
    }
}

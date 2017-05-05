﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;
using System.Linq;

public class Queen : Piece {

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
        MoveDice.InitDice(3, 6);
        AttackDice.InitDice(3, 6);
        DefendDice.InitDice(3, 6);
    }

    protected override List<Move> GetAvailableMoves()
    {
        List<Move> moves = new List<Move>();

        if (CurrentDirection == Move.Direction.NONE)
        {
            moves = GetAvailableMovesByDirectionArray(Move.Directions_All);
        }
        else
        {
            moves = GetAvailableMovesByDirectionArray(Move.Directions_All).Where(x => x.Dir == CurrentDirection).ToList();
        }

        return moves;
    }
}

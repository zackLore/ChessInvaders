using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;
using System.Linq;
using System;

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

    protected override List<Move> GetAvailableMoves()
    {
        List<Move> moves = new List<Move>();

        if (CurrentDirection == Move.Direction.NONE)
        {
            moves = GetAvailableMovesByDirectionArray(Move.Directions_NoDiagonals);
        }
        else
        {
            moves = GetAvailableMovesByDirectionArray(Move.Directions_NoDiagonals).Where(x => x.Dir == CurrentDirection).ToList();
        }

        return moves;
    }

    public override Type GetTransformType()
    {
        return typeof(Bomb);
    }
}

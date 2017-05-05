using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Assets.Scripts;
using System.Linq;

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

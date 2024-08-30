using Godot;
using System.Collections.Generic;
using test.core.Controllers;
using test.core.Pieces.Resources;

namespace test.core.Pieces
{
    public class Horse : Piece
    {
        public Horse(string position, int team, GameController game) : base(position, team, game)
        {
            mesh = "res://TSCN/horse.tscn";
            posVector = new Vector3(posVector.X, -0.25f, posVector.Z);

        }


        public override List<AvailableMove> CheckValidMovesVirt(Board board)
        {
            List<AvailableMove> ans = new List<AvailableMove>();

            if (board.kingIsInDoubleCheck || validDirections.Count > 0) { return ans; }

            ans.AddRange(HorseMoves(false, false, board));


            if (board.kingIsInCheck) { RemoveMoves(ans, board); }

            return ans;

        }


        public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
        {
            List<AvailableMove> ans = new List<AvailableMove>();
            ans.AddRange(HorseMoves(true, false, board));
            return ans;
        }


    }
}

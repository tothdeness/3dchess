﻿using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test.core.Controllers;
using test.core.Pieces.Resources;
using test.core.Moves;

namespace test.core.Pieces
{
    public class Bishop : Piece
    {


        public Bishop(string position, int team, GameController gameController) : base(position, team, gameController)
        {
            mesh = "res://TSCN/bishop.tscn";
            posVector = new Vector3(posVector.X, -0.25f, posVector.Z);
            SetDirections();
        }

        private void SetDirections()
        {
            directions = new HashSet<Vector3>(new Vector3Comparer())
            {
                { new Vector3(1, 0, 1) },
                { new Vector3(-1, 0, -1)},
                { new Vector3(1, 0, -1)},
                { new Vector3(-1, 0, 1)}
            };

        }



        public override List<AvailableMove> CheckValidMovesVirt(Board board)
        {
            List<AvailableMove> ans = new List<AvailableMove>();

            if (board.kingIsInDoubleCheck) { return ans; }

            if (validDirections.Count > 0)
            {
                if (!directions.Contains(validDirections[0])) { return ans; }

                ans.AddRange(SlidingMoves(false, false, board, validDirections));
            }
            else
            {
                ans.AddRange(DiagnolMoves(false, false, board));
            }

            if (board.kingIsInCheck) { RemoveMoves(ans, board); }

            return ans;

        }


        public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
        {
            List<AvailableMove> ans = new List<AvailableMove>();
            ans.AddRange(DiagnolMoves(false, true, board));


            return ans;
        }



    }
}

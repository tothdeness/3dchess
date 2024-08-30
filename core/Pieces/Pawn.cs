using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using test.core.Controllers;
using test.core.Moves;
using test.core.Pieces.Resources;
using test.core.Pieces;

namespace test.core.Pieces
{

    public class Pawn : Piece
    {


        public Pawn(string position, int team, GameController game) : base(position, team, game)
        {
            mesh = "res://TSCN/pawn.tscn";
            posVector = new Vector3(posVector.X, -0.25f, posVector.Z);
            SetDirections();
        }

        private void SetDirections()
        {
            if (team == 1)
            {
                directions = new HashSet<Vector3>(new Vector3Comparer())
                {
                    { new Vector3(1, 0, 1) },
                    { new Vector3(1, 0, -1) },
                    { new Vector3(1, 0, 0) },
                };
            }
            else
            {
                directions = new HashSet<Vector3>(new Vector3Comparer())
                {
                    { new Vector3(-1, 0, -1) },
                    { new Vector3(-1, 0, 1) },
                    { new Vector3(-1, 0, 0) },
                };
            }
        }



        public void PromotePawn(Board board, AvailableMove move, bool visual)
        {
            if (posVector.X == 8 || posVector.X == 1)
            {
                var addQueen = new Queen(TableController.ConvertReverse(posVector), team, gameController);

                board.table[posVector] = addQueen;

                if (visual)
                {
                    Delete();
                    addQueen.AddVisuals();
                }

                move.promoted = true;
            }

        }


        public override List<AvailableMove> CheckValidMovesVirt(Board board)
        {
            List<AvailableMove> ans = new List<AvailableMove>();

            if (board.kingIsInDoubleCheck) { return ans; }

            bool locked = validDirections.Count > 0;

            Vector3 direc = new Vector3(0, 0, 0);

            if (locked) { direc = validDirections[0]; }

            if (locked && !directions.Contains(direc)) { return ans; }

            ans.AddRange(pawnMoves(false, false, board));

            if (locked)
            {
                Vector3Comparer v = new Vector3Comparer();

                if (firstMove && (v.Equals(direc, new Vector3(1, 0, 0)) || v.Equals(direc, new Vector3(-1, 0, 0))))
                {
                    ans.RemoveAll(item => item.move.Z != posVector.Z);

                }
                else
                {
                    ans.RemoveAll(item => item.move != validDirections[0] + posVector);
                }

            }

            if (board.kingIsInCheck) { RemoveMoves(ans, board); }

            return ans;

        }

        public override List<AvailableMove> CheckValidCoveredMovesOnVirtualBoard(Board board)
        {
            List<AvailableMove> ans = new List<AvailableMove>();
            ans.AddRange(pawnMoves(true, false, board));
            return ans;
        }



    }
}

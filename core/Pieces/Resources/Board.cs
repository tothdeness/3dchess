using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using test.core.Moves;
using test.core.Pieces;


namespace test.core.Pieces.Resources
{
    public class Board
    {
        public Dictionary<Vector3, Piece> table;

        public King kingWhite;

        public King kingBlack;

        public int current;

        public bool kingIsInCheck = false;

        public bool kingIsInDoubleCheck = false;

        public HashSet<Vector3> attackedSquares = new HashSet<Vector3>(new Vector3Comparer());

        public HashSet<Vector3> block = new HashSet<Vector3>(new Vector3Comparer());

        public Board(Dictionary<Vector3, Piece> table)
        {
            this.table = table;
        }


        public struct Target
        {

            public int num;
            public Piece p;

            public Target(int num, Piece p)
            {
                this.num = num;
                this.p = p;
            }

        }

        public Target Find(Vector3 vector, Piece curr)
        {

            bool ans = table.TryGetValue(vector, out Piece value);

            if (!ans) { return new Target(0, null); }

            if (value.team == curr.team)
            {
                return new Target(1, value);
            }
            else
            {
                return new Target(2, value);
            }


        }


        public void Updatekey(AvailableMove move)
        {
            table.Remove(move.oldPositon);
            table.Add(move.moving.posVector, move.moving);
        }


        public void UpdateWithAttack(AvailableMove move)
        {
            table.Remove(move.oldPositon);
            table[move.target.posVector] = move.moving;

        }


        public Piece FindPiece(Vector3 vector)
        {
            return table.TryGetValue(vector, out Piece value) ? value : null;
        }



        //return true if the target position is outside of the map
        public static bool CheckBoundaries(Vector3 ij)
        {
            return ij.Z < 1 || ij.Z > 8 || ij.X < 1 || ij.X > 8;
        }


        public List<AvailableMove> CheckAllMoves()
        {
            // Use ConcurrentBag for thread-safe collection.
            ConcurrentBag<AvailableMove> moves = new ConcurrentBag<AvailableMove>();

            // Use Parallel.ForEach to parallelize the loop.
            Parallel.ForEach(table, piece =>
            {
                // Skip pieces that are not on the current team.
                if (piece.Value.team != current) return;

                // Get valid moves for the piece and add them to the moves collection.
                var pieceMoves = piece.Value.CheckValidMovesVirt(this);
                foreach (var move in pieceMoves)
                {
                    moves.Add(move); // Adding to ConcurrentBag is thread-safe.
                }
            });

            // Convert ConcurrentBag back to List and return.
            return moves.ToList();
        }



        public void TakeBackMove(AvailableMove move)
        {
            if (move.firstMove) { move.moving.firstMove = true; }

            if (move.attack)
            {
                table[move.moving.posVector] = move.target;
                table.Add(move.oldPositon, move.moving);
                move.moving.posVector = move.oldPositon;
                return;
            }

            move.moving.posVector = move.oldPositon;
            table.Remove(move.move);
            table.Add(move.oldPositon, move.moving);

            if (move.castle) { TakeBackMove(new AvailableMove(move.rook, move.rookNewPos, false, move.rookOldPos, true)); }

        }



        public struct GameState
        {
            public string name;
            public int id;
            public int winner;
            public GameState(string name, int id, int winner)
            {
                this.name = name;
                this.id = id;
                this.winner = winner;
            }
        }


        // 0 = game is not over 1 = game is over (current player won) 2 = stalemate
        public GameState CheckGameState(List<AvailableMove> moves)
        {
            if (kingIsInCheck && moves.Count == 0)
            {
                return new GameState("Game is over! Winner is " + (current == 1 ? "Black" : "White"), 1, current * -1);

            }
            else if (!kingIsInCheck && moves.Count == 0)
            {
                return new GameState("Game is ended in a stalemate!", 2, 0);
            }

            return new GameState("Game is not over.", 0, 0);
        }


    }
}

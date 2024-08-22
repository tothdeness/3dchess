using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using test.Bots.Resources;
using test.Controllers;
using test.Moves;
using test.Pieces;
using test.Pieces.Resources;


namespace test.Bot
{
    public class Bot
	{

		private int depth;

		private int team;

		private int numOfEv;

		private const float pawn = 1f;

		private const float knight = 3f;

		private const float bishop = 3.3f;

		private const float rook = 5f;

		private const float queen = 9f;
		
		public Bot(int depth, int team) { this.depth = depth; this.team = team; }

		public void ExecuteNextMove(Board board)
		{

			numOfEv = 0;
			Stopwatch stop = new Stopwatch();
			stop.Start();
			NodeStatus s = MiniMax(depth, board, -1000000, 1000000, team);
			stop.Stop();

			GD.Print("ELAPSED MILISEC: " + stop.ElapsedMilliseconds + " " + "KIERTEKELT: " + numOfEv);

			try
			{

			s.move.moving.MovePieceWithVisualUpdate(TableController.CalculatePosition(s.move.move), s.move);

			}catch (Exception e)
			{

				GD.Print("Bot move error");
			}


		}



		private struct NodeStatus
		{
			public float positionPoints;
			public AvailableMove move;

			public NodeStatus(float positionPoints, AvailableMove move)
			{
				this.positionPoints = positionPoints;
				this.move = move;
			}
		}


		private NodeStatus MiniMax(int depth, Board board, float alpha, float beta, int currPlayer = 0)
		{

			var best = new NodeStatus();

			List<AvailableMove> allmoves = null;

			if (depth == 0)
			{
				var ans = EvaluateBoard(board);
				numOfEv++;
				return ans;
			}
			else
			{
				board.current = currPlayer;
				MoveGenerator.CheckValidMoves(board);

				allmoves = board.CheckAllMoves();
				Board.GameState state = board.CheckGameState(allmoves);

				if (state.id == 1) {  return new NodeStatus(500 * state.winner * depth, null); }
				else if (state.id == 2) { return new NodeStatus(0, null); }

			}


			if (currPlayer == 1) {


				 best = new NodeStatus(-10000f, null);

				foreach (AvailableMove move in CalculateCurrTeamAllMove(board, 1, allmoves))
				{
					move.moving.VirtualMove(move.move, move, board);


					var val = MiniMax(depth - 1, board, alpha, beta,  -1);

					if (val.positionPoints > best.positionPoints)
					{
						best.positionPoints = val.positionPoints;
						best.move = move;
					}

					board.TakeBackMove(move);

					alpha = Math.Max(alpha, val.positionPoints);
					if(beta <= alpha) { break; }
		

				}


			}
			else
			{
				best = new NodeStatus(10000f, null);

				List<AvailableMove> moves = CalculateCurrTeamAllMove(board, -1, allmoves);

				foreach (AvailableMove move in moves)
				{

					move.moving.VirtualMove(move.move, move, board);

					var val = MiniMax(depth - 1, board, alpha, beta, 1);


					if (val.positionPoints < best.positionPoints)
					{
						best.positionPoints = val.positionPoints;
						best.move = move;
					}

					board.TakeBackMove(move);

					beta = Math.Min(beta, val.positionPoints);
					if (beta <= alpha) { break; }

				}

			}

			return best;
		}






		private NodeStatus EvaluateBoard(Board board)
		{
			NodeStatus ans = new NodeStatus(0f, null);

			var pieceBaseScores = new Dictionary<Type, float>
			{
				{ typeof(Pawn), pawn },
				{ typeof(Horse), knight },
				{ typeof(Bishop), bishop },
				{ typeof(Rook), rook },
				{ typeof(Queen), queen }
			};

			foreach (var pieceEntry in board.table)
			{
				var piece = pieceEntry.Value;
				var pieceType = piece.GetType();

				if (pieceBaseScores.TryGetValue(pieceType, out float baseScore))
				{
					float score = baseScore + ReadSquareScore(piece);
					ans.positionPoints += piece.team == 1 ? score : -score;
				}
				else
				{
					var score = SquareScore.ReadSquareScoreKing(piece.posVector);

					ans.positionPoints += piece.team == 1 ? score : -score;

				}


			}

			return ans;
		}

		private float ReadSquareScore(Piece piece)
		{
			return piece switch
			{
				Pawn => SquareScore.ReadSquareScorePawn(piece.posVector, piece.team),
				Horse => SquareScore.ReadSquareScoreKnight(piece.posVector),
				_ => 0f 
			};
		}



		private List<AvailableMove> CalculateCurrTeamAllMove(Board board, int currteam, List<AvailableMove> moves)
		{
			moves.Sort((left, right) => Sorting(left, right, board));
			return moves;
		}


		private int Sorting(AvailableMove left, AvailableMove right, Board board)
		{
			int left_move_value = CalculatePoints(left,board);
			int right_move_value = CalculatePoints(right,board);

			if(left_move_value == right_move_value) { return 0; }		
			if(left_move_value > right_move_value) { return -1; }

			return 1;
		}

		private int CalculatePoints(AvailableMove move, Board board)
		{
			int ans = 0;

			if (move.attack) ans++;

			return ans;
		}



	}
}

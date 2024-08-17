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

		public void executeNextMove(Board board)
		{

			numOfEv = 0;
			Stopwatch stop = new Stopwatch();
			stop.Start();
			NodeStatus s = miniMax(depth, board, -1000000, 1000000, team);
			stop.Stop();

			GD.Print("ELAPSED MILISEC: " + stop.ElapsedMilliseconds + " " + "KIERTEKELT: " + numOfEv);


			if(s.move == null) { return; }

			GD.Print(s.move.to_String());

			try { s.move.moving.Move(TableController.calculatePosition(s.move.move), s.move); }
			catch
			{
				GD.Print("Off screen move!");
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


		private NodeStatus miniMax(int depth, Board board, float alpha, float beta, int currPlayer = 0)
		{

			var best = new NodeStatus();

			List<AvailableMove> allmoves = null;

			if (depth == 0)
			{
				var ans = evaluateBoard(board);
				numOfEv++;
				return ans;
			}
			else
			{
				board.current = currPlayer;
				MoveGenerator.checkValidMoves(board);

				allmoves = board.checkAllMoves();
				Board.gameState state = board.checkGameState(allmoves);

				if (state.id == 1) {  return new NodeStatus(500 * state.winner * depth, null); }
				else if (state.id == 2) { return new NodeStatus(0, null); }

			}




			if (currPlayer == 1) {


				 best = new NodeStatus(-10000f, null);

				foreach (AvailableMove move in calculateCurrTeamAllMove(board, 1, allmoves))
				{
					move.moving.VirtualMove(TableController.calculatePosition(move.move), move, board);


					var val = miniMax(depth - 1, board, alpha, beta,  -1);

					if (val.positionPoints > best.positionPoints)
					{
						best.positionPoints = val.positionPoints;
						best.move = move;
					}

					board.takeBackMove(move);

					alpha = Math.Max(alpha, val.positionPoints);
					if(beta <= alpha) { break; }
		

				}


			}
			else
			{
				best = new NodeStatus(10000f, null);

				List<AvailableMove> moves = calculateCurrTeamAllMove(board, -1, allmoves);

				foreach (AvailableMove move in moves)
				{

					move.moving.VirtualMove(TableController.calculatePosition(move.move), move, board);

					var val = miniMax(depth - 1, board, alpha, beta, 1);


					if (val.positionPoints < best.positionPoints)
					{
						best.positionPoints = val.positionPoints;
						best.move = move;
					}

					board.takeBackMove(move);

					beta = Math.Min(beta, val.positionPoints);
					if (beta <= alpha) { break; }

				}



			}


			return best;
		}






		private NodeStatus evaluateBoard(Board board)
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
					var score = SquareScore.ReadSquareScoreKing(piece.pos_vector);

					ans.positionPoints += piece.team == 1 ? score : -score;

				}


			}

			return ans;
		}

		private float ReadSquareScore(Piece piece)
		{
			return piece switch
			{
				Pawn => SquareScore.ReadSquareScorePawn(piece.pos_vector, piece.team),
				Horse => SquareScore.ReadSquareScoreKnight(piece.pos_vector),
				_ => 0f 
			};
		}



		private List<AvailableMove> calculateCurrTeamAllMove(Board board, int currteam, List<AvailableMove> moves)
		{
			moves.Sort((left, right) => sorting(left, right));
			return moves;
		}


		private int sorting(AvailableMove left, AvailableMove right)
		{
			int left_move_value = 0;
			int right_move_value = 0;

			if (left.attack) { 
				left_move_value ++;

			}

			if (right.attack) { 
				right_move_value ++;

			}


			if(left_move_value == right_move_value) { return 0; }		

			if(left_move_value > right_move_value) { return -1; }

			return 1;
		}

	}
}

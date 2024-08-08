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

		public double copyTime;

		public int numofEv;

		public int depth;

		public int team;

		public List<Piece> currentBoard;

		private static float pawn = 1f;

		private static float knight = 3f;

		private static float bishop = 3.3f;

		private static float rook = 5f;

		private static float queen = 9f;

		public NodeStatus currBestNode;

		public Bot(int depth, int team) { this.depth = depth; this.team = team; }



		public struct NodeStatus
		{
			public float positionPoints;
			public AvailableMove move;

			public NodeStatus(float positionPoints, AvailableMove move)
			{
				this.positionPoints = positionPoints;
				this.move = move;
			}
		}


		public NodeStatus miniMax(int depth, Board board, float alpha, float beta, int currPlayer = 0)
		{

			var best = new NodeStatus();


			if (depth == 0)
			{
				var ans = evaluateBoard(board,currPlayer);
				return ans;
			}



			if (currPlayer == 1) {


				 best = new NodeStatus(-10000f, null);

				foreach (AvailableMove move in calculateCurrTeamAllMove(board, 1))
				{

					move.moving = board.findPieceID(move.moving.ID);
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

				List<AvailableMove> moves = calculateCurrTeamAllMove(board, -1);

				foreach (AvailableMove move in moves)
				{
					move.moving = board.findPieceID(move.moving.ID);

					move.moving.VirtualMove(TableController.calculatePosition(move.move), move, board);

					var val = miniMax(depth - 1, board, alpha, beta, 1);


					//GD.Print(val.positionPoints + " BEST: " + best.positionPoints);

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






		private NodeStatus evaluateBoard(Board board, int currplayer)
		{
			NodeStatus ans = new NodeStatus(0f, null);

			foreach (Piece piece in board.table)
			{		

				if (piece is Pawn) {

					float score = pawn + SquareScore.ReadSquareScorePawn(piece.pos_vector,piece.team);

					if(piece.team == 1)
					{
						ans.positionPoints += score;
					}
					else
					{
						ans.positionPoints -= score;
					}

					continue;
				}


				if (piece is Horse) {

					float score = knight + SquareScore.ReadSquareScoreKnight(piece.pos_vector);

					if(piece.team == 1)
					{
						ans.positionPoints += score;
					}
					else
					{
						ans.positionPoints -= score;

					}
					continue;

				}


				if (piece is Bishop) { _ = piece.team == 1 ? ans.positionPoints += bishop : ans.positionPoints -= bishop; continue; }
				if (piece is Rook) { _ = piece.team == 1 ? ans.positionPoints += rook : ans.positionPoints -= rook; continue; }
				if (piece is Queen) { _ = piece.team == 1 ? ans.positionPoints += queen : ans.positionPoints -= queen; continue; }

			}	
	

			return ans;
		}



		public void executeNextMove(Board board)
		{

			NodeStatus s = miniMax(depth, board, -1000000, 1000000, team);

			normalizeMove(s.move);


			try { s.move.moving.Move(TableController.calculatePosition(s.move.move), s.move); } catch
			{
				GD.Print("Off screen move!");
			}
			

		}




		private void normalizeMove(AvailableMove move)
		{

			move.moving = TableController.findPieceID(move.moving.ID);

			if (move.attack)
			{
				move.target = TableController.findPieceID(move.target.ID);
			}

		}


		//fixed

		public List<AvailableMove> calculateCurrTeamAllMove(Board board, int currteam)
		{

			board.current = currteam;

			MoveGenerator.checkValidMoves(board);

			List<AvailableMove> ans = new List<AvailableMove>();

			List<Piece> boardPieces = board.table.ToList();


			Stopwatch stopwatch = new Stopwatch();

			stopwatch.Start();


			for (int i = 0; i < boardPieces.Count;i++)
			{


					if (boardPieces[i].team == currteam)
					{


						List<AvailableMove> temp = boardPieces[i].CheckValidMovesVirt(board);

				
						ans.AddRange(temp);


					}

			}


			ans.Sort((left, right) => sorting(left, right));



			stopwatch.Stop();

			copyTime += (double)stopwatch.ElapsedTicks / Stopwatch.Frequency * 1000;

			return ans;
		}


		public int sorting(AvailableMove left, AvailableMove right)
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

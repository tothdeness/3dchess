using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test.Mode;
using test.Moves;
using test.Pieces;
using test.Pieces.Resources;

namespace test.Controllers
{
    public class GameController
	{

		private bool botGame;

		private int depth;

		public Node3D tableGraphics;

		private int currPlayer;

		private int player1;

		public Dictionary<string,Piece> table;

		private int player2; //bot 

		public Board board;

		public GameController(bool botGame, int depth, Node3D tableGraphics, int player1)
		{
			this.botGame = botGame;
			this.depth = depth;
			this.tableGraphics = tableGraphics;
			this.currPlayer = 1;
			this.player1 = player1;
			this.player2 = player1 * -1;
			TableController.tableGraphics = tableGraphics;
			table = TableController.table;
			board = new Board(table);
			SetupBaseGame.AddPiecesStandardGame(this, board);
			if(player1 == -1) { NextMove(player1); }
		}


		public void NextMove(int team)
		{
	
			board.current = team * -1;

			MoveGenerator.CheckValidMoves(board);

			List<AvailableMove> moves = board.CheckAllMoves();

			GD.Print(board.CheckGameState(moves).name);

			if (team == player1 && botGame)
			{
				Bot.Bot bot = new Bot.Bot(depth,player2);

				Thread bot_thread = new Thread(() => bot.ExecuteNextMove(board)); 
				
				bot_thread.Start();
				
			}

		}




	}
}

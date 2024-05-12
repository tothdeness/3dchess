using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test.Mode;

namespace test.Controllers
{
	public class GameController
	{

		bool botGame;

		int depth;

		Node3D tableGraphics;

		int currPlayer;

		int player1;

		int player2; //bot 

		public GameController(bool botGame, int depth, Node3D tableGraphics,  int player1)
		{
			this.botGame = botGame;
			this.depth = depth;
			this.tableGraphics = tableGraphics;
			this.currPlayer = 1;
			this.player1 = player1;
			this.player2 = player1 * -1;
			TableController.tableGraphics = tableGraphics;
			TableController.game = this;
			SetupBaseGame.AddPiecesStandardGame();
			if(player1 == -1) { NextMove(player1); }
		}


		public void NextMove(int team)
		{
			currPlayer *= -1;

			if (team == player1 && botGame)
			{

		    	Bot.Bot bot = new Bot.Bot(depth,player2);

				Thread bot_thread = new Thread(bot.executeNextMove);
				
				bot_thread.Start();
			}


		}


	}
}

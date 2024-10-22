using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test.core.Bots;
using test.core.Mode;
using test.core.Moves;
using test.core.Pieces;
using test.core.Pieces.Resources;

namespace test.core.Controllers
{
    public class GameController
    {

        private bool botGame;

        public Node3D tableGraphics;

        private int currPlayer;

        private int player1;

        public Dictionary<Vector3, Piece> table;

        private int player2; //bot 

        public Board board;

        private Bot bot;

        public GameController(bool botGame, int depth, Node3D tableGraphics, int player1)
        {
            this.botGame = botGame;
            this.tableGraphics = tableGraphics;
            currPlayer = 1;
            this.player1 = player1;
            player2 = player1 * -1;
            TableController.tableGraphics = tableGraphics;
            table = TableController.table;
            board = new Board(table);
            bot = new Bot(depth, player2);
            SetupBaseGame.AddPiecesStandardGame(this, board);
            if (player1 == -1) { NextMove(player1); }
        }


        public void NextMove(int team)
        {

            board.current = team * -1;

            MoveGenerator.CheckValidMoves(board);

            List<AvailableMove> moves = board.CheckAllMoves();

            GD.Print(board.CheckGameState(moves).name);

            if (team == player1 && botGame)
            {
                Thread bot_thread = new Thread(() => bot.ExecuteNextMove(board));

                bot_thread.Start();

            }

        }




    }
}

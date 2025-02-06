using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test.core.Bots;
using test.core.Mode;
using test.core.Moves;
using test.core.Network;
using test.core.Pieces;
using test.core.Pieces.Resources;

namespace test.core.Controllers
{
    public class GameController
    {

        private bool botGame;

        public Node3D tableGraphics;

        public int player1;

        public Dictionary<Vector3, Piece> table;

        private int player2; //bot 

        public Board board;

        private Bot bot;

        private TcpConnect server;

        public int current = 1;

        public int gameMode;

        // 0 local
        // 1 bot
        // 2 lan

		public GameController(bool botGame, int depth, Node3D tableGraphics, int player1,TcpConnect server,int gameMode)
        {
            this.botGame = botGame;
            this.tableGraphics = tableGraphics;
            this.player1 = player1;
            player2 = player1 * -1;
            TableController.tableGraphics = tableGraphics;
            table = TableController.table;
            board = new Board(table);
            bot = new Bot(depth, player2);
            SetupBaseGame.AddPiecesStandardGame(this, board);
            this.gameMode = gameMode;
            this.server = server;
            if(gameMode == 2 && server != null)
            {
                server.ReceivedMove += ReceivedMove;
            }
            if (player1 == -1 && gameMode == 1) { NextMove(player1,null); }
        }





		public void NextMove(int team,AvailableMove move)
        {

            board.current = team * -1;

            current = board.current;

            MoveGenerator.CheckValidMoves(board);

            List<AvailableMove> moves = board.CheckAllMoves();

            GD.Print(board.CheckGameState(moves).name);

            if (team == player1 && botGame)
            {
                Thread bot_thread = new Thread(() => bot.ExecuteNextMove(board));

                bot_thread.Start();

            }else if(server != null && move.moving.team == player1)
            {
                server.SendMove(Serializer.Serialize(move));
            }

        }


        public void ReceivedMove(string move)
        {


            try
            {

				AvailableMove moveDeserialized = Serializer.Deserialize<AvailableMove>(move);

                Piece p = TableController.FindWithVector(moveDeserialized.oldPositon);

                foreach(AvailableMove m in p.CheckValidMovesVirt(board))
                {
                    GD.Print(m.move + "  " + moveDeserialized.move);

                    if(m.move.X == moveDeserialized.move.X && m.move.Z == moveDeserialized.move.Z)
                    {
						m.moving.MovePieceWithVisualUpdate(TableController.CalculatePosition(moveDeserialized.move), m);
					}
                }

			}
			catch (Exception ex)
            {
                GD.Print(ex);
            }

        }




    }
}

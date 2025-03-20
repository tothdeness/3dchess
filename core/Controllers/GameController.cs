using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using test.core.Bots;
using test.core.Logging;
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

        public string gameID;

        public bool load = true;

        public Thread bot_thread;

        public LinkedList<AvailableMove> moves = new LinkedList<AvailableMove> ();

        // 0 local
        // 1 bot
        // 2 lan

		public GameController(bool botGame, int depth, Node3D tableGraphics, int player1,TcpConnect server,int gameMode,string gameID)
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
            this.gameID = gameID;
		}


        private void AddVisuals()
        {
			SetupBaseGame.AddVisuals(this);
		}
        
        private void UpdateVisual()
        {
            SetupBaseGame.UpdateVisuals(this);
        }


        public void MoveBack()
        {

            if(moves.Count == 0) return;

            int currTeam = moves.Last().moving.team;

            if (gameMode == 1)
            {
   
                    board.TakeBackMove(moves.Last());
					moves.RemoveLast();
					board.TakeBackMove(moves.Last());
					currTeam = moves.Last().moving.team;
					moves.RemoveLast();


            }
            else
            {
                board.TakeBackMove(moves.Last());
                moves.RemoveLast();
            }


            UpdateVisual();


			current = currTeam;
			board.current = currTeam;


		
		
			MoveGenerator.CheckValidMoves(board);
		

		}



        public void StartGame()
        {
            LanGameStart();
			if (player1 == -1 && gameMode == 1) { NextMove(player1, null); }
		}

        private void LanGameStart()
        {
			if (gameMode == 2 && server != null)
			{
				server.ReceivedMove += ReceivedMove;
			}
		}


		public static GameController CreateAndStartGame(bool botGame, int depth, Node3D tableGraphics, int player1, TcpConnect server, int gameMode, string gameID)
        {
		   var game = new GameController(botGame, depth, tableGraphics, player1, server, gameMode, gameID);
           game.AddVisuals();
		   game.StartGame();
            return game;
        }


        public static  GameController  LoadAndStartGame(bool botGame, int depth, Node3D tableGraphics, int player1, TcpConnect server, int gameMode, string gameID)
        {
			var game = new GameController(botGame, depth, tableGraphics, player1, server, gameMode, gameID);

			LinkedList<Move> moves = MoveDataReader.GetMoveData(gameID);

            GD.Print(player1);

            game.board.current = 1;

            AvailableMove lastmove = null;

			foreach (Move move in moves)
			{

                var piece = TableController.FindWithVector(move.StartCoord);

				MoveGenerator.CheckValidMoves(game.board);

				var piece_moves = piece.CheckValidMovesVirt(game.board);

               foreach(AvailableMove mo in piece_moves)
                {
                    if(mo.move.X == move.EndCoord.X && mo.move.Z == move.EndCoord.Z)
                    {
                        piece.VirtualMove(move.EndCoord, mo, game.board);
                        lastmove = mo;
						game.moves.AddLast(lastmove);
						break;
                    }
                }

				game.board.current *= -1;
			}

			game.AddVisuals();
            game.LanGameStart();

      
			game.NextMove(lastmove.moving.team, lastmove);
		
            return game;

		}

     
		public void NextMove(int team,AvailableMove move)
        {

            if(move != null) this.moves.AddLast(move);

            board.current = team * -1;

            current = board.current;

            MoveGenerator.CheckValidMoves(board);

            List<AvailableMove> moves = board.CheckAllMoves();

            GD.Print(board.CheckGameState(moves).name);

            if (team == player1 && botGame)
            {

                GD.Print("sEARCHING");

                bot_thread = new Thread(() => bot.ExecuteNextMove(board));
                bot_thread.Start();
			}
			else if(server != null && move.moving.team == player1)
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

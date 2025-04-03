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

        public main tableGraphics;

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

        public bool isProcessingMove = false;

        public readonly object _moveLock = new object();

        public bool takeBackPending = false;

        public LinkedList<AvailableMove> moves = new LinkedList<AvailableMove>();

        // 0 local
        // 1 bot
        // 2 lan

        public GameController(bool botGame, int depth, main tableGraphics, int player1, TcpConnect server, int gameMode, string gameID)
        {
            this.botGame = botGame;
            this.tableGraphics = tableGraphics;
            this.player1 = player1;
            player2 = player1 * -1;
            TableController.tableGraphics = tableGraphics;
            table = TableController.table;
            board = new Board(table);
            bot_thread = new Thread(() => bot.ExecuteNextMove(board));
			bot = new Bot(depth, player2, this);
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


        public void MoveBack() { 

            lock (_moveLock)
            {

				if (moves.Count < 2 || isProcessingMove || gameMode == 1 && bot_thread.IsAlive) return;

			}


            board.TakeBackMove(moves.Last());
			moves.RemoveLast();
			board.TakeBackMove(moves.Last());
			int currTeam = moves.Last().moving.team;
			moves.RemoveLast();
            UpdateVisual();
			current = currTeam;
			board.current = currTeam;
            MoveLogger.DeleteLastMove(gameID);
			MoveLogger.DeleteLastMove(gameID);
			MoveGenerator.CheckValidMoves(board);
		
		}



        public async Task StartGameAsync(int nextplayer)
        {

			if (!tableGraphics.IsNodeReady()) // Optional: wait for basic ready first
			{
				await tableGraphics.ToSignal(tableGraphics, "ready");

			}

			if (!tableGraphics.IsFullyInitialized()) // Check our custom flag/wait for signal
			{
				GD.Print("GameController: Waiting for main to be fully initialized...");
				await tableGraphics.ToSignal(tableGraphics, main.SignalName.FullyInitialized);
				GD.Print("GameController: Main reports fully initialized.");
			}

			// Now call the method
			tableGraphics.CallDeferred("SetCameraPosition", player1);

			LanGameStart();
			if (player1 == -1 && gameMode == 1) { NextMove(nextplayer, null); }
		}

        private void LanGameStart()
        {
			if (gameMode == 2 && server != null)
			{
				server.ReceivedMove += ReceivedMove;
				server.OnTakeBackRequested += HandleTakeBackRequest;
				server.OnTakeBackAccepted += AcceptIncome;
				server.OnTakeBackDeclined += DeclineIncome;

			}
		}

		private void HandleTakeBackRequest()
		{
            tableGraphics.CallDeferred("ShowTakeBackPopup");
		}

		public static GameController CreateAndStartGame(bool botGame, int depth, main tableGraphics, int player1, TcpConnect server, int gameMode, string gameID)
        {
		   var game = new GameController(botGame, depth, tableGraphics, player1, server, gameMode, gameID);
           game.AddVisuals();
		   game.StartGameAsync(player1);
           return game;
        }


        public static  GameController  LoadAndStartGame(bool botGame, int depth, main tableGraphics, int player1, TcpConnect server, int gameMode, string gameID)
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

			game.current = game.board.current;
			game.AddVisuals();
            game.StartGameAsync(lastmove.moving.team);

            return game;

		}
		public void AcceptTakeBack()
		{
			if (gameMode == 2 && server != null)
			{
				server.SendAcceptTakeBack();
				MoveBack(); // Execute the take back on the accepting side
				GD.Print("Accepted opponent's take back request.");
                takeBackPending = false;
			}
		}

		// Decline a take back request
		public void DeclineTakeBack()
		{
			if (gameMode == 2 && server != null)
			{
				server.SendDeclineTakeBack();
				GD.Print("Declined opponent's take back request.");
				takeBackPending = false;
			}
		}


        public void DeclineIncome()
        {
            if (gameMode == 2 && server != null)
            {
                tableGraphics.CallDeferred("ResponseDeclined");
                takeBackPending = false;
            }
        }

        public void AcceptIncome()
        {
			if (gameMode == 2 && server != null)
			{
				tableGraphics.CallDeferred("ResponseAccept");
				MoveBack();
				takeBackPending = false;
			}
		}


		public void RequestTakeBack()
		{
			if (gameMode == 2 && server != null && current == player1) // LAN mode
			{
				takeBackPending = true;
				server.SendTakeBackMove();
				GD.Print("Requested take back from opponent.");
            }
            else
            {
                GD.Print("Cant send request!");
            }
		}




		public void NextMove(int team,AvailableMove move)
        {

            if(move != null) this.moves.AddLast(move);

            board.current = team * -1;

            current = board.current;

            MoveGenerator.CheckValidMoves(board);

            List<AvailableMove> moves = board.CheckAllMoves();

            GD.Print(board.CheckGameState(moves).name);

			lock (_moveLock) { isProcessingMove = false; }


			if (team == player1 && botGame)
            {
                bot_thread = new Thread(() => bot.ExecuteNextMove(board));
                bot_thread.Start();
			}
			else if(server != null && move.moving.team == player1)
            {
                server.SendMove(Serializer.Serialize(move));
            }

        }




		public void Cleanup()
		{
			GD.Print("Cleaning up GameController resources...");

			// --- Stop Bot Thread ---
			if (bot_thread != null && bot_thread.IsAlive)
			{
				GD.Print("Attempting to stop bot thread...");
				try
				{
					// NOTE: Thread.Abort() is generally discouraged due to potential state corruption.
					// A cooperative cancellation mechanism (e.g., using a CancellationToken
					// or a flag checked by the bot periodically) is strongly preferred if possible.
					// If you must use Abort, be aware of the risks.
					#pragma warning disable SYSLIB0006 // Disable Obsolete warning for Thread.Abort
					bot_thread.Abort();
					#pragma warning restore SYSLIB0006
					bot_thread.Join(TimeSpan.FromSeconds(1)); // Give it a moment to abort
					GD.Print("Bot thread stopped.");
				}
				catch (PlatformNotSupportedException pex)
				{
					GD.PrintErr($"Thread.Abort is not supported on this platform: {pex.Message}");
					// You MUST implement cooperative cancellation if Abort is not supported.
				}
				catch (ThreadStateException tex)
				{
					GD.PrintErr($"Error stopping bot thread (ThreadStateException): {tex.Message}");
					// Thread might be already stopped or in a state where it cannot be aborted.
				}
				catch (Exception ex)
				{
					GD.PrintErr($"Error stopping bot thread: {ex.Message}");
				}
			}
			bot_thread = null; // Release thread reference


			// --- Close Network Connection ---
			if (server != null)
			{
				GD.Print("Closing network connection...");
				server.CloseConnection(); // Call the cleanup method we added
			}
			server = null; // Release server reference

			// --- Optional: Clean up other resources ---
			// If GameController holds other disposable resources, clean them up here.
			// For example, unsubscribe from events on tableGraphics if necessary, etc.


			GD.Print("GameController cleanup finished.");
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

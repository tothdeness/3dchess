using System;
using System.Collections.Generic;
using System.Globalization; // For CultureInfo if needed later, good practice
using System.IO;
using System.Linq;

namespace test.core.Logging
{
	public static class CreateGameFile
	{
		// Allowed game types - includes LAN
		private static readonly HashSet<string> ValidGameTypes = new HashSet<string> { "Bot", "PvP", "LAN" };
		// Allowed player teams
		private static readonly HashSet<string> ValidPlayerTeams = new HashSet<string> { "White", "Black" };

		// Default directory path
		private const string DefaultDirectoryPath = "Games";

		/// <summary>
		/// Creates a new game file with game metadata.
		/// </summary>
		/// <param name="gameType">Type of game ("Bot", "PvP", or "LAN").</param>
		/// <param name="playerTeam">Player's team ("White" or "Black"). Required for Bot, optional for PvP and LAN.</param>
		/// <param name="botDepth">Depth of the bot AI (1-20), required for Bot game type only.</param>
		/// <param name="directoryPath">Directory to save the game file (defaults to "Games").</param>
		/// <returns>The generated Game ID.</returns>
		/// <exception cref="ArgumentException">Thrown for invalid parameter combinations.</exception>
		/// <exception cref="IOException">Thrown if directory/file creation fails.</exception>
		/// <exception cref="UnauthorizedAccessException">Thrown if permission is denied.</exception>
		public static string CreateNewGameFile(string gameType, string playerTeam = null, int? botDepth = null, string directoryPath = DefaultDirectoryPath)
		{
			// Validate game type
			if (!ValidGameTypes.Contains(gameType))
			{
				throw new ArgumentException($"Game type must be one of: {string.Join(", ", ValidGameTypes)}");
			}

			// Validate Bot specific requirements
			if (gameType == "Bot")
			{
				// Player team is mandatory for Bot
				if (string.IsNullOrWhiteSpace(playerTeam) || !ValidPlayerTeams.Contains(playerTeam))
				{
					throw new ArgumentException($"Player team must be specified as 'White' or 'Black' for Bot game type.");
				}
				// Bot depth is mandatory and must be in range
				if (botDepth == null)
				{
					throw new ArgumentException("Bot depth must be provided for Bot game type.");
				}
				if (botDepth < 1 || botDepth > 20)
				{
					throw new ArgumentException("Bot depth must be between 1 and 20.");
				}
			}
			else // For PvP or LAN
			{
				// Bot depth should not be provided
				if (botDepth != null)
				{
					throw new ArgumentException($"Bot depth should not be provided for {gameType} game type.");
				}
				// Player team is optional, but if provided, must be valid
				if (!string.IsNullOrWhiteSpace(playerTeam) && !ValidPlayerTeams.Contains(playerTeam))
				{
					// Allow playerTeam to be null or empty, but if it's not, validate it.
					throw new ArgumentException($"Player team, if provided for {gameType}, must be 'White' or 'Black'.");
				}
			}

			// Ensure directory exists
			try
			{
				Directory.CreateDirectory(directoryPath); // Creates if not exists, does nothing if exists
			}
			catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
			{
				// Catch specific exceptions related to directory creation
				throw new IOException($"Failed to create or access directory '{directoryPath}'.", ex);
			}


			// Generate random game ID (6-digit number)
			Random random = new Random();
			// Ensure 6 digits with leading zeros if necessary
			string gameId = random.Next(0, 1000000).ToString("D6");

			// Get current date/time in a consistent format
			string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

			// Create file name (e.g., Game_LAN_123456.txt)
			string fileName = $"Game_{gameType}_{gameId}.txt";
			string fullPath = Path.Combine(directoryPath, fileName);

			try
			{
				// Write initial game data to file using StreamWriter for efficiency
				// Use 'using' to ensure the writer is disposed correctly
				using (StreamWriter writer = new StreamWriter(fullPath, false)) // false to overwrite if exists
				{
					writer.WriteLine($"[GameType \"{gameType}\"]");
					writer.WriteLine($"[GameID \"{gameId}\"]");
					writer.WriteLine($"[Date \"{date}\"]");

					// Write BotDepth only if it's a Bot game and depth is provided
					if (gameType == "Bot" && botDepth.HasValue)
					{
						writer.WriteLine($"[BotDepth \"{botDepth.Value}\"]");
					}

					// Write PlayerTeam if it has been provided (for Bot, PvP, or LAN)
					if (!string.IsNullOrWhiteSpace(playerTeam))
					{
						writer.WriteLine($"[PlayerTeam \"{playerTeam}\"]");
					}

					writer.WriteLine(); // Empty line before moves start
				}

				return gameId; // Return the gameID
			}
			catch (IOException ex)
			{
				// Wrap IO exception for better context
				throw new IOException($"Failed to create or write to game file '{fullPath}'.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				// Wrap UnauthorizedAccess exception
				throw new UnauthorizedAccessException($"Permission denied when trying to write to game file '{fullPath}'.", ex);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace test.core.Logging
{
	public static class CreateGameFile
	{
		// Method to create a game file with game type, random ID, date, optional bot depth, and player team
		public static string CreateNewGameFile(string gameType, string playerTeam = null, int? botDepth = null, string directoryPath = "Games")
		{
			// Validate game type
			if (gameType != "Bot" && gameType != "PvP")
			{
				throw new ArgumentException("Game type must be 'Bot' or 'PvP'");
			}

			// Validate player team
			if (gameType == "Bot" && (playerTeam != "White" && playerTeam != "Black"))
			{
				throw new ArgumentException("Player team must be 'White' or 'Black' for Bot game type");
			}
			if (gameType == "PvP" && playerTeam != null && playerTeam != "White" && playerTeam != "Black")
			{
				throw new ArgumentException("Player team, if provided for PvP, must be 'White' or 'Black'");
			}

			// Validate botDepth for Bot game
			if (gameType == "Bot" && botDepth == null)
			{
				throw new ArgumentException("Bot depth must be provided for Bot game type");
			}
			if (gameType == "PvP" && botDepth != null)
			{
				throw new ArgumentException("Bot depth should not be provided for PvP game type");
			}
			if (botDepth.HasValue && (botDepth < 1 || botDepth > 20))
			{
				throw new ArgumentException("Bot depth must be between 1 and 20");
			}

			// Ensure directory exists
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}

			// Generate random game ID (e.g., 6-digit number)
			Random random = new Random();
			string gameId = random.Next(100000, 999999).ToString();

			// Get current date
			string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

			// Create file name (e.g., Game_Bot_123456.txt)
			string fileName = $"Game_{gameType}_{gameId}.txt";
			string fullPath = Path.Combine(directoryPath, fileName);

			try
			{
				// Write initial game data to file
				using (StreamWriter writer = new StreamWriter(fullPath, false)) // false to overwrite if exists
				{
					writer.WriteLine($"[GameType \"{gameType}\"]");
					writer.WriteLine($"[GameID \"{gameId}\"]");
					writer.WriteLine($"[Date \"{date}\"]");
					if (gameType == "Bot" && botDepth.HasValue)
					{
						writer.WriteLine($"[BotDepth \"{botDepth}\"]");
					}
					if (playerTeam != null) // Write player team if provided
					{
						writer.WriteLine($"[PlayerTeam \"{playerTeam}\"]");
					}
					writer.WriteLine(); // Empty line before moves start
				}

				return gameId; // Return the gameID
			}
			catch (IOException ex)
			{
				throw new IOException($"Failed to create game file: {ex.Message}");
			}
		}
	}
}
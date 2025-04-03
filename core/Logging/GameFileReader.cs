using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Needed for LINQ FirstOrDefault

namespace test.core.Logging
{
	public static class GameFileReader
	{
		private const string DirectoryPath = "Games"; // Matches CreateGameFile default

		/// <summary>
		/// Finds the game file path for a given game ID, searching across known types.
		/// </summary>
		/// <param name="gameId">The ID of the game.</param>
		/// <param name="directoryPath">The directory to search in.</param>
		/// <returns>The full path to the game file, or null if not found.</returns>
		private static string FindGameFilePath(string gameId, string directoryPath = DirectoryPath)
		{
			if (string.IsNullOrWhiteSpace(gameId)) return null;
			// Check if directory exists before attempting to search
			if (!Directory.Exists(directoryPath))
			{
				Console.WriteLine($"Error: Directory '{directoryPath}' not found.");
				return null;
			}


			try
			{
				// Search for files matching the pattern Game_*_GameID.txt
				// This is robust and handles Bot, PvP, LAN, or any future types
				var matchingFiles = Directory.EnumerateFiles(directoryPath, $"Game_*_{gameId}.txt");
				return matchingFiles.FirstOrDefault(); // Return the first match, or null if none
			}
			catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
			{
				// Catch potential IO errors during search
				Console.WriteLine($"Error searching for game file with ID {gameId} in {directoryPath}: {ex.Message}");
				return null;
			}
		}


		/// <summary>
		/// Retrieves game metadata from a game file based on gameId.
		/// </summary>
		/// <param name="gameId">The ID of the game.</param>
		/// <returns>A dictionary containing game metadata. Values might be empty if not found or file is invalid.</returns>
		/// <exception cref="ArgumentException">Thrown if gameId is null or empty.</exception>
		public static Dictionary<string, string> GetGameMetadata(string gameId)
		{
			// Initialize with expected keys for consistency
			var metadata = new Dictionary<string, string>
			{
				{ "GameType", string.Empty },
				{ "GameID", string.Empty },
				{ "Date", string.Empty },
				{ "BotDepth", string.Empty }, // Keep even if not Bot game for consistency
                { "PlayerTeam", string.Empty }
			};

			if (string.IsNullOrWhiteSpace(gameId))
			{
				throw new ArgumentException("Game ID cannot be null or empty.", nameof(gameId));
			}

			// Use the robust FindGameFilePath method
			string filePath = FindGameFilePath(gameId);

			if (filePath == null) // No need to check File.Exists again, FindGameFilePath handles it
			{
				// Message printed within FindGameFilePath if directory exists but file doesn't
				Console.WriteLine($"Info: Game file for ID {gameId} not found. Returning default metadata.");
				return metadata; // Return default metadata
			}

			try
			{
				// Read lines efficiently using ReadLines (better for potentially large files)
				foreach (string line in File.ReadLines(filePath))
				{
					string trimmedLine = line.Trim();
					// Refined check for metadata format [Key "Value"]
					if (trimmedLine.Length > 4 && trimmedLine.StartsWith("[") && trimmedLine.EndsWith("\"]"))
					{
						// Find the position separating key and value part "..."
						int keyEndIndex = trimmedLine.IndexOf(" \"", StringComparison.Ordinal);
						if (keyEndIndex > 1) // Ensure key is not empty ("[") and space exists
						{
							string key = trimmedLine.Substring(1, keyEndIndex - 1);
							// Extract value between the quotes "Value" -> Value
							string value = trimmedLine.Substring(keyEndIndex + 2, trimmedLine.Length - keyEndIndex - 4); // Adjusted length calculation

							if (metadata.ContainsKey(key))
							{
								metadata[key] = value; // Assign extracted value
							}
						}
					}
					// Stop reading after the first empty line (usually separates metadata from moves)
					if (string.IsNullOrWhiteSpace(trimmedLine))
					{
						break;
					}
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Error reading game metadata file '{filePath}': {ex.Message}");
				// Return potentially partially filled metadata or default if error occurred early
			}
			catch (UnauthorizedAccessException ex)
			{
				Console.WriteLine($"Permission denied reading game metadata file '{filePath}'.", ex);
			}


			return metadata;
		}
	}
}
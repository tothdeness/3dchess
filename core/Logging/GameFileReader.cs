using System;
using System.Collections.Generic;
using System.IO;

namespace test.core.Logging
{
	public static class GameFileReader
	{
		private const string DirectoryPath = "Games"; // Matches CreateGameFile default

		// Retrieves game metadata from a file based on gameId
		public static Dictionary<string, string> GetGameMetadata(string gameId)
		{
			var metadata = new Dictionary<string, string>
			{
				{ "GameType", "" },
				{ "GameID", "" },
				{ "Date", "" },
				{ "BotDepth", "" },
				{ "PlayerTeam", "" }
			};

			if (string.IsNullOrWhiteSpace(gameId))
			{
				throw new ArgumentException("Game ID cannot be null or empty", nameof(gameId));
			}

			// Try both Bot and PvP file names
			string filePath = Path.Combine(DirectoryPath, $"Game_Bot_{gameId}.txt");
			if (!File.Exists(filePath))
			{
				filePath = Path.Combine(DirectoryPath, $"Game_PvP_{gameId}.txt");
				if (!File.Exists(filePath))
				{
					Console.WriteLine($"Error: Game file for ID {gameId} not found in {DirectoryPath}");
					return metadata; // Return empty/default metadata if file not found
				}
			}

			try
			{
				string[] lines = File.ReadAllLines(filePath);
				foreach (string line in lines)
				{
					string trimmedLine = line.Trim();
					if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
					{
						// Extract key and value from [Key "Value"]
						string content = trimmedLine.Substring(1, trimmedLine.Length - 2); // Remove [ and ]
						int quoteIndex = content.IndexOf('"');
						if (quoteIndex > 0 && content.EndsWith("\""))
						{
							string key = content.Substring(0, quoteIndex).Trim();
							string value = content.Substring(quoteIndex + 1, content.Length - quoteIndex - 2); // Remove quotes
							if (metadata.ContainsKey(key))
							{
								metadata[key] = value;
							}
						}
					}
				}
			}
			catch (IOException ex)
			{
				Console.WriteLine($"Error reading file {filePath}: {ex.Message}");
			}

			return metadata;
		}
	}
}
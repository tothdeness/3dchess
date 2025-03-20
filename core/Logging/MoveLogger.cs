using System;
using System.IO;
using System.Linq;
using Godot;

namespace test.core.Logging
{
	public static class MoveLogger
	{
		private const string DirectoryPath = "Games"; // Matches CreateGameFile default

		// Logs a move with gameId, start, and end coordinates as Vector3, determining the next move number
		public static void LogMove(string gameId, Vector3 startCoord, Vector3 endCoord)
		{
			if (string.IsNullOrWhiteSpace(gameId))
			{
				throw new ArgumentException("Game ID cannot be null or empty", nameof(gameId));
			}

			// Construct file path (assumes file follows CreateGameFile naming convention)
			string filePath = Path.Combine(DirectoryPath, $"Game_Bot_{gameId}.txt");
			if (!File.Exists(filePath))
			{
				filePath = Path.Combine(DirectoryPath, $"Game_PvP_{gameId}.txt"); // Try PvP if Bot not found
				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException($"Game file for ID {gameId} not found in {DirectoryPath}");
				}
			}

			try
			{
				// Read existing moves to determine next move number
				int nextMoveNumber = 1;
				if (new FileInfo(filePath).Length > 0) // Check if file has content
				{
					var lines = File.ReadAllLines(filePath);
					// Find lines that look like moves (e.g., "1. (x, y, z) (x, y, z)")
					var moveLines = lines.Where(line => line.Trim().StartsWith($"{nextMoveNumber}.")).ToList();
					nextMoveNumber = moveLines.Count + 1; // Next number after last move
				}

				// Format coordinates with invariant culture (dots for decimals)
				System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InvariantCulture;
				string startStr = $"({startCoord.X.ToString(ci)}, {startCoord.Y.ToString(ci)}, {startCoord.Z.ToString(ci)})";
				string endStr = $"({endCoord.X.ToString(ci)}, {endCoord.Y.ToString(ci)}, {endCoord.Z.ToString(ci)})";
				string move = $"{startStr} {endStr}";
				string moveLine = $"{nextMoveNumber}. {move}";

				// Append to file on a new line
				using (StreamWriter writer = new StreamWriter(filePath, true)) // true to append
				{
					writer.WriteLine(moveLine);
				}
			}
			catch (IOException ex)
			{
				throw new IOException($"Failed to log move to file {filePath}: {ex.Message}");
			}
		}
	}
}
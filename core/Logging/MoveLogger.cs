using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;

namespace test.core.Logging
{
	public static class MoveLogger
	{
		private const string DirectoryPath = "Games"; // Matches CreateGameFile default

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
				filePath = Path.Combine(DirectoryPath, $"Game_PvP_{gameId}.txt");
				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException($"Game file for ID {gameId} not found in {DirectoryPath}");
				}
			}

			try
			{
				// Read existing lines to count move lines
				int moveCount = 0;
				if (new FileInfo(filePath).Length > 0)
				{
					var lines = File.ReadAllLines(filePath);
					// Count lines that start with a number followed by a period (e.g., "1.", "2.", etc.)
					moveCount = lines.Count(line => Regex.IsMatch(line.Trim(), @"^\d+\."));
				}
				int nextMoveNumber = moveCount + 1;

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


		public static void DeleteLastMove(string gameId)
		{
			if (string.IsNullOrWhiteSpace(gameId))
			{
				throw new ArgumentException("Game ID cannot be null or empty", nameof(gameId));
			}

			string filePath = GetFilePath(gameId);
			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException($"Game file for ID {gameId} not found in {DirectoryPath}");
			}

			try
			{
				// Read all lines into a list
				var lines = File.ReadAllLines(filePath).ToList();
				// Find the last move line
				int lastMoveIndex = -1;
				for (int i = lines.Count - 1; i >= 0; i--)
				{
					if (Regex.IsMatch(lines[i].Trim(), @"^\d+\."))
					{
						lastMoveIndex = i;
						break;
					}
				}

				if (lastMoveIndex != -1)
				{
					// Remove the last move and update the file
					lines.RemoveAt(lastMoveIndex);
					File.WriteAllLines(filePath, lines);
				}
				else
				{
					GD.Print("No moves found to delete.");
				}
			}
			catch (IOException ex)
			{
				throw new IOException($"Failed to delete last move from file {filePath}: {ex.Message}");
			}
		}


		private static string GetFilePath(string gameId)
		{
			string filePath = Path.Combine(DirectoryPath, $"Game_Bot_{gameId}.txt");
			if (!File.Exists(filePath))
			{
				filePath = Path.Combine(DirectoryPath, $"Game_PvP_{gameId}.txt");
				if (!File.Exists(filePath))
				{
					throw new FileNotFoundException($"Game file for ID {gameId} not found in {DirectoryPath}");
				}
			}
			return filePath;
		}




	}

}
using System;
using System.Collections.Generic;
using System.Globalization; // For CultureInfo
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Godot; // Assuming Godot context for Vector3 and GD.Print

namespace test.core.Logging
{
	// Struct definition remains the same
	public struct Move
	{
		public Vector3 StartCoord { get; }
		public Vector3 EndCoord { get; }

		public Move(Vector3 startCoord, Vector3 endCoord)
		{
			StartCoord = startCoord;
			EndCoord = endCoord;
		}

		// Optional: Improve ToString for clarity if needed
		public override string ToString() => $"Move from ({StartCoord.X:F1}, {StartCoord.Z:F1}) to ({EndCoord.X:F1}, {EndCoord.Z:F1})"; // Example showing X,Z with 1 decimal place
	}


	public static class MoveDataReader
	{
		private const string DirectoryPath = "Games";
		// Regex to find coordinates like "(1.2, 3.4, 5.6)" more reliably
		private static readonly Regex CoordinateRegex = new Regex(@"\(\s*(-?\d+(\.\d+)?)\s*,\s*(-?\d+(\.\d+)?)\s*,\s*(-?\d+(\.\d+)?)\s*\)", RegexOptions.Compiled);
		// Regex to identify a move line (starts with number, dot, space)
		private static readonly Regex MoveLineRegex = new Regex(@"^\d+\.\s+", RegexOptions.Compiled);


		/// <summary>
		/// Finds the game file path for a given game ID, searching across known types.
		/// </summary>
		/// <param name="gameId">The ID of the game.</param>
		/// <param name="directoryPath">The directory to search in.</param>
		/// <returns>The full path to the game file, or null if not found.</returns>
		private static string FindGameFilePath(string gameId, string directoryPath = DirectoryPath)
		{
			if (string.IsNullOrWhiteSpace(gameId)) return null;
			if (!Directory.Exists(directoryPath)) return null;

			try
			{
				var matchingFiles = Directory.EnumerateFiles(directoryPath, $"Game_*_{gameId}.txt");
				return matchingFiles.FirstOrDefault();
			}
			catch (IOException ex)
			{
				GD.Print($"Error searching for game file with ID {gameId} in {directoryPath}: {ex.Message}");
				return null;
			}
		}


		/// <summary>
		/// Reads move data from a game file.
		/// </summary>
		/// <param name="gameId">The ID of the game.</param>
		/// <returns>A LinkedList of moves read from the file.</returns>
		/// <exception cref="ArgumentException">Thrown if gameId is null or empty.</exception>
		public static LinkedList<Move> GetMoveData(string gameId)
		{
			var moves = new LinkedList<Move>();

			if (string.IsNullOrWhiteSpace(gameId))
			{
				throw new ArgumentException("Game ID cannot be null or empty.", nameof(gameId));
			}

			string filePath = FindGameFilePath(gameId);

			if (filePath == null || !File.Exists(filePath))
			{
				GD.Print($"Error: Game file for ID {gameId} not found in {DirectoryPath}. Cannot read moves.");
				return moves; // Return empty list
			}

			try
			{
				GD.Print($"Reading moves from: {filePath}");
				// Use File.ReadLines for potentially large files (memory efficient)
				foreach (string line in File.ReadLines(filePath))
				{
					string trimmedLine = line.Trim();
					// Check if it looks like a move line (e.g., "1. (x,y,z) (x,y,z)")
					Match moveLineMatch = MoveLineRegex.Match(trimmedLine);
					if (moveLineMatch.Success)
					{
						// Extract the part after "N. "
						string moveData = trimmedLine.Substring(moveLineMatch.Length);
						//GD.Print($"Processing move data: '{moveData}'");

						// Find all coordinate pairs in the move data part
						var coordinateMatches = CoordinateRegex.Matches(moveData);

						if (coordinateMatches.Count == 2)
						{
							try
							{
								Vector3 startCoord = ParseCoordinate(coordinateMatches[0].Value);
								Vector3 endCoord = ParseCoordinate(coordinateMatches[1].Value);
								moves.AddLast(new Move(startCoord, endCoord));
								//GD.Print($"Added move: {startCoord} to {endCoord}");
							}
							catch (FormatException ex)
							{
								GD.Print($"Warning: Failed to parse coordinates in move line '{trimmedLine}'. Skipping. Error: {ex.Message}");
							}
							catch (OverflowException ex)
							{
								GD.Print($"Warning: Coordinate value out of range in move line '{trimmedLine}'. Skipping. Error: {ex.Message}");
							}
						}
						else
						{
							// Log only if the line started like a move line but format was wrong
							GD.Print($"Warning: Invalid coordinate format or count ({coordinateMatches.Count}) in move line: '{trimmedLine}'. Expected 2 coordinate sets.");
						}
					}
					// Optional: Log lines that are not metadata and not move lines if needed for debugging
					// else if (!trimmedLine.StartsWith("[") && !string.IsNullOrEmpty(trimmedLine))
					// {
					//     GD.Print($"Info: Skipping non-move, non-metadata line: '{trimmedLine}'");
					// }
				}
				GD.Print($"Finished reading. Found {moves.Count} valid moves.");
			}
			catch (IOException ex)
			{
				GD.Print($"Error reading moves file '{filePath}': {ex.Message}");
			}
			catch (UnauthorizedAccessException ex)
			{
				GD.Print($"Permission denied reading moves file '{filePath}'.", ex);
			}

			return moves;
		}

		/// <summary>
		/// Parses a coordinate string "(x, y, z)" into a Vector3.
		/// Uses InvariantCulture for reliable float parsing.
		/// </summary>
		/// <param name="coordString">The coordinate string.</param>
		/// <returns>A Vector3 representing the coordinates.</returns>
		/// <exception cref="FormatException">Thrown if the format is invalid or parsing fails.</exception>
		/// <exception cref="OverflowException">Thrown if a number is outside the range of Single.</exception>
		private static Vector3 ParseCoordinate(string coordString)
		{
			// Reuse the compiled regex for efficiency and accuracy
			Match match = CoordinateRegex.Match(coordString);
			if (match.Success)
			{
				// Groups[1] is X, Groups[3] is Y, Groups[5] is Z due to inner capture groups for decimals
				// Use InvariantCulture to ensure '.' is the decimal separator
				float x = float.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
				float y = float.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
				float z = float.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture);
				return new Vector3(x, y, z);
			}
			else
			{
				throw new FormatException($"Invalid coordinate format: \"{coordString}\". Expected \"(x, y, z)\".");
			}
		}
	}
}
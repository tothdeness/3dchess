using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;

namespace test.core.Logging
{
	public struct Move
	{
		public Vector3 StartCoord { get; }
		public Vector3 EndCoord { get; }

		public Move(Vector3 startCoord, Vector3 endCoord)
		{
			StartCoord = startCoord;
			EndCoord = endCoord;
		}

		public override string ToString() => $"[{StartCoord.X},{StartCoord.Z}] [{EndCoord.X},{EndCoord.Z}]";
	}

	public static class MoveDataReader
	{
		private const string DirectoryPath = "Games";

		public static LinkedList<Move> GetMoveData(string gameId)
		{
			var moves = new LinkedList<Move>();

			if (string.IsNullOrWhiteSpace(gameId))
			{
				throw new ArgumentException("Game ID cannot be null or empty", nameof(gameId));
			}

			string filePath = Path.Combine(DirectoryPath, $"Game_Bot_{gameId}.txt");
			if (!File.Exists(filePath))
			{
				filePath = Path.Combine(DirectoryPath, $"Game_PvP_{gameId}.txt");
				if (!File.Exists(filePath))
				{
					GD.Print($"Error: Game file for ID {gameId} not found in {DirectoryPath}");
					return moves;
				}
			}

			try
			{
				string[] lines = File.ReadAllLines(filePath);
				GD.Print($"Reading {lines.Length} lines from {filePath}");
				foreach (string line in lines)
				{
					string trimmedLine = line.Trim();
					GD.Print($"Line: '{trimmedLine}'");
					if (trimmedLine.Length > 0 && char.IsDigit(trimmedLine[0]) && trimmedLine.Contains("."))
					{
						string[] parts = trimmedLine.Split(new[] { '.' }, 2, StringSplitOptions.RemoveEmptyEntries);
						if (parts.Length == 2)
						{
							string moveData = parts[1].Trim();
							GD.Print($"Move data: '{moveData}'");

							// Use regex to find all coordinates in parentheses
							var coordinateMatches = Regex.Matches(moveData, @"\([^)]*\)");
							GD.Print($"Found {coordinateMatches.Count} coordinate(s) in '{moveData}'");

							if (coordinateMatches.Count == 2)
							{
								string startCoordStr = coordinateMatches[0].Value;
								string endCoordStr = coordinateMatches[1].Value;
								try
								{
									Vector3 startCoord = ParseCoordinate(startCoordStr);
									Vector3 endCoord = ParseCoordinate(endCoordStr);
									moves.AddLast(new Move(startCoord, endCoord));
									GD.Print($"Added move: {startCoord} to {endCoord}");
								}
								catch (FormatException ex)
								{
									GD.Print($"Failed to parse coordinates in '{moveData}': {ex.Message}");
								}
							}
							else
							{
								GD.Print($"Invalid coordinate count: expected 2, found {coordinateMatches.Count} in '{moveData}'");
							}
						}
						else
						{
							GD.Print($"Invalid move line format: {parts.Length} parts in '{trimmedLine}'");
						}
					}
				}
			}
			catch (IOException ex)
			{
				GD.Print($"Error reading file {filePath}: {ex.Message}");
			}

			return moves;
		}

		private static Vector3 ParseCoordinate(string coord)
		{
			string cleanCoord = coord.Trim('(', ')');
			string[] parts = cleanCoord.Split(',');
			if (parts.Length == 3)
			{
				System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InvariantCulture;
				// Ensure any commas in numbers are replaced with periods (for cultures where decimal is comma)
				string xStr = parts[0].Trim().Replace(',', '.');
				string yStr = parts[1].Trim().Replace(',', '.');
				string zStr = parts[2].Trim().Replace(',', '.');
				if (float.TryParse(xStr, System.Globalization.NumberStyles.Float, ci, out float x) &&
					float.TryParse(yStr, System.Globalization.NumberStyles.Float, ci, out float y) &&
					float.TryParse(zStr, System.Globalization.NumberStyles.Float, ci, out float z))
				{
					return new Vector3(x, y, z);
				}
			}
			throw new FormatException($"Invalid coordinate format: {coord}");
		}
	}
}
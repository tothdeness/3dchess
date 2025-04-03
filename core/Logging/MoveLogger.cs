using System;
using System.Collections.Generic; // For List
using System.Globalization; // For CultureInfo
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Godot; // Assuming Godot context

namespace test.core.Logging
{
	public static class MoveLogger
	{
		private const string DirectoryPath = "Games";
		// Regex to identify a move line (starts with number, dot, space) - same as reader
		private static readonly Regex MoveLineRegex = new Regex(@"^\d+\.\s+", RegexOptions.Compiled);

		/// <summary>
		/// Finds the game file path for a given game ID, searching across known types.
		/// Centralized helper method.
		/// </summary>
		/// <param name="gameId">The ID of the game.</param>
		/// <param name="directoryPath">The directory to search in.</param>
		/// <returns>The full path to the game file.</returns>
		/// <exception cref="FileNotFoundException">Thrown if the game file cannot be found.</exception>
		/// <exception cref="IOException">Thrown if there's an error accessing the directory.</exception>
		private static string GetGameFilePath(string gameId, string directoryPath = DirectoryPath)
		{
			if (string.IsNullOrWhiteSpace(gameId))
			{
				throw new ArgumentException("Game ID cannot be null or empty.", nameof(gameId));
			}
			if (!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException($"Game directory '{directoryPath}' not found.");
			}

			try
			{
				// Search for the specific game file pattern
				var matchingFiles = Directory.EnumerateFiles(directoryPath, $"Game_*_{gameId}.txt");
				string filePath = matchingFiles.FirstOrDefault(); // Get the first match

				if (filePath == null)
				{
					throw new FileNotFoundException($"Game file for ID '{gameId}' not found in '{directoryPath}'.");
				}
				return filePath;
			}
			catch (IOException ex) // Catch potential IO errors during search
			{
				// Wrap the exception for better context
				throw new IOException($"An error occurred while searching for game file '{gameId}' in '{directoryPath}'.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new IOException($"Permission denied accessing directory '{directoryPath}'.", ex);
			}
		}


		/// <summary>
		/// Logs a single move to the specified game file.
		/// </summary>
		/// <param name="gameId">The ID of the game.</param>
		/// <param name="startCoord">The starting coordinate of the move.</param>
		/// <param name="endCoord">The ending coordinate of the move.</param>
		/// <exception cref="ArgumentException">Thrown if gameId is null or empty.</exception>
		/// <exception cref="FileNotFoundException">Thrown if the game file is not found.</exception>
		/// <exception cref="IOException">Thrown if logging fails due to IO or permission issues.</exception>
		public static void LogMove(string gameId, Vector3 startCoord, Vector3 endCoord)
		{
			string filePath = GetGameFilePath(gameId); // Throws if not found or invalid ID

			try
			{
				int nextMoveNumber = 1; // Default if file is empty or no moves yet
										// Read lines only if file exists and has content to determine next move number
				if (File.Exists(filePath) && new FileInfo(filePath).Length > 0)
				{
					// Use LINQ to count existing move lines efficiently
					// ReadAllLines might be okay for typical game logs, but ReadLines is safer for huge files
					var lines = File.ReadLines(filePath);
					int lastMoveNumber = lines.Select(line => MoveLineRegex.Match(line.Trim()))
										   .Where(match => match.Success)
										   .Select(match => int.Parse(match.Value.Trim('.', ' '))) // Extract number
										   .DefaultIfEmpty(0) // Handle empty file or no moves
										   .Max(); // Get the highest move number
					nextMoveNumber = lastMoveNumber + 1;

					// Alternative using Count: Less robust if numbers are skipped, but simpler
					// moveCount = lines.Count(line => MoveLineRegex.IsMatch(line.Trim()));
					// nextMoveNumber = moveCount + 1;
				}


				// Format coordinates using InvariantCulture for consistency
				CultureInfo ci = CultureInfo.InvariantCulture;
				string startStr = $"({startCoord.X.ToString(ci)}, {startCoord.Y.ToString(ci)}, {startCoord.Z.ToString(ci)})";
				string endStr = $"({endCoord.X.ToString(ci)}, {endCoord.Y.ToString(ci)}, {endCoord.Z.ToString(ci)})";

				// Construct the move line
				string moveLine = $"{nextMoveNumber}. {startStr} {endStr}";

				// Append the move line to the file
				// Use using for proper disposal of StreamWriter
				using (StreamWriter writer = new StreamWriter(filePath, true)) // true to append
				{
					writer.WriteLine(moveLine);
				}
			}
			catch (IOException ex)
			{
				// Provide more context in the exception
				throw new IOException($"Failed to log move to file '{filePath}'.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new IOException($"Permission denied writing to file '{filePath}'.", ex);
			}
			catch (FormatException ex) // Catch potential errors from int.Parse if regex logic were flawed
			{
				throw new IOException($"Error determining next move number in file '{filePath}'. File might be corrupt.", ex);
			}
		}


		/// <summary>
		/// Deletes the last logged move from the game file.
		/// </summary>
		/// <param name="gameId">The ID of the game.</param>
		/// <exception cref="ArgumentException">Thrown if gameId is null or empty.</exception>
		/// <exception cref="FileNotFoundException">Thrown if the game file is not found.</exception>
		/// <exception cref="IOException">Thrown if reading/writing fails due to IO or permission issues.</exception>
		public static void DeleteLastMove(string gameId)
		{
			string filePath = GetGameFilePath(gameId); // Throws if not found or invalid ID

			try
			{
				// Read all lines into a list (necessary for removing an item)
				var lines = File.ReadAllLines(filePath).ToList();

				// Find the index of the last line that matches the move pattern
				int lastMoveIndex = lines.FindLastIndex(line => MoveLineRegex.IsMatch(line.Trim()));

				if (lastMoveIndex != -1)
				{
					// Remove the last move line
					lines.RemoveAt(lastMoveIndex);

					// Overwrite the file with the modified list of lines
					File.WriteAllLines(filePath, lines);
					GD.Print($"Deleted last move from file: {filePath}");
				}
				else
				{
					// Only print if the file existed but contained no moves
					GD.Print($"No moves found in file '{filePath}' to delete.");
				}
			}
			catch (IOException ex)
			{
				throw new IOException($"Failed to read or write file '{filePath}' while deleting last move.", ex);
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new IOException($"Permission denied accessing file '{filePath}' while deleting last move.", ex);
			}
		}
	}
}
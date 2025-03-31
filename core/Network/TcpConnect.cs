using Godot;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test.core.Network
{
	public class TcpConnect
	{
		private string ip;
		private int port;
		private TcpClient client;
		private TcpListener listener;
		private Thread receiveThread;

		public event Action<TcpConnect> OnConnectionEstablished;
		public event Action<string> ReceivedMove;
		public event Action OnTakeBackRequested;
		public event Action OnTakeBackAccepted;
		public event Action OnTakeBackDeclined; // New event for decline


		private volatile bool _stopReceiving = false;

		public TcpConnect(string ip, int port)
		{
			this.ip = ip;
			this.port = port;
		}

		public async Task CreateTcpListener()
		{
			listener = new TcpListener(IPAddress.Any, port);
			listener.Start();
			GD.Print("Waiting for connection ... ");
			try
			{
				client = await listener.AcceptTcpClientAsync();
				GD.Print("Connection done!");
				OnConnectionEstablished?.Invoke(this);
				StartReceiving();
			}
			catch (ObjectDisposedException)
			{
				GD.Print("Listener stopped before connection could be accepted.");
				// Listener was stopped, likely during cleanup, so don't proceed
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Error accepting TCP client: {ex.Message}");
				listener?.Stop(); // Ensure listener is stopped on other errors
			}
		}

		public void ConnectToPeer()
		{
			try
			{
				client = new TcpClient();
				client.Connect(ip, port);
				GD.Print("Connected to peer!");
				StartReceiving();
			}
			catch (SocketException ex)
			{
				GD.PrintErr($"Failed to connect to peer {ip}:{port} - {ex.Message}");
				// Handle connection failure (e.g., notify UI)
				client?.Close();
				client = null;
			}
			catch (Exception ex)
			{
				GD.PrintErr($"An unexpected error occurred during ConnectToPeer: {ex.Message}");
				client?.Close();
				client = null;
			}
		}

		private void StartReceiving()
		{
			// Ensure we don't start multiple threads if called again accidentally
			if (receiveThread == null || !receiveThread.IsAlive)
			{
				_stopReceiving = false; // Reset stop flag
				receiveThread = new Thread(() => ReceiveMoves());
				receiveThread.IsBackground = true; // Allows application to exit even if thread is running
				receiveThread.Start();
			}
			else
			{
				GD.Print("Receive thread is already running.");
			}
		}

		// Helper method to send data safely
		private void SendData(string message)
		{
			if (client == null || !client.Connected)
			{
				GD.Print($"Client is not connected. Cannot send message: {message}");
				return;
			}

			try
			{
				NetworkStream stream = client.GetStream();
				byte[] dataBytes = Encoding.UTF8.GetBytes(message);
				stream.Write(dataBytes, 0, dataBytes.Length);
				// GD.Print($"Sent: {message}"); // Optional: uncomment for verbose logging
			}
			catch (IOException ex) // More specific exception for network stream issues
			{
				GD.PrintErr($"IO Error sending message '{message}': {ex.Message} - Client may have disconnected.");
				// Consider closing the connection here or notifying higher level
				CloseConnection(); // Attempt to clean up on send error
			}
			catch (ObjectDisposedException ex) // Happens if client/stream is closed during write
			{
				GD.PrintErr($"ObjectDisposed Error sending message '{message}': {ex.Message} - Connection likely closed.");
				// Connection is already closing or closed.
			}
			catch (Exception ex)
			{
				GD.PrintErr($"Error sending message '{message}': {ex.Message}");
			}
		}

		public void SendMove(string moveJson)
		{
			SendData(moveJson);
		}

		public void SendTakeBackMove()
		{
			SendData("TAKE_BACK");
			GD.Print("Sent take back request."); // Keep specific log for this action
		}

		public void SendAcceptTakeBack()
		{
			SendData("ACCEPT_TAKE_BACK");
			GD.Print("Sent accept take back response."); // Keep specific log
		}

		public void SendDeclineTakeBack()
		{
			SendData("DECLINE_TAKE_BACK");
			GD.Print("Sent decline take back response."); // Keep specific log
		}

		public void ReceiveMoves()
		{
			GD.Print("ReceiveMoves thread started.");
			byte[] buffer = new byte[1024]; // Reusable buffer

			try
			{
				// Use the flag in the loop condition
				while (!_stopReceiving && client != null && client.Connected) // Check client before accessing stream
				{
					NetworkStream stream = client.GetStream();
					int bytesRead = 0;

					try
					{
						// Check if data is available or if stop is requested
						if (stream.DataAvailable)
						{
							bytesRead = stream.Read(buffer, 0, buffer.Length);
						}
						else if (_stopReceiving)
						{
							GD.Print("Stop receiving requested while no data available.");
							break; // Exit if stop is requested
						}
						else
						{
							// No data and not stopping, sleep briefly
							Thread.Sleep(100); // Sleep for 100ms
							continue; // Skip rest of loop iteration
						}
					}
					catch (IOException ex) when (ex.InnerException is SocketException sockEx && (sockEx.SocketErrorCode == SocketError.Interrupted || sockEx.SocketErrorCode == SocketError.ConnectionReset || sockEx.SocketErrorCode == SocketError.ConnectionAborted))
					{
						GD.Print($"Socket read interrupted or connection reset ({sockEx.SocketErrorCode}). Likely closing.");
						break; // Exit loop if socket is closed or reset
					}
					catch (ObjectDisposedException)
					{
						GD.Print("NetworkStream or TcpClient disposed during read. Exiting ReceiveMoves.");
						break; // Exit loop if objects are disposed
					}


					if (bytesRead == 0 && !_stopReceiving) // Check _stopReceiving again in case it changed during Read
					{
						// If Read returns 0, the remote side has gracefully closed the connection
						GD.Print("Client disconnected gracefully (read 0 bytes).");
						break;
					}

					if (bytesRead > 0)
					{
						string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
						GD.Print($"Received message: {message}");

						// --- Switch statement with breaks ---
						switch (message)
						{
							case "TAKE_BACK":
								GD.Print("Take back request received.");
								// Ensure event handler runs on main thread if it modifies Godot nodes
								Godot.Callable.From(() => OnTakeBackRequested?.Invoke()).CallDeferred();
								break; // <-- Fixed: Added break

							case "ACCEPT_TAKE_BACK":
								GD.Print("Take back accepted by opponent.");
								Godot.Callable.From(() => OnTakeBackAccepted?.Invoke()).CallDeferred();
								break; // <-- Fixed: Added break

							case "DECLINE_TAKE_BACK":
								GD.Print("Take back declined by opponent.");
								Godot.Callable.From(() => OnTakeBackDeclined?.Invoke()).CallDeferred();
								break; // <-- Fixed: Added break

							default:
								// Only execute this if none of the specific cases matched
								GD.Print("Assuming message is a move.");
								// Capture message locally for the lambda/closure
								string moveMessage = message;
								Godot.Callable.From(() => ReceivedMove?.Invoke(moveMessage)).CallDeferred();
								break; // Break for default case
						}
						// --- End switch ---
					}
				} // End while loop
			}
			catch (Exception ex)
			{
				// Catch any other unexpected errors in the loop logic
				if (!_stopReceiving) // Don't log errors if we are intentionally stopping
				{
					GD.PrintErr($"Unexpected error in ReceiveMoves loop: {ex.Message}\n{ex.StackTrace}");
				}
			}
			finally
			{
				// Final cleanup check, though CloseConnection should handle most of this
				if (client != null)
				{
					// client.Close(); // CloseConnection should be called externally or if loop breaks unexpectedly
					// client = null;
				}
				GD.Print("ReceiveMoves thread finished.");
			}
		}


		public void CloseConnection()
		{
			// Prevent multiple cleanup attempts
			if (_stopReceiving && client == null && listener == null)
			{
				GD.Print("CloseConnection already called or resources are null.");
				return;
			}
			GD.Print("Attempting to close TCP connection...");
			_stopReceiving = true; // Signal the receiving thread to stop


			// Close the listener first (if active) to prevent new connections
			if (listener != null)
			{
				try
				{
					listener.Stop();
					GD.Print("Listener stopped.");
				}
				catch (Exception ex)
				{
					GD.PrintErr($"Error stopping listener: {ex.Message}");
				}
				listener = null; // Release listener reference
			}


			// Close the client connection
			if (client != null)
			{
				GD.Print("Closing TCP client...");
				try
				{
					// Closing the client should interrupt blocking operations in ReceiveMoves
					client.Close();
					GD.Print("TCP Client closed.");
				}
				catch (Exception ex)
				{
					GD.PrintErr($"Error closing TCP client: {ex.Message}");
				}
				client = null; // Release client reference
			}


			// Wait briefly for the receive thread to exit
			if (receiveThread != null && receiveThread.IsAlive)
			{
				GD.Print("Waiting for receive thread to terminate...");
				bool finished = receiveThread.Join(TimeSpan.FromSeconds(1)); // Wait up to 1 second
				if (!finished)
				{
					GD.PrintErr("Receive thread did not terminate gracefully in time.");
					// Avoid Abort if possible
				}
				else
				{
					GD.Print("Receive thread terminated.");
				}
			}
			receiveThread = null; // Release thread reference

			// Unsubscribe events to prevent memory leaks
			// Check if delegates are not null before clearing might be safer, but = null works
			OnConnectionEstablished = null;
			ReceivedMove = null;
			OnTakeBackRequested = null;
			OnTakeBackAccepted = null;
			OnTakeBackDeclined = null;

			GD.Print("TCP Connection cleanup complete.");
		}


	}
}
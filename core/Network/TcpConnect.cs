using Godot;
using System;
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
		private Thread receiveThread; // Keep the thread alive

		public event Action<TcpConnect> OnConnectionEstablished;
		public event Action<string> ReceivedMove;

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
			client = await listener.AcceptTcpClientAsync();
			GD.Print("Connection done!");
			OnConnectionEstablished?.Invoke(this);

			StartReceiving(); // Use a method to start receiving
		}

		public void ConnectToPeer()
		{
			client = new TcpClient();
			client.Connect(ip, port);
			Console.WriteLine("Connected to peer!");

			StartReceiving(); // Use a method to start receiving
		}

		private void StartReceiving()
		{
			receiveThread = new Thread(() => ReceiveMoves());
			receiveThread.IsBackground = true; // Important: Allow the application to exit even if the thread is running
			receiveThread.Start();
		}

		public void SendMove(string move)
		{
			if (client != null && client.Connected) // Check if connected before sending
			{
				try
				{
					NetworkStream stream = client.GetStream();
					byte[] moveBytes = Encoding.UTF8.GetBytes(move);
					stream.Write(moveBytes, 0, moveBytes.Length);
				}
				catch (Exception ex)
				{
					GD.Print($"Error sending move: {ex.Message}");
					// Handle disconnection appropriately, e.g., raise an event.
				}
			}
			else
			{
				GD.Print("Client is not connected. Cannot send move.");
			}
		}


		public void ReceiveMoves()
		{
			try
			{
				while (client != null && client.Connected) // Loop to continuously receive
				{
					NetworkStream stream = client.GetStream();
					byte[] buffer = new byte[1024];

					int bytesRead = stream.Read(buffer, 0, buffer.Length);

					if (bytesRead == 0)
					{
						GD.Print("Client disconnected.");
						// Handle disconnection (e.g., raise an event, close the socket)
						break; // Exit the loop
					}

					string move = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					GD.Print($"Received move: {move}");
					ReceivedMove?.Invoke(move);
				}
			}
			catch (Exception ex)
			{
				GD.Print($"Connection error: {ex.Message}");
				// Handle disconnection appropriately
			}
			finally
			{
				// Clean up resources when the loop exits (due to disconnection or error)
				client?.Close();
				client = null; // Important: Set client to null to prevent further use
							   // Optionally, raise a disconnect event here
			}
		}
	}
}
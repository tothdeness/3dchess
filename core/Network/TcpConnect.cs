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
		private Thread receiveThread;

		public event Action<TcpConnect> OnConnectionEstablished;
		public event Action<string> ReceivedMove;
		public event Action OnTakeBackRequested;
		public event Action OnTakeBackAccepted;
		public event Action OnTakeBackDeclined; // New event for decline

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
			StartReceiving();
		}

		public void ConnectToPeer()
		{
			client = new TcpClient();
			client.Connect(ip, port);
			GD.Print("Connected to peer!");
			StartReceiving();
		}

		private void StartReceiving()
		{
			receiveThread = new Thread(() => ReceiveMoves());
			receiveThread.IsBackground = true;
			receiveThread.Start();
		}

		public void SendMove(string move)
		{
			if (client != null && client.Connected)
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
				}
			}
			else
			{
				GD.Print("Client is not connected. Cannot send move.");
			}
		}

		public void SendTakeBackMove()
		{
			if (client != null && client.Connected)
			{
				try
				{
					NetworkStream stream = client.GetStream();
					string takeBackMessage = "TAKE_BACK";
					byte[] takeBackBytes = Encoding.UTF8.GetBytes(takeBackMessage);
					stream.Write(takeBackBytes, 0, takeBackBytes.Length);
					GD.Print("Sent take back request.");
				}
				catch (Exception ex)
				{
					GD.Print($"Error sending take back request: {ex.Message}");
				}
			}
			else
			{
				GD.Print("Client is not connected. Cannot send take back request.");
			}
		}

		public void SendAcceptTakeBack()
		{
			if (client != null && client.Connected)
			{
				try
				{
					NetworkStream stream = client.GetStream();
					string acceptMessage = "ACCEPT_TAKE_BACK";
					byte[] acceptBytes = Encoding.UTF8.GetBytes(acceptMessage);
					stream.Write(acceptBytes, 0, acceptBytes.Length);
					GD.Print("Sent accept take back response.");
				}
				catch (Exception ex)
				{
					GD.Print($"Error sending accept take back: {ex.Message}");
				}
			}
			else
			{
				GD.Print("Client is not connected. Cannot send accept take back.");
			}
		}

		// New method to send decline of take back
		public void SendDeclineTakeBack()
		{
			if (client != null && client.Connected)
			{
				try
				{
					NetworkStream stream = client.GetStream();
					string declineMessage = "DECLINE_TAKE_BACK";
					byte[] declineBytes = Encoding.UTF8.GetBytes(declineMessage);
					stream.Write(declineBytes, 0, declineBytes.Length);
					GD.Print("Sent decline take back response.");
				}
				catch (Exception ex)
				{
					GD.Print($"Error sending decline take back: {ex.Message}");
				}
			}
			else
			{
				GD.Print("Client is not connected. Cannot send decline take back.");
			}
		}

		public void ReceiveMoves()
		{
			try
			{
				while (client != null && client.Connected)
				{
					NetworkStream stream = client.GetStream();
					byte[] buffer = new byte[1024];
					int bytesRead = stream.Read(buffer, 0, buffer.Length);

					if (bytesRead == 0)
					{
						GD.Print("Client disconnected.");
						break;
					}

					string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					GD.Print($"Received message: {message}");

					switch (message)
					{
						case "TAKE_BACK":
							GD.Print("Take back request received.");
							OnTakeBackRequested?.Invoke();
							break;
						case "ACCEPT_TAKE_BACK":
							GD.Print("Take back accepted by opponent.");
							OnTakeBackAccepted?.Invoke();
							break;
						case "DECLINE_TAKE_BACK":
							GD.Print("Take back declined by opponent.");
							OnTakeBackDeclined?.Invoke();
							break;
						default:
							GD.Print("Assuming message is a move.");
							ReceivedMove?.Invoke(message);
							break;
					}
				}
			}
			catch (Exception ex)
			{
				GD.Print($"Connection error: {ex.Message}");
			}
			finally
			{
				client?.Close();
				client = null;
			}
		}
	}
}
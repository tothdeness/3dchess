using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public event Action<TcpConnect> OnConnectionEstablished;

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

			GetMoves();
		}


		public async void  ConnectToPeer()
		{
			client = new TcpClient();
			client.Connect(ip, port);
			Console.WriteLine("Connected to peer!");

			GetMoves();
		}

		private void GetMoves()
		{
			Thread receiveThread = new Thread(() => ReceiveMoves());
			receiveThread.Start();
		}


		public void SendMove(string move)
		{
			NetworkStream stream = client.GetStream();
			byte[] moveBytes = Encoding.UTF8.GetBytes(move);
			stream.Write(moveBytes, 0, moveBytes.Length);
		}

		public void ReceiveMoves()
		{
			NetworkStream stream = client.GetStream();
			byte[] buffer = new byte[1024];

			try
			{
				while (true)
				{
					int bytesRead = stream.Read(buffer, 0, buffer.Length);
					if (bytesRead == 0)
						break; // Connection closed

					string move = Encoding.UTF8.GetString(buffer, 0, bytesRead);
					GD.Print($"Received move: {move}");
				}
			}
			catch (Exception ex)
			{
				GD.Print($"Connection error: {ex.Message}");
			}

		}




	}
}

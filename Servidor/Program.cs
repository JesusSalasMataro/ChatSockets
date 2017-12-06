using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Servidor
{

    class Program
    {
        static void Main(string[] args)
        {
            Server servidor = new Server();
            servidor.Run();
            Console.ReadKey();
        }
    }


    class Server
    {
        private const int MAX_CLIENTES = 1000;
        private const int PUERTO = 55555;

        private TcpListener listener;
        private TcpClient[] tcpClientes;
        private NetworkStream[] streams;
        private BinaryReader[] r;
        private BinaryWriter[] w;
        private bool[] ClientesConectados;
        private int iNumClientes;

        public void Run()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, PUERTO);
                listener.Start();

                tcpClientes = new TcpClient[MAX_CLIENTES];
                streams = new NetworkStream[MAX_CLIENTES];
                r = new BinaryReader[MAX_CLIENTES];
                w = new BinaryWriter[MAX_CLIENTES];
                ClientesConectados = new bool[MAX_CLIENTES];
                iNumClientes = 0;
                Console.WriteLine("Servidor iniciado.");

                while (true)
                { 
                    tcpClientes[iNumClientes] = listener.AcceptTcpClient();
                    Console.WriteLine("Cliente " + (iNumClientes + 1).ToString() + " conectado.");

                    streams[iNumClientes] = tcpClientes[iNumClientes].GetStream();
                    r[iNumClientes] = new BinaryReader(streams[iNumClientes]);
                    w[iNumClientes] = new BinaryWriter(streams[iNumClientes]);
                    ClientesConectados[iNumClientes] = true;
                    
                    startClientAsync(iNumClientes);
                    iNumClientes++;
                }
                listener.Stop();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async void startClientAsync( int iNumC )
        {
            int i;
            string mensaje = "";
            bool bDesconectar = false;
            do
            {
                try
                {
                    mensaje = r[iNumC].ReadString();
                }
                catch (EndOfStreamException ex)
                {
                    Console.WriteLine("Cliente " + (iNumC + 1).ToString() + " desconectado.");
                    ClientesConectados[iNumC] = false;
                    mensaje = "";
                    bDesconectar = true;
                }

                if (!mensaje.Equals(""))
                {
                    Console.WriteLine(mensaje);
                    for (i = 0; i < iNumClientes; i++)
                    {
                        if (ClientesConectados[i])
                        {
                            w[i].Write(mensaje);
                            w[i].Flush();
                        }
                    }
                }
                await Task.Delay(10);
            } while (!bDesconectar);
        }

    }

}

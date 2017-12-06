using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace Cliente
{
    public partial class ClienteTcpIp : Form
    {
        private bool bEnviar, bDesconectar;
        private string sRespuesta = "";
        BinaryReader r;
        NetworkStream stream;
        TcpClient cliente;

        public ClienteTcpIp()
        {
            InitializeComponent();
            txbNombre.Select();
        }

        public async void RunAsync()
        {
            
            try
            {
                cliente = new TcpClient("localhost", 55555);

                stream = cliente.GetStream();
                r = new BinaryReader(stream);
                BinaryWriter w = new BinaryWriter(stream);
                w.Write(""); 
                w.Flush();
             
                bEnviar = false;
                bDesconectar = false;
                do
                {
                    if (bEnviar)
                    {
                        w.Write(txbNombre.Text + ": " + txbEntrada.Text);
                        w.Flush();
                        sRespuesta = r.ReadString();
                        lsbChat.Items.Add(sRespuesta);
                        bEnviar = false;
                        txbEntrada.Text = "";
                    }

                    LeerMensaje();
                    await Task.Delay(100);

                } while (!bDesconectar);
            }
            catch (SocketException e) { }
        }


        private void LeerMensaje()
        {
            if( stream.DataAvailable ) {
                sRespuesta = r.ReadString();
                lsbChat.Items.Add(sRespuesta);
            }
            sRespuesta = "";

        }


        private void btnEnviar_Click(object sender, EventArgs e)
        {
            bEnviar = true;  
        }


        private void btnConectar_Click(object sender, EventArgs e)
        {
            btnEnviar.Enabled = true;
            txbEntrada.Enabled = true;
            btnConectar.Enabled = false;
            txbNombre.Enabled = false;
            btnConectar.Visible = false;
            btnDesconectar.Visible = true;

            if(txbNombre.Text.Equals("")) txbNombre.Text = "Anónimo";
            RunAsync();
        }


        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            btnEnviar.Enabled = false;
            txbEntrada.Enabled = false;
            btnConectar.Enabled = true;
            txbNombre.Enabled = true;
            btnConectar.Visible = true;
            btnDesconectar.Visible = false;

            bDesconectar = true;
            Task.Delay(10);
            cliente.Close();
        }


        private void ClienteTcpIp_FormClosed(object sender, FormClosedEventArgs e)
        {
            bDesconectar = true;
            Task.Delay(10);
            cliente.Close();
        }

    }

}


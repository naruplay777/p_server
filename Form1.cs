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
using System.Threading;
using System.IO;

namespace p_server
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private Thread serverThread;
        private bool isServerRunning = false;
        private List<TcpClient> connectedClients = new List<TcpClient>();
        private Queue<string> requestQueue = new Queue<string>();
        private object lockObject = new object();

        public Form1()
        {
            InitializeComponent();
        }

        private void StartServer()
        {
            try
            {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(ip, 5000);
                server.Start();

                while (isServerRunning)
                {
                    TcpClient client = server.AcceptTcpClient();
                    connectedClients.Add(client);
                    UpdateClientList(client.Client.RemoteEndPoint.ToString());

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"Error: {ex.Message}");
            }
        }

        private void StopServer()
        {
            isServerRunning = false;

            // Guardar el log antes de detener el servidor
            SaveLogToFile();

            server.Stop();
            foreach (TcpClient client in connectedClients)
            {
                client.Close();
            }
            connectedClients.Clear();
            UpdateLog("Servidor detenido.");
        }

        private void SaveLogToFile()
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(logDirectory); // Crear la carpeta si no existe

                string logFileName = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.log";
                string logFilePath = Path.Combine(logDirectory, logFileName);

                File.WriteAllText(logFilePath, txtLog.Text); // Guardar el contenido del txtLog en el archivo
                UpdateLog($"Log guardado en: {logFilePath}");
            }
            catch (Exception ex)
            {
                UpdateLog($"Error al guardar el log: {ex.Message}");
            }
        }


        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Agregar solicitud a la cola
                    lock (lockObject)
                    {
                        requestQueue.Enqueue(request);
                        UpdateLog($"Solicitud recibida de {client.Client.RemoteEndPoint}");
                    }

                    // Simular procesamiento y enviar respuesta
                    string response = "SOLICITUD_PROCESADA";
                    byte[] responseData = Encoding.ASCII.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                // Verificar si el cliente se desconectó
                if (ex is IOException || ex is SocketException)
                {
                    UpdateLog($"Cliente desconectado: {client.Client.RemoteEndPoint}");
                }
                else
                {
                    UpdateLog($"Error: {ex.Message}");
                }

                connectedClients.Remove(client);
                client.Close();
            }
        }

        private void timerQuantum_Tick(object sender, EventArgs e)
        {
            if (requestQueue.Count > 0)
            {
                lock (lockObject)
                {
                    string request = requestQueue.Dequeue();
                    ProcessRequest(request);
                }
            }
        }

        private void ProcessRequest(string request)
        {
            // Ejemplo: Analizar el archivo de texto recibido
            string[] lines = request.Split('\n');
            string resourceType = lines[0]; // Primera línea: tipo de recurso
            int linesToPrint = int.Parse(lines[1]); // Segunda línea: cantidad de líneas

            // Simular impresión (mostrar en RichTextBox)
            Invoke((MethodInvoker)delegate {
                txtLog.AppendText($"Imprimiendo {linesToPrint} líneas...\n");
            });

            // Registrar en archivo log
            LogToFile($"Operación procesada: {linesToPrint} líneas");
        }



        private void LogToFile(string message)
        {
            string logPath = "servidor_log.txt";
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n";

            File.AppendAllText(logPath, logEntry);
            UpdateLog($"Registro guardado: {message}");
        }

        private void UpdateLog(string message)
        {
            if (txtLog.InvokeRequired)
            {
                Invoke(new Action<string>(UpdateLog), message);
            }
            else
            {
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss} - {message}\n");
            }
        }


        private void UpdateClientList(string clientInfo)
        {
            if (listClients.InvokeRequired)
            {
                Invoke(new Action<string>(UpdateClientList), clientInfo);
            }
            else
            {
                listClients.Items.Add(clientInfo);
            }
        }


        private void btnStartServer_Click(object sender, EventArgs e)
        {
           
            if (!isServerRunning) 
            {
                serverThread = new Thread(new ThreadStart(StartServer));
                serverThread.IsBackground = true;
                serverThread.Start();
                btnStartServer.BackColor = Color.DarkSlateGray;
                btnStartServer.Text = "Detener Servidor";
                label1.Text = "Detener Servidor";
                isServerRunning = true;
                UpdateLog("Servidor iniciado en 127.0.0.1:5000");
            }
            else
            {
                StopServer();
                btnStartServer.BackColor = Color.Orange;
                btnStartServer.Text = "Iniciar Servidor";
                label1.Text = "Iniciar Servidor";
                isServerRunning = false;
            

            }
        }


              private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listClients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }
    }
}

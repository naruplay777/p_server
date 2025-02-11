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
                isServerRunning = true;

                while (isServerRunning)
                {
                    if (server.Pending()) // Solo aceptar si hay clientes esperando
                    {
                        TcpClient client = server.AcceptTcpClient();
                        string clientInfo = client.Client.RemoteEndPoint.ToString();

                        lock (lockObject)
                        {
                            connectedClients.Add(client);
                            UpdateClientList(clientInfo);
                        }

                        UpdateLog($"Cliente conectado: {client.Client.RemoteEndPoint}");


                        Thread clientThread = new Thread(() => HandleClient(client));
                        clientThread.Start();
                    }
                    else
                    {
                        Thread.Sleep(100); // Pequeña pausa para evitar alto consumo de CPU
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"Error: {ex.Message}");
            }
        }

        private void ClearClientList()
        {
            // Aseguramos que la actualización de la interfaz sea en el hilo principal.
            Invoke((MethodInvoker)delegate
            {
                listClients.Items.Clear(); // Limpiamos todos los clientes de la lista.
            });
        }

        private void StopServer()
        {
            isServerRunning = false;

            // Detener el servidor primero para evitar nuevas conexiones
            if (server != null)
            {
                server.Stop();
            }

            // Cerrar todos los clientes de forma segura
            lock (lockObject)
            {
                foreach (TcpClient client in connectedClients.ToList()) // Usar ToList para evitar modificaciones durante la iteración
                {
                    try
                    {
                        if (client.Connected)
                        {
                            CloseClientSafely(client);
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateLog($"Error al cerrar cliente: {ex.Message}");
                    }
                }
                connectedClients.Clear();
            }

            // Limpiar la lista de clientes conectados en la interfaz de usuario
            Invoke((MethodInvoker)delegate
            {
                listClients.Items.Clear(); // Limpiar todos los elementos de la lista en la interfaz
            });

            // Esperar a que el hilo del servidor termine
            if (serverThread != null && serverThread.IsAlive)
            {
                serverThread.Join(); // Esperar a que el hilo termine
            }

            SaveLogToFile();
            UpdateLog("Servidor detenido.");
        }


        private string GetLogDirectory()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDirectory); // Crea la carpeta si no existe
            return logDirectory;
        }


        private void SaveLogToFile()
        {
            try
            {
                string logDirectory = GetLogDirectory();
                // Crear la carpeta si no existe

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
            string clientInfo = client.Client.RemoteEndPoint.ToString();
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while (isServerRunning && (bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    lock (lockObject)
                    {
                        requestQueue.Enqueue(request);
                        UpdateLog($"Solicitud recibida de {clientInfo}");
                    }

                    // Enviar respuesta
                    string response = "SOLICITUD_PROCESADA";
                    byte[] responseData = Encoding.ASCII.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                }
            }
            catch (Exception ex)
            {
                if (ex is IOException || ex is SocketException)
                {
                    UpdateLog($"Cliente desconectado: {clientInfo}");
                }
                else
                {
                    UpdateLog($"Error: {ex.Message}");
                }
            }
            finally
            {
                lock (lockObject)
                {
                    if (connectedClients.Contains(client))
                    {
                        connectedClients.Remove(client);
                    }
                }
                CloseClientSafely(client);
            }
        }

        private void CloseClientSafely(TcpClient client)
        {
            try
            {
                if (client.Connected)
                {
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                UpdateLog($"Error al cerrar cliente: {ex.Message}");
            }
            finally
            {
                RemoveClient(client);
            }
        }


        private void timerQuantum_Tick(object sender, EventArgs e)
        {
            if (requestQueue.Count > 0)
            {
                lock (lockObject)
                {
                    string request = SafeDequeueRequest();
                    if (request != null)
                    {
                        ProcessRequest(request);
                    }
                    ProcessRequest(request);
                }
            }
        }

        private string SafeDequeueRequest()
        {
            lock (lockObject)
            {
                if (requestQueue.Count > 0)
                {
                    return requestQueue.Dequeue();
                }
            }
            return null;
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

        private void RemoveClient(TcpClient client)
        {
            lock (lockObject)
            {
                if (connectedClients.Contains(client))
                {
                    connectedClients.Remove(client);
                }
            }

            // Intentamos verificar la conexión de manera segura
            try
            {
                if (client.Connected)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        // Solo intentamos acceder a RemoteEndPoint si el Socket no ha sido cerrado
                        if (client.Client != null && client.Client.Connected)
                        {
                            string clientInfo = client.Client.RemoteEndPoint.ToString();
                            if (listClients.Items.Contains(clientInfo))
                            {
                                listClients.Items.Remove(clientInfo);
                            }
                        }
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                // Manejo de la excepción si el objeto ha sido desechado
                Console.WriteLine("El cliente ya ha sido desconectado y su socket ha sido desechado.");
            }
        }





        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if (!isServerRunning)
            {
                try
                {
                    serverThread = new Thread(new ThreadStart(StartServer));
                    serverThread.IsBackground = true;
                    serverThread.Start();
                    btnStartServer.Text = "Detener Servidor";
                    label5.BackColor = Color.YellowGreen;
                    label5.ForeColor = Color.Black;
                    label5.Text = "Activo";

                    isServerRunning = true;
                    UpdateLog("Servidor iniciado en 127.0.0.1:5000");
                }
                catch (Exception ex)
                {
                    UpdateLog($"Error al iniciar el servidor: {ex.Message}");
                }
            }
            else
            {
                label5.Text = "Inactivo";
                label5.ForeColor = Color.Gainsboro;
                label5.BackColor = Color.Red;
                StopServer();
                btnStartServer.Text = "Iniciar Servidor";
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

        private void btnOpenLogs_Click(object sender, EventArgs e)
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

                if (Directory.Exists(logDirectory))
                {
                    System.Diagnostics.Process.Start("explorer.exe", logDirectory);
                }
                else
                {
                    MessageBox.Show("La carpeta de logs no existe aún.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir la carpeta: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

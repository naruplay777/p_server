﻿using System;
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
        private object lockObject = new object();
        private Dictionary<TcpClient, string> clientIPs = new Dictionary<TcpClient, string>();
        private bool isProcessingRequests;
        private bool hasProcessingStarted = false;
        private int deadlockCycleCount = 0; // Contador de ciclos de interbloqueo

        private int availablePaper = 2;
        private int availablePrinter = 1;

        private Queue<Request> requestQueue = new Queue<Request>();
        private Queue<Request> executionQueue = new Queue<Request>();
        private Queue<Request> readyQueue = new Queue<Request>();

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

                serverThread = new Thread(new ThreadStart(ServerLoop));
                serverThread.IsBackground = true;
                serverThread.Start();

                UpdateLog("Servidor iniciado en 127.0.0.1:5000");
            }
            catch (Exception ex)
            {
                UpdateLog($"Error: {ex.Message}");
            }
        }

        private void ServerLoop()
        {
            while (isServerRunning)
            {
                if (server.Pending()) // Solo aceptar si hay clientes esperando
                {
                    TcpClient client = server.AcceptTcpClient();
                    AddClient(client); // Llamamos la nueva función

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
            NetworkStream stream = null;

            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                while (isServerRunning && (bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string content = Encoding.UTF8.GetString(buffer, 0, bytesRead); // Cambiado a UTF8
                    var request = new Request
                    {
                        ClientInfo = clientInfo,
                        Content = content,
                        HasPaper = false,
                        HasPrinter = false,
                        QueueNumber = requestQueue.Count + 1, // Asignar número de cola
                        StartTime = DateTime.Now, // Registrar el tiempo de inicio
                        Status = "Listo" // Estado inicial
                    };

                    lock (lockObject)
                    {
                        requestQueue.Enqueue(request);
                        UpdateLog($"Solicitud recibida de {clientInfo} con número de cola {request.QueueNumber}");
                        UpdateRequestStatus(request);
                    }

                    // Enviar respuesta inicial
                    string response = $"SOLICITUD_RECIBIDA|{request.QueueNumber}";
                    byte[] responseData = Encoding.UTF8.GetBytes(response); // Cambiado a UTF8
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
                // Aseguramos que la conexión y el stream se cierren correctamente
                if (stream != null)
                {
                    try
                    {
                        stream.Close(); // Cerrar el NetworkStream
                    }
                    catch (Exception ex)
                    {
                        UpdateLog($"Error al cerrar NetworkStream: {ex.Message}");
                    }
                }

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
            if (client == null)
                return;

            try
            {
                if (client.Connected)
                {
                    client.GetStream().Close();
                }
                client.Close();
            }
            catch (Exception ex)
            {
                UpdateLog($"Error cerrando cliente: {ex.Message}");
            }
            finally
            {
                RemoveClient(client);
            }
        }

        private bool DetectDeadlock()
        {
            if (!isServerRunning)
            {
                return false;
            }

            lock (lockObject)
            {
                bool deadlockDetected = true;

                foreach (var request in requestQueue)
                {
                    if (!request.HasPaper || !request.HasPrinter)
                    {
                        deadlockDetected = false;
                        break;
                    }
                }

                if (deadlockDetected && requestQueue.Count >= 3)
                {
                    UpdateLog("Interbloqueo detectado.");
                    return true;
                }

                return false;
            }
        }
        private void ResolveDeadlock()
        {
            lock (lockObject)
            {
                UpdateLog("Resolviendo interbloqueo...");

                // Expropiar recursos de todos los procesos bloqueados
                foreach (var request in requestQueue)
                {
                    if (request.Status == "Bloqueado")
                    {
                        if (request.HasPaper)
                        {
                            request.HasPaper = false;
                            availablePaper++;
                            UpdateLog($"Recurso papel expropiado de la solicitud de cola {request.QueueNumber}. Hojas disponibles: {availablePaper}");
                        }

                        if (request.HasPrinter)
                        {
                            request.HasPrinter = false;
                            availablePrinter++;
                            UpdateLog($"Recurso impresora expropiado de la solicitud de cola {request.QueueNumber}. Impresoras disponibles: {availablePrinter}");
                        }
                    }
                }

                // Intentar reasignar recursos a todos los procesos bloqueados
                foreach (var request in requestQueue.ToList())
                {
                    if (request.Status == "Bloqueado")
                    {
                        if (!request.HasPaper && availablePaper > 0)
                        {
                            request.HasPaper = true;
                            availablePaper--;
                            UpdateLog($"Recurso papel asignado a la solicitud de cola {request.QueueNumber}. Hojas disponibles: {availablePaper}");
                        }

                        if (!request.HasPrinter && availablePrinter > 0)
                        {
                            request.HasPrinter = true;
                            availablePrinter--;
                            UpdateLog($"Recurso impresora asignado a la solicitud de cola {request.QueueNumber}. Impresoras disponibles: {availablePrinter}");
                        }

                        if (request.HasPaper && request.HasPrinter)
                        {
                            request.Status = "Listo";
                            readyQueue.Enqueue(request);
                            requestQueue = new Queue<Request>(requestQueue.Where(r => r.QueueNumber != request.QueueNumber));
                            UpdateRequestStatus(request);
                        }
                        else
                        {
                            request.Status = "Bloqueado";
                            UpdateRequestStatus(request);
                        }
                    }
                }

                // Reiniciar la ejecución
                isProcessingRequests = true;
            }
        }
        private void timerQuantum_Tick(object sender, EventArgs e)
        {
            if (!isServerRunning || !isProcessingRequests)
            {
                return;
            }

            lock (lockObject)
            {
                if (!hasProcessingStarted && requestQueue.Count < 3)
                {
                    // No comenzar el procesamiento hasta que haya al menos 3 solicitudes en la cola
                    return;
                }

                hasProcessingStarted = true; // Indicar que el procesamiento ha comenzado

                if (DetectDeadlock())
                {
                    // Si se detecta un interbloqueo, continuar la ejecución pero registrar el interbloqueo
                    return;
                }

                if (executionQueue.Count > 0)
                {
                    Request request = executionQueue.Dequeue();
                    if (request != null)
                    {
                        ProcessExecution(request);
                    }
                }
                else if (readyQueue.Count > 0) // Verificar si hay solicitudes en la cola de listo
                {
                    Request request = readyQueue.Dequeue();
                    if (request != null)
                    {
                        executionQueue.Enqueue(request);
                        ProcessRequest(request);
                    }
                }
                else if (requestQueue.Count > 0) // Verificar si hay solicitudes en la cola de espera
                {
                    Request request = SafeDequeueRequest();
                    if (request != null)
                    {
                        executionQueue.Enqueue(request);
                        ProcessRequest(request);
                    }
                }
            }
        }

        private Request SafeDequeueRequest()
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

        private void ProcessRequest(Request request)
        {
            // Ejemplo: Analizar el archivo de texto recibido
            string[] lines = request.Content.Split('\n');
            if (lines.Length == 0) return; // Verificar que haya al menos una línea

            string[] header = lines[0].Split('|');
            if (header.Length < 2) return; // Verificar que haya al menos dos elementos en el encabezado

            string resourceType = header[0]; // Primera línea: tipo de recurso
            if (!int.TryParse(header[1], out int linesToPrint)) return; // Segunda línea: cantidad de líneas

            // Simular impresión (mostrar en RichTextBox)
            Invoke((MethodInvoker)delegate
            {
                txtLog.AppendText($"Solicitud de impresión recibida. Líneas a imprimir: {linesToPrint}\n");
            });

            // Registrar en archivo log
            LogToFile($"Solicitud de impresión recibida. Líneas a imprimir: {linesToPrint}");

            // Agregar mensaje de log adicional
            UpdateLog($"Procesando solicitud de cola {request.QueueNumber}: {header[0]}|{header[1]}");

            // Verificar si hay al menos 3 solicitudes en la cola antes de comenzar el procesamiento
            lock (lockObject)
            {
                if (requestQueue.Count >= 3 || hasProcessingStarted)
                {
                    executionQueue.Enqueue(request);
                }
                else
                {
                    UpdateLog($"No hay suficientes solicitudes en la cola para procesar la solicitud de cola {request.QueueNumber}.");
                }
            }
        }

        private void ProcessExecution(Request request)
        {
            // Simular la ejecución de una acción por quantum
            string[] lines = request.Content.Split('\n');
            if (lines.Length == 0) return; // Verificar que haya al menos una línea

            string[] header = lines[0].Split('|');
            if (header.Length < 2) return; // Verificar que haya al menos dos elementos en el encabezado

            string resourceType = header[0]; // Primera línea: tipo de recurso
            if (!int.TryParse(header[1], out int linesToPrint)) return; // Segunda línea: cantidad de líneas

            // Agregar mensaje de log adicional
            UpdateLog($"Ejecutando solicitud de cola {request.QueueNumber}: {header[0]}|{header[1]}");
            request.Status = "En ejecución";
            UpdateRequestStatus(request);

            // Asignar recursos
            if (!request.HasPaper && availablePaper > 0)
            {
                request.HasPaper = true;
                availablePaper--;
                UpdateLog($"Recurso papel asignado a la solicitud de cola {request.QueueNumber}. Hojas disponibles: {availablePaper}");
            }
            else if (!request.HasPrinter && availablePrinter > 0)
            {
                request.HasPrinter = true;
                availablePrinter--;
                UpdateLog($"Recurso impresora asignado a la solicitud de cola {request.QueueNumber}. Impresoras disponibles: {availablePrinter}");
            }
            else if (request.HasPaper && request.HasPrinter)
            {
                // Realizar una sola operación con el recurso asignado
                if (linesToPrint > 0 && lines.Length >= linesToPrint)
                {
                    string lineToPrint = lines[lines.Length - linesToPrint];
                    Invoke((MethodInvoker)delegate
                    {
                        txtLog.AppendText($"{lineToPrint}\n");
                    });
                    linesToPrint--;

                    // Actualizar el contenido de la solicitud
                    request.Content = $"{header[0]}|{linesToPrint}\n" + string.Join("\n", lines.Skip(1));

                    // Registrar en archivo log
                    LogToFile($"Ejecutando solicitud de cola {request.QueueNumber}: {header[0]}|{linesToPrint}");

                    // Volver a encolar la solicitud si aún tiene líneas por imprimir
                    if (linesToPrint > 0)
                    {
                        request.Status = "Bloqueado";
                        executionQueue.Enqueue(request);
                    }
                    else
                    {
                        // Liberar los recursos
                        request.HasPaper = false;
                        request.HasPrinter = false;
                        availablePaper++;
                        availablePrinter++;
                        UpdateLog($"Recurso papel liberado por la solicitud de cola {request.QueueNumber}. Hojas disponibles: {availablePaper}");
                        UpdateLog($"Recurso impresora liberado por la solicitud de cola {request.QueueNumber}. Impresoras disponibles: {availablePrinter}");

                        // Mover a la cola de listo
                        readyQueue.Enqueue(request);

                        // Calcular el tiempo de procesamiento en quantums
                        TimeSpan duration = DateTime.Now - request.StartTime;
                        int quantums = (int)duration.TotalSeconds;

                        // Enviar respuesta final al cliente con el contenido del archivo
                        string response = $"SOLICITUD_TERMINADA|{request.QueueNumber}|{quantums}|{request.OperationsProcessed}|{request.QueueNumber}|RoundRobin|{lineToPrint}";
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        // Aquí se debe usar el stream del cliente para enviar la respuesta
                        TcpClient client = connectedClients.FirstOrDefault(c => clientIPs[c] == request.ClientInfo);
                        if (client != null)
                        {
                            client.GetStream().Write(responseData, 0, responseData.Length);
                        }

                        // No mostrar la solicitud si terminó su ejecución
                        request.Status = "Terminado";
                    }
                }
            }
            else
            {
                // No se pudo asignar el recurso, volver a la cola de espera
                requestQueue.Enqueue(request);
                UpdateLog($"Recurso no disponible para la solicitud de cola {request.QueueNumber}. Solicitud devuelta a la cola de espera.");
                request.Status = "Bloqueado";
            }

            UpdateRequestStatus(request);
        }

        private void UpdateRequestStatus(Request request)
        {
            Invoke((MethodInvoker)delegate
            {
                var lines = richTextBox1.Lines.ToList();
                var existingLineIndex = lines.FindIndex(line => line.StartsWith($"Número de Cola: {request.QueueNumber}"));

                if (existingLineIndex >= 0)
                {
                    lines[existingLineIndex] = $"Número de Cola: {request.QueueNumber}, Cliente: {request.ClientInfo}, Estado: {request.Status}";
                }
                else
                {
                    lines.Add($"Número de Cola: {request.QueueNumber}, Cliente: {request.ClientInfo}, Estado: {request.Status}");
                }

                // No mostrar la solicitud si terminó su ejecución
                if (request.Status == "Terminado")
                {
                    lines.RemoveAt(existingLineIndex);
                }

                richTextBox1.Lines = lines.ToArray();
            });
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
            if (client == null)
                return;

            string clientInfo = "";

            lock (lockObject)
            {
                if (clientIPs.ContainsKey(client))
                {
                    clientInfo = clientIPs[client]; // Recuperamos la IP guardada
                    clientIPs.Remove(client); // Eliminamos del diccionario
                }
                connectedClients.Remove(client);
            }

            // Verificar si la IP existe en el ListBox antes de eliminar
            Invoke((MethodInvoker)delegate
            {
                if (listClients.Items.Contains(clientInfo))
                {
                    listClients.Items.Remove(clientInfo);
                    UpdateLog($"Cliente eliminado correctamente: {clientInfo}");
                }
                else
                {
                    UpdateLog($"Error: No se encontró {clientInfo} en listClients.");
                }
            });
        }
        private void AddClient(TcpClient client)
        {
            lock (lockObject)
            {
                connectedClients.Add(client);
                clientIPs[client] = client.Client.RemoteEndPoint.ToString();
            }
            UpdateClientList(client.Client.RemoteEndPoint.ToString());
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

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            ResolveDeadlock();
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

        private void btnEjecutar_Click(object sender, EventArgs e)
        {
            isProcessingRequests = true; // Iniciar el procesamiento de solicitudes
            UpdateLog("Procesamiento de solicitudes iniciado.");
        }

    }
}
       
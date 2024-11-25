using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using System.Data.SqlClient;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Threading;
using NLog;
using System.Linq.Expressions;
using System.Reflection;

namespace Socket_.Clases
{
    internal class Funciones_
    {


        //Raiz proyecyo
        string RaizProyecto = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        //Carpeta del ProgramData
        string programdata = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        //Aqui manejaremos los logs
        public static readonly Logger logger = LogManager.GetCurrentClassLogger();
        //Ruta para almacenar el TXT
        String RutaARchivo = @"C:\ProgramData\Homini\ValidaCedula\Socket\error.log";

        //Ruta para modificar o monitoriar
        //Funcion Para Crear Las rutas necesarias que se van a utilizar
        SqlConnection Conexion = new SqlConnection();


        public void LoggerError(string mesajeError)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(RutaARchivo, true))
                {

                    writer.WriteLine($"Fecha: {DateTime.Now.ToString("dd/MM/yyyy - HH:mm:ss " + "\r")}{mesajeError} \r");
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerError($"Error ubicado " + ex.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + ex.Message.ToString() + "\n");
            }
        }
        public void CrearRutasNecesarias()
        {
            try
            {
                string[] carpetas = {
            "\\Homini\\ValidaCedula\\Socket",
            "\\Homini\\ValidaCedula\\Socket\\Execution",
            "\\Homini\\ValidaCedula\\Socket\\Input",
            "\\Homini\\ValidaCedula\\Socket\\Output",
            "\\Homini\\ValidaCedula\\Socket\\Compilation"
            };
                foreach (string carpeta in carpetas)
                {
                    string rutaCompleta = Path.Combine(programdata, carpeta);
                    if (!Directory.Exists(rutaCompleta))
                    {
                        Directory.CreateDirectory(rutaCompleta);
                    }
                }

                string[] archivos = {
                    "\\Homini\\ValidaCedula\\Socket\\Main.conf",
                    "\\Homini\\ValidaCedula\\Socket\\Version.dat"
                };

                foreach (string archivo in archivos)
                {
                    string rutaCompleta = Path.Combine(programdata, archivo);
                    if (!System.IO.File.Exists(rutaCompleta))
                    {
                        System.IO.File.Create(rutaCompleta).Close();
                    }
                }
              
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }
        }
        //Verificamos si es necesario crear el Acceso Directo o si ya existe
        public void CrearAccesoDirecto()
        {
            try
            {
                string targetPath = @"C:\ProgramData\Homini\ValidaCedula\Socket\Socket_.exe";
                string shortcutPath = @"C:\ProgramData\Homini\ValidaCedula\Socket\Socket.lnk";

                if (!System.IO.File.Exists(shortcutPath))
                {
                    CreateShortcut(targetPath, shortcutPath);
                }
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }

        }
        //Creamos el Acceso directo en %Programdata%
        public void CreateShortcut(string targetPath, string shortcutPath)
        {
            try
            {
                // Crear una instancia de WshShell
                WshShell shell = new WshShell();

                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

                shortcut.TargetPath = targetPath;
                shortcut.Save();
                //Console.WriteLine("Se ha creado el acceso directo correctamente.");

            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }
        }
        //Tomamos el Acceso Directo que va a ir directamente en el StarUp de cada Equipo
        public void MoverAccesoAlStartUp()
        {
            string rutaValidar = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Startup\";
            string RutaINK = @"C:\ProgramData\Homini\ValidaCedula\Socket\Socket.lnk";

            try
            {

                // Verificar si el acceso directo ya existe en la carpeta de inicio de Windows
                if (System.IO.File.Exists(Path.Combine(rutaValidar, "Socket.lnk")))
                {
                    Console.WriteLine("El archivo ya existe en la carpeta de inicio de Windows.");
                    return;
                }

                // Verificar si el archivo de origen existe
                if (!System.IO.File.Exists(RutaINK))
                {
                    Console.WriteLine("El archivo de origen no existe.");
                    return;
                }

                string batchFilePath = (@RaizProyecto.Replace(@"\bin\Debug", "")) + @"\Settings\batch.bat";
                string rutaOrigen = RutaINK;
                string rutaDestino = rutaValidar;

                // Verificar si la ruta de origen existe
                if (!System.IO.File.Exists(rutaOrigen))
                {
                    Console.WriteLine("El archivo de origen no existe.");
                    return;
                }
                string batContent = "@echo off" + Environment.NewLine + Environment.NewLine;
                batContent += "REM Comando para copiar el archivo ejecutable a la carpeta de inicio de Windows" + Environment.NewLine;
                batContent += $"copy \"{rutaOrigen}\" \"{rutaDestino}\" /Y" + Environment.NewLine;
                //batContent += "REM Finalizo";
                //batContent += "pause";
                batContent += "REM Pausa para ver el resultado" + Environment.NewLine;
                //batContent += "pause";

                // Guardar el contenido en el archivo bat
                System.IO.File.WriteAllText(batchFilePath, batContent);

                // Crea un proceso con privilegios de administrador
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                startInfo.Verb = "runas"; // Ejecuta con privilegios de administrador
                startInfo.Arguments = "/C \"" + batchFilePath + "\""; // Agrega comillas alrededor de la ruta del archivo bat

                // Inicia el proceso
                //Process.Start(startInfo);
                //System.IO.File.WriteAllText(batchFilePath, string.Empty);
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
                //Thread.Sleep(10000);

            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }

        }

        public void CopiarExe(bool Version)
        {
            try
            {
                string rutaOrigen = RaizProyecto.Replace(@"\bin\Debug", "") + @"\Socket_.exe";
                //Para Produccion Replace(@"\bin\Debug", "")
                string rutaDestino = @"C:\ProgramData\Homini\ValidaCedula\Socket\" + @"Socket_.exe";

                // Verificar si el archivo existe en la carpeta y no se necesita una nueva versión
                if (System.IO.File.Exists(rutaDestino) && !Version)
                {
                    Console.WriteLine("El archivo existe en la carpeta");
                    return;
                }
                // Verificar si la ruta de origen existe
                if (!System.IO.File.Exists(rutaOrigen))
                {
                    Console.WriteLine("El archivo de origen no existe.");
                    return;
                }

                string batchFilePath = (RaizProyecto.Replace(@"\bin\Debug", "")) + @"\Settings\batch.bat";


                string batContent = "@echo off" + Environment.NewLine + Environment.NewLine;
                batContent += "REM Comando para copiar el archivo ejecutable a la carpeta de inicio de Windows" + Environment.NewLine;
                batContent += $"copy \"{rutaOrigen}\" \"{rutaDestino}\" /Y" + Environment.NewLine;
                //batContent += "REM Finalizo";
                //batContent += "pause";
                batContent += "REM Pausa para ver el resultado" + Environment.NewLine;
                //batContent += "pause";

                // Guardar el contenido en el archivo bat
                System.IO.File.WriteAllText(batchFilePath, batContent);

                // Crea un proceso con privilegios de administrador
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = "cmd.exe";
                //startInfo.Verb = "runas"; // Ejecuta con privilegios de administrador
                startInfo.Arguments = "/C \"" + batchFilePath + "\""; // Agrega comillas alrededor de la ruta del archivo bat

                // Inicia el proceso
                //Process.Start(startInfo);
                //System.IO.File.WriteAllText(batchFilePath, string.Empty);
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }


        }

        public void ValidarNuevaVersion(string version)
        {
            try
            {
                using (StreamReader reader = new StreamReader(programdata + "\\Homini\\ValidaCedula\\Socket\\Version.dat"))
                {
                    if (reader.Read() == -1)
                    {
                        reader.Close();
                        using (StreamWriter writer = new StreamWriter((programdata + "\\Homini\\ValidaCedula\\Socket\\Version.dat"), false))
                        {
                            // Escribir los datos en el archivo
                            writer.WriteLine("VERSION=" + version);

                        }
                    }
                    else
                    {
                        string line;
                        //int lineNumber = 1;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] partes = line.Split('=');
                            string old_Version = partes[1];

                            if (old_Version != version)
                            {
                                //Aqui logica para tomar el exe y enviarlo a la ruta de Programda/Homini
                                Console.WriteLine("La version Cambio");

                                CopiarExe(true);

                                reader.Close();
                                using (StreamWriter writer = new StreamWriter((programdata + "\\Homini\\ValidaCedula\\Socket\\Version.dat"), false))
                                {
                                    // Escribir los datos en el archivo
                                    writer.WriteLine("VERSION=" + version);
                                }
                                break;

                            }
                        }
                    }
                }

            }
            catch (FileNotFoundException e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }
            catch (Exception ex)
            {
                LoggerError($"Error ubicado " + ex.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + ex.Message.ToString() + "\n");
            }
        }

        private void ProcessLine(string line)
        {
            try
            {
                // Dividir la línea por el símbolo '='
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    // Asignar el valor a las variables correspondientes según la clave
                    if (key == "ID")
                    {
                        int.TryParse(value, out Info.intID);
                    }
                    else if (key == "NOMBRE")
                    {
                        Info.vchNombre = value;
                    }
                    else if (key == "IP")
                    {
                        Info.vchIp = value;
                    }
                }
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }

        }
        public bool Informacion_()
        {
            bool validacion = false;
            try
            {
                using (StreamReader reader = new StreamReader(programdata + "\\Homini\\ValidaCedula\\Socket\\Main.conf"))
                {
                    string firstLine = reader.ReadLine();
                    if ((reader.ReadLine()) == null)
                    {
                        reader.Close();
                        GetPrivateIPAddress();
                        return validacion;
                    }
                    else
                    {
                        // Procesar la primera línea
                        if (string.IsNullOrEmpty(Info.vchNombre))
                        {
                            ProcessLine(firstLine);
                            // Volver al inicio del archivo (saltar la primera línea que ya hemos leído)
                            reader.BaseStream.Seek(0, SeekOrigin.Begin);
                            //reader.ReadLine(); // Leer y descartar la primera línea

                            // Leer línea por línea hasta el final del archivo
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                // Procesar cada línea
                                ProcessLine(line);
                            }
                        }

                        return true;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
                return false;
            }
        }


        public void GetPrivateIPAddress()
        {
            try
            {
                // 
                string[] Permitidas = { "10.59", "10.60", "10.61", "10.62", "10.63" };
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ipAddress = ip.ToString();

                        foreach (string permitida in Permitidas)
                        {
                            if (ipAddress.StartsWith(permitida + "."))
                            {
                                // Obtener la posición del último punto en la dirección IP
                                int lastDotIndex = ipAddress.LastIndexOf('.');

                                // Eliminar el último dígito utilizando Substring
                                string ipAddressWithoutLastDigit = ipAddress.Substring(0, lastDotIndex);

                                InsertarDatosInformacion(ipAddressWithoutLastDigit);

                                return; // Salir del método después de procesar la dirección IP
                            }
                        }
                    }

                }
                // Si no se encuentra una dirección IP privada válida, lanzar una excepción o manejarla de alguna manera
                Console.WriteLine("No se encontró una dirección IP privada válida.");
                logger.Error("Error en GetPrivateIPAddress():", "No se encontró una dirección IP privada válida.");
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");

            }
        }

        public void InsertarDatosInformacion(string ip_)
        {
            try
            {
                Conexion = DB.ObtenerConexion();

                if (Conexion != null && Conexion.State == System.Data.ConnectionState.Open)
                {
                    string consulta = "spDatosSocket @ip";

                    using (SqlCommand comando = new SqlCommand(consulta, Conexion))
                    {
                        comando.Parameters.AddWithValue("@ip", ip_ + ".0");

                        using (SqlDataReader reader = comando.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string nombre = reader.GetString(1);

                                // Escribir los datos en el archivo de configuración una vez que se han leído todos los resultados
                                using (StreamWriter writer = new StreamWriter(Path.Combine(programdata, "Homini\\ValidaCedula\\Socket\\Main.conf")))
                                {
                                    writer.WriteLine("ID=" + id);
                                    writer.WriteLine("NOMBRE=" + nombre);
                                    writer.WriteLine("IP=" + ip_ + ".0");

                                    // Actualizar los datos en la clase Info si es necesario
                                    Info.intID = id;
                                    Info.vchNombre = nombre;
                                    Info.vchIp = ip_ + ".0";
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No se pudo obtener una conexión válida a la base de datos.");
                }
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }
            finally
            {
                // Cerrar la conexión si está abierta
                if (Conexion != null && Conexion.State == System.Data.ConnectionState.Open)
                {
                    Conexion.Close();
                }
            }

        }
        public int[] ValidarNuevosComandos()
        {
            List<int> ejecuciones_ = new List<int>();

            try
            {
                string path_ = programdata + "\\Homini\\ValidaCedula\\Socket\\Execution\\Execution.conf";
                if (!System.IO.File.Exists(path_)) { System.IO.File.Create(path_).Close(); }
                using (StreamReader reader = new StreamReader(path_))
                {
                    string line;
                    int lineNumber = 1;
                    while ((line = reader.ReadLine()) != null)
                    {
                        ejecuciones_.Add(int.Parse(line));
                        lineNumber++;
                    }
                    reader.Close();
                }
            }
            catch (FileNotFoundException e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }

            int[] ejecuciones = ejecuciones_.ToArray();
            return ejecuciones;
        }

        public void CrearEntrada(string input, string nombre)
        {
            try
            {
                string path_ = (programdata + "\\Homini\\ValidaCedula\\Socket\\Input\\");
                //Por si las moscas
                //if (!Directory.Exists(path_)) { Directory.CreateDirectory(path_); }
                using (StreamWriter writer = new StreamWriter(path_ + nombre + ".txt"))
                {
                    // Escribir los datos en el archivo
                    writer.WriteLine(input);
                }
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }

        }

        public void CrearRegistroSalidaError(string input, string nombre)
        {
            try
            {
                string path_ = (programdata + "\\Homini\\ValidaCedula\\Socket\\Output\\");
                //Por si las moscas
                if (!Directory.Exists(path_)) { Directory.CreateDirectory(path_); }
                using (StreamWriter writer = new StreamWriter(path_ + nombre + ".txt"))
                {
                    // Escribir los datos en el archivo
                    writer.WriteLine(input);
                }
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }

        }
        public void crearRegistroEjecucion( int id)
        {
            try
            {
                using (StreamWriter writer = System.IO.File.AppendText(programdata + "\\Homini\\ValidaCedula\\Socket\\Execution\\Execution.conf"))

                {
                    // Escribir los datos en el archivo

                    writer.WriteLine(id.ToString());

                    writer.Close();
                }
            }
            catch (Exception e)

            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }
        }
        public void CrearRegistroEjecucionSQL( int Idtarea, string Respuesta,byte Estado, string Fecha_ejecucion)
            {
            
            try
            {
                // Obtain database connection
                Conexion = DB.ObtenerConexion();

                // Check if the connection is valid and open
                if (Conexion != null && Conexion.State == System.Data.ConnectionState.Open)
                {
                    // Define SQL query
                    string Query = "INSERT INTO Crearregistro (intidTareaProgramada, intIdSucursal, vchRespuesta, bitEstado, vchFecha ) VALUES (@Idtarea, @IntIDsucursal, @Respuesta, @Estado ,@Fecha_ejecucion)";

                    using (SqlCommand comando = new SqlCommand(Query, Conexion))
                    {
                        comando.Parameters.AddWithValue("@Idtarea", Idtarea); //Parametro //propio de tabla 1,1
                        comando.Parameters.AddWithValue("@IntIDsucursal", Info.intID); //No necesario
                        comando.Parameters.AddWithValue("@Respuesta", Respuesta); //No necesario
                        comando.Parameters.AddWithValue("@Estado", Estado); //No necesari
                        comando.Parameters.AddWithValue("@Fecha_ejecucion", Fecha_ejecucion); //No necesari

                        try
                        {
                            int result = comando.ExecuteNonQuery();
                            // Check if the query was successful
                            if (result < 0)
                            {
                                Console.WriteLine("Error al insertar en la base de datos.");
                                Conexion.Close();
                            }
                            else
                            {
                                Console.WriteLine("Datos insertados correctamente.");
                                Conexion.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("La conexión a la base de datos no está abierta o es nula.");
                }
            }
            catch (Exception e)
            {
                 LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");

            }
         }

        public void EjecutarTareasProgramadas(Label ID_, Label NOMBRE_, Label IP_, Label ESTADO_, Label VERIFICACION_)
        {


            try
            {
                //Validamos: IP y seteamos datos
                bool validado = Informacion_();

                if (validado)
                {
                    if (ID_.Text == "______________")
                    {
                        ID_.Text = Info.intID.ToString();
                        NOMBRE_.Text = Info.vchNombre.ToString();
                        IP_.Text = Info.vchIp.ToString();
                    }
                    ESTADO_.Text = "DESCONECTADO.";
                    VERIFICACION_.Text = DateTime.Now.ToString();


                   
                        SqlConnection sqlConnection = null;
                        sqlConnection = DB.ObtenerConexion();

                        if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        //Cambiamos el estado a Conectado 
                        ESTADO_.Text = "CONECTADO.";
                        VERIFICACION_.Text = DateTime.Now.ToString();
                        //Validamos los id pendientes por ejecutar.
                        int[] idPendiente = ValidarNuevosComandos();

                        System.Data.DataTable dataTable = new System.Data.DataTable();
                        dataTable.Columns.Add("Value", typeof(int));

                        foreach (int value in idPendiente)
                        {
                            dataTable.Rows.Add(value);
                        }

                        //SqlConnection conexion = ConexionSQL.ObtenerConexion();
                        try
                        {
                            SqlCommand command = new SqlCommand();
                            command.Connection = sqlConnection;
                            // Crear la tabla de valores en la base de datos
                            command.CommandText = "CREATE TABLE #TempTable (Value INT)";
                            // Ejecutar el comando para crear la tabla de valores
                            command.ExecuteNonQuery();
                            // Copiar los datos del DataTable a la tabla de valores en la base de datos
                            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
                            {
                                bulkCopy.DestinationTableName = "#TempTable";
                                bulkCopy.WriteToServer(dataTable);
                            }
                            command.CommandText = "SELECT TOP(1) * FROM tblTareasProgramadas WHERE ID NOT IN (SELECT Value FROM #TempTable)  AND (intIdSucursal = @Sucursal OR intIdSucursal IS NULL) AND BitEstado = 1;";
                            command.Parameters.Clear();
                            command.Parameters.Add("@Sucursal", System.Data.SqlDbType.Int).Value = Info.intID;
                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                // Procesar los resultados
                                while (reader.Read())
                                {
                                    // Obtener los valores de cada fila
                                    int id = Convert.ToInt32(reader["id"]);
                                    string nombre = reader["vchNombreTarea"].ToString();
                                    string tarea = Convert.ToString(reader["vchTarea"]);
                                      //idTarea 
                            //IdSucursal
                            //Respuesta OK\\Error

                                    ejecutarTareaFinal(nombre, tarea, id);
                                }
                            }
                            //Borramos y cerramos
                            // Eliminar la tabla de valores de la base de datos
                            command.CommandText = "DROP TABLE #TempTable";
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
                            // Cerrar el lector
                        }
                        catch (Exception e)
                        {
                            LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
                            Console.WriteLine("Error en EjecutarTareasProgramadas():", e.Message);
                        }
                    }
                }
               
            }
            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
                
            }
        }
        public void ejecutarTareaFinal(string nombre, string tarea, int id)
        {

            try
            {
                string codigoCSharp = @tarea;
                var Fecha_ejecucion = DateTime.Now.ToString();

                // Crea un proveedor de compilación de C#
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters();

                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Data.dll");
                parameters.ReferencedAssemblies.Add("System.Data.OleDb.dll");
                parameters.ReferencedAssemblies.Add("System.Data.SqlClient.dll");

                parameters.GenerateInMemory = true;

                // Compila el código C#
                CompilerResults results = provider.CompileAssemblyFromSource(parameters, codigoCSharp);


                // Verifica si hubo errores de compilación
                if (results.Errors.HasErrors)
                {
                    //Por si las moscas
                    if (!System.IO.File.Exists(programdata + "\\Homini\\ValidaCedula\\Socket\\Compilation\\" + nombre + ".txt")) { System.IO.File.Create(programdata + "\\Homini\\ValidaCedula\\Socket\\Compilation\\" + nombre + ".txt").Close(); }
                    // Manejo de errores de compilación
                    using (StreamWriter writer = new StreamWriter(programdata + "\\Homini\\ValidaCedula\\Socket\\Compilation\\" + nombre + ".txt"))
                    {
                        foreach (CompilerError error in results.Errors)
                        {
                            // Escribir los datos en el archivo
                            writer.WriteLine("Error en línea {0}: {1}", error.Line, error.ErrorText);
                        }
                    }
                }
                else
                {
                    try
                    {
                        Type ejecutarTareaFinal1 = results.CompiledAssembly.GetType("class_Ejecutar");
                        Console.WriteLine("Porque no quiero11" + ejecutarTareaFinal1);
                        Dictionary<string, object> resultado = (Dictionary<string, object>)ejecutarTareaFinal1.GetMethod("Ejecutar").Invoke(null, null);
                        Console.WriteLine("Porque quiero "+resultado);

                        // Obtener el método "Ejecutar" del tipo
                        //   Console.WriteLine("resultado" + resultado);
                        if ((bool)resultado["Estado"])
                        {
                            // Código a ejecutar si la respuesta es verdadera (true)
                            CrearEntrada(codigoCSharp, nombre);
                            CrearRegistroEjecucionSQL(id, resultado["Estado"].ToString(), 1, Fecha_ejecucion);
                            crearRegistroEjecucion(id);
                        }
                        else
                        {
                            // Código a ejecutar si la respuesta es verdadera (false)
                            CrearRegistroSalidaError(resultado["Respuesta"].ToString(), nombre);
                            CrearRegistroEjecucionSQL(id, resultado["Respuesta"].ToString(), 0, Fecha_ejecucion);
                            crearRegistroEjecucion(id);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Manejo de excepciones
                        Console.WriteLine("Ocurrió una excepción: " + ex.Message);
                        // Puedes agregar más lógica de manejo de excepciones aquí si es necesario
                    }

                    Thread.Sleep(1000); // Pausa de 1 segundo 
                }
            }


            catch (Exception e)
            {
                LoggerError($"Error ubicado " + e.StackTrace.ToString().Trim() + "\n" + "Mesaje error: " + e.Message.ToString() + "\n");
            }
        }
    }
}

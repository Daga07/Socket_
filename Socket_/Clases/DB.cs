using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Socket_.Clases
{
    internal class DB
    {
        public static SqlConnection ObtenerConexion()
        {
            string servidor = @"PC1NQ0M4\SQLPAGAFACIL";
            string baseDatos = "PF_dbCorresco";
            string usuario = "SA";
            string contraseña = @"%#Corbanti2024#%";

            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = servidor;
                builder.InitialCatalog = baseDatos;
                builder.UserID = usuario;
                builder.Password = contraseña;

                SqlConnection conexion = new SqlConnection(builder.ConnectionString);
                conexion.Open();

                // Verificar si la conexión está abierta antes de devolverla
                if (conexion.State != System.Data.ConnectionState.Open)
                {
                    throw new Exception("No se pudo abrir la conexión.");
                }

                return conexion;
            }
            catch (SqlException ex)
            {
                // Manejo de errores específicos de SQL Server
                switch (ex.Number)
                {
                    case -2: // Error de timeout
                        Console.WriteLine("Error de timeout al intentar conectar.");
                        break;
                    // Puedes agregar más casos según sea necesario
                    default:
                        Console.WriteLine($"Error SQL {ex.Number}: {ex.Message}");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                Console.WriteLine("Error al intentar conectar: " + ex.Message);
            }

            return null; // Si hay un error, devolvemos null
        }
    }
}


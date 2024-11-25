using Microsoft.CSharp;
using NLog;
using Socket_.Clases;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Socket_
{
    public partial class Start : Form
    {
        private System.Windows.Forms.Timer timer;
        //Version de la APP
        Version version_APP = Assembly.GetEntryAssembly().GetName().Version;
        //Creamos Instancia para Ejecutar cada una de las Funciones Iniciales
        Funciones_ funciones = new Funciones_();
        //Aqui manejaremos los logs
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public Start()
        {
            InitializeComponent();
            try
            {
                //string[] array = { "elemento1", "elemento2", "elemento3" };
                //string elementoInvalido = array[5];
                //ACOMODAMOS LA VERSION
                VERSION_.Text = ("Version:" + version_APP);


                // Creamos directorios en caso de que no existan
                funciones.CrearRutasNecesarias();
                //Creamos el Acceso Directo
                funciones.CrearAccesoDirecto();
                //Movemos el Acceso Directo Antes creado a la carpeta de inicio de windows
                funciones.MoverAccesoAlStartUp();
                //Pasamos el archivo .EXE de la aplicacion a la carpeta de Homini
                //Inicia en Falso para validar si ya existe el archivo
                funciones.CopiarExe(false);
                //Por ultimo:Se Valida la Version de la app para reemplazar el archivo exe con  la nueva version.
                funciones.ValidarNuevaVersion(version_APP.ToString());

                // Crear un Timer
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 10000; // Intervalo de 1 minuto en milisegundos
                timer.Tick += TimerTick;
                timer.Start();
            }
            catch (Exception ex)
            {
                logger.Error("Error en Start : Form():", ex.Message);
                Console.WriteLine("Error en Start : Form():", ex.Message);
            }
        }
        public void TimerTick(object sender, EventArgs e)
        {
            try
            {
                funciones.EjecutarTareasProgramadas(ID_, NOMBRE_, IP_, ESTADO_, VERIFICACION_);
            }
            catch (Exception ex)
            {
                //logger.Error("Error en TimerTick():", ex.Message);
                Console.WriteLine("Error en TimerTick():", ex.Message);
            }
        }
    }
}


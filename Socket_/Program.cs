
using Microsoft.CSharp;
using NLog;
using Socket_.Clases;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    internal static class Program
    {
        static NotifyIcon notifyIcon;
        static Form mainForm; // Cambiado a Start en lugar de Form

        [STAThread]
        static void Main()
        {
            // Nombre único para el Mutex (puedes cambiarlo por uno significativo para tu aplicación)
            string mutexName = "Socket_";

            // Intenta crear el Mutex
            using (Mutex mutex = new Mutex(true, mutexName, out bool createdNew))
            {
                if (createdNew)
                {
                    // La aplicación es la primera instancia, continúa con la ejecución normal
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    CreateNotifyIcon();
                    CreateMainForm();

                    Application.Run();

                }
                else
                {
                    // La aplicación ya está en ejecución, muestra un mensaje y cierra esta instancia
                    Console.WriteLine("La aplicación ya está en ejecución.");
                }
            }
        }

        static void CreateMainForm()
        {
            mainForm = new Start();
            mainForm.MaximizeBox = false;
            mainForm.Resize += MainForm_Resize; // Suscribir al evento Resize aquí
            mainForm.FormClosing += MainForm_FormClosing;
            mainForm.VisibleChanged += MainForm_VisibleChanged;
        }


        static void CreateNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Text = "Control remoto para gestionar y automatizar procesos";

            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            notifyIcon.MouseUp += NotifyIcon_MouseUp;

            notifyIcon.Visible = true;
        }

        static void ShowForm()
        {
            mainForm.Show();
            mainForm.Activate();
            mainForm.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }

        static void HideForm()
        {
            mainForm.Hide();
            notifyIcon.Visible = true;
        }

        static void MainForm_Resize(object sender, EventArgs e)
        {
            // Manejar el evento Resize aquí
            if (mainForm.WindowState == FormWindowState.Minimized)
            {
                HideForm();
            }
        }

        static void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                HideForm();
            }
        }

        static void MainForm_VisibleChanged(object sender, EventArgs e)
        {
            if (mainForm.Visible)
            {
                notifyIcon.Visible = false;
            }
            else
            {
                notifyIcon.Visible = true;
            }
        }

        static void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        static void NotifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu contextMenu = new ContextMenu();
                MenuItem closeMenuItem = new MenuItem("Cerrar Aplicacion", CloseMenuItem_Click);
                contextMenu.MenuItems.Add(closeMenuItem);

                notifyIcon.ContextMenu = contextMenu;
            }
        }

        static void CloseMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            mainForm.Close();
            Application.Exit();
        }
    }

    public static class Info
    {
        public static int intID;
        public static string vchNombre;
        public static string vchIp;
    }
}

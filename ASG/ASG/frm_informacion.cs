using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASG
{
    public partial class frm_informacion : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        BackgroundWorker m_oWorker;
        string user;
        string sucursal;
        public frm_informacion(string userL, string sucursalB)
        {

            InitializeComponent();
            user = userL;
            sucursal = sucursalB;
            m_oWorker = new BackgroundWorker();

            // Create a background worker thread that ReportsProgress &
            // SupportsCancellation
            // Hook up the appropriate events.
            m_oWorker.DoWork += new DoWorkEventHandler(m_oWorker_DoWork);
            m_oWorker.ProgressChanged += new ProgressChangedEventHandler
                    (m_oWorker_ProgressChanged);
            m_oWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler
                    (m_oWorker_RunWorkerCompleted);
            m_oWorker.WorkerReportsProgress = true;
            m_oWorker.WorkerSupportsCancellation = true;
        }
        void m_oWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // The background process is complete. We need to inspect
            // our response to see if an error occurred, a cancel was
            // requested or if we completed successfully.  
            if (e.Cancelled)
            {
                lblStatus.Text = "PROCESO CANCELADO.";
            }

            // Check to see if an error occurred in the background process.

            else if (e.Error != null)
            {
                lblStatus.Text = "HA OCURRIDO UN ERORO INESPERADO (COD ERROR -1).";
            }
            else
            {
                // Everything completed normally.
                lblStatus.Text = "COMPLETADO!";
            }

            //Change the status of the buttons on the UI accordingly
            button2.Enabled = true;
            button1.Enabled = false;
        }
        void m_oWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            // This function fires on the UI thread so it's safe to edit

            // the UI control directly, no funny business with Control.Invoke :)

            // Update the progressBar with the integer supplied to us from the

            // ReportProgress() function.  
            progressBar1.Visible = true;
            lblStatus.Visible = true;
            progressBar1.Value = e.ProgressPercentage;
            lblStatus.Text = "Restaurando Datos......" + progressBar1.Value.ToString() + "%";
        }
        void m_oWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // The sender is the BackgroundWorker object we need it to
            // report progress and check for cancellation.
            //NOTE : Never play with the UI thread here...
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);

                // Periodically report progress to the main thread so that it can
                // update the UI.  In most cases you'll just need to send an
                // integer that will update a ProgressBar                    
                m_oWorker.ReportProgress(i);
                // Periodically check if a cancellation request is pending.
                // If the user clicks cancel the line
                // m_AsyncWorker.CancelAsync(); if ran above.  This
                // sets the CancellationPending to true.
                // You must check this flag in here and react to it.
                // We react to it by setting e.Cancel to true and leaving
                if (m_oWorker.CancellationPending)
                {
                    // Set the e.Cancel flag so that the WorkerCompleted event
                    // knows that the process was cancelled.
                    e.Cancel = true;
                    m_oWorker.ReportProgress(0);
                    return;
                }
            }

            //Report 100% completion on operation completed
            m_oWorker.ReportProgress(100);
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frm_informacion_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_informacion_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_informacion_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_informacion_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result;
            result = MessageBox.Show("¿ESTA SEGURO QUE DESEA RESTAURAR UNA COPIA DE SEGURIDAD?", "MANTENIMIENTO DEL SISTEMA", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                progressBar1.Visible = true;
                lblStatus.Visible = true;
                string constring = "server=localhost;user=root;pwd=1234;database=santo_tomas;";
                string file = "C:\\backup.sql";
                using (MySqlConnection conn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();

                            OpenFileDialog Guardar = new OpenFileDialog();
                            Guardar.Filter = "sql(*.sql)|*.sql|sql(*.sql)|*.sql";

                            if (Guardar.ShowDialog() == DialogResult.OK)
                            {
                                button2.Enabled = false;
                                button1.Enabled = false;
                                this.KeyPreview = false;
                                pictureBox3.Enabled = false;
                                progressBar1.Visible = true;
                                lblStatus.Visible = true;
                                file = Guardar.FileName;
                                m_oWorker.RunWorkerAsync();
                                mb.ImportFromFile(file);
                                this.KeyPreview = true;
                                button1.Enabled = true;
                                pictureBox3.Enabled = true;
                                adressUser.ingresaBitacora(sucursal, user, "COPIA DATA BASE", "santo_tomas.sql");

                                // Kickoff the worker thread to begin it's DoWork function.


                            } else
                            {
                                progressBar1.Visible = false;
                                lblStatus.Visible = false;
                            }
                            conn.Close();
                        }
                    }
                }
            }
        }

        private void frm_informacion_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (m_oWorker.IsBusy)
            {

                // Notify the worker thread that a cancel has been requested.

                // The cancel will not actually happen until the thread in the

                // DoWork checks the m_oWorker.CancellationPending flag. 

                m_oWorker.CancelAsync();
                
            } else
            {
                this.Close();
            }
        }

        private void pictureBox3_MouseHover(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Red;
        }

        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            pictureBox3.BackColor = Color.Transparent;
        }
    }
}

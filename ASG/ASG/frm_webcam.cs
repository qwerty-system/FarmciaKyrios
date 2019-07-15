using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ASG
{
    public partial class frm_webcam : Form
    {
        private bool ExisteDispositivo = false;
        private FilterInfoCollection DispositivoDeVideo;
        private VideoCaptureDevice FuenteDeVideo = null;
        private bool fotografiaHecha = false;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        Image nuevaImagen;
        string pathImagen;
        public frm_webcam()
        {
            InitializeComponent();
            BuscarDispositivos();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            TerminarFuenteDeVideo();
            this.Close();
           
        }
        public void CargarDispositivos(FilterInfoCollection Dispositivos)
        {
            for (int i = 0; i < Dispositivos.Count; i++) ;

            comboBox1.Items.Add(Dispositivos[0].Name.ToString());
            comboBox1.Text = comboBox1.Items[0].ToString();

        }
        public void BuscarDispositivos()
        {
            DispositivoDeVideo = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (DispositivoDeVideo.Count == 0)
            {
                ExisteDispositivo = false;
            }

            else
            {
                ExisteDispositivo = true;
                CargarDispositivos(DispositivoDeVideo);

            }
        }
        public void TerminarFuenteDeVideo()
        {
            if (!(FuenteDeVideo == null))
                if (FuenteDeVideo.IsRunning)
                {
                    FuenteDeVideo.SignalToStop();
                    FuenteDeVideo = null;
                }
        }
        public void Video_NuevoFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap Imagen = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = Imagen;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (button5.Text == "INICIAR")

            {
                if (ExisteDispositivo)
                {

                    FuenteDeVideo = new VideoCaptureDevice(DispositivoDeVideo[comboBox1.SelectedIndex].MonikerString);
                    FuenteDeVideo.NewFrame += new NewFrameEventHandler(Video_NuevoFrame);
                    FuenteDeVideo.Start();
                    button5.Text = "DETENER";
                    comboBox1.Enabled = false;
                }
                else
                {
                    MessageBox.Show("NO SE ENCONTRO DISPOSITIVO");
                }

            }
            else
            {
                if (FuenteDeVideo.IsRunning)
                {
                    TerminarFuenteDeVideo();
                    button5.Text = "INICIAR";
                    comboBox1.Enabled = true;
                    if (!fotografiaHecha)
                    {
                        pictureBox1.Image = null;
                    }
                }
            }
        }
        private void Capturar()
        {
            if (FuenteDeVideo.IsRunning)
            {
                pictureBox1.Image = pictureBox1.Image;

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Capturar();
            fotografiaHecha = true;
            button5.PerformClick();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            button5.PerformClick();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TerminarFuenteDeVideo();
            DialogResult = DialogResult.Cancel;
        }

        private void frm_webcam_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_webcam_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_webcam_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }
        internal frm_usuarios.imageUser currenProfile
        {
            get
            {
                return new frm_usuarios.imageUser()
                {
                    getProfile = nuevaImagen
                };
            }
            set
            {
                currenProfile.getProfile = nuevaImagen;
            }
        }
        internal frm_usuarios.imageUser currentPath
        {
            get
            {
                return new frm_usuarios.imageUser()
                {
                    getPath = pathImagen
                };
            }
            set
            {
                currenProfile.getPath = pathImagen;
            }
        }
        private void guardaImagen()
        {
            SaveFileDialog Guardar = new SaveFileDialog();
            Guardar.Filter = "JPEG(*.JPG)|*.JPG|BMP(*.BMP)|*.BMP";
            Image Imagen = pictureBox1.Image;
            if (Guardar.ShowDialog() == DialogResult.OK)
            {
                pathImagen = Guardar.FileName;
                Imagen.Save(Guardar.FileName);
                DialogResult = DialogResult.OK;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            nuevaImagen = pictureBox1.Image;
            guardaImagen();
        }

        private void frm_webcam_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                TerminarFuenteDeVideo();
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

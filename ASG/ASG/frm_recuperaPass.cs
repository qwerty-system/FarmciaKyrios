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
using System.Net.NetworkInformation;
using System.Runtime;
using System.Runtime.InteropServices;

namespace ASG
{
    public partial class frm_recuperaPass : Form
    {
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_recuperaPass()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Enter(object sender, EventArgs e)
        {
            button1.BackColor = Color.LightGray;
        }

        private void button1_Leave(object sender, EventArgs e)
        {
            button1.BackColor = Color.Transparent;
        }

        private void frm_recuperaPass_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_recuperaPass_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }

        }

        private void frm_recuperaPass_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }

        private void frm_recuperaPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                this.Close();
        }

        
        private void button8_Click(object sender, EventArgs e)
        {
            
                var forma = new frm_getPassword();
                forma.ShowDialog();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var forma = new frm_Inactivo();
            forma.ShowDialog();
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

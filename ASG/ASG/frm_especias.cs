using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASG
{
    public partial class frm_especias : Form
    {
        string presentacion;
        string cantidad;
        Point DragCursor;
        Point DragForm;
        bool Dragging;
        public frm_especias()
        {
            InitializeComponent();
        }
        internal frm_compras.Presentaciones CurrentCantidad
        {
            get
            {
                return new frm_compras.Presentaciones
                {
                    getCantidad = cantidad
                };
            }
            set
            {
                CurrentCantidad.getCantidad = cantidad;
            }
        }
        internal frm_compras.Presentaciones CurrentPresentacion
        {
            get
            {
                return new frm_compras.Presentaciones
                {
                    getPresentacion = presentacion
                };
            }
            set
            {
                CurrentPresentacion.getPresentacion = presentacion;
            }
        }
        private void frm_especias_Load(object sender, EventArgs e)
        {
            button8.Focus();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            presentacion = button8.Text;
            cantidad = "0.0044642857142857";
            DialogResult = DialogResult.OK;
        }

        private void button8_Enter(object sender, EventArgs e)
        {
            button8.BackColor = Color.DimGray;
        }

        private void button1_Enter(object sender, EventArgs e)
        {
            button1.BackColor = Color.DimGray;
        }

        private void button2_Enter(object sender, EventArgs e)
        {
            button2.BackColor = Color.DimGray;
        }

        private void button3_Enter(object sender, EventArgs e)
        {
            button3.BackColor = Color.DimGray;
        }

        private void button4_Enter(object sender, EventArgs e)
        {
            button4.BackColor = Color.DimGray;
        }

        private void button5_Enter(object sender, EventArgs e)
        {
            button5.BackColor = Color.DimGray;
        }

        private void button8_Leave(object sender, EventArgs e)
        {
            button8.BackColor = Color.Transparent;
        }

        private void button1_Leave(object sender, EventArgs e)
        {
            button1.BackColor = Color.Transparent;
        }

        private void button2_Leave(object sender, EventArgs e)
        {
            button2.BackColor = Color.Transparent;
        }

        private void button3_Leave(object sender, EventArgs e)
        {
            button3.BackColor = Color.Transparent;
        }

        private void button4_Leave(object sender, EventArgs e)
        {
            button4.BackColor = Color.Transparent;
        }

        private void button5_Leave(object sender, EventArgs e)
        {
            button5.BackColor = Color.Transparent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            presentacion = button1.Text;
            cantidad = "0.0625";
            DialogResult = DialogResult.OK;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            presentacion = button3.Text;
            cantidad = "1";
            DialogResult = DialogResult.OK;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            presentacion = button4.Text;
            cantidad = "25";
            DialogResult = DialogResult.OK;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            presentacion = button5.Text;
            cantidad = "100";
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            presentacion = button2.Text;
            cantidad = "0.50";
            DialogResult = DialogResult.OK;
        }

        private void frm_especias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frm_especias_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragCursor = Cursor.Position;
            DragForm = this.Location;
        }

        private void frm_especias_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging == true)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(DragCursor));
                this.Location = Point.Add(DragForm, new Size(dif));
            }
        }

        private void frm_especias_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
        }
    }
}

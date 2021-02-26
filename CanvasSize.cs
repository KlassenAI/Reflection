using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KPO
{
    public partial class CanvasSize : Form
    {
        public int CanvasWidth;
        public int CanvasHeight;

        public CanvasSize()
        {
            InitializeComponent();
        }

        private void CanvasSize_Load(object sender, EventArgs e)
        {
            textBoxWidth.Text = CanvasWidth.ToString();
            textBoxHeight.Text = CanvasHeight.ToString();
        }

        private void textBoxWidth_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CanvasWidth = Convert.ToInt32(textBoxWidth.Text);
            }
            catch
            {
                MessageBox.Show("Значение должно быть целым положительным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void textBoxHeight_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CanvasHeight = Convert.ToInt32(textBoxHeight.Text);
            }
            catch
            {
                MessageBox.Show("Значение должно быть целым положительным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if (!char.IsDigit(e.KeyChar) && number != 8)
            {
                e.Handled = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                CanvasWidth = Convert.ToInt32(textBoxWidth.Text);
                CanvasHeight = Convert.ToInt32(textBoxHeight.Text);
                if (CanvasWidth == 0 || CanvasHeight == 0) throw new Exception();
            }
            catch
            {
                MessageBox.Show("Значения должно быть целыми положительными числами.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }
    }
}

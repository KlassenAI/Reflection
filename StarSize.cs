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
    public partial class StarSize : Form
    {
        private int number;

        public int getNumber()
        {
            return number;
        }

        public StarSize()
        {
            InitializeComponent();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                number = Convert.ToInt32(textBox.Text);
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

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                number = Convert.ToInt32(textBox.Text);
                if (number == 0) throw new Exception();
            }
            catch
            {
                MessageBox.Show("Значение должно быть целым положительным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }
    }
}

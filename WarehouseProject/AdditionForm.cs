using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarehouseProject
{
    public partial class AdditionForm : Form
    {
        public AdditionForm()
        {
            InitializeComponent();
        }

        public String productName;
        public int productQuantity;

        public AdditionForm(String productName, int quantity)
        {
            InitializeComponent();
            this.productName = productName;
            this.productQuantity = quantity;
            textBox1.Text = this.productName;
            textBox2.Text = productQuantity.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = textBox1.Text.Trim();
            int quantity;
            try
            {
                quantity = int.Parse(textBox2.Text);
                if (String.Equals(name, ""))
                {
                    throw new Exception();
                }
                this.productName = name;
                this.productQuantity = quantity;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Invalid product name or quantity!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}

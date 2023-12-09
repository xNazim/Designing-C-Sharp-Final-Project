using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WarehouseProject
{
    public partial class Form1 : Form
    {
        String createTableProductsCmd = "CREATE TABLE IF NOT EXISTS products (id serial, name varchar(255), quantity int);";
        String createTableOperationsCmd = "CREATE TABLE IF NOT EXISTS operations (id serial, name varchar(255), type varchar(255), date varchar(255));";
        
        String databaseURI = "Server=localhost;Port=5432;Database=warehouse;User Id=postgres;Password=1234";
        NpgsqlConnection conn;
        NpgsqlCommand cmd;

        String readAllProductsCmd = "SELECT * FROM products;";
        String readAllOperationsCmd = "SELECT * FROM operations;";

        public Form1()
        {
            InitializeComponent();
            conn = new NpgsqlConnection(databaseURI);
            conn.Open();
            init();
            refreshTables();
        }

        private void init()
        {
            cmd = createCommand(createTableProductsCmd);
            cmd.ExecuteNonQuery();
            cmd = createCommand(createTableOperationsCmd);
            cmd.ExecuteNonQuery();
        }

        private void refreshTables()
        {
            cmd = createCommand(readAllProductsCmd);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows) 
            { 
                DataTable table = new DataTable();
                table.Load(reader);
                dataGridView1.DataSource = table;
            }
            reader.Close();

            cmd = createCommand(readAllOperationsCmd);
            reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                DataTable table = new DataTable();
                table.Load(reader);
                dataGridView2.DataSource = table;
            }
            reader.Close();
        }

        private NpgsqlCommand createCommand(String query) { 
            NpgsqlCommand newCmd = conn.CreateCommand();
            newCmd.CommandType = CommandType.Text;
            newCmd.CommandText = query;
            return newCmd;
        }

        private void button1_Click(object sender, EventArgs e) // Add new product
        {
            AdditionForm additionForm = new AdditionForm();

            if (additionForm.ShowDialog() == DialogResult.Cancel)
                return;

            String productName = additionForm.productName;
            int productQuantity = additionForm.productQuantity;

            String createProductCmd = String.Format("INSERT INTO products(name, quantity) VALUES ('{0}', {1});", productName, productQuantity);
            cmd = createCommand(createProductCmd);
            cmd.ExecuteNonQuery();

            recordAddOperation(productName, productQuantity);
            refreshTables();
        }

        private void button3_Click(object sender, EventArgs e) // Edit product
        {
            try
            {
                int productId = int.Parse(textBox1.Text);
                if (!checkProductExistence(productId))
                {
                    throw new Exception();
                }
                else
                {
                    String readCmd = String.Format("SELECT name, quantity FROM products WHERE id={0}", productId);
                    cmd = createCommand(readCmd);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    String oldName = reader[0].ToString();
                    int oldQuantity = int.Parse(reader[1].ToString());
                    reader.Close();

                    AdditionForm editForm = new AdditionForm(oldName, oldQuantity);
                    if (editForm.ShowDialog() == DialogResult.Cancel)
                        return;

                    String newName = editForm.productName;
                    int newQuantity = editForm.productQuantity;
                    String updateTaskCmd = String.Format("UPDATE products SET name='{0}', quantity={1} WHERE id={2};",
                        newName, newQuantity, productId);
                    cmd = createCommand(updateTaskCmd);
                    cmd.ExecuteNonQuery();
                    recordEditOperation(oldName);
                    refreshTables();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                MessageBox.Show("Given task ID is invalid or non-existent!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool checkProductExistence(int productId) 
        {
            String checkCmd = String.Format("SELECT * FROM products WHERE id={0};", productId);
            cmd = createCommand(checkCmd);
            NpgsqlDataReader reader = cmd.ExecuteReader();
            bool value = reader.HasRows;
            reader.Close();
            return value;
        }

        private void button2_Click(object sender, EventArgs e) // Delete product
        {
            try
            {
                int productId = int.Parse(textBox1.Text);
                if (!checkProductExistence(productId))
                {
                    throw new Exception();
                }
                else
                {
                    String readCmd = String.Format("SELECT name FROM products WHERE id={0}", productId);
                    cmd = createCommand(readCmd);
                    NpgsqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    String productName = reader[0].ToString();
                    reader.Close();

                    String deleteCmd = String.Format("DELETE FROM products WHERE id={0};", productId);
                    cmd = createCommand(deleteCmd);
                    cmd.ExecuteNonQuery();
                    recordDeleteOperation(productName);
                    refreshTables();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                MessageBox.Show("Given task ID is invalid or non-existent!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void recordAddOperation(String productName, int quantity) 
        {
            recordOperation("ADD", String.Format("Added {0} x{1}", productName, quantity));
        }

        private void recordEditOperation(String productName) 
        {
            recordOperation("EDIT", String.Format("Edited {0}", productName));
        }

        private void recordDeleteOperation(String productName)
        {
            recordOperation("DELETE", String.Format("Deleted {0}", productName));
        }

        private void recordOperation(String operationType, String name) 
        {
            String createOperationCmd = String.Format("INSERT INTO operations (name, type, date) VALUES('{0}', '{1}', '{2}')", 
                name, operationType, DateTime.Now.ToString());
            cmd = createCommand(createOperationCmd);
            cmd.ExecuteNonQuery();
        }
    }
}

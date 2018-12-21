using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace try_bi
{
    public partial class w_edit_SPG_ID : Form
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String id_spg, nama_spg, sub_string, sub_string2, id_trans_line, id_trans;




        //==============================================================================================================
        private void combo_spg_SelectedIndexChanged(object sender, EventArgs e)
        {
            sub_string = combo_spg.Text;
            sub_string2 = sub_string.Substring(0, 9);
            //MessageBox.Show(" " + sub_string2);
            String sql = "UPDATE transaction_line SET SPG_ID = '" + sub_string2 + "' WHERE ARTICLE_ID='" + id_trans_line + "' AND TRANSACTION_ID='"+ id_trans +"'";
            CRUD update = new CRUD();
            update.ExecuteNonQuery(sql);
           
            uc_coba.Instance.retreive();
            
            this.Close();
        }
        //====================================================================================================================
        public w_edit_SPG_ID(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }

        private void w_edit_SPG_ID_Load(object sender, EventArgs e)
        {
            combo_spg.Items.Clear();
            isi_combo_spg();
        }
        //===========================METHOD ISI COMBO=============================================
        public void isi_combo_spg()
        {
            combo_spg.Items.Clear();
            //String sql = "SELECT employee.EMPLOYEE_ID, employee.NAME FROM employee INNER JOIN position ON employee.POSITION_ID = position._id WHERE position._id = '4' OR position._id = '3' OR position._id = '2'";
            //String sql = "SELECT * FROM employee WHERE POSITION_ID = '2' OR POSITION_ID = '3' OR POSITION_ID = '4'";
            String sql = "SELECT * FROM employee";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    //String name = ckon.myReader.GetString("ID_SPG");
                    //combo_spg.Items.Add(name);
                    id_spg = ckon.myReader.GetString("EMPLOYEE_ID");
                    nama_spg = ckon.myReader.GetString("NAME");
                    combo_spg.Items.Add(id_spg + "--" + nama_spg);
                }
                ckon.con.Close();
            }
            catch
            { MessageBox.Show("Data gagal ditambilkan untuk combobox"); }

        }
        //====================================================================================
        public void get_data(String id, String id_trans2)
        {
            id_trans_line = id;
            id_trans = id_trans2;
        }

        //===================CLOSE FORM BY CTRL + X==========================
        private void w_edit_SPG_ID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode.ToString() == "X")
            {
                this.Close();
            }
        }
        //================================================================
    }
}

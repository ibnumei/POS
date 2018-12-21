using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace try_bi
{
    public partial class Form_Login : Form
    {
        koneksi ckon = new koneksi();
        String nm_employe, id_employee;
        public Form_Login()
        {
            InitializeComponent();
        }



        private void Form_Login_Load(object sender, EventArgs e)
        {
            this.ActiveControl = t_username;
            t_username.Focus();
            t_pass.isPassword = true;
            //Form1 f1 = new Form1();
            //f1.Close();


        }

        private void b_login_Click(object sender, EventArgs e)
        {
            login();
            //this.Hide();
            ////Form_Popup fp = new Form_Popup();
            //Form1 fm1 = new Form1();
            ////fp.ShowDialog();
            //fm1.ShowDialog();
            //this.Close();
        }
        //===============================================================================
        private void b_close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //===LOGIN MENGGUNAKAN ENTER=============
        private void t_username_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                b_login_Click(null, null);
            }
        }
        //===LOGIN MENGGUNAKAN ENTER=============
        private void t_pass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                b_login_Click(null, null);
            }
        }

        //===================LINK GANTI PASSWORD=======================================
        private void l_changePass_Click(object sender, EventArgs e)
        {
            W_ChangePass change = new W_ChangePass();
            change.ShowDialog();
        }

        //==========================LOGIN================================================
        public void login()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM employee WHERE EMPLOYEE_ID='" + t_username.Text + "' and Pass = '"+ t_pass.Text +"'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            int count0 = 0;
            if(ckon.myReader.HasRows)
            {
                while(ckon.myReader.Read())
                {
                    count0 = count0 + 1;
                    nm_employe = ckon.myReader.GetString("NAME");
                    id_employee = ckon.myReader.GetString("EMPLOYEE_ID");
                }
                ckon.con.Close();
                if(count0 == 1)
                {
                    this.Hide();
                    Form1 fm1 = new Form1();
                    fm1.nama_employee = nm_employe;
                    fm1.id_employee = id_employee;
                    fm1.ShowDialog();
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Username or Password Not Valid");
            }
        }
        //===============================================================================
    }
}

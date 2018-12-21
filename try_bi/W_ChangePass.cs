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
    public partial class W_ChangePass : Form
    {
        koneksi ckon = new koneksi();
        String replace_pass;
        public W_ChangePass()
        {
            InitializeComponent();
        }

        private void W_ChangePass_Load(object sender, EventArgs e)
        {

        }
        //========================================================================
        private void b_login_Click(object sender, EventArgs e)
        {
            replace_pass = t_passBaru.Text.Replace(" ", "");
            cek_username();
        }
        //======================================================================
        private void b_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //=================CEK APAKAH USERNAME TERSEBUT DAN PASSWORD SAMA======
        public void cek_username()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM employee WHERE EMPLOYEE_ID='" + t_user.Text + "' and Pass = '" + t_passLama.Text + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            int count0 = 0;
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    count0 = count0 + 1;
                }
                ckon.con.Close();
                if (count0 == 1)
                {
                    cek_pass_baru();
                }
            }
            else
            {
                MessageBox.Show("Username or Password Not Valid");
            }
        }
        //==============CEK PASS BARU SAMA ATO TIDAK DENGAN PASS BARU 2
        public void cek_pass_baru()
        {
            if(t_passBaru.Text == t_passBaru2.Text)
            {
                //METHOD API CHANGE PASSWORD(UPDATE DATABASE)
                Post_ChangePass().Wait();
            }
            else
            {
                MessageBox.Show("Your New Password Not Match", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        //================================================================================
        public async Task Post_ChangePass()
        {
            ChangePass change = new ChangePass()
            {
                username = t_user.Text,
                currentpassword = t_passLama.Text,
                newpassword = replace_pass
            };
            var stringPayload = JsonConvert.SerializeObject(change);
            String response = "";
            var credentials = new NetworkCredential("username", "password");
            var handler = new HttpClientHandler { Credentials = credentials };
            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (var client = new HttpClient(handler))
            {
                try
                {
                    HttpResponseMessage message = client.PostAsync("http://mpos.biensicore.co.id/api/ChangePasswordAPI", httpContent).Result;
                    String sql= "UPDATE Employee Set Pass = '"+ replace_pass +"' where EMPLOYEE_ID='"+ t_user.Text +"'";
                    CRUD ganti = new CRUD();
                    ganti.ExecuteNonQuery(sql);
                    MessageBox.Show("Password Successfully Changed, Please Login Again", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Please Make Sure You're Connected To Internet", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }


        }


        //==========================================================================================
    }
}

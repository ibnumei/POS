using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace try_bi
{
    public partial class UC_RO_list : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String id_trans, article_id, jam, id_list, qty2, date2;
        //======================================================
        private static UC_RO_list _instance;
        public static UC_RO_list Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_RO_list(f1);
                return _instance;
            }
        }

        //=======================================================
        public UC_RO_list(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }



        //======================LIST HOLD TRANSACTION============================================
        public void holding(String query)
        {
            dgv_hold.Rows.Clear();
            koneksi2 ckon2 = new koneksi2();
            ckon.con.Close();

            //date = mydate.ToString("yyyy-MM-dd");
            String sql = query;
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            List<string> numbersList = new List<string>();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_trans = ckon.myReader.GetString("REQUEST_ORDER_ID");
                    jam = ckon.myReader.GetString("TIME");
                    String sql2 = "SELECT article.ARTICLE_NAME FROM requestorder_line, article  WHERE article.ARTICLE_ID = requestorder_line.ARTICLE_ID AND requestorder_line.REQUEST_ORDER_ID='" + id_trans + "'";
                    ckon2.cmd2 = new MySqlCommand(sql2, ckon2.con2);
                    ckon2.con2.Open();
                    ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
                    while (ckon2.myReader2.Read())
                    {
                        article_id = ckon2.myReader2.GetString("ARTICLE_NAME");
                        numbersList.Add(Convert.ToString(ckon2.myReader2["ARTICLE_NAME"]));
                    }
                    string[] numbersArray = numbersList.ToArray();
                    numbersList.Clear();
                    string result = String.Join(", ", numbersArray);
                    int n = dgv_hold.Rows.Add();
                    dgv_hold.Rows[n].Cells[0].Value = id_trans;
                    dgv_hold.Rows[n].Cells[1].Value = result;
                    dgv_hold.Rows[n].Cells[2].Value = jam; 
                    ckon2.con2.Close();
                }

            }
            ckon.con.Close();
        }



        //=============================================================================================

        //=====================KLIK HOLD MASUK KE KANAN================================================
        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            id_list = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            l_transaksi.Text = id_list;
            get_data();
            retreive();
        }
        //============================================================================================

        //====================GET DATA FROM ID =======================================================
        public void get_data()
        {
            int amount;
            String sql = "SELECT * FROM requestorder WHERE REQUEST_ORDER_ID ='"+ l_transaksi.Text +"'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    date2 = ckon.myReader.GetString("DATE");
                    qty2 = ckon.myReader.GetString("TOTAL_QTY");
                    t_desc.Text = ckon.myReader.GetString("DESCRIPTION");
                    amount = ckon.myReader.GetInt32("TOTAL_AMOUNT");
                    l_amount.Text = string.Format("{0:#,###}" + ",00", amount);
                    no_sj.Text = ckon.myReader.GetString("NO_SJ");
                }
                ckon.con.Close();
            }
            catch
            { }
            l_date.Text = date2;
            l_quantity.Text = qty2;
        }
        //============================================================================================
        //=====================ITEM REQUEST ORDER ==========================================
        public void retreive()
        {
            dgv_request.Rows.Clear();
            String sql = "SELECT  requestorder_line.ARTICLE_ID ,requestorder_line.QUANTITY, requestorder_line.UNIT, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE FROM requestorder_line, article  WHERE article.ARTICLE_ID = requestorder_line. ARTICLE_ID AND requestorder_line.REQUEST_ORDER_ID='" + l_transaksi.Text + "'ORDER BY requestorder_line._id ASC";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                ckon.adapter.Fill(ckon.dt);
                foreach (DataRow row in ckon.dt.Rows)
                {
                    int n = dgv_request.Rows.Add();
                    dgv_request.Rows[n].Cells[0].Value = row["ARTICLE_ID"].ToString();
                    dgv_request.Rows[n].Cells[1].Value = row["ARTICLE_NAME"].ToString();
                    dgv_request.Rows[n].Cells[2].Value = row["SIZE"].ToString();
                    dgv_request.Rows[n].Cells[3].Value = row["COLOR"].ToString();
                    dgv_request.Rows[n].Cells[4].Value = row["PRICE"];
                    dgv_request.Rows[n].Cells[5].Value = row["QUANTITY"].ToString();
                    dgv_request.Rows[n].Cells[6].Value = row["UNIT"];
                }
                dgv_request.Columns[4].DefaultCellStyle.Format = "#,###";

                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch
            { }
        }
        //==================================================================================

        //=============BUTTON SEARCH ===============================
        private void t_search_trans_OnTextChange(object sender, EventArgs e)
        {
            if(t_search_trans.text == "")
            {
                String sql = "SELECT * FROM requestorder WHERE  STATUS='1' AND DATE='" + tanggal_RO.Text + "'";
                holding(sql);
            }
            else
            {
                String sql = "SELECT * FROM requestorder WHERE  STATUS='1' AND REQUEST_ORDER_ID LIKE '%"+ t_search_trans.text + "%' AND DATE='" + tanggal_RO.Text + "'";
                holding(sql);
            }
        }
        //=====================================================
        private void tanggal_RO_ValueChanged(object sender, EventArgs e)
        {
            String SQL = "SELECT * FROM requestorder WHERE  STATUS='1' AND DATE='" + tanggal_RO.Text + "'";
            holding(SQL);
        }
        //================================================

        private void b_back_PC_Click(object sender, EventArgs e)
        {
            f1.p_kanan.Controls.Clear();
            if (!f1.p_kanan.Controls.Contains(UC_Req_order.Instance))
            {
                f1.p_kanan.Controls.Add(UC_Req_order.Instance);
                UC_Req_order.Instance.Dock = DockStyle.Fill;
                UC_Req_order.Instance.BringToFront();
            }
            else
                UC_Req_order.Instance.BringToFront();
        }
    }
}

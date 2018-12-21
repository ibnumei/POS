using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace try_bi
{
    public partial class UC_DO_List : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        DateTime mydate = DateTime.Now;
        String id_do, article_name, id_list, total_qty;
        //======================================================
        private static UC_DO_List _instance;
        public static UC_DO_List Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_DO_List(f1);
                return _instance;
            }
        }
        //=======================================================
        public UC_DO_List(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //=============memberi warna orange pada kolom dispute
        public void color_dgv()
        {
            dgv_inv.Columns[6].HeaderCell.Style.ForeColor = Color.Orange;
        }
        //================kembali ke halaman do confirmation
        private void b_back_DO_Click(object sender, EventArgs e)
        {
            f1.p_kanan.Controls.Clear();
            if (!f1.p_kanan.Controls.Contains(UC_DO.Instance))
            {
                f1.p_kanan.Controls.Add(UC_DO.Instance);
                UC_DO.Instance.Dock = DockStyle.Fill;
                UC_DO.Instance.BringToFront();
            }
            else
                UC_DO.Instance.BringToFront();
        }
        //======================LIST HOLD TRANSACTION============================================
        public void holding(String query)
        {
            ckon.con.Close();
            dgv_hold.Rows.Clear();
            koneksi2 ckon2 = new koneksi2();
            ckon.cmd = new MySqlCommand(query, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            List<string> numbersList = new List<string>();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_do = ckon.myReader.GetString("DELIVERY_ORDER_ID");
                    total_qty = ckon.myReader.GetString("TOTAL_QTY");
                    String sql2 = "SELECT article.ARTICLE_NAME FROM deliveryorder_line, article  WHERE article._id = deliveryorder_line.ARTICLE_ID AND deliveryorder_line.DELIVERY_ORDER_ID='" + id_do + "'";
                    ckon2.cmd2 = new MySqlCommand(sql2, ckon2.con2);
                    ckon2.con2.Open();
                    ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
                    while (ckon2.myReader2.Read())
                    {
                        article_name = ckon2.myReader2.GetString("ARTICLE_NAME");
                        numbersList.Add(Convert.ToString(ckon2.myReader2["ARTICLE_NAME"]));
                    }
                    string[] numbersArray = numbersList.ToArray();
                    numbersList.Clear();
                    string result = String.Join(", ", numbersArray);
                    int n = dgv_hold.Rows.Add();
                    dgv_hold.Rows[n].Cells[0].Value = id_do;
                    dgv_hold.Rows[n].Cells[1].Value = total_qty;
                    ckon2.con2.Close();
                }

            }
            ckon.con.Close();
        }
        //=============================================================================================

        //===============================KLIK HOLD DO =================================================
        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            id_list = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            l_transaksi.Text = id_list;
            retreive2(id_list);
            get_data(id_list);
        }
        //============================================================================================

        public void retreive2(String id_do_new)
        {
            ckon.con.Close();
            dgv_inv.Rows.Clear();

            String sql = "SELECT deliveryorder_line._id, deliveryorder_line.QTY_DELIVER, deliveryorder_line.QTY_RECEIVE, deliveryorder_line.QTY_DISPUTE,article.ARTICLE_ID,article.ARTICLE_NAME, article.SIZE, article.COLOR FROM deliveryorder_line, article WHERE deliveryorder_line.ARTICLE_ID = article._id AND deliveryorder_line.DELIVERY_ORDER_ID='" + id_do_new + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                ckon.adapter.Fill(ckon.dt);
                foreach (DataRow row in ckon.dt.Rows)
                {
                    int n = dgv_inv.Rows.Add();
                    dgv_inv.Rows[n].Cells[0].Value = row["_id"].ToString();
                    dgv_inv.Rows[n].Cells[1].Value = row["ARTICLE_ID"];
                    dgv_inv.Rows[n].Cells[2].Value = row["ARTICLE_NAME"].ToString();
                    dgv_inv.Rows[n].Cells[3].Value = row["SIZE"].ToString();
                    dgv_inv.Rows[n].Cells[4].Value = row["COLOR"].ToString();
                    dgv_inv.Rows[n].Cells[5].Value = row["QTY_DELIVER"];
                    dgv_inv.Rows[n].Cells[6].Value = row["QTY_RECEIVE"];
                    dgv_inv.Rows[n].Cells[7].Value = row["QTY_DISPUTE"];

                }
                //dgv_inv.Columns[3].DefaultCellStyle.ForeColor = Color.Orange;
                ckon.dt.Rows.Clear();
                ckon.con.Close();

            }
            catch
            { }
        }
        //==================================TEXTBOXT SEARCH====================================================
        private void t_search_trans_OnTextChange(object sender, EventArgs e)
        {
            if (t_search_trans.text == "")
            {
                String sql = "SELECT * FROM deliveryorder WHERE  STATUS='1' AND DATE='" + tanggal_MO.Text + "'";
                holding(sql);
            }
            else
            {
                String sql = "SELECT * FROM deliveryorder WHERE  STATUS='1' AND DELIVERY_ORDER_ID LIKE '%" + t_search_trans.text + "%' AND DATE='" + tanggal_MO.Text + "'";
                holding(sql);
            }
        }
    

        //============================================================================================
        //============================GET DATA FROM ID================================================
        public void get_data(String id)
        {
            int tot_amount;
            ckon.con.Close();
            String sql = "SELECT * FROM deliveryorder WHERE DELIVERY_ORDER_ID = '" + id + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                l_dev_date.Text = ckon.myReader.GetString("DELIVERY_DATE");
                l_qty.Text = ckon.myReader.GetString("TOTAL_QTY");
                tot_amount = ckon.myReader.GetInt32("TOTAL_AMOUNT");
                label4.Text = string.Format("{0:#,###}" + ",00", tot_amount);
            }
            ckon.con.Close();
        }
        //=============================================================================================
        private void tanggal_MO_ValueChanged(object sender, EventArgs e)
        {
            String SQL = "SELECT * FROM deliveryorder WHERE  STATUS='1' AND DATE='" + tanggal_MO.Text + "'";
            holding(SQL);
        }
        //=============================================================================================
    }
}

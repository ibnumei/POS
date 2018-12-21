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
using Tulpep.NotificationWindow;

namespace try_bi
{
    public partial class UC_DO : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        DateTime mydate = DateTime.Now;
        String id_do, article_name, id_list, epy_id, epy_name, date, total_qty;
        //======================================================
        private static UC_DO _instance;
        public static UC_DO Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_DO(f1);
                return _instance;
            }
        }
        //=======================================================
        public UC_DO(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //memberi warna orange pada kolom ke 6
        public void color_dgv()
        {
            
            dgv_inv.Columns[7].HeaderCell.Style.ForeColor = Color.Orange;
        }
        //-----melihat DO yang telah di konfirmasi, do list tampilkan sesuai tanggal hari ini, 
        private void b_do_list_Click(object sender, EventArgs e)
        {
            date = mydate.ToString("yyyy-MM-dd");
            String sql = "SELECT * FROM deliveryorder WHERE  STATUS='1' AND DATE='"+ date +"'";
            UC_DO_List c = new UC_DO_List(f1);
            f1.p_kanan.Controls.Clear();
            if (!f1.p_kanan.Controls.Contains(UC_DO_List.Instance))
            {
                f1.p_kanan.Controls.Add(UC_DO_List.Instance);
                UC_DO_List.Instance.Dock = DockStyle.Fill;
                UC_DO_List.Instance.holding(sql);
                UC_DO_List.Instance.color_dgv();
                UC_DO_List.Instance.Show();
            }
            else
                UC_DO_List.Instance.holding(sql);
            UC_DO_List.Instance.color_dgv();
            UC_DO_List.Instance.Show();
        }
        //===================GET EMPLOYE ID AND EMPLOYEE NAME dari form 1==============
        public void set_name(String id, String name)
        {
            epy_id = id;
            epy_name = name;
        }


        //======================LIST HOLD TRANSACTION============================================
        public void holding(String sql)
        {
            dgv_hold.Rows.Clear();
            koneksi2 ckon2 = new koneksi2();
            ckon.con.Close();
            //String sql = "SELECT * FROM deliveryorder WHERE STATUS='0' ";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
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

        //==================================KLIK HOLD TABEL, GET DATA AND RETREIVE DATA===============
        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            id_list = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            retreive2(id_list);
            l_transaksi.Text = id_list;
            get_data(id_list);
        }
        //============================================================================================

        //============================GET DATA FROM ID, untuk ditampilkan detail dari do header=======================
        public void get_data(String id)
        {
            int tot_amount;
            ckon.con.Close();
            String sql = "SELECT * FROM deliveryorder WHERE DELIVERY_ORDER_ID = '" + id + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            try
            {
                while (ckon.myReader.Read())
                {
                    l_dev_date.Text = ckon.myReader.GetString("DELIVERY_DATE");
                    l_qty.Text = ckon.myReader.GetString("TOTAL_QTY");
                    tot_amount = ckon.myReader.GetInt32("TOTAL_AMOUNT");
                    l_amount.Text = string.Format("{0:#,###}" + ",00", tot_amount);
                }
            }
            catch
            {
                tot_amount = 0;
                l_amount.Text = "0,00";
            }

            //if(tot_amount==0)
            //{

            //}
            //else
            //{

            //}
        }

        //=========menampilkan do line dengan detail dari article======================================== 
        public void retreive2(String id_do_new)
        {
            ckon.con.Close();
            dgv_inv.Rows.Clear();

            String sql = "SELECT deliveryorder_line._id, deliveryorder_line.QTY_DELIVER, deliveryorder_line.QTY_RECEIVE, deliveryorder_line.QTY_DISPUTE, deliveryorder_line.PACKING_NUMBER ,article.ARTICLE_ID,article.ARTICLE_NAME, article.SIZE, article.COLOR FROM deliveryorder_line, article WHERE deliveryorder_line.ARTICLE_ID = article._id AND deliveryorder_line.DELIVERY_ORDER_ID='" + id_do_new + "'";
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
                    dgv_inv.Rows[n].Cells[3].Value = row["PACKING_NUMBER"];
                    dgv_inv.Rows[n].Cells[4].Value = row["SIZE"].ToString();
                    dgv_inv.Rows[n].Cells[5].Value = row["COLOR"].ToString();
                    dgv_inv.Rows[n].Cells[6].Value = row["QTY_DELIVER"];
                    dgv_inv.Rows[n].Cells[7].Value = row["QTY_RECEIVE"];
                    dgv_inv.Rows[n].Cells[8].Value = row["QTY_DISPUTE"];

                }
                //dgv_inv.Columns[3].DefaultCellStyle.ForeColor = Color.Orange;
                ckon.dt.Rows.Clear();
                ckon.con.Close();

            }
            catch
            { }
        }
        //============================================================================================

        //====================================KLIK AMOUNT IN DO LINE AND OPEN POP UP WINDOWS========
        private void dgv_inv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv_inv.Columns[e.ColumnIndex].Name == "Column6")
            {
                String id = dgv_inv.Rows[e.RowIndex].Cells["Column8"].Value.ToString();
                String name = dgv_inv.Rows[e.RowIndex].Cells["Column2"].Value.ToString();
                String good = dgv_inv.Rows[e.RowIndex].Cells["Column3"].Value.ToString();
                String reject = dgv_inv.Rows[e.RowIndex].Cells["Column6"].Value.ToString();
                //String w_good = dgv_inv.Rows[e.RowIndex].Cells["Column5"].Value.ToString();
                //String w_reject = dgv_inv.Rows[e.RowIndex].Cells["Column6"].Value.ToString();
                //String total = dgv_inv.Rows[e.RowIndex].Cells["Column7"].Value.ToString();
                //String inv_article = dgv_inv.Rows[e.RowIndex].Cells["Column8"].Value.ToString();
                W_EditQty_DO edit_do = new W_EditQty_DO(f1);
                //stock.get_data(id, name, good, reject, w_good, w_reject, total, inv_article);
                edit_do.get_data(id, name, good, reject, id_list);
                edit_do.ShowDialog();
                //MessageBox.Show(" "+id+ " "+name);
            }
        }



        //====================================================================================
        private void tanggal_req_ValueChanged(object sender, EventArgs e)
        {
            String SQL = "SELECT * FROM deliveryorder WHERE  STATUS='0' AND DATE='" + tanggal_DO.Text + "'";
            holding(SQL);
        }
        //================================================================================================
        private void t_search_trans_OnTextChange(object sender, EventArgs e)
        {
            if (t_search_trans.text == "")
            {
                String sql = "SELECT * FROM deliveryorder WHERE  STATUS='0' AND DATE='" + tanggal_DO.Text + "'";
                holding(sql);
            }
            else
            {
                String sql = "SELECT * FROM deliveryorder WHERE  STATUS='0' AND DELIVERY_ORDER_ID LIKE '%" + t_search_trans.text + "%' AND DATE='" + tanggal_DO.Text + "'";
                holding(sql);
            }
        }

        //==========================================================================================

        private void b_confirm_Click(object sender, EventArgs e)
        {
            if (l_transaksi.Text == "-")
            {
                //PopupNotifier pop = new PopupNotifier();
                //pop.TitleText = "Warning";
                //pop.ContentText = "Your Cart Is Empety";
                //pop.Popup();
                MessageBox.Show("Your Cart Is Empety", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                /*
                Inv_Line inv = new Inv_Line();
                String type_trans = "2";
                inv.cek_type_trans(type_trans);
                inv.get_do_line(l_transaksi.Text);
                ckon.con.Close();
                String sql = "UPDATE deliveryorder SET STATUS='1', EMPLOYEE_ID = '"+ epy_id +"', EMPLOYEE_NAME='"+ epy_name +"' WHERE DELIVERY_ORDER_ID='" + l_transaksi.Text + "'";
                ckon.con.Open();
                ckon.cmd = new MySqlCommand(sql, ckon.con);
                ckon.cmd.ExecuteNonQuery();
                ckon.con.Close();
                reset();
                */
                /*DILAKUKAN PENGECEKAN APAKAH LINE DO YANG TAMPIL SUDAH SESUAI DENGAN SURAT JALAN DARI HO*/
                int total_sj=0, total_line=0;
                ckon.con.Close();
                string sql = "SELECT COUNT(*) as total_sj FROM deliveryorder_line where delivery_order_id = '" + l_transaksi.Text + "'";
                ckon.cmd = new MySqlCommand(sql, ckon.con);
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    total_sj = ckon.myReader.GetInt32("total_sj");
                }
                ckon.con.Close();

                string sql2 = "SELECT COUNT(*) as total_line FROM deliveryorder_line, article WHERE deliveryorder_line.ARTICLE_ID = article._id AND deliveryorder_line.DELIVERY_ORDER_ID='" + l_transaksi.Text + "';";
                ckon.cmd = new MySqlCommand(sql2, ckon.con);
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    total_line = ckon.myReader.GetInt32("total_line");
                }
                ckon.con.Close();

                if(total_line == total_sj)
                {
                    W_DO_Confirm confirm = new W_DO_Confirm();
                    confirm.simpan_do_header(l_transaksi.Text, epy_id, epy_name);
                    confirm.store();
                    //confirm.count_dispute(l_transaksi.Text);
                    confirm.retreive();
                    confirm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please get data Article first form POS Connector before confirm this DO", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
               
            }

        }
        //================================================================================================
        //====================PUBLIC RESET===============================================
        public void reset()
        {
            l_transaksi.Text = "-";
            l_dev_date.Text = "-";
            l_qty.Text = "0";
            dgv_inv.Rows.Clear();
            String sql2 = "SELECT * FROM deliveryorder WHERE STATUS='0' ";
            holding(sql2);
        }

        //===============================================================================

    }
}

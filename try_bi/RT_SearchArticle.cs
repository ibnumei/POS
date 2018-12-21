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
    public partial class RT_SearchArticle : Form
    {
        public String t_id, S_barcode, S_article, S_nama, S_price, S_ID, tgl_req, store_code2, cust_id_store2, _id, good_qty, unit, bulan2,no_sj;
        int new_price, number_trans;
        public int i, new_jumlah, new_harga, totall, price;
        koneksi ckon = new koneksi();
        koneksi2 ckon2 = new koneksi2();

        public RT_SearchArticle()
        {
            InitializeComponent();
        }
        public void save_auto_number(String bulan, int number)
        {
            bulan2 = bulan;
            number_trans = number;
        }
        //============================LOAD FORM==================================
        private void RT_SearchArticle_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.OnnOrOff == "Offline")
            {
                //====HIT INVEN LOCAL==========================
                API_Inventory InvLocal = new API_Inventory();
                InvLocal.SetInvHitLocal();
                //=============================================
                dgv_2.EnableHeadersVisualStyles = false;
                String sql = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.GOOD_QTY >=1 LIMIT 100";
                get_load_data(sql);
                dgv_2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv_2.MultiSelect = true;
            }
            else
            {
                dgv_2.EnableHeadersVisualStyles = false;
                String sql = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.GOOD_QTY >=1 LIMIT 100";
                get_load_data(sql);
                dgv_2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv_2.MultiSelect = true;
            }
            
        }
        //======================================================================

        //======================RETREIVE DATA ===============================================
        public void get_load_data(String query)
        {
            dgv_2.Rows.Clear();
            string sql = query;
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                ckon.adapter.Fill(ckon.dt);

                foreach (DataRow row in ckon.dt.Rows)
                {
                    int n = dgv_2.Rows.Add();
                    dgv_2.Rows[n].Cells[0].Value = row["_id"];
                    dgv_2.Rows[n].Cells[1].Value = row["ARTICLE_ID"].ToString();
                    dgv_2.Rows[n].Cells[2].Value = row["ARTICLE_NAME"].ToString();
                    dgv_2.Rows[n].Cells[3].Value = row["SIZE"].ToString();
                    dgv_2.Rows[n].Cells[4].Value = row["COLOR"].ToString();
                    dgv_2.Rows[n].Cells[5].Value = row["PRICE"];
                    dgv_2.Rows[n].Cells[6].Value = row["GOOD_QTY"];
                }
                dgv_2.Columns[5].DefaultCellStyle.Format = "#,###";
                //con.Close();
                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ckon.con.Close();
            }

        }
        //===================================================================================

        //=======================COMBOBOX KATEGORI =========================================
        private void combo_ktg2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo_ktg2.Text == "ALL")
            {
                combo_value.Items.Clear();
                String sql = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.GOOD_QTY >=1 LIMIT 100";
                get_load_data(sql);
                combo_value.Text = "ALL";
            }
            else if (combo_ktg2.Text == "Brand")
            {
                String query = "SELECT * FROM brand";
                isi_combo_value(query);
                combo_value.Text = "3 SECOND";

            }
            else if (combo_ktg2.Text == "Department")
            {
                String query = "SELECT * FROM departement";
                isi_combo_value(query);
                combo_value.Text = "Shirt";
            }
            else if (combo_ktg2.Text == "Department_Type")
            {
                String query = "SELECT * FROM departementtype";
                isi_combo_value(query);
                combo_value.Text = "Denim";
            }
            else if (combo_ktg2.Text == "Size")
            {
                String query = "SELECT * FROM Size";
                isi_combo_value(query);
                combo_value.Text = "L";
            }
            else if (combo_ktg2.Text == "Color")
            {
                String query = "SELECT * FROM Color";
                isi_combo_value(query);
                combo_value.Text = "Blue";
            }
            else if (combo_ktg2.Text == "Gender")
            {
                String query = "SELECT * FROM Gender";
                isi_combo_value(query);
                combo_value.Text = "Men";
            }
            else
            { }
        }
        //===================================================================================

        //========================ISI VALUE COMBO =======================================
        public void isi_combo_value(String query)
        {
            combo_value.Items.Clear();
            String sql = query;
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    String name = ckon.myReader.GetString("DESCRIPTION");
                    combo_value.Items.Add(name);
                }
                ckon.con.Close();

            }
            catch
            { }
        }
        //=============================================================================

        //=================COMBO VALUE DIILIH =========================================
        private void combo_value_SelectedIndexChanged(object sender, EventArgs e)
        {
            String sql = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE  article." + combo_ktg2.Text + " = '" + combo_value.Text + "' AND inventory.GOOD_QTY >=1 LIMIT 100";
            get_load_data(sql);
        }
        //=============================================================================

        private void t_find_article_OnTextChange(object sender, EventArgs e)
        {
            String count_article = t_find_article.text;
            int count_article_int = count_article.Count();
            if (count_article_int < 5)
            {
                if (t_find_article.text == "")
                {

                    String sql2a = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.GOOD_QTY >=1  LIMIT 100";
                    get_load_data(sql2a);

                }
                if (t_find_article.text == "" && (combo_ktg2.Text == "Brand" || combo_ktg2.Text == "Department" || combo_ktg2.Text == "Department_Type" || combo_ktg2.Text == "Size" || combo_ktg2.Text == "Color" || combo_ktg2.Text == "Gender"))
                {
                    String sql4 = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.GOOD_QTY >=1 AND article." + combo_ktg2.Text + " = '" + combo_value.Text + "' LIMIT 100";
                    get_load_data(sql4);
                    ckon.con.Close();
                }
            }
            if (count_article_int >= 5)
            {
                if (t_find_article.text != "")
                {
                    String sql2 = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.GOOD_QTY >=1 AND (article.ARTICLE_ID LIKE '%" + t_find_article.text + "%' OR article.ARTICLE_NAME LIKE '%" + t_find_article.text + "%' ) LIMIT 100";
                    get_load_data(sql2);

                }
                if (t_find_article.text != "" && (combo_ktg2.Text == "Brand" || combo_ktg2.Text == "Department" || combo_ktg2.Text == "Department_Type" || combo_ktg2.Text == "Size" || combo_ktg2.Text == "Color" || combo_ktg2.Text == "Gender"))
                {

                    String sql3 = "select article._id,article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE article." + combo_ktg2.Text + " = '" + combo_value.Text + "' AND inventory.GOOD_QTY >=1 AND(article.ARTICLE_ID LIKE '%" + t_find_article.text + "%' OR article.ARTICLE_NAME LIKE '%" + t_find_article.text + "%' ) LIMIT 100";
                    get_load_data(sql3);
                }
            }
                
           
           
        }
        //================================================================================

        //======================SIMPAN TRANSACTION HEADER============================================
        public void save_trans_header()
        {
            DateTime mydate = DateTime.Now;
            DateTime myhour = DateTime.Now;
            ckon.con.Close();
            String sql0 = "SELECT * FROM returnorder WHERE RETURN_ORDER_ID ='" + t_id + "'";
            ckon.cmd = new MySqlCommand(sql0, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            int count0 = 0;
            while (ckon.myReader.Read())
            {
                count0 = count0 + 1;
            }
            ckon.con.Close();
            if (count0 == 0)
            {
                String sql = "INSERT INTO returnorder (RETURN_ORDER_ID,STORE_CODE,TOTAL_QTY,STATUS, DATE, TIME,CUST_ID_STORE,NO_SJ) VALUES ('" + t_id + "', '"+ store_code2 +"' ,'0','0','" + mydate.ToString("yyyy-MM-dd") + "', '" + myhour.ToLocalTime().ToString("H:mm:ss") + "','"+ cust_id_store2 +"','"+ no_sj +"') ";
                ckon.con.Open();
                ckon.cmd = new MySqlCommand(sql, ckon.con);
                ckon.cmd.ExecuteNonQuery();
                ckon.con.Close();

                String query = "UPDATE auto_number SET Month = '" + bulan2 + "', Number = '" + number_trans + "' WHERE Type_Trans='4'";
                CRUD ubah = new CRUD();
                ubah.ExecuteNonQuery(query);
            }
            else
            {

            }

        }
        //==========================================================================================
        //================DATAGRIDVIEW TO DATABASE =========================================
        public void cek_article2()
        {
            int total_amount;
            //mendapatkan good_qty dari tabel inventory berdasarkan (_id)
            ckon.con.Close();
            String query = "SELECT * FROM article WHERE ARTICLE_ID = '" + S_ID + "'";
            ckon.cmd = new MySqlCommand(query, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    _id = ckon.myReader.GetString("_id");
                    unit = ckon.myReader.GetString("UNIT");
                    //===CARI TOTAL QTY BERDASARKAN _id yang didapat ke tabel inventory==
                    String query2 = "SELECT * FROM inventory WHERE ARTICLE_ID = '" + _id + "'";
                    ckon2.cmd2 = new MySqlCommand(query2, ckon2.con2);
                    ckon2.con2.Open();
                    ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
                    while (ckon2.myReader2.Read())
                    {
                        good_qty = ckon2.myReader2.GetString("GOOD_QTY");
                    }
                    ckon2.con2.Close();
                }
            ckon.con.Close();
            //====================================================
            new_price = int.Parse(S_price);
            total_amount = new_price * Convert.ToInt32(good_qty);
            string j = "1";
            //i = 0;
            string sql = "SELECT * FROM returnorder_line WHERE RETURN_ORDER_ID ='" + t_id + "' AND ARTICLE_ID='" + S_ID + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            int count = 0;
            while (ckon.myReader.Read())
            {
                count = count + 1;
                new_jumlah = ckon.myReader.GetInt32("QUANTITY");
                //new_harga = ckon.myReader.GetInt32("SUBTOTAL");
            }
            ckon.con.Close();
            if (count == 0)
            {
                string sql2 = "INSERT INTO returnorder_line (RETURN_ORDER_ID,ARTICLE_ID,QUANTITY,UNIT, SUBTOTAL) VALUES ('" + t_id + "','" + S_ID + "', '" + good_qty + "', '"+ unit +"','"+ total_amount +"')";
                CRUD input = new CRUD();
                input.ExecuteNonQuery(sql2);
                save_trans_header();
                back_uc();
            }
            else
            {
                //new_jumlah = new_jumlah + 1;
                //new_harga = new_harga + new_price;
                //String sql3 = "UPDATE returnorder_line SET QUANTITY='" + new_jumlah + "' WHERE RETURN_ORDER_ID='" + t_id + "' AND ARTICLE_ID='" + S_ID + "'";
                //CRUD input = new CRUD();
                //input.ExecuteNonQuery(sql3);
                //this.Close();
                MessageBox.Show("This Article Has Been Entered");
                
            }
        }
        //===================================================================================

        //======================MASUK KO FORM UTAMA=====================================
        public void back_uc()
        {
            Form1 f1 = new Form1();
            UC_Ret_order.Instance.retreive();
            UC_Ret_order.Instance.qty();
            this.Close();
        }
        //=============================================================================

        private void dgv_2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                _id = dgv_2.SelectedRows[0].Cells[0].Value.ToString();
                S_ID = dgv_2.SelectedRows[0].Cells[1].Value.ToString();
                S_price = dgv_2.SelectedRows[0].Cells[5].Value.ToString();

                cek_article2();
                //save_trans_header();
                //back_uc();
            }
            catch
            {
                MessageBox.Show("Select a value in the table");
            }
           
        }
    }
}

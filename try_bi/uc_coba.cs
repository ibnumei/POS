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
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;


namespace try_bi
{
    public partial class uc_coba : UserControl
    {
        public static Form1 f1;
        public String t_id, S_Articleid, S_article, S_nama, S_price, id_trans, article_id, date, nm_cur2, cust_id_store2, shift_name_trans, id_employe2;
        int noo_inv_new, status_diskon_api, subtotal, qty = 1, disc = 0, new_disc, tot_diskon, no_trans2;
        koneksi ckon = new koneksi();
        koneksi2 ckon2 = new koneksi2();
        String id_spg, nama_spg, sub_string, sub_string2, store_code, id_shift, id_CStore, article_id_API, id_trans_line, discount_code_get, id_inv, customer, code_store, disc_code, disc_type, bulan2, tipe2, disc_desc1, VarBackDate, DateHeaderTrans;
        //Variable untuk running number baru
        String bulan_now, tahun_now, bulan_trans, number_trans_string, final_running_number;
        int number_trans;
        //================DGV WARNA===========================
        String kd_diskon, status;

        private void label5_Click(object sender, EventArgs e)
        {
         
        }

        //====================================================
        //======================================================
        private static uc_coba _instance;
        public static uc_coba Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new uc_coba(f1);
                return _instance;
            }
        }
        //=======================================================
        string id_list;
        public int i, new_jumlah, new_harga, totall, price, totall_real, get_diskon, get_voucher, get_dis_vou;
        public uc_coba(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
            dgv_purchase.AllowUserToAddRows = false;
        }
        //================CEK TANGGAL APAKAH BACKDATE ATAU TIDAK==============================
        public void cekBackDate()
        {
           
        }
        //====================================================================================
        //================MEMBERIKAN AUTOFOKUS KE SCAN BARCODE SAAT PEMILIHAN SPG DI PILIH====
        private void combo_spg_SelectedIndexChanged(object sender, EventArgs e)
        {
            //===FOKUES KE SCAN BARCODE
            this.ActiveControl = t_barcode;
            t_barcode.Focus();
        }
        //=================GET NUMBER RUNNING=========================
        /*
        public void get_running_number(String number, String bulan, int no_trans, String tipe)
        {
            l_transaksi.Text = number;
            bulan2 = bulan;
            no_trans2 = no_trans;
            tipe2 = tipe;
        }
        */
        //=================================GENERATOR NUMBER=================================
        public void new_invoice()
        {
            dgv_purchase.Rows.Clear(); t_custId.Text = ""; l_total.Text = "0,00"; l_diskon.Text = "0,00";
            this.ActiveControl = t_custId;
            t_custId.Focus();
            //===================AMBIL ID DARI TABEL CLOSING SHIFT===================
            ckon.con.Close();
            String sql3 = "SELECT * FROM closing_shift ORDER BY _id DESC LIMIT 1";
            ckon.cmd = new MySqlCommand(sql3, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_shift = ckon.myReader.GetString("ID_SHIFT");
                }
            }
            ckon.con.Close();
            //==================AMBIL ID DARI TABEL CLOSING STORE=====================
            ckon.con.Close();
            String sql4 = "SELECT * FROM closing_store ORDER BY _id DESC LIMIT 1";
            ckon.cmd = new MySqlCommand(sql4, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_CStore = ckon.myReader.GetString("ID_C_STORE");
                }
            }
            ckon.con.Close();
            //==================AMBIL ID DARI TABLE STORE=============================
            ckon.con.Close();
            String sql2 = "SELECT * FROM store";
            ckon.cmd = new MySqlCommand(sql2, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                if (ckon.myReader.HasRows)
                {
                    while (ckon.myReader.Read())
                    {
                        store_code = ckon.myReader.GetString("CODE");
                    }
                }
                ckon.con.Close();
            }
            catch
            { MessageBox.Show("Failed when get data from store data"); }
           
        }
        //=================================================================================================

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
                    id_spg = ckon.myReader.GetString("EMPLOYEE_ID");
                    nama_spg = ckon.myReader.GetString("NAME");
                    combo_spg.Items.Add(id_spg + " -- " + nama_spg);
                }
                ckon.con.Close();
                combo_spg.SelectedIndex = 0;
            }
            catch
            {
            }

        }
        //===HAPUS DATA DI GRIDVIEW==================================
        public void delete_rows()
        {
            dgv_diskon.Rows.Clear();
            dgv_purchase.Rows.Clear();
        }

        //===============TAMPILKAN DATA PENJUALAN===================================================
        public void retreive()
        {
            String art_id, art_name, spg_id, size, color, qty, disc_desc, sub_total2;
            int price, sub_total, disc;

            dgv_diskon.Rows.Clear();
            Transaction transaction = new Transaction();
            transaction.storeCode = store_code;
            transaction.customerId = t_custId.Text;
            List<TransactionLine> transLine = new List<TransactionLine>();


            dgv_purchase.Rows.Clear();
            String sql = "SELECT transaction_line._id ,transaction_line.ARTICLE_ID ,transaction_line.QUANTITY, transaction_line.SUBTOTAL, transaction_line.SPG_ID, transaction_line.DISCOUNT, transaction_line.DISCOUNT_DESC, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE FROM transaction_line, article  WHERE article.ARTICLE_ID = transaction_line.ARTICLE_ID AND transaction_line.TRANSACTION_ID='" + l_transaksi.Text + "' ORDER BY transaction_line._id ASC";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                //=====================================================================================
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    article_id_API = ckon.myReader.GetString("ARTICLE_ID");
                    art_id = ckon.myReader.GetString("ARTICLE_ID");
                    art_name = ckon.myReader.GetString("ARTICLE_NAME");
                    spg_id = ckon.myReader.GetString("SPG_ID");
                    size = ckon.myReader.GetString("SIZE");
                    color = ckon.myReader.GetString("COLOR");
                    price = ckon.myReader.GetInt32("PRICE");
                    qty = ckon.myReader.GetString("QUANTITY");
                    disc_desc = ckon.myReader.GetString("DISCOUNT_DESC");
                    sub_total = ckon.myReader.GetInt32("SUBTOTAL");
                    disc = ckon.myReader.GetInt32("DISCOUNT");

                    if (sub_total == 0)
                    {
                        sub_total2 = "0,00";
                        int n = dgv_purchase.Rows.Add();
                        dgv_purchase.Rows[n].Cells[0].Value = art_id;
                        dgv_purchase.Rows[n].Cells[1].Value = art_name;
                        dgv_purchase.Rows[n].Cells[2].Value = spg_id;
                        dgv_purchase.Rows[n].Cells[3].Value = size;
                        dgv_purchase.Rows[n].Cells[4].Value = color;
                        dgv_purchase.Rows[n].Cells[5].Value = price;
                        dgv_purchase.Rows[n].Cells[6].Value = qty;
                        dgv_purchase.Rows[n].Cells[7].Value = disc_desc;
                        dgv_purchase.Rows[n].Cells[8].Value = sub_total2;
                    }
                    else
                    {

                        int n = dgv_purchase.Rows.Add();
                        dgv_purchase.Rows[n].Cells[0].Value = art_id;
                        dgv_purchase.Rows[n].Cells[1].Value = art_name;
                        dgv_purchase.Rows[n].Cells[2].Value = spg_id;
                        dgv_purchase.Rows[n].Cells[3].Value = size;
                        dgv_purchase.Rows[n].Cells[4].Value = color;
                        dgv_purchase.Rows[n].Cells[5].Value = price;
                        dgv_purchase.Rows[n].Cells[6].Value = qty;
                        dgv_purchase.Rows[n].Cells[7].Value = disc_desc;
                        dgv_purchase.Rows[n].Cells[8].Value = sub_total;
                    }
                    //==============================================================================
                    string query = "SELECT * FROM ARTICLE WHERE ARTICLE_ID='" + article_id_API + "'";
                    ckon2.cmd2 = new MySqlCommand(query, ckon2.con2);
                    ckon2.con2.Open();
                    ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
                    Article articleFromDb = new Article();
                    while (ckon2.myReader2.Read())
                    {

                        articleFromDb.articleId = ckon2.myReader2.GetString("ARTICLE_ID");
                        articleFromDb.articleName = ckon2.myReader2.GetString("ARTICLE_NAME");
                        articleFromDb.brand = ckon2.myReader2.GetString("BRAND");
                        articleFromDb.color = ckon2.myReader2.GetString("COLOR");
                        articleFromDb.department = ckon2.myReader2.GetString("DEPARTMENT");
                        articleFromDb.departmentType = ckon2.myReader2.GetString("DEPARTMENT_TYPE");
                        articleFromDb.gender = ckon2.myReader2.GetString("GENDER");
                        articleFromDb.id = ckon2.myReader2.GetInt32("_id");
                        articleFromDb.price = ckon2.myReader2.GetInt32("PRICE");
                        articleFromDb.size = ckon2.myReader2.GetString("SIZE");
                        articleFromDb.unit = ckon2.myReader2.GetString("UNIT");
                        articleFromDb.articleIdAlias = ckon2.myReader2.GetString("ARTICLE_ID_ALIAS");
                    }
                    ckon2.con2.Close();
                    //======================================================================
                    TransactionLine t = new TransactionLine();

                    t.discount = disc;
                    t.subtotal = sub_total;
                    t.quantity = Int32.Parse(qty);
                    t.price = price;

                    t.article = articleFromDb;
                    transLine.Add(t);
                }
                //=====================================================================================
                //ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                //ckon.adapter.Fill(ckon.dt);
                //foreach (DataRow row in ckon.dt.Rows)
                //{

                //    int n = dgv_purchase.Rows.Add();
                //    article_id_API = row["ARTICLE_ID"].ToString();//satu
                //    dgv_purchase.Rows[n].Cells[0].Value = row["ARTICLE_ID"].ToString();
                //    dgv_purchase.Rows[n].Cells[1].Value = row["ARTICLE_NAME"].ToString();
                //    dgv_purchase.Rows[n].Cells[2].Value = row["SPG_ID"];
                //    dgv_purchase.Rows[n].Cells[3].Value = row["SIZE"].ToString();
                //    dgv_purchase.Rows[n].Cells[4].Value = row["COLOR"].ToString();
                //    dgv_purchase.Rows[n].Cells[5].Value = row["PRICE"];
                //    dgv_purchase.Rows[n].Cells[6].Value = row["QUANTITY"].ToString();
                //    dgv_purchase.Rows[n].Cells[7].Value = row["DISCOUNT_DESC"];
                //    dgv_purchase.Rows[n].Cells[8].Value = row["SUBTOTAL"];
                //    //=========================================
                //    string query = "SELECT * FROM ARTICLE WHERE ARTICLE_ID='" + article_id_API + "'";
                //    ckon2.cmd2 = new MySqlCommand(query, ckon2.con2);
                //    ckon2.con2.Open();
                //    ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
                //    Article articleFromDb = new Article();
                //    while (ckon2.myReader2.Read())
                //    {

                //        articleFromDb.articleId = ckon2.myReader2.GetString("ARTICLE_ID");
                //        articleFromDb.articleName = ckon2.myReader2.GetString("ARTICLE_NAME");
                //        articleFromDb.brand = ckon2.myReader2.GetString("BRAND");
                //        articleFromDb.color = ckon2.myReader2.GetString("COLOR");
                //        articleFromDb.department = ckon2.myReader2.GetString("DEPARTMENT");
                //        articleFromDb.departmentType = ckon2.myReader2.GetString("DEPARTMENT_TYPE");
                //        articleFromDb.gender = ckon2.myReader2.GetString("GENDER");
                //        articleFromDb.id = ckon2.myReader2.GetInt32("_id");
                //        articleFromDb.price = ckon2.myReader2.GetInt32("PRICE");
                //        articleFromDb.size = ckon2.myReader2.GetString("SIZE");
                //        articleFromDb.unit = ckon2.myReader2.GetString("UNIT");

                //    }
                //    ckon2.con2.Close();
                //    //=========================================
                //    TransactionLine t = new TransactionLine();
                //    t.discount = Int32.Parse(row["DISCOUNT"]+"");
                //    t.subtotal = Int32.Parse(row["SUBTOTAL"] + "");
                //    t.quantity = Int32.Parse(row["QUANTITY"] + "");
                //    t.price = Int32.Parse(row["PRICE"] + "");

                //    t.article = articleFromDb;
                //    transLine.Add(t);

                //}
                transaction.transactionLines = transLine;
                dgv_purchase.Columns[5].DefaultCellStyle.Format = "#,###";
                dgv_purchase.Columns[7].DefaultCellStyle.Format = "#,###";
                dgv_purchase.Columns[8].DefaultCellStyle.Format = "#,###";
                ckon.dt.Rows.Clear();
                ckon.con.Close();
                /*
                //call discount here
                var stringPayload = JsonConvert.SerializeObject(transaction);
                String response = "";
                var credentials = new NetworkCredential("username", "password");
                var handler = new HttpClientHandler { Credentials = credentials };
                var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
                using (var client = new HttpClient(handler))
                {
                    HttpResponseMessage message = client.PostAsync("http://retailbiensi.azurewebsites.net/api/Discount", httpContent).Result;
                    if (message.Content != null)
                    {
                        // GET RETURN VALUE FROM POST API

                        var serializer = new DataContractJsonSerializer(typeof(DiscountHeader));
                        var responseContent = message.Content.ReadAsStringAsync().Result;
                        byte[] byteArray = Encoding.UTF8.GetBytes(responseContent);
                        MemoryStream stream = new MemoryStream(byteArray);
                        DiscountHeader resultData = serializer.ReadObject(stream) as DiscountHeader;

                        //for (int i = 0; i < resultData.Count; i++)
                        //{
                        //    discount_code_get = resultData[i].discountCode;
                        //    data_diskon(discount_code_get);
                        //}
                        //=================================================
                        foreach (var c in resultData.discounts)
                        {
                            discount_code_get = c.discountCode;
                            status_diskon_api = c.status;
                            String art_diskon = c.articleId;
                            int disc_type = c.discountType;
                            //=================UPADTE STATUS DISKON KE DATABASE LOKAL(PROMOTION HEADER)=============
                            String sql2 = "UPDATE promotion SET STATUS='" + status_diskon_api + "', ARTICLE_ID='"+ art_diskon +"', DISCOUNT_TYPE='"+ disc_type +"', DISCOUNT_DESC = '"+ c.discountDesc +"' WHERE DISCOUNT_CODE='" + discount_code_get + "'";
                            CRUD UPDATE = new CRUD();
                            UPDATE.ExecuteNonQuery(sql2);
                            //==============UPDATE DATA DISKON_PERCENT DAN DISKON_PRICE (PROMOTION LINES)
                            String sql3 = "UPDATE promotion_line SET DISCOUNT_PERCENT = '"+ c.discountPercent +"', DISCOUNT_PRICE = '"+ c.discountAmount +"' WHERE DISCOUNT_CODE = '"+ discount_code_get +"'";
                            CRUD UPDATE2 = new CRUD();
                            UPDATE2.ExecuteNonQuery(sql3);
                            //=====================================================================
                            data_diskon(discount_code_get);
                           
                        }
                        //=================================================
                    }
                }
                */

                BiensiPosDbContext.BiensiPosDbDataContext contex = new BiensiPosDbContext.BiensiPosDbDataContext();
                DiscountCalculate dc = new DiscountCalculate(contex);
                DiscountMaster resultData = dc.Post(transaction);
                Console.WriteLine(JsonConvert.SerializeObject(transaction));
                //=================================================
                //for (int i = 0; i < resultData.discounts.Count; i++)
                //{
                //    discount_code_get = resultData.discounts[i].discountCode;
                //    data_diskon(discount_code_get);
                //}
                foreach (var c in resultData.discounts)
                {
                    discount_code_get = c.discountCode;
                    status_diskon_api = c.status;
                    String art_diskon = c.articleId;
                    int disc_type = c.discountType;
                    //=================UPADTE STATUS DISKON KE DATABASE LOKAL(PROMOTION HEADER)=============
                    String sql2 = "UPDATE promotion SET STATUS='" + status_diskon_api + "', ARTICLE_ID='" + art_diskon + "', DISCOUNT_TYPE='" + disc_type + "', DISCOUNT_DESC = '" + c.discountDesc + "' WHERE DISCOUNT_CODE='" + discount_code_get + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql2);
                    //==============UPDATE DATA DISKON_PERCENT DAN DISKON_PRICE (PROMOTION LINES)
                    String sql3 = "UPDATE promotion_line SET DISCOUNT_PERCENT = '" + c.discountPercent + "', DISCOUNT_PRICE = '" + c.discountAmount + "' WHERE DISCOUNT_CODE = '" + discount_code_get + "'";
                    CRUD UPDATE2 = new CRUD();
                    UPDATE2.ExecuteNonQuery(sql3);
                    //=====================================================================
                    data_diskon(discount_code_get);

                }

            }
            catch
            { }

        }
        //===================================================================================
        public async Task Post_Discount()
        {
            String S_ID = t_barcode.Text;
            Transaction transaction = new Transaction();
            transaction.storeCode = store_code;
            transaction.customerId = t_custId.Text;
            List<TransactionLine> transLine = new List<TransactionLine>();
            //==================================================================================
            string query = "SELECT * FROM ARTICLE WHERE ARTICLE_ID='" + S_ID + "'";
            ckon2.cmd2 = new MySqlCommand(query, ckon2.con2);
            ckon2.con2.Open();
            ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
            Article articleFromDb = new Article();
            while (ckon2.myReader2.Read())
            {
                articleFromDb.articleId = ckon2.myReader2.GetString("ARTICLE_ID");
                articleFromDb.articleName = ckon2.myReader2.GetString("ARTICLE_NAME");
                articleFromDb.brand = ckon2.myReader2.GetString("BRAND");
                articleFromDb.color = ckon2.myReader2.GetString("COLOR");
                articleFromDb.department = ckon2.myReader2.GetString("DEPARTMENT");
                articleFromDb.departmentType = ckon2.myReader2.GetString("DEPARTMENT_TYPE");
                articleFromDb.gender = ckon2.myReader2.GetString("GENDER");
                articleFromDb.id = ckon2.myReader2.GetInt32("_id");
                articleFromDb.price = ckon2.myReader2.GetInt32("PRICE");
                articleFromDb.size = ckon2.myReader2.GetString("SIZE");
                articleFromDb.unit = ckon2.myReader2.GetString("UNIT");
                subtotal = ckon2.myReader2.GetInt32("PRICE");
                articleFromDb.articleIdAlias = ckon2.myReader2.GetString("ARTICLE_ID_ALIAS");
            }
            ckon2.con2.Close();
            //====================================================================
            TransactionLine t = new TransactionLine();
            t.subtotal = subtotal;
            t.quantity = qty;
            t.discount = 0;
            t.price = subtotal;

            t.article = articleFromDb;
            transLine.Add(t);

            transaction.transactionLines = transLine;
            BiensiPosDbContext.BiensiPosDbDataContext contex = new BiensiPosDbContext.BiensiPosDbDataContext();
            DiscountCalculate dc = new DiscountCalculate(contex);
            DiscountMaster resultData = dc.Post(transaction);
            //=================================================
            foreach (var c in resultData.discountItems)
            {
                disc = (Int32)c.amountDiscount;
                disc_code = c.discountCode;
                disc_type = c.discountType;
                disc_desc1 = c.discountDesc;
                //MessageBox.Show("" + disc.ToString());
            }
        }
        //===========================ITUNG TOTAL BELANJA=====================================================
        public void itung_total()
        {
            ckon.con.Close();
            //==DIGUNAKAN UNTUNG MENGHITUNG TOTAL DISCOUNT DARI TRANS_LINE, LALU MASUKAN TOTAL KE DALAM TRANS_HEADER
            String sql2 = "SELECT SUM(transaction_line.DISCOUNT) as total FROM transaction_line WHERE TRANSACTION_ID='" + l_transaksi.Text + "'";
            ckon.cmd = new MySqlCommand(sql2, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    tot_diskon = ckon.myReader.GetInt32("total");
                    //ganti nilai discount di transaksi header
                    //String query = "UPDATE transaction SET DISCOUNT='"+ tot_diskon +"' WHERE TRANSACTION_ID='"+ l_transaksi.Text +"'";
                    //CRUD replace = new CRUD();
                    //replace.ExecuteNonQuery(query);

                    //get_dis_vou = ckon.myReader.GetInt32("total");
                    //if (get_dis_vou == 0)
                    //{ l_diskon.Text = "0"; }
                    //else
                    //{ l_diskon.Text = string.Format("{0:#,###}" + ",00", get_dis_vou); }

                }
                ckon.con.Close();
            }
            catch
            {
                tot_diskon = 0;

                //String query = "UPDATE transaction SET DISCOUNT='0' WHERE TRANSACTION_ID='" + l_transaksi.Text + "'";
                //CRUD replace = new CRUD();
                //replace.ExecuteNonQuery(query);
                //l_diskon.Text = "0,00"; 
            }
            //=====================================GET VALUE DISCOUNT FROM TRANSACTION HEADER======================
            String query2 = "SELECT * FROM TRANSACTION WHERE TRANSACTION_ID ='" + l_transaksi.Text + "'";
            ckon.cmd = new MySqlCommand(query2, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    //get_diskon = ckon.myReader.GetInt32("DISCOUNT");
                    get_voucher = ckon.myReader.GetInt32("VOUCHER");
                }
                get_dis_vou = tot_diskon + get_voucher;
                //get_dis_vou = get_voucher;
                ckon.con.Close();
                //============PERCABANGAN UNTUK LABEL DISKON===========================
                if (tot_diskon == 0)
                { l_diskon.Text = "0,00"; }
                else
                { l_diskon.Text = string.Format("{0:#,###}" + ",00", tot_diskon); }
                //==============PERCABANGAN UNTUK LABEL VOUCHER======================
                if (get_voucher == 0)
                { l_voucher.Text = "0,00"; }
                else
                { l_voucher.Text = string.Format("{0:#,###}" + ",00", get_voucher); }
            }
            catch
            {
                get_dis_vou = 0;
                l_diskon.Text = "0,00";
                l_voucher.Text = "0,00";
            }
            //=================================COUNT TOTAL TRANSAKSI FROM TRANSAKSI LINE=============================
            ckon.con.Close();
            String sql = "SELECT SUM(transaction_line.SUBTOTAL) as total FROM transaction_line WHERE TRANSACTION_ID='" + l_transaksi.Text + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    totall = ckon.myReader.GetInt32("total");
                    totall_real = ckon.myReader.GetInt32("total");
                    totall = totall - get_voucher;
                    l_total.Text = string.Format("{0:#,###}" + ",00", totall);
                }
                ckon.con.Close();
            }
            catch
            { l_total.Text = "0,00"; }

        }
        //===================================================================================

        //==========KLIK HOLD TRANS MASUK KE KRANGJANG BELANJA===================================
        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            DateTime mydate = DateTime.Now;
            this.ActiveControl = t_barcode;
            t_barcode.Focus();

            get_data_combo();
            //save_trans_header();
            update_trans_header();
            date = mydate.ToString("yyyy-MM-dd");
            id_list = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            l_transaksi.Text = id_list;
            holding(date);
            retreive();
            get_data_id();
            itung_total();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(nm_cur2 + "--" + cust_id_store2);
        }

        //============================================================================================



        //===========================CHANGE SPG ID IN TRANSACTION LINE================================
        private void dgv_purchase_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.ActiveControl = t_barcode;
            t_barcode.Focus();

            if (dgv_purchase.Columns[e.ColumnIndex].Name == "spgid")
            {
                String id = dgv_purchase.Rows[e.RowIndex].Cells["Column7"].Value.ToString();
                String idspg = dgv_purchase.Rows[e.RowIndex].Cells["spgid"].Value.ToString();
                w_edit_SPG_ID spg = new w_edit_SPG_ID(f1);
                spg.get_data(id, l_transaksi.Text);
                spg.ShowDialog();
            }
        }
        //============================================================================================

        //=================================GET ID SPG FROM COMBO BOX =================================
        public void get_data_combo()
        {
            try
            {
                sub_string = combo_spg.Text;
                sub_string2 = sub_string.Substring(0, 9);
            }
            catch
            {
                sub_string2 = "";
            }
        }
        //============================================================================================

        //================DELETE LIST BELANJA=========================================================
        private void dgv_purchase_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.ActiveControl = t_barcode;
            t_barcode.Focus();
            if (dgv_purchase.Columns[e.ColumnIndex].Name == "Delete")
            {
                String id = dgv_purchase.Rows[e.RowIndex].Cells["Column7"].Value.ToString();
                String qty = dgv_purchase.Rows[e.RowIndex].Cells["Column5"].Value.ToString();
                //S_Articleid = dgv_purchase.SelectedRows[0].Cells[0].Value.ToString();
                String sql = "DELETE FROM transaction_line WHERE TRANSACTION_ID='" + l_transaksi.Text + "' AND ARTICLE_ID='" + id + "'";
                CRUD input = new CRUD();
                input.ExecuteNonQuery(sql);
                retreive();
                itung_total();

                Inv_Line inv = new Inv_Line();
                int qty2 = int.Parse(qty);
                inv.get_id(id);
                String type_trans = "1";
                inv.cek_type_trans(type_trans);
                inv.cek_inv_line(l_transaksi.Text, qty2);
                //Update_Inv inv = new Update_Inv();
                //inv.Search_id(id, qty);
                //inv.return_inv();
            }
        }

        //============================================================================================

        //=============================== SCAN BARCODE SAVE TO DB=================================
        private void t_barcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if(Properties.Settings.Default.OnnOrOff == "Offline")
                    {
                        //====HIT INVEN LOCAL==========================
                        API_Inventory InvLocal = new API_Inventory();
                        InvLocal.SetInvHitLocal();
                        //=============================================
                        get_data_combo();
                        save_trans_header();
                        cek_article();
                        retreive();
                        itung_total();
                        t_barcode.Text = "";
                    }
                    else
                    {
                        //SearchArticle diskon = new SearchArticle();
                        //diskon.scan_discount(t_barcode.Text, store_code, t_custId.Text);
                        //diskon.Post_Discount();
                        get_data_combo();
                        save_trans_header();
                        cek_article();
                        retreive();
                        itung_total();
                        t_barcode.Text = "";
                    }
                }
                else
                { }
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message);
            }
            //String barcode = t_barcode.Text;

        }

        //======================MASUKAN DATA PENJUALAN================================================
        public void cek_article()
        {
            String upper = t_barcode.Text.ToUpper();
            ckon.con.Close();
            //String sql0 = "SELECT * FROM article WHERE ARTICLE_ID ='" + t_barcode.Text + "'";
            String sql0 = "SELECT article._id, article.PRICE FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.GOOD_QTY >= 1 AND article.ARTICLE_ID = '" + t_barcode.Text + "'";
            ckon.cmd = new MySqlCommand(sql0, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            int count0 = 0;
            while (ckon.myReader.Read())
            {
                count0 = count0 + 1;
                id_inv = ckon.myReader.GetString("_id");
                price = ckon.myReader.GetInt32("PRICE");
            }
            ckon.con.Close();
            if (count0 == 1)
            {
                AddTransLine add = new AddTransLine();
                add.get_data(store_code, t_custId.Text, upper, sub_string2);
                add.get_data_trans_line(price, l_transaksi.Text);
                add.Post_Discount();
                add.cek_article2();
                //Post_Discount();
                
                /*
                string j = "1";
                string sql = "SELECT * FROM transaction_line WHERE TRANSACTION_ID ='" + l_transaksi.Text + "' AND ARTICLE_ID='" + t_barcode.Text + "'";
                ckon.cmd = new MySqlCommand(sql, ckon.con);
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                int count = 0;
                while (ckon.myReader.Read())
                {
                    count = count + 1;
                    new_jumlah = ckon.myReader.GetInt32("QUANTITY");
                    new_harga = ckon.myReader.GetInt32("SUBTOTAL");
                    new_disc = ckon.myReader.GetInt32("DISCOUNT");
                }
                ckon.con.Close();
                if (count == 0)
                {
                    int convert_harga;
                    convert_harga = price;
                    new_harga = convert_harga - disc;

                    //string sql2 = "INSERT INTO transaction_line (TRANSACTION_ID,ARTICLE_ID,QUANTITY,PRICE,DISCOUNT,SUBTOTAL, SPG_ID, DISCOUNT_CODE,DISCOUNT_TYPE) VALUES ('" + t_id + "','" + S_ID + "', '" + j + "', '" + S_price + "', '"+ disc +"' ,'" + S_price + "', '"+ id_spg +"','"+ disc_code +"','"+ disc_type +"')";
                    //string sql2 = "INSERT INTO transaction_line (TRANSACTION_ID,ARTICLE_ID,QUANTITY,PRICE,SUBTOTAL, SPG_ID) VALUES ('" + l_transaksi.Text + "','" + t_barcode.Text + "', '" + j + "', '" + price + "', '" + price + "', '" + sub_string2 + "')";
                    string sql2 = "INSERT INTO transaction_line (TRANSACTION_ID,ARTICLE_ID,QUANTITY,PRICE,DISCOUNT,SUBTOTAL, SPG_ID, DISCOUNT_CODE,DISCOUNT_TYPE,DISCOUNT_DESC) VALUES ('" + l_transaksi.Text + "','" + upper + "', '" + j + "', '" + price + "', '" + disc + "','" + new_harga + "', '" + sub_string2 + "','" + disc_code + "','" + disc_type + "','" + disc_desc1 + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);
                }
                else
                {
                    price = price - disc;
                    new_jumlah = new_jumlah + 1;
                    new_harga = new_harga + price;
                    new_disc = new_disc + disc;
                    String sql3 = "UPDATE transaction_line SET QUANTITY='" + new_jumlah + "',DISCOUNT ='" + new_disc + "' ,SUBTOTAL='" + new_harga + "' WHERE TRANSACTION_ID='" + l_transaksi.Text + "' AND ARTICLE_ID='" + upper + "'";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql3);
                }
                */

                //Update_Inv inv = new Update_Inv();
                //inv.get_inv(id_inv);
                //inv.change_inv();
                Inv_Line inv = new Inv_Line();
                int qty_min_plus = -1;
                String type_trans = "1";
                inv.cek_qty_inv(id_inv);

                inv.cek_type_trans(type_trans);
                inv.cek_inv_line(l_transaksi.Text, qty_min_plus);
            }
            else
            {
                MessageBox.Show("Article Not Found Or Quantity Empty");
                ckon.con.Close();
            }

        }

        //============================================================================================

        //======================LIST HOLD TRANSACTION============================================
        public void holding(String tanggal)
        {
            dgv_hold.Rows.Clear();
            koneksi2 ckon2 = new koneksi2();
            ckon.con.Close();
            string date = tanggal;
            String sql = "SELECT * FROM transaction WHERE STATUS='0' AND ID_SHIFT='" + id_shift + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            List<string> numbersList = new List<string>();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_trans = ckon.myReader.GetString("TRANSACTION_ID");
                    String sql2 = "SELECT article.ARTICLE_NAME FROM transaction_line, article  WHERE article.ARTICLE_ID = transaction_line.ARTICLE_ID AND transaction_line.TRANSACTION_ID='" + id_trans + "'";
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
                    ckon2.con2.Close();
                }

            }
            ckon.con.Close();
        }
        //=============================================================================================

        //======================SIMPAN TRANSACTION HEADER============================================
        public void save_trans_header()
        {
            /*
            VarBackDate = Properties.Settings.Default.mBackDate;
            if (VarBackDate == "YES")
            {
                DateHeaderTrans = Properties.Settings.Default.ValueBackDate;
            }
            else
            {
                //DateTime mydate = DateTime.Now;
                //DateHeaderTrans = mydate.ToString("yyyy-MM-dd");
            }
            */
            DateTime myhour = DateTime.Now;
            DateTime mydate = DateTime.Now;
            DateHeaderTrans = mydate.ToString("yyyy-MM-dd");

            ckon.con.Close();
            String sql0 = "SELECT * FROM transaction WHERE TRANSACTION_ID ='" + l_transaksi.Text + "'";
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
                String sql = "INSERT INTO transaction (TRANSACTION_ID,STORE_CODE,CUSTOMER_ID,EMPLOYEE_ID,SPG_ID,STATUS, DATE, TIME, CUST_ID_STORE, CURRENCY,ID_SHIFT,ID_C_STORE) VALUES ('" + l_transaksi.Text + "','" + store_code + "' ,'" + t_custId.Text + "','" + id_employe2 + "' ,'" + sub_string2 + "','0','" + DateHeaderTrans + "', '" + myhour.ToLocalTime().ToString("H:mm:ss") + "', '" + cust_id_store2 + "', '" + nm_cur2 + "','" + id_shift + "','" + id_CStore + "') ";
                ckon.con.Open();
                ckon.cmd = new MySqlCommand(sql, ckon.con);
                ckon.cmd.ExecuteNonQuery();
                ckon.con.Close();

                String query = "UPDATE auto_number SET Month = '" + bulan_now + "', Number = '" + number_trans + "' WHERE Type_Trans='1'";
                CRUD ubah = new CRUD();
                ubah.ExecuteNonQuery(query);

            }
        }
        //==========================================================================================

        //=======================UPDATE TRANSAKSI HEADER============================================
        public void update_trans_header()
        {
            String sql = "UPDATE transaction SET CUSTOMER_ID ='" + t_custId.Text + "', SPG_ID='" + sub_string2 + "' WHERE  TRANSACTION_ID='" + l_transaksi.Text + "'";
            CRUD update = new CRUD();
            update.ExecuteNonQuery(sql);
        }
        //==========================================================================================

        //===========================GET DATA BY ID ================================================
        public void get_data_id()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM transaction WHERE TRANSACTION_ID='" + id_list + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                t_custId.Text = ckon.myReader.GetString("CUSTOMER_ID");
                combo_spg.Text = ckon.myReader.GetString("SPG_ID");
            }
        }
        //==========================================================================================

        //============================DATAGRIDVIEW DISKON===========================================
        public void data_diskon(String discountCode)
        //public void data_diskon()
        {
            //dgv_diskon.Rows.Clear();
            String sql = "SELECT * FROM promotion where discount_code = '" + discountCode + "'";
            //String sql = "SELECT * FROM promotion";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            {
                try
                {
                    ckon.con.Open();
                    ckon.myReader = ckon.cmd.ExecuteReader();
                    while (ckon.myReader.Read())
                    {
                        kd_diskon = ckon.myReader.GetString("DISCOUNT_CODE");
                        status = ckon.myReader.GetString("STATUS");

                        int n = dgv_diskon.Rows.Add();
                        //dgv_diskon.Rows[n].Cells[0].Value = status;
                        dgv_diskon.Rows[n].Cells[1].Value = kd_diskon;
                        //dgv_color();
                        if (status == "0")
                        {
                            dgv_diskon.Rows[n].DefaultCellStyle.ForeColor = Color.Black;

                        }
                    }


                    ckon.con.Close();
                }
                catch
                { }
            }

        }
        //==========================================================================================
        public void dgv_color()
        {
            for (int i = 0; i < dgv_diskon.Rows.Count; i++)
            {
                int val = Int32.Parse(status);
                //String val = status;
                if (val == 0)
                {
                    dgv_diskon.Rows[i].DefaultCellStyle.ForeColor = Color.Red;
                    //timer1.Start();
                }


            }
        }
        //=============================================================================================
        private void dgv_diskon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            this.ActiveControl = t_barcode;
            t_barcode.Focus();
            try
            {
                if (dgv_diskon.Columns[e.ColumnIndex].Name == "kd_dskn")
                {
                    //untuk ambil data spg
                    get_data_combo();
                    //ambil id diskon
                    String id = dgv_diskon.Rows[e.RowIndex].Cells["kd_dskn"].Value.ToString();
                    W_Promotion_Popup diskon = new W_Promotion_Popup(f1);
                    diskon.get_id(id, totall_real, l_transaksi.Text, sub_string2);
                    diskon.ShowDialog();
                }
            }
            catch
            {
                MessageBox.Show("Please Select Value In List");
            }

        }
        //============BUTTON CHARGE 2 (BARU)==================
        private void b_charge2_Click(object sender, EventArgs e)
        {
            if (l_total.Text == "0,00")
            {
                //PopupNotifier pop = new PopupNotifier();
                //pop.TitleText = "Warning";
                //pop.ContentText = "Your Cart Is Empety";
                //pop.Popup();
                MessageBox.Show("Your Cart Is Empty");
            }
            else
            {
                get_data_combo();
                //save_trans_header();
                update_trans_header();
                charge c = new charge(f1);
                f1.p_kanan.Controls.Clear();
                if (!f1.p_kanan.Controls.Contains(charge.Instance))
                {
                    f1.p_kanan.Controls.Add(charge.Instance);
                    charge.Instance.Dock = DockStyle.Fill;
                    charge.Instance.id_trans(l_transaksi.Text, totall, get_dis_vou, get_voucher, tot_diskon);
                    charge.Instance.get_data_id(l_transaksi.Text);

                    charge.Instance.QuickCash();
                    //charge.Instance.AddButtons2();
                    charge.Instance.Show();
                }
                else
                {
                    charge.Instance.Show();
                }
            }
        }

        //===================BUTTON CLEAR 2(BARU)=======================================
        private void b_clear2_Click(object sender, EventArgs e)
        {
            DateTime mydate = DateTime.Now;
            if (l_total.Text == "0,00")
            {
                //PopupNotifier pop = new PopupNotifier();
                //pop.TitleText = "Warning";
                //pop.ContentText = "No Item On List";
                //pop.Popup();
                MessageBox.Show("No Item On List");
            }
            else
            {
                date = mydate.ToString("yyyy-MM-dd");
                //ckon.con.Open();
                //String sql = "DELETE FROM transaction_line WHERE TRANSACTION_ID ='" + l_transaksi.Text + "'";
                //ckon.cmd = new MySqlCommand(sql, ckon.con);
                //ckon.cmd.ExecuteNonQuery();
                Inv_Line inv = new Inv_Line();
                String type_trans = "4";
                inv.cek_type_trans(type_trans);
                inv.void_trans(l_transaksi.Text);
                inv.del_inv_line(l_transaksi.Text);

                ckon.con.Close();
                holding(date);
                //new_invoice();
                retreive();
                l_total.Text = "0,00";
                l_diskon.Text = "0,00";
                l_voucher.Text = "0,00";
            }
        }

        //===========BUTTON HOLD 2====================================
        private void b_hold2_Click(object sender, EventArgs e)
        {
            DateTime mydate = DateTime.Now;
            if (l_total.Text == "0,00")
            {
                //PopupNotifier pop = new PopupNotifier();
                //pop.TitleText = "Warning";
                //pop.ContentText = "Pick Item First";
                //pop.Popup();
                MessageBox.Show("Pick Item First");
            }

            else
            {
                //RunningNumber running = new RunningNumber();
                //running.get_data_before("1", "TR");
                get_data_combo();
                //save_trans_header();
                update_trans_header();
                date = mydate.ToString("yyyy-MM-dd");
                new_invoice();
                set_running_number();
                holding(date);
                dgv_purchase.Rows.Clear();
                l_total.Text = "0,00";
                l_diskon.Text = "0,00";
                l_voucher.Text = "0,00";
                t_custId.Text = ""; //combo_spg.SelectedIndex = 0;
                isi_combo_spg();
                delete_rows();
            }
        }
        //==================BUTTON FIND ITEM (BARU)===================================
        private void B_FIND2_Click(object sender, EventArgs e)
        {
            //===FOKUES KE SCAN BARCODE
            this.ActiveControl = t_barcode;
            t_barcode.Focus();
            get_data_combo();
            SearchArticle filter = new SearchArticle();
            filter.get_data_store(cust_id_store2, t_custId.Text, store_code);
            filter.t_id = l_transaksi.Text;
            filter.id_spg = sub_string2;
            filter.store_code2 = store_code;
            filter.ShowDialog();
        }
        //=======================SHORTCUT BUTTON FROM T_CUST_ID=======================================
        private void t_custId_KeyDown(object sender, KeyEventArgs e)
        {
            //====BUTTON SEARCH ARTICLE (FIND ITEM)==============
            if (e.Control && e.KeyCode.ToString() == "F")
            {
                B_FIND2_Click(null, null);
            }
            //=======BUTTON CLEAR ALL (DELETE ALL)==============
            if (e.Control && e.KeyCode.ToString() == "D")
            {
                b_clear2_Click(null, null);
            }
            //======BUTTON HOLD========================
            if (e.Control && e.KeyCode.ToString() == "H")
            {
                b_hold2_Click(null, null);
            }
            //======BUTTON CHARGE===============
            if (e.Control && e.KeyCode.ToString() == "C")
            {
                b_charge2_Click(null, null);
            }
            //======BUTTON VOUCHER==================
            if (e.Control && e.KeyCode.ToString() == "V")
            {
                b_voucher2_Click(null, null);
                t_custId.Text = "";
            }
        }
        //============================================================================================
        //======================SHORTCUT BUTTON FROM T_EMPLOYE_ID=====================================
        //private void t_employId_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.Control && e.KeyCode.ToString() == "S")
        //    {
        //        bunifuFlatButton1_Click(null, null);
        //    }
        //    if (e.Control && e.KeyCode.ToString() == "D")
        //    {
        //        b_clear_Click(null, null);
        //    }
        //    if (e.Control && e.KeyCode.ToString() == "H")
        //    {
        //        b_hold_Click(null, null);
        //    }
        //    if (e.Control && e.KeyCode.ToString() == "C")
        //    {
        //        b_charge_Click(null, null);
        //    }
        //    if (e.Control && e.KeyCode.ToString() == "V")
        //    {
        //        t_custId.Text = ""; t_barcode.Text = ""; combo_spg.SelectedIndex = 0;
        //        b_voucher_Click(null, null);
        //    }
        //}
        //============================================================================================
        //============================================================================================
        private void t_barcode_KeyDown(object sender, KeyEventArgs e)
        {
            //====BUTTON SEARCH ARTICLE (FIND ITEM)==============
            if (e.Control && e.KeyCode.ToString() == "F")
            {
                B_FIND2_Click(null, null);
            }
            //=======BUTTON CLEAR ALL (DELETE ALL)==============
            if (e.Control && e.KeyCode.ToString() == "D")
            {
                b_clear2_Click(null, null);
            }
            //======BUTTON HOLD========================
            if (e.Control && e.KeyCode.ToString() == "H")
            {
                b_hold2_Click(null, null);
            }
            //======BUTTON CHARGE===============
            if (e.Control && e.KeyCode.ToString() == "C")
            {
                b_charge2_Click(null, null);
            }
            //======BUTTON VOUCHER==================
            if (e.Control && e.KeyCode.ToString() == "V")
            {
                b_voucher2_Click(null, null);
                t_barcode.Text = "";
            }
        }
        //============================================================================================
        private void combo_spg_KeyDown(object sender, KeyEventArgs e)
        {
            //====BUTTON SEARCH ARTICLE (FIND ITEM)==============
            if (e.Control && e.KeyCode.ToString() == "F")
            {
                B_FIND2_Click(null, null);
            }
            //=======BUTTON CLEAR ALL (DELETE ALL)==============
            if (e.Control && e.KeyCode.ToString() == "D")
            {
                b_clear2_Click(null, null);
            }
            //======BUTTON HOLD========================
            if (e.Control && e.KeyCode.ToString() == "H")
            {
                b_hold2_Click(null, null);
            }
            //======BUTTON CHARGE===============
            if (e.Control && e.KeyCode.ToString() == "C")
            {
                b_charge2_Click(null, null);
            }
            //======BUTTON VOUCHER==================
            if (e.Control && e.KeyCode.ToString() == "V")
            {
                b_voucher2_Click(null, null);
                combo_spg.SelectedIndex = 0;
            }
        }
        //============================================================================================

        //=================================FOCUS FROM CHARGE MENU ====================================
        public void barcode_fokus()
        {
            this.ActiveControl = t_barcode;
            t_barcode.Focus();

        }
        //============================================================================================

        //=======B_VOUCHER2 (BARU)=======================
        private void b_voucher2_Click(object sender, EventArgs e)
        {
            if (l_total.Text == "0,00")
            {
                MessageBox.Show("Your Shopping Cart Is Empty");
            }
            else
            {
                W_Voucher vou = new W_Voucher();
                vou.id_transaksi2 = l_transaksi.Text;
                vou.store_code2 = store_code;
                vou.ShowDialog();
            }
        }
        //====FUNGSI BARU RUNNING NUMBER, UNTUK MENGATASI BUG NUMBER SEQ MACET
        public void set_running_number()
        {
            get_year_month();
            get_running_number();
        }
        //====METHOD GET MOUNT AND YEAR PRESENT=================
        public void get_year_month()
        {
            DateTime mydate = DateTime.Now;
            DateTime myhour = DateTime.Now;

            bulan_now = mydate.ToString("MM");
            tahun_now = mydate.ToString("yy");
        }
        //=========METHOD GET DATA FROM AUTO_NUMBER TABLE FOR SALES TRANSACTION
        public void get_running_number()
        {
            DevCode code = new DevCode();

            String device_code = "";
            device_code = code.aDevCode;

            String sql = "";
            if (Properties.Settings.Default.DevCode == "null")
            {
                sql = "SELECT * FROM auto_number WHERE Store_Code = '" + store_code + "' AND Type_Trans = '1'";
            }
            else
            {
                sql = "SELECT * FROM auto_number WHERE Store_Code = '" + store_code + "' AND Type_Trans = '1' AND Dev_Code='" + device_code + "'";
            }
            //String sql = "SELECT * FROM auto_number WHERE Store_Code = '" + store_code + "' AND Type_Trans = '1'";
            ckon.con.Open();
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    //tahun_trans = ckon.myReader.GetString("Year");
                    bulan_trans = ckon.myReader.GetString("Month");
                    number_trans = ckon.myReader.GetInt32("Number");
                }
                if (bulan_now == bulan_trans)
                {
                    number_trans = number_trans + 1;
                    if (number_trans < 10)
                    { number_trans_string = "0000" + number_trans.ToString(); }
                    else if (number_trans < 100)
                    { number_trans_string = "000" + number_trans.ToString(); }
                    else if (number_trans < 1000)
                    { number_trans_string = "00" + number_trans.ToString(); }
                    else if (number_trans < 10000)
                    { number_trans_string = "0" + number_trans.ToString(); }
                    else
                    { number_trans_string = number_trans.ToString(); }
                    //==MEMBUAT STRING FINAL RUNNING NUMBER
                    if (Properties.Settings.Default.DevCode == "null")
                    {
                        final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-" + number_trans_string;
                    }
                    else
                    {
                        final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-" + number_trans_string + "-" + Properties.Settings.Default.DevCode;
                    }
                    //final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-" + number_trans_string;
                    l_transaksi.Text = final_running_number;

                }
                else
                {
                    number_trans = 1;
                    bulan_trans = bulan_now;//MENJADIKAN BULAN TRANSAKSI = BULAN SEKARANG
                    //==MEMBUAT STRING FINAL RUNNING NUMBER
                    if (Properties.Settings.Default.DevCode == "null")
                    {
                        final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                    }
                    else
                    {
                        final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001-" + Properties.Settings.Default.DevCode;
                    }
                    //final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                    l_transaksi.Text = final_running_number;
                }


            }
            else
            {
                String query = "";
                number_trans = 1;
                bulan_trans = bulan_now;//BULAN TRANSAKSI = BULAN SEKARANG
                if (Properties.Settings.Default.DevCode == "null")
                {
                    final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                    query = "INSERT INTO auto_number (Store_Code,Month,Number,Type_Trans) VALUES ('" + store_code + "','" + bulan_trans + "','0','1')";
                }
                else
                {
                    final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001-" + Properties.Settings.Default.DevCode;
                    query = "INSERT INTO auto_number (Store_Code,Month,Number,Type_Trans,Dev_Code) VALUES ('" + store_code + "','" + bulan_trans + "','0','1','" + Properties.Settings.Default.DevCode + "')";
                }
                //final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                l_transaksi.Text = final_running_number;

                //String query = "INSERT INTO auto_number (Store_Code,Month,Number,Type_Trans) VALUES ('" + store_code + "','" + bulan_trans + "','0','1')";
                CRUD ubah = new CRUD();
                ubah.ExecuteNonQuery(query);

                //MessageBox.Show(final_running_number);
            }
            ckon.con.Close();
        }
    }
}

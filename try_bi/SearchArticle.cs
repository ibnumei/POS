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
using Newtonsoft.Json;

namespace try_bi
{
    public partial class SearchArticle : Form
    {
        koneksi ckon = new koneksi();
        koneksi2 ckon2 = new koneksi2();
        public String t_id, S_barcode, S_article, S_nama, S_price, S_ID, id_spg, store_code2, id_inv;
        int new_price;
        DateTime mydate = DateTime.Now;
        //===============FOR API DISCOUNT==========
        String cust_store, customer, code_store, disc_code, disc_type, disc_desc;
        int qty = 1, subtotal, disc=0, new_disc ;
        //========================================= 
        public int i, new_jumlah, new_harga, totall, price;


        private void dgv_2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                S_ID = dgv_2.SelectedRows[0].Cells[0].Value.ToString();

                S_price = dgv_2.SelectedRows[0].Cells[4].Value.ToString();
                id_inv = dgv_2.SelectedRows[0].Cells[5].Value.ToString();

                //============================================================
                //Post_Discount();
                AddTransLine add = new AddTransLine();
                add.get_data(code_store, customer, S_ID, id_spg);
                add.get_data_trans_line(Convert.ToInt32(S_price),t_id);
                add.Post_Discount();
                add.cek_article2();
                //============================================================
               // cek_article2();
                //MEMOTONG ARTICLE
                Inv_Line inv = new Inv_Line();
                int qty_min_plus = -1;
                String type_trans = "1";
                inv.cek_qty_inv(id_inv);
                inv.cek_type_trans(type_trans);
                inv.cek_inv_line(t_id, qty_min_plus);

                back_uc();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
        //================get CustIdStore, customer, store_code========
        public void get_data_store(String custId, String cust, String code)
        {
            cust_store = custId;
            customer = cust;
            code_store = code;
        }
        //======================
        public void scan_discount(String id, String store_code, String cust)
        {
            S_ID = id;
            code_store = store_code;
            customer = cust;
        }
        //===================API DISCOUNT============================
        /*  public async Task Post_Discount()
          {
              Transaction transaction = new Transaction();
              transaction.storeCode = code_store;
              transaction.customerId = customer;
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

              //call discount here
              var stringPayload = JsonConvert.SerializeObject(transaction);
              String response = "";
              var credentials = new NetworkCredential("username", "password");
              var handler = new HttpClientHandler { Credentials = credentials };
              var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
              using (var client = new HttpClient(handler))
              {
                  try
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
                          //=================================================
                          foreach (var c in resultData.discountItems)
                          {
                              disc = c.amountDiscount;
                              disc_code = c.discountCode;
                              disc_type = c.discountType;
                              disc_desc = c.discountDesc;
                              //MessageBox.Show("" + disc.ToString());
                          }
                          //=================================================
                      }
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }

              }
          }

          */

        public async Task Post_Discount()
        {
            Transaction transaction = new Transaction();
            transaction.storeCode = code_store;
            transaction.customerId = customer;
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
            try
            {
                foreach (var c in resultData.discountItems)
                {
                    disc = (Int32)c.amountDiscount;
                    disc_code = c.discountCode;
                    disc_type = c.discountType;
                    disc_desc = c.discountDesc;
                    //MessageBox.Show("" + disc.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        //=======================END OF DISCOUNT=============================================================
        public SearchArticle()
        {
            InitializeComponent();
        }

        //=============TEXTBOX SEARCH CHANGED=====================================
        private void t_find_article_OnTextChange(object sender, EventArgs e)
        {
            String count_article = t_find_article.text;
            int count_article_int = count_article.Count();
            //if(count_article_int < 5)
            //{
                if (t_find_article.text == "")
                {

                    String sql2a = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >=1 LIMIT 100";
                    get_load_data(sql2a);

                }
                if (t_find_article.text == "" && (combo_ktg2.Text == "Brand" || combo_ktg2.Text == "Department" || combo_ktg2.Text == "Department_Type" || combo_ktg2.Text == "Size" || combo_ktg2.Text == "Color" || combo_ktg2.Text == "Gender"))
                {
                    String sql4 = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >=1  AND article." + combo_ktg2.Text + " = '" + combo_value.Text + "' LIMIT 100";
                    get_load_data(sql4);
                    ckon.con.Close();
                }
            //}
            //if(count_article_int >= 5)
            //{
                if (t_find_article.text != "")
                {
                    String sql2 = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >= 1 AND (article.ARTICLE_ID LIKE '%" + t_find_article.text + "%' OR article.ARTICLE_NAME LIKE '%" + t_find_article.text + "%' ) LIMIT 100";
                    get_load_data(sql2);

                }
                if (t_find_article.text != "" && (combo_ktg2.Text == "Brand" || combo_ktg2.Text == "Department" || combo_ktg2.Text == "Department_Type" || combo_ktg2.Text == "Size" || combo_ktg2.Text == "Color" || combo_ktg2.Text == "Gender"))
                {

                    String sql3 = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >= 1 AND article." + combo_ktg2.Text + " = '" + combo_value.Text + "' AND (article.ARTICLE_ID LIKE '%" + t_find_article.text + "%' OR article.ARTICLE_NAME LIKE '%" + t_find_article.text + "%' ) LIMIT 100";
                    get_load_data(sql3);
                }
            //}
           
            

        }
        //===================================================================================

        //=================FORM KE LOAD========================================
        private void SearchArticle_Load(object sender, EventArgs e)
        {
            this.ActiveControl = t_find_article;
            t_find_article.Focus();
            if (Properties.Settings.Default.OnnOrOff == "Offline")
            {
                //====HIT INVEN LOCAL==========================
                API_Inventory InvLocal = new API_Inventory();
                InvLocal.SetInvHitLocal();
                //==============================================
                //dataGridView1.Columns[0].HeaderCell.Style.ForeColor = Color.Orange;
                dgv_2.EnableHeadersVisualStyles = false;
                String sql = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >= 1 LIMIT 100";
                get_load_data(sql);
                dgv_2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv_2.MultiSelect = true;
            }
            else
            {
                dgv_2.EnableHeadersVisualStyles = false;
                String sql = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >= 1 LIMIT 100";
                get_load_data(sql);
                dgv_2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv_2.MultiSelect = true;
            }
           
          
        }
        //================DATAGRIDVIEW TO DATABASE =========================================
        public void cek_article2()
        {
            new_price = int.Parse(S_price);
            string j = "1";
            //i = 0;
            string sql = "SELECT * FROM transaction_line WHERE TRANSACTION_ID ='" + t_id + "' AND ARTICLE_ID='" + S_ID + "'";
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
                int convert_harga;//convert harga menjadi integer
                //jika diskon tidak ada, maka subtotal dikurangi diskon

                    convert_harga = Int32.Parse(S_price);
                    new_harga = convert_harga - disc;

                string sql2 = "INSERT INTO transaction_line (TRANSACTION_ID,ARTICLE_ID,QUANTITY,PRICE,DISCOUNT,SUBTOTAL, SPG_ID, DISCOUNT_CODE,DISCOUNT_TYPE,DISCOUNT_DESC) VALUES ('" + t_id + "','" + S_ID + "', '" + j + "', '" + S_price + "', '"+ disc +"' ,'" + new_harga + "', '"+ id_spg +"','"+ disc_code +"','"+ disc_type +"','"+ disc_desc +"')";
                CRUD input = new CRUD();
                input.ExecuteNonQuery(sql2);
            }
            else
            {
                new_price = new_price - disc;
                new_jumlah = new_jumlah + 1;
                new_harga = new_harga + new_price;
                new_disc = new_disc + disc;
                //int subtotal_diskon = new_harga - new_disc;
                String sql3 = "UPDATE transaction_line SET QUANTITY='" + new_jumlah + "',DISCOUNT='"+ new_disc +"' ,SUBTOTAL='" + new_harga + "' WHERE TRANSACTION_ID='" + t_id + "' AND ARTICLE_ID='" + S_ID + "'";
                CRUD input = new CRUD();
                input.ExecuteNonQuery(sql3);
            }
        }
        //======================MASUK KO FORM UTAMA=====================================
        public void back_uc()
        {

            Form1 f1 = new Form1();
            uc_coba.Instance.get_data_combo();
            uc_coba.Instance.save_trans_header();
           uc_coba.Instance.retreive();
            uc_coba.Instance.itung_total();
            this.Close();
        }
        //======================RETREIVE DATA ====================================================

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
                    dgv_2.Rows[n].Cells[0].Value = row["ARTICLE_ID"].ToString();
                    dgv_2.Rows[n].Cells[1].Value = row["ARTICLE_NAME"].ToString();
                    dgv_2.Rows[n].Cells[2].Value = row["SIZE"].ToString();
                    dgv_2.Rows[n].Cells[3].Value = row["COLOR"].ToString();
                    dgv_2.Rows[n].Cells[4].Value = row["PRICE"];
                    dgv_2.Rows[n].Cells[5].Value = row["_id"];
                    dgv_2.Rows[n].Cells[6].Value = row["GOOD_QTY"];
                }
                dgv_2.Columns[4].DefaultCellStyle.Format = "#,###";
                //con.Close();
                ckon.dt.Rows.Clear();
                ckon.con.Close();
                if(dgv_2.Rows.Count > 1 || dgv_2.Rows.Count < 6)
                {
                    fokus_dgv();
                }
                if(dgv_2.Rows.Count > 5)
                {
                    this.ActiveControl = t_find_article;
                    t_find_article.Focus();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ckon.con.Close();
            }

        }
        //===================================================================================

        private void combo_ktg2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo_ktg2.Text == "ALL")
            {
                combo_value.Items.Clear();
                String sql = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >= 1 LIMIT 100";
                combo_value.Text = "ALL";
                get_load_data(sql);
                
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
        //========================ISI VALUE COMBO ==============================================
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
        private void combo_value_SelectedIndexChanged(object sender, EventArgs e)
        {
            String sql = "select article.ARTICLE_ID, article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE, inventory._id, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE inventory.STATUS = '0' AND inventory.GOOD_QTY >= 1 AND article." + combo_ktg2.Text + " = '" + combo_value.Text + "' LIMIT 100";
            get_load_data(sql);

        }
        //==============CLOSE BY KEY DOWN IN FORM =====================================
        private void SearchArticle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode.ToString() == "X")
            {
                this.Close();
            }

        }
        //=================SELECT VALUE BY PRESS ENTER=======================
        private void dgv_2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                try
                {
                    S_ID = dgv_2.SelectedRows[0].Cells[0].Value.ToString();

                    S_price = dgv_2.SelectedRows[0].Cells[4].Value.ToString();
                    id_inv = dgv_2.SelectedRows[0].Cells[5].Value.ToString();

                    //============================================================
                    Post_Discount();
                    //============================================================
                    cek_article2();
                    //MEMOTONG ARTICLE
                    Inv_Line inv = new Inv_Line();
                    int qty_min_plus = -1;
                    String type_trans = "1";
                    inv.cek_qty_inv(id_inv);
                    inv.cek_type_trans(type_trans);
                    inv.cek_inv_line(t_id, qty_min_plus);

                    back_uc();
                }
                catch
                {
                    MessageBox.Show("Select a value in the table");
                }
                //MessageBox.Show("Enter pressed");
            }
        }
        //=================MENCOBA MEMINDAHKAN FOKUS KE DATAGRIDVIEW SAAT SUDAH MEMASUKAN 5 ARTICLE
        public void fokus_dgv()
        {
            this.ActiveControl = dgv_2;
            dgv_2.Focus();

        }
    }
}

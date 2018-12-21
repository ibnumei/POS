using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace try_bi
{
    public partial class W_Promotion_Popup : Form
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String id_diskon, diskon_kode, diskon_name, diskon_ktg, diskon_desc, jenis, id_transaksi, status, art_id_diskon, art_bonus, disc_type, disc_desc;
        int the_real_totall, net_diskon, net_price, total_kotor, total_bersih, sub_total_TransLine, count=0, count_disc_Tline=0;
        String field_none = "None", field_kosong="0", spg_id;
        //===VARIABEL UNTUK MENDAPATKAN DISKON DARI HARGA ASLI BARANG DIKALI DENGAN QUANTITY
        int qty_kali, price_kali;
        //=============================BUTTON USE========================================
        private void b_ok_Click(object sender, EventArgs e)
        {
            if(status=="0")
            {
                MessageBox.Show("Discount Not Match");
            }
            else
            {
                //===CEK APAKAH DISKON SUDAH PERNAH DIGUNAKAN DI TRANSAKSI INI
                cek_discount_line();
                //jika sudah ada maka tampilkan pesan agar tidak bisa digunakan lagi
                if(count_disc_Tline >= 1)
                {
                    MessageBox.Show("Discounts Have Been Used");
                }
                else
                {
                    //jika tipe diskon adalah 3, atau buy and get
                    if (disc_type == "3")
                    {
                        //========HITUNG ADA BRAPA BANYAK DISCOUNT ITEM DARI DISCOUNT CODE
                        count_artcile();
                        //JIKA ADA 1, LANGSUNG APPLY, JIKA ADA BANYAK, OPEN DIALOG
                        if (count == 1)
                        {
                            //MessageBox.Show("data hanya ada 1");
                            W_disc_GetArticle disc = new W_disc_GetArticle();
                            //disc.message();
                            disc.get_disc_code(diskon_kode, disc_type, id_transaksi, spg_id);
                            disc.disc_1item();
                            disc.search_data_article();
                            disc.insert();
                            uc_coba.Instance.retreive();
                            uc_coba.Instance.itung_total();
                            this.Close();
                        }
                        //jumlah diskon_item lebih dari 1, open dialog
                        else
                        {
                            W_disc_GetArticle disc = new W_disc_GetArticle();
                            disc.get_disc_code(diskon_kode, disc_type, id_transaksi, spg_id);
                            disc.retreive();
                            disc.ShowDialog();
                            this.Close();
                        }

                    }
                    //============akhir tipe diskon 3================

                    //jenis diskon 4, atau selain tipe 3
                    else
                    {
                        //=====JENIS DISKON==========
                        get_type_diskon();
                        //=====HARGA SETELAH DISKON===
                        get_total_price();
                        //MessageBox.Show(total_kotor.ToString());
                        //=====MASUKAN HARGA SETELAH DISKON KE TRANS_LINE YG SESUAI DENGAN
                        update();
                    }
                    //====akhir diskon tipe selain 3
                }
                //====akhir jika diskon belom pernah digunakan
            }
            
        }
        //============================BUTTON CANCEL=======================================
        private void b_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //===================================================================================
        private void W_Promotion_Popup_Load(object sender, EventArgs e)
        {
            //========DATA PROMOTION HEADER======
            data_from_id();
            //===TAMPILKAN DATA PROMOTION LINE=======
            retreive();
            //=====CEK TIPE DISKON DARI PROMOTION LINE===
            get_data();
            //===========MENDAPATKAN SUBTOTAL DARI TRANS ID DAN ARTICLE ID YG DI DAPAT DARI POST DISCOUT, UTK DIHITUNG HARGA KENA DISKON
            get_price_TransLine();
        }
        //====================================================================================
        public W_Promotion_Popup(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //==================================METHOD FOR GET DATA FORM TRANSACTION============================
        public void get_id(String id, int the_real, String id_trans, String spg)
        {
            id_diskon = id;
            the_real_totall = the_real;
            id_transaksi = id_trans;
            spg_id = spg;
        }
        //===========================METHOD FOR GET DATA FROM DISCOUNT CODE FROM DISCOUNT HEADER============================
        public void data_from_id()
        {
            String sql = "Select * from promotion where DISCOUNT_CODE='" + id_diskon + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            {
                try
                {
                    ckon.con.Open();
                    ckon.myReader = ckon.cmd.ExecuteReader();
                    while(ckon.myReader.Read())
                    {
                        diskon_kode = ckon.myReader.GetString("DISCOUNT_CODE");
                        diskon_name = ckon.myReader.GetString("DISCOUNT_NAME");
                        diskon_ktg = ckon.myReader.GetString("DISCOUNT_CATEGORY");
                        diskon_desc = ckon.myReader.GetString("DESCRIPTION");
                        status = ckon.myReader.GetString("STATUS");
                        art_id_diskon = ckon.myReader.GetString("ARTICLE_ID");
                        disc_type = ckon.myReader.GetString("DISCOUNT_TYPE");
                        disc_desc = ckon.myReader.GetString("DISCOUNT_DESC");
                        //l_diskon_name.Text = diskon_name;
                        t_disc_name.Text = diskon_name;
                        l_d_ctg.Text = diskon_ktg;
                        l_d_code.Text = diskon_kode;
                        l_d_desc.Text = diskon_desc;
                    }
                }
                catch
                { }
            }
        }
        //===============================================================================================
        public void get_data()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM promotion_line WHERE DISCOUNT_CODE='" + id_diskon + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    //try
                    //{ net_diskon = ckon.myReader.GetInt32("DISCOUNT_PERCENT"); }
                    //catch
                    //{ net_diskon = 0; }
                    ////===============================
                    //try
                    //{ net_price = ckon.myReader.GetInt32("DISCOUNT_PRICE"); }
                    //catch
                    //{ net_price = 0; }
                    art_bonus = ckon.myReader.GetString("ARTICLE_ID_DISCOUNT");
                    net_diskon = ckon.myReader.GetInt32("DISCOUNT_PERCENT");
                    net_price = ckon.myReader.GetInt32("DISCOUNT_PRICE");
                }
                ckon.con.Close();
            }
            catch
            { }

        }
        
        //===============================================================================================

        //===========================METHOD FOR GET DATA FROM DISCOUNT LINE================================
        public void retreive()
        {
            ckon.con.Close();
            dgv_purchase.Rows.Clear();
            String sql = "SELECT * FROM promotion_line WHERE DISCOUNT_CODE='" + id_diskon + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                //====================================================
                //ckon.con.Close();
                //ckon.con.Open();
                //ckon.myReader = ckon.cmd.ExecuteReader();
                //while (ckon.myReader.Read())
                //{
                //    net_diskon = ckon.myReader.GetInt32("DISCOUNT_PERCENT");
                //    net_price = ckon.myReader.GetInt32("DISCOUNT_PRICE");
                //}
                //ckon.con.Close();
                //====================================================
                ckon.con.Open();
                ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                ckon.adapter.Fill(ckon.dt);
                foreach (DataRow row in ckon.dt.Rows)
                {
                    int n = dgv_purchase.Rows.Add();
                    dgv_purchase.Rows[n].Cells[0].Value = row["ARTICLE_ID"];
                    dgv_purchase.Rows[n].Cells[1].Value = row["ARTICLE_NAME"];
                    dgv_purchase.Rows[n].Cells[2].Value = row["BRAND"];
                    dgv_purchase.Rows[n].Cells[3].Value = row["SIZE"];
                    dgv_purchase.Rows[n].Cells[4].Value = row["COLOR"];
                    dgv_purchase.Rows[n].Cells[5].Value = row["GENDER"];
                    dgv_purchase.Rows[n].Cells[6].Value = row["DEPARTMENT"];
                    dgv_purchase.Rows[n].Cells[7].Value = row["DEPARTMENT_TYPE"];
                    dgv_purchase.Rows[n].Cells[8].Value = row["CUSTOMER_GROUP"];
                    dgv_purchase.Rows[n].Cells[9].Value = row["QTA"];
                    dgv_purchase.Rows[n].Cells[10].Value = row["AMOUNT"];
                    dgv_purchase.Rows[n].Cells[11].Value = row["BANK"];
                    dgv_purchase.Rows[n].Cells[12].Value = row["DISCOUNT_PERCENT"];
                    dgv_purchase.Rows[n].Cells[13].Value = row["DISCOUNT_PRICE"];
                    dgv_purchase.Rows[n].Cells[14].Value = row["SPESIAL_PRICE"];
                    
                }
                //dgv_purchase.Columns[5].DefaultCellStyle.Format = "#,###";
                
                //==========================FUNCTION FOR HIDE FIELD WHEN FIELD EMPTY===============================================
                foreach (DataGridViewColumn clm in dgv_purchase.Columns)
                {
                    dgv_purchase.Columns[clm.Index].Visible = false;
                    //bool notAvailable = true;
                    bool notAvailable = false;

                    foreach (DataGridViewRow row in dgv_purchase.Rows)
                    {

                        if (row.Cells[clm.Index].Value != null)
                        {
                            // If string of value is empty
                            if (row.Cells[clm.Index].Value.ToString() != field_none )
                            {
                                if(row.Cells[clm.Index].Value.ToString() != field_kosong)
                                {
                                    if (row.Cells[clm.Index].Value.ToString() != "")
                                    {
                                        notAvailable = true;
                                        break;
                                    }
                                }
                                //notAvailable = false;
                                
                            }
                        }
                    }

                    if (notAvailable)
                    {
                        //dgv_purchase.Columns[clm.Index].Visible = false;
                        dgv_purchase.Columns[clm.Index].Visible = true;
                    }
                }
                //===================================================================================================================
                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch
            { }
        }
        //===========================================================================================================

        //=======================GET TYPE DISKON=====================================================================
        public void get_type_diskon()
        {
            //if(net_price == 0)
            //if(net_price == 0 && art_bonus == "")
            if (net_price == 0 )
            {
                jenis = "Percent";
            }
            //if(net_diskon == 0)
            //else if(net_diskon==0 && art_bonus == "")
            else if (net_diskon == 0 )
            {
                jenis = "Price";
            }
            else
            //if(net_diskon == 0 && net_price == 0)
            {
                jenis = "Bonus";
            }

        }
        //===========================================================================================================

        //==================================HITUNG TOTAL BELANJA SETELAH DISKON======================================
        public void get_total_price()
        {
            if(jenis == "Percent")
            {
                //total_kotor = (sub_total_TransLine * net_diskon) / 100;
                //total_bersih = the_real_totall - total_kotor;
                //======BARU, UNTUK MENGHITUNG TOTAL DISKON DARI HARGA ASLI PRICE x QUANTITY
                total_kotor = (sub_total_TransLine * net_diskon) / 100;
                total_bersih = sub_total_TransLine - total_kotor;
            }
            if(jenis == "Price")
            {
                //total_kotor = net_price; 
                //total_bersih = the_real_totall - total_kotor;
                //======BARU, UNTUK MENGHITUNG TOTAL DISKON DARI HARGA ASLI PRICE x QUANTITY
                total_kotor = net_price;
                total_bersih = sub_total_TransLine - total_kotor;

            }
        }
        //===========================================================================================================

        //========================UPDATE VALUES DISKON INTO DATABASE TRANSACTION HEADER==============================
        public void update()
        {
            String sql = "UPDATE transaction_line SET DISCOUNT='" + total_kotor + "', SUBTOTAL = '"+ total_bersih +"', DISCOUNT_CODE = '"+ diskon_kode + "', DISCOUNT_DESC = '"+ disc_desc +"' WHERE TRANSACTION_ID='" + id_transaksi + "' AND ARTICLE_ID='" + art_id_diskon + "'";
            CRUD update = new CRUD();
            update.ExecuteNonQuery(sql);
            uc_coba.Instance.retreive();
            uc_coba.Instance.itung_total();
            this.Close();
        }
        //===========================================================================================================

        //================ambil total harga dari article id yg didapat dari saat post discount, untuk diganti harga discount nya==
        public void get_price_TransLine()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM transaction_line WHERE TRANSACTION_ID='" + id_transaksi + "' AND ARTICLE_ID = '" + art_id_diskon + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                qty_kali = ckon.myReader.GetInt32("QUANTITY");
                price_kali = ckon.myReader.GetInt32("PRICE");
                //sub_total_TransLine = ckon.myReader.GetInt32("SUBTOTAL");
                sub_total_TransLine = qty_kali * price_kali;
            }
            ckon.con.Close();
        }
        //===============================================================
        //===================hitung ada berapa item disc sesuai discount code======
        public void count_artcile()
        {
            String sql = "SELECT * FROM discount_item WHERE DISCOUNT_CODE='" + diskon_kode + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    count = count + 1;
                }
                ckon.con.Close();
            }
            else
            {

            }
            ckon.con.Close();
        }
        //CEK DI TRANS LINE APAKAH DISCOUNT CODE INI SUDAH ADA APA BLOM, JIKA ADA=TOLAK, JIKA TIDAK ADA=LANJUTKAN
        public void cek_discount_line()
        {
            String sql = "SELECT * FROM transaction_line WHERE TRANSACTION_ID='" + id_transaksi + "' AND DISCOUNT_CODE = '" + id_diskon + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    count_disc_Tline = count_disc_Tline + 1;
                }
                ckon.con.Close();
            }
            else
            {
                count_disc_Tline = 0;
            }
            ckon.con.Close();
        }

    }
}

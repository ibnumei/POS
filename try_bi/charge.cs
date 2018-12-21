﻿using System;
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
    public partial class charge : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        public String l_trans;
        public int total, the_real_diskon, get_voucher2, tot_diskon2;
        Double count;
        //string result;
        List<double> iList = new List<double>();
        List<double> iList2 = new List<double>();
        private static charge _instance;
        public static charge Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new charge(f1);
                return _instance;
            }
        }
        public charge(Form1 form1)
        {
            // iList2.Clear();
            f1 = form1;
            InitializeComponent();
           

        }
        //======================TERIMA ID DARI FORM SEBELUMNYA===========================
        public void id_trans(String id, int harga, int diskon, int voucher, int dapat_diskon)
        {
            this.ActiveControl = t_cust;
            t_cust.Focus();
            the_real_diskon = diskon;//dapat dari variabel get_dis_vou dari form sebelumnya
            get_voucher2 = voucher;//mengambil nilai voucher dari trans header
            tot_diskon2 = dapat_diskon;//mengambil total diskon dari transaction line dari form sebelumnya
            //====PERCABANGAN UNTUK MENENTUKAN KOMA DI LABEL DISKON=====
            if(tot_diskon2==0)
            { l_diskon2.Text = "0,00"; }
            else
            { l_diskon2.Text = string.Format("{0:#,###}" + ",00", tot_diskon2); }
            //======PERCABANGAN UNTUK MENENTUKAN KOMA DI LABEL VOUCHER====
            if(get_voucher2==0)
            { l_voucher.Text = "0,00"; }
            else
            { l_voucher.Text = string.Format("{0:#,###}" + ",00", get_voucher2); }
            //if(diskon == 0)
            //{
            //    l_diskon2.Text = "0,00";
            //}
            //else
            //{ l_diskon2.Text = string.Format("{0:#,###}" + ",00", diskon); }
            l_transaksi2.Text = id;
            l_total.Text = string.Format("{0:#,###}"+",00", harga);
            l_total2.Text = string.Format("{0:#,###}"+",00", harga);
            retreive();
            total = harga;
        }
        //===============================================================================

        //======================GET DATA FROM ID TRANSAKSI===========================
        public void get_data_id(String id)
        {
            ckon.con.Close();
            String sql = "SELECT * FROM transaction WHERE TRANSACTION_ID='" + id + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                t_cust.Text = ckon.myReader.GetString("CUSTOMER_ID");
                //t_employe.Text = ckon.myReader.GetString("EMPLOYEE_ID");
                t_spg.Text = ckon.myReader.GetString("SPG_ID");
            }

        }
        //==========================================================================

        //==================TAMPILKAN DATA KERANJANG BELANJA================================
        public void retreive()
        {
            String art_id, art_name, spg_id, size, color, qty, disc_desc, sub_total2;
            int price, sub_total;
            ckon.con.Close();
            dgv_purchase2a.Rows.Clear();
            String sql = "SELECT transaction_line.QUANTITY, transaction_line.SUBTOTAL,transaction_line.SPG_ID,transaction_line.DISCOUNT,transaction_line.DISCOUNT_DESC, article.ARTICLE_ID ,article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE FROM transaction_line, article  WHERE article.ARTICLE_ID = transaction_line.ARTICLE_ID AND transaction_line.TRANSACTION_ID='" + l_transaksi2.Text + "' ORDER BY transaction_line._id ASC";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                //ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                //ckon.adapter.Fill(ckon.dt);
                //foreach (DataRow row in ckon.dt.Rows)
                //{
                //    int n = dgv_purchase2a.Rows.Add();
                //    dgv_purchase2a.Rows[n].Cells[0].Value = row["ARTICLE_ID"].ToString();
                //    dgv_purchase2a.Rows[n].Cells[1].Value = row["ARTICLE_NAME"].ToString();
                //    dgv_purchase2a.Rows[n].Cells[2].Value = row["SPG_ID"].ToString();
                //    dgv_purchase2a.Rows[n].Cells[3].Value = row["SIZE"].ToString();
                //    dgv_purchase2a.Rows[n].Cells[4].Value = row["COLOR"].ToString();
                //    dgv_purchase2a.Rows[n].Cells[5].Value = row["PRICE"];
                //    dgv_purchase2a.Rows[n].Cells[6].Value = row["QUANTITY"].ToString();
                //    dgv_purchase2a.Rows[n].Cells[7].Value = row["DISCOUNT_DESC"];
                //    dgv_purchase2a.Rows[n].Cells[8].Value = row["SUBTOTAL"];
                //}
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    art_id = ckon.myReader.GetString("ARTICLE_ID");
                    art_name = ckon.myReader.GetString("ARTICLE_NAME");
                    spg_id = ckon.myReader.GetString("SPG_ID");
                    size = ckon.myReader.GetString("SIZE");
                    color = ckon.myReader.GetString("COLOR");
                    price = ckon.myReader.GetInt32("PRICE");
                    qty = ckon.myReader.GetString("QUANTITY");
                    disc_desc = ckon.myReader.GetString("DISCOUNT_DESC");
                    sub_total = ckon.myReader.GetInt32("SUBTOTAL");

                    if (sub_total == 0)
                    {
                        sub_total2 = "0,00";
                        int n = dgv_purchase2a.Rows.Add();
                        dgv_purchase2a.Rows[n].Cells[0].Value = art_id;
                        dgv_purchase2a.Rows[n].Cells[1].Value = art_name;
                        dgv_purchase2a.Rows[n].Cells[2].Value = spg_id;
                        dgv_purchase2a.Rows[n].Cells[3].Value = size;
                        dgv_purchase2a.Rows[n].Cells[4].Value = color;
                        dgv_purchase2a.Rows[n].Cells[5].Value = price;
                        dgv_purchase2a.Rows[n].Cells[6].Value = qty;
                        dgv_purchase2a.Rows[n].Cells[7].Value = disc_desc;
                        dgv_purchase2a.Rows[n].Cells[8].Value = sub_total2;
                    }
                    else
                    {

                        int n = dgv_purchase2a.Rows.Add();
                        dgv_purchase2a.Rows[n].Cells[0].Value = art_id;
                        dgv_purchase2a.Rows[n].Cells[1].Value = art_name;
                        dgv_purchase2a.Rows[n].Cells[2].Value = spg_id;
                        dgv_purchase2a.Rows[n].Cells[3].Value = size;
                        dgv_purchase2a.Rows[n].Cells[4].Value = color;
                        dgv_purchase2a.Rows[n].Cells[5].Value = price;
                        dgv_purchase2a.Rows[n].Cells[6].Value = qty;
                        dgv_purchase2a.Rows[n].Cells[7].Value = disc_desc;
                        dgv_purchase2a.Rows[n].Cells[8].Value = sub_total;
                    }

                }
                dgv_purchase2a.Columns[5].DefaultCellStyle.Format = "#,###";
                dgv_purchase2a.Columns[7].DefaultCellStyle.Format = "#,###";
                dgv_purchase2a.Columns[8].DefaultCellStyle.Format = "#,###";
                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch
            { }
        }
        //===============================================================================

        
        //=================FORM LOAD=====================================================
        private void charge_Load(object sender, EventArgs e)
        {
            l_transaksi2.Text = l_trans;
        }
        //===============================================================================

        //=============================SETUP QUICK CASK==============================================
        public void QuickCash()
        {
            btnArray = new System.Windows.Forms.Button[0];
            ckon.con.Close();
            Double new_total = System.Convert.ToDouble(total);
            Double no_koma;
            String sql = "SELECT * FROM denomination ORDER BY NOMINAL ASC";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    iList.Add(System.Convert.ToDouble(ckon.myReader["NOMINAL"]));
                }
                for (int i = 0; i < iList.Count; i++)
                {
                    count = new_total / iList[i];
                    no_koma = Math.Floor(count);

                    Double Sugestion;
                    if (i == 0)
                    {

                        Sugestion = new_total;
                    }
                    else
                    {

                        Sugestion = no_koma * iList[i];
                        Sugestion = Sugestion + iList[i];
                    }
                    //ADD ITEM TO LIST2
                    iList2.Add(Sugestion);

                    //REMOVE SAME ITEM IN LIST2
                    int index = 0;
                    while (index < iList2.Count - 1)
                    {

                        if (iList2[index] == iList2[index + 1])
                        { iList2.RemoveAt(index); }
                        else
                        {
                            index++;
                        }

                        //AddButtons2();
                    }

                    //MessageBox.Show(Sugestion.ToString());

                    //iList.Clear();
                    AddButtons2();
                    //iList2.Clear();
                }
               //ASLINYA DISNI
                iList2.Clear();
            }
            //iList2.Clear();
            iList.Clear();
            //==akhir IF HAS.Rows

            ckon.con.Close();
        }
        //===========================================================================================
        //=============================================================================================================================
        System.Windows.Forms.Button[] btnArray;
        public void AddButtons2()
        {
            int xPos = 0;
            int yPos = 0;
            // Declare and assign number of buttons = 26 
           btnArray = new System.Windows.Forms.Button[iList2.Count];
           // pnlButtons.Invalidate();
            pnlButtons.Controls.Clear();
            //System.Windows.Forms.FlatButtonAppearance
            // Create (26) Buttons: 
            for (int i = 0; i < iList2.Count; i++)
            {
                // Initialize one variable 
                btnArray[i] = new System.Windows.Forms.Button();
            }
            int n = 0;

            while (n < iList2.Count)
            {
                btnArray[n].Tag = n + 1; // Tag of button 
                btnArray[n].Width = 120; // Width of button 
                btnArray[n].Height = 60; // Height of button 
                if (n == 4) // Location of second line of buttons: 
                {
                    xPos = 0;
                    yPos = 80;
                }
                // Location of button: 
                btnArray[n].Left = xPos;
                btnArray[n].Top = yPos;
                // Add buttons to a Panel: 
                pnlButtons.Controls.Add(btnArray[n]); // Let panel hold the Buttons 
                xPos = xPos + btnArray[n].Width; // Left of next button 
                                                 // Write English Character: 
                Double value = double.Parse(iList2[n].ToString());
                String value2 = string.Format("{0:#,###}", value);
                btnArray[n].Text = value2;
                btnArray[n].Font = new Font("Arial", 14, FontStyle.Bold);
                btnArray[n].BackColor = Color.White;

                /* **************************************************************** 
                You can use following code instead previous line 
                char[] Alphabet = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 
                'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 
                'W', 'X', 'Y', 'Z'}; btnArray[n].Text = Alphabet[n].ToString(); 
                //**************************************************************** */

                // the Event of click Button 
                btnArray[n].Click += new System.EventHandler(ClickButton2);
                n++;

            }
            //}

            //btnAddButton.Enabled = false; // not need now to this button now 
            //label1.Visible = true;
        }
        //=============================================================================================================================
        public void ClickButton2(Object sender, System.EventArgs e)
        {
            //==MENGEMBALIKAN FOCUS KE TEXTBOXT CUSTOMER ID
            this.ActiveControl = t_cust;
            t_cust.Focus();
            //============================================
            Button btn = (Button)sender;
            String a = btn.Text;
            String b = "quick_cash";
            //MessageBox.Show("You clicked character [" + a + "]");
            payment py = new payment(f1);
            py.quick_cash(a);
            py.aksi_total(total, b, the_real_diskon, tot_diskon2);
            //py.textBox1.Text = "aaaaa";
            py.id_trans = l_transaksi2.Text;
            py.new_total = l_total2.Text;
            py.ShowDialog();
        }
        //==================BUTTON BACK BARU WITH UNDERLINE============================
        private void b_back2_Click(object sender, EventArgs e)
        {
            f1.p_kanan.Controls.Clear();
            //bunifuFlatButton1.selected = true;
            if (!f1.p_kanan.Controls.Contains(uc_coba.Instance))
            {
                f1.p_kanan.Controls.Add(uc_coba.Instance);
                uc_coba.Instance.Dock = DockStyle.Fill;
                //charge.Instance.id_trans(l_transaksi.Text, l_total.Text);
                //uc_coba.Instance.Show();
                uc_coba.Instance.barcode_fokus();
                uc_coba.Instance.BringToFront();
            }
            else
                //charge.Instance.Show();
                uc_coba.Instance.barcode_fokus();
            uc_coba.Instance.BringToFront();
        }
        //===========BUTTON IMAGE SPLIT EDC ============================================
        private void b_Split_edc_Click(object sender, EventArgs e)
        {
            w_split_edc s_edc = new w_split_edc(f1);
            s_edc.get_nilai(total, l_transaksi2.Text, the_real_diskon, tot_diskon2);
            
            s_edc.ShowDialog();
        }
        //=================BUTTON IMAGE SPLIT ===========================================
        private void b_split2_Click(object sender, EventArgs e)
        {
            w_split split = new w_split(f1);

            split.get_nilai(total, l_transaksi2.Text, the_real_diskon, tot_diskon2);
            split.ShowDialog();
        }
        //===============BUTTON IMAGE EDC ==============================================
        private void b_edc2_Click(object sender, EventArgs e)
        {
            w_edc edc = new w_edc(f1);
            edc.aksi_total(total, the_real_diskon, tot_diskon2);
            edc.id_trans = l_transaksi2.Text;
            //py.new_total = l_total2.Text;
            edc.ShowDialog();
        }
        //==================BUTTON IMAGE CASH ==========================================
        private void b_cash2_Click(object sender, EventArgs e)
        {
            String a = "cash";
            payment py = new payment(f1);
            py.aksi_total(total, a, the_real_diskon, tot_diskon2);
            py.id_trans = l_transaksi2.Text;
            //py.new_total = l_total2.Text;
            py.ShowDialog();
        }
        //===============================================================================
        private void t_cust_KeyDown(object sender, KeyEventArgs e)
        {
            //==========BUTTON CASH==============
            if (e.Control && e.KeyCode.ToString() == "C")
            {
                b_cash2_Click(null, null);
            }
            //======BUTTON EDC====================
            if (e.Control && e.KeyCode.ToString() == "E")
            {
                b_edc2_Click(null, null);
            }
            //=============BUTTON SPLIT=============
            if (e.Control && e.KeyCode.ToString() == "S")
            {
                b_split2_Click(null, null);
            }
            //======BUTTON SPLIT EDC===============
            if (e.Control && e.KeyCode.ToString() == "T")
            {
                b_Split_edc_Click(null, null);
            }
            //==============BUTTON BACK==============
            if (e.Control && e.KeyCode.ToString() == "B")
            {
                b_back2_Click(null, null);
            }
        }
        //===============================================================================
    }
}

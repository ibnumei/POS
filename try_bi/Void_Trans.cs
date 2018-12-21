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
    public partial class Void_Trans : Form
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String id_trans, new_idTrans,bulan2,tipe2;
        public String cust_id, employe_id, receipt_id, spg_id, discount, total, status, payment, cash, edc, changee, bank_name, no_ref, store_code, spg_id2,store_code2, no_ref2,id_shift2,id_c_store,edc2,cust_id_Store,curr, bank_name2, spg_id_line, vou_id, vou_code;
        int noo_inv_new, voucher, no_trans2;
        //======================variabel yang digunakan di number sequence==========
        String bulan_now, tahun_now, type_trans="1",bulan_trans, number_trans_string, final_running_number, awal_number= "TR";
        int number_trans;
        private void b_ok_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(final_running_number + "," + bulan_trans + "," + number_trans + ",");
            new_idTrans = final_running_number;
            DateTime mydate = DateTime.Now;

            Inv_Line inv = new Inv_Line();
            String type_trans = "4";
            inv.cek_type_trans(type_trans);
            inv.void_trans(id_trans);
            //====================================
            String a = mydate.ToString("yyyy-MM-dd");
            save_trans_header();
            save_trans_line();
            String sql = "SELECT * FROM transaction WHERE DATE = '" + a + "' AND (STATUS='1' or STATUS='2')";
            UC_Trans_hist.Instance.holding(sql);
            this.Close();
        }

        private void b_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Void_Trans_Load(object sender, EventArgs e)
        {
            this.ActiveControl = t_remark;
            t_remark.Focus();
            new_invoice();
            get_data_id();

            //get_year_month();
            //get_running_number();
            set_running_number();
        }


        public Void_Trans(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }

        public void get_id_trans(String id)
        {
            id_trans = id;
        }

        //=================================GENERATOR NUMBER=================================
        public void new_invoice()
        {
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
            //========================================================================
            //string sql = "SELECT SUBSTRING(TRANSACTION_ID, 8) AS inv FROM transaction ORDER BY TRANSACTION_ID DESC LIMIT 1";
            //ckon.cmd = new MySqlCommand(sql, ckon.con);
            //try
            //{
            //    ckon.con.Open();
            //    ckon.myReader = ckon.cmd.ExecuteReader();
            //    if (ckon.myReader.HasRows)
            //    {
            //        while (ckon.myReader.Read())
            //        {
            //            noo_inv_new = ckon.myReader.GetInt32("inv");
            //            noo_inv_new = noo_inv_new + 1;
            //        }
            //        if (noo_inv_new < 10)
            //        { new_idTrans = store_code + "/TR-000" + noo_inv_new.ToString(); }
            //        else if (noo_inv_new < 100)
            //        { new_idTrans = store_code + "/TR-00" + noo_inv_new.ToString(); }
            //        else if (noo_inv_new < 1000)
            //        { new_idTrans = store_code + "/TR-0" + noo_inv_new.ToString(); }
            //        else if (noo_inv_new < 10000)
            //        { new_idTrans = store_code + "TR-" + noo_inv_new.ToString(); }
            //        else
            //        { }
            //    }
            //    else
            //    { new_idTrans = store_code + "/TR-0001"; }
            //    ckon.con.Close();
            //}
            //catch
            //{ }
        }
        //=================== GET DATA ID===================================================
        public void get_data_id()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM transaction WHERE TRANSACTION_ID='" + id_trans + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                 cust_id = ckon.myReader.GetString("CUSTOMER_ID");
                store_code2 = ckon.myReader.GetString("STORE_CODE");
                 receipt_id = ckon.myReader.GetString("RECEIPT_ID");
                 spg_id = ckon.myReader.GetString("SPG_ID");
                 discount = ckon.myReader.GetString("DISCOUNT");
                 total = ckon.myReader.GetString("TOTAL");
                 status = ckon.myReader.GetString("STATUS");
                 payment =  ckon.myReader.GetString("PAYMENT_TYPE");
                 cash = ckon.myReader.GetString("CASH");
                 edc = ckon.myReader.GetString("EDC");
                edc2 = ckon.myReader.GetString("EDC2");
                changee = ckon.myReader.GetString("CHANGEE");
                voucher = ckon.myReader.GetInt32("VOUCHER");
                 bank_name = ckon.myReader.GetString("BANK_NAME");
                bank_name2 = ckon.myReader.GetString("BANK_NAME2");
                no_ref = ckon.myReader.GetString("NO_REF");
                no_ref2 = ckon.myReader.GetString("NO_REF2");
                cust_id_Store  = ckon.myReader.GetString("CUST_ID_STORE");
                curr  = ckon.myReader.GetString("CURRENCY");
                id_shift2  = ckon.myReader.GetString("ID_SHIFT");
                id_c_store = ckon.myReader.GetString("ID_C_STORE");
                vou_id = ckon.myReader.GetString("VOUCHER_ID");
                vou_code = ckon.myReader.GetString("VOUCHER_CODE");
            }
            ckon.con.Close();
        }
        //======================INPUT TRANSAKSI HEADER======================================
        public void save_trans_header()
        {
            try
            {
                ckon.con.Close();
                DateTime mydate = DateTime.Now;
                DateTime myhour = DateTime.Now;

                String sql = "INSERT INTO transaction (TRANSACTION_ID,STORE_CODE,CUSTOMER_ID,RECEIPT_ID,SPG_ID,DISCOUNT,TOTAL,STATUS,PAYMENT_TYPE,CASH,EDC,EDC2,CHANGEE,VOUCHER,BANK_NAME,BANK_NAME2,NO_REF,NO_REF2, DATE, TIME,CUST_ID_STORE,CURRENCY,ID_SHIFT,ID_C_STORE,VOUCHER_ID,VOUCHER_CODE) VALUES ('" + new_idTrans + "', '" + store_code2 + "','" + cust_id + "'  ,'" + receipt_id + "','" + spg_id + "','-" + discount + "', '-" + total + "', '2', '" + payment + "', '-" + cash + "', '-" + edc + "','-" + edc2 + "', '-" + changee + "', '-" + voucher + "','" + bank_name + "', '" + bank_name2 + "','" + no_ref + "','" + no_ref2 + "', '" + mydate.ToString("yyyy-MM-dd") + "', '" + myhour.ToLocalTime().ToString("H:mm:ss") + "','" + cust_id_Store + "','" + curr + "','" + id_shift2 + "','" + id_c_store + "','" + vou_id + "', '" + vou_code + "') ";
                ckon.con.Open();
                ckon.cmd = new MySqlCommand(sql, ckon.con);
                ckon.cmd.ExecuteNonQuery();
                ckon.con.Close();

                String query = "UPDATE auto_number SET Month = '" + bulan_now + "', Number = '" + number_trans + "' WHERE Type_Trans='1'";
                CRUD ubah = new CRUD();
                ubah.ExecuteNonQuery(query);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }
        //======================INPUT TRANSAKSI LINE========================================
        public void save_trans_line()
        {
            try
            {
                ckon.con.Close();
                koneksi2 ckon2 = new koneksi2();

                String sql = "SELECT * FROM transaction_line WHERE TRANSACTION_ID = '" + id_trans + "'";
                ckon.cmd = new MySqlCommand(sql, ckon.con);
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    String article_id = ckon.myReader.GetString("ARTICLE_ID");
                    String qty = ckon.myReader.GetString("QUANTITY");
                    String price = ckon.myReader.GetString("PRICE");
                    String subtotal = ckon.myReader.GetString("SUBTOTAL");
                    String discount = ckon.myReader.GetString("DISCOUNT");
                    String spg_id_line = ckon.myReader.GetString("SPG_ID");
                    String dis_code = ckon.myReader.GetString("DISCOUNT_CODE");
                    String dis_type = ckon.myReader.GetString("DISCOUNT_TYPE");

                    String SQL2 = "INSERT INTO transaction_line(TRANSACTION_ID, ARTICLE_ID, QUANTITY, PRICE,DISCOUNT, SUBTOTAL,SPG_ID,DISCOUNT_CODE, DISCOUNT_TYPE) VALUES ('" + new_idTrans + "', '" + article_id + "', '-" + qty + "', '" + price + "', '-" + discount + "','-" + subtotal + "','" + spg_id_line + "','" + dis_code + "', '" + dis_type + "')";
                    ckon2.con2.Open();
                    ckon2.cmd2 = new MySqlCommand(SQL2, ckon2.con2);
                    ckon2.cmd2.ExecuteNonQuery();
                    ckon2.con2.Close();
                }
                ckon.con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
            String sql = "SELECT * FROM auto_number WHERE Store_Code = '" + store_code + "' AND Type_Trans = '1'";
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
                    final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-" + number_trans_string;
                    //l_transaksi.Text = final_running_number;

                }
                else
                {
                    number_trans = 1;
                    bulan_trans = bulan_now;//MENJADIKAN BULAN TRANSAKSI = BULAN SEKARANG
                    //==MEMBUAT STRING FINAL RUNNING NUMBER
                    final_running_number = "TR/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                    //l_transaksi.Text = final_running_number;
                }


            }
           
            ckon.con.Close();
        }
    }
}

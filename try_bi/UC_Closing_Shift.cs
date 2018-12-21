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
    public partial class UC_Closing_Shift : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        koneksi2 ckon2 = new koneksi2();
        String id_trans, view_date, article_id, shift, id_list, shift_name, id_shift, epy_id, epy_name, date_closing_shift, date_closing_shift2;
        int amount, cash, change, diskon, edc, edc2, voucher, totall, get_voucher, get_dis_vou,get_diskon;
        int total_qty2 = 0;

        //===================klik button edc==========================
        private void b_edc_Click(object sender, EventArgs e)
        {
            w_edc_closing_shift close = new w_edc_closing_shift();
            close.shift_code = shift;
            close.id_shift2 = id_shift;
            close.ShowDialog();

        }
        //==============SET EMPLOYEE ID AND EMPLOYEEE NAME======
        public void set_name(String id, String name)
        {
            epy_id = id;
            epy_name = name;
        }
        //================================BUTTON CLOSE===================
        private void b_close_Click(object sender, EventArgs e)
        {
            w_form_closing_shift close = new w_form_closing_shift(f1);
            close.get_qty(total_qty2);
            close.set_name2(epy_id, epy_name);
            close.shift_code = shift;
            close.id_shift2 = id_shift;
            close.date_closing_shift3 = date_closing_shift2;
            close.ShowDialog();
        }

        //=======================================================
        private static UC_Closing_Shift _instance;
        public static UC_Closing_Shift Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_Closing_Shift(f1);
                return _instance;
            }
        }
        //=======================================================
        public UC_Closing_Shift(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //===method untuk mengambil id terakhir dati tabel close shift============================
        public void get_id_shift()
        {
            ckon.con.Close();
            String sql3 = "SELECT * FROM closing_shift WHERE STATUS_CLOSE='0' ORDER BY _id DESC LIMIT 1";
            ckon.cmd = new MySqlCommand(sql3, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_shift = ckon.myReader.GetString("ID_SHIFT");
                    l_shift_name.Text = ckon.myReader.GetString("SHIFT");
                    date_closing_shift = ckon.myReader.GetString("OPENING_TIME");
                }
            }
            else
            { id_shift = "0"; }
            date_closing_shift2 = date_closing_shift.Substring(0, 10);
            ckon.con.Close();
        }
        //==========METHOD YANG AKAN DIPANGGIL DARI FORM1, AKAN DIJALANKAN UNTUK CLOSING SHIFT==================
        public void from_form1()
        {
            //String query2 = "SELECT * FROM transaction WHERE STATUS='1' AND IS_CLOSE='0' AND SHIFT_CODE='" + shift + "' AND CLOSE_SHIFT='0'";
            //holding(query2);
            //String sql = "SELECT transaction.TRANSACTION_ID FROM transaction INNER JOIN closing_shift ON transaction.ID_SHIFT = closing_shift._id WHERE closing_shift.STATUS_CLOSE = '0' AND transaction.STATUS='1'";
            //holding(sql);
            String sql = "SELECT * FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            holding(sql);
        }
        //======================================================================================================
        //====================HOLDING SHIFT===================================
        public void holding(String tanggal)
        {
            //String date2;
            //date2 = tanggal;
            dgv_hold.Rows.Clear();
            koneksi2 ckon2 = new koneksi2();
            ckon.con.Close();
            //String sql = "SELECT * FROM transaction WHERE DATE='" + date2 + "' ";
            String sql = tanggal;
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            List<string> numbersList = new List<string>();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_trans = ckon.myReader.GetString("TRANSACTION_ID");
                    view_date = ckon.myReader.GetString("TIME");
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
                    dgv_hold.Rows[n].Cells[2].Value = view_date;
                    ckon2.con2.Close();
                }

            }
            ckon.con.Close();
        }
        //=====================================================================================

        //===============TAMPILKAN DATA PENJUALAN===================================================
        public void retreive()
        {
            String art_id, art_name, spg_id, size, color, qty, disc_desc, sub_total2;
            int price, sub_total;
            ckon.con.Close();
            dgv_purchase.Rows.Clear();
            String sql = "SELECT  transaction_line.ARTICLE_ID ,transaction_line.QUANTITY, transaction_line.SUBTOTAL, transaction_line.SPG_ID, transaction_line.DISCOUNT, transaction_line.DISCOUNT_DESC,article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE FROM transaction_line, article  WHERE article.ARTICLE_ID = transaction_line. ARTICLE_ID AND transaction_line.TRANSACTION_ID='" + l_transaksi.Text + "' ORDER BY transaction_line._id ASC";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                //ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                //ckon.adapter.Fill(ckon.dt);
                //foreach (DataRow row in ckon.dt.Rows)
                //{
                //    int n = dgv_purchase.Rows.Add();
                //    dgv_purchase.Rows[n].Cells[0].Value = row["ARTICLE_ID"].ToString();
                //    dgv_purchase.Rows[n].Cells[1].Value = row["ARTICLE_NAME"].ToString();
                //    dgv_purchase.Rows[n].Cells[2].Value = row["SPG_ID"];
                //    dgv_purchase.Rows[n].Cells[3].Value = row["SIZE"].ToString();
                //    dgv_purchase.Rows[n].Cells[4].Value = row["COLOR"].ToString();
                //    dgv_purchase.Rows[n].Cells[5].Value = row["PRICE"];
                //    dgv_purchase.Rows[n].Cells[6].Value = row["QUANTITY"].ToString();
                //    dgv_purchase.Rows[n].Cells[7].Value = row["DISCOUNT_DESC"];
                //    dgv_purchase.Rows[n].Cells[8].Value = row["SUBTOTAL"];
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

                }
                dgv_purchase.Columns[5].DefaultCellStyle.Format = "#,###";
                dgv_purchase.Columns[7].DefaultCellStyle.Format = "#,###";
                dgv_purchase.Columns[8].DefaultCellStyle.Format = "#,###";
                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch
            { }

        }



        //===========================================================================================

        //===========================GET DATA TERAKHIR DI MODAL STORE================================
        public void get_shift()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM modal_store ORDER BY _id DESC LIMIT 1";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    shift = ckon.myReader.GetString("SHIFT_CODE");
                    shift_name = ckon.myReader.GetString("SHIFT_NAME");
                }
                l_shift_name.Text = shift_name;
            }
            ckon.con.Close();
        }
        //===========================================================================================

        //=========================TEXTBOXT SEARCH TRANSAKSI=========================================
        private void t_search_trans_OnTextChange(object sender, EventArgs e)
        {
            if (t_search_trans.text == "")
            {
                //String query2 = "SELECT * FROM transaction WHERE STATUS='1' AND IS_CLOSE='0' AND SHIFT_CODE='"+ shift +"' AND CLOSE_SHIFT='0'";
                //holding(query2);
                String sql = "SELECT *  FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
                holding(sql);
            }
            else
            {
                //String query2 = "SELECT * FROM transaction WHERE STATUS='1' AND IS_CLOSE='0' AND SHIFT_CODE='" + shift + "' AND CLOSE_SHIFT='0' AND TRANSACTION_ID LIKE '%" + t_search_trans.text + "%'";
                //holding(query2);
                String sql = "SELECT * FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2') AND TRANSACTION_ID LIKE '%" + t_search_trans.text +"%'";
                holding(sql);
            }
        }
        //===========================================================================================

        //=================== GET DATA ID===================================================
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
                //t_employId.Text = ckon.myReader.GetString("EMPLOYEE_ID");
                t_spgId.Text = ckon.myReader.GetString("SPG_ID");
                //l_diskon.Text = ckon.myReader.GetString("DISCOUNT");
            }
            ckon.con.Close();
        }
        //==================================================================================

        //==================klik data di tabel hold============================
        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            id_list = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            l_transaksi.Text = id_list;
            retreive();
            itung_total();
            get_data_id();
        }
        //=====================================================================

        //===========================ITUNG TOTAL BELANJA=====================================================
        public void itung_total()
        {
            ckon.con.Close();
            //=====================================GET VALUE DISCOUNT FROM TRANSACTION HEADER======================
            String sql2 = "SELECT * FROM TRANSACTION WHERE TRANSACTION_ID ='" + l_transaksi.Text + "'";
            ckon.cmd = new MySqlCommand(sql2, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    get_diskon = ckon.myReader.GetInt32("DISCOUNT");
                    get_voucher = ckon.myReader.GetInt32("VOUCHER");
                }
                get_dis_vou = get_diskon + get_voucher;
                ckon.con.Close();
                if (get_diskon == 0)
                { l_diskon.Text = "0,00"; }
                else
                { l_diskon.Text = string.Format("{0:#,###}" + ",00", get_diskon); }
                if(get_voucher == 0)
                { l_vou.Text = "0,00"; }
                else
                { l_vou.Text = string.Format("{0:#,###}" + ",00", get_voucher); }
            }
            catch
            {
                get_dis_vou = 0;
                l_diskon.Text = "0,00";
                l_vou.Text = "0,00";
            }
            //String sql2 = "SELECT SUM(transaction_line.DISCOUNT) as total FROM transaction_line WHERE TRANSACTION_ID='" + l_transaksi.Text + "'";
            //ckon.cmd = new MySqlCommand(sql2, ckon.con);
            //try
            //{
            //    ckon.con.Open();
            //    ckon.myReader = ckon.cmd.ExecuteReader();
            //    while (ckon.myReader.Read())
            //    {
            //        get_dis_vou = ckon.myReader.GetInt32("total");
            //        if (get_dis_vou == 0)
            //        { l_diskon.Text = "0"; }
            //        else
            //        { l_diskon.Text = string.Format("{0:#,###}" + ",00", get_dis_vou); }

            //    }
            //    ckon.con.Close();
            //}
            //catch
            //{ l_diskon.Text = "0,00"; }
            //==========================================================================================================
            String sql = "SELECT SUM(transaction_line.SUBTOTAL) as total FROM transaction_line WHERE TRANSACTION_ID='" + l_transaksi.Text + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {

                    totall = ckon.myReader.GetInt32("total");
                    totall = totall - get_voucher;
                    l_total.Text = string.Format("{0:#,###}" + ",00", totall);
                }
                ckon.con.Close();
            }
            catch
            { }

            //l_total.Text = totall.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
        }
        //===================================================================================

        //==================================================================================
        public void total_trans()
        {
            ckon.con.Close();
            int qty = 0;
            int total_qty = 0;
            String sql1 = "SELECT * FROM transaction WHERE ID_SHIFT = '" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            //String sql1 = "SELECT * FROM transaction WHERE ID_SHIFT = '" + id_shift + "' AND STATUS='1' AND TRANSACTION_ID='AAA/TR-0003'";
            ckon.cmd = new MySqlCommand(sql1, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while(ckon.myReader.Read())
            {
                String id_trans = ckon.myReader.GetString("TRANSACTION_ID");
                String sql11 = "SELECT SUM(transaction_line.QUANTITY) as total from transaction_line WHERE TRANSACTION_ID='"+ id_trans +"'";
                ckon2.cmd2 = new MySqlCommand(sql11, ckon2.con2);
                ckon2.con2.Open();
                ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
                while(ckon2.myReader2.Read())
                {
                    qty = ckon2.myReader2.GetInt32("total");
                    //MessageBox.Show(qty + "");
                    total_qty = total_qty + (qty);
                }
                ckon2.con2.Close();
                total_qty2 = total_qty;
            }
            l_qty.Text = total_qty.ToString();
            ckon.con.Close();
            //============================================================================================
            String sql = "SELECT SUM(transaction.TOTAL) as total FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Close();
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {

                try
                {
                    amount = ckon.myReader.GetInt32("total");
                    if (amount <= 0)
                    { l_total_amount.Text = "0,00"; }
                    else
                    { l_total_amount.Text = string.Format("{0:#,###}" + ",00", amount); }

                    //l_total_amount.Text = amount.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
                }
                catch
                {
                    l_total_amount.Text = "0,00";
                }
            }
            ckon.con.Close();

            //===================================================================================================

            String sql2 = "SELECT SUM(transaction.CASH) as total FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            ckon.cmd = new MySqlCommand(sql2, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                try
                {
                    cash = ckon.myReader.GetInt32("total");
                    //l_cash.Text = cash.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
                }
                catch
                { cash = 0; }
            }
            ckon.con.Close();

            String sql2a = "SELECT SUM(transaction.CHANGEE) as total FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            ckon.cmd = new MySqlCommand(sql2a, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                try
                {
                    change = ckon.myReader.GetInt32("total");
                    //l_cash.Text = cash.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
                }
                catch
                { change = 0; }
            }
            ckon.con.Close();
            int cash2 = cash - change;
            if (cash2 <= 0)
            { l_cash.Text = "0,00"; }
            else
            { l_cash.Text = string.Format("{0:#,###}" + ",00", cash2); }

            //l_cash.Text = cash2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //===================================================================================================
            String sql3 = "SELECT SUM(transaction.DISCOUNT) as total FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            ckon.cmd = new MySqlCommand(sql3, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                try
                {
                    diskon = ckon.myReader.GetInt32("total");
                    if (diskon <= 0)
                    { l_discount.Text = "0,00"; }
                    else
                    { l_discount.Text = string.Format("{0:#,###}" + ",00", diskon); }
                    //l_discount.Text = diskon.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
                }
                catch
                {
                    l_discount.Text = "0,00";
                }
            }
            ckon.con.Close();

            //===================================================================================================
            String sql4 = "SELECT SUM(transaction.EDC) as total FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            ckon.cmd = new MySqlCommand(sql4, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                try
                {
                    edc = ckon.myReader.GetInt32("total");
                    //if(edc <= 0)
                    //{ l_edc.Text = "0,00"; }
                    //else
                    //{ l_edc.Text = string.Format("{0:#,###}" + ",00", edc); }
                    //l_edc.Text = edc.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
                }
                catch
                {
                    edc = 0;
                    //l_edc.Text = "0,00";
                }
            }
            ckon.con.Close();
            String sql5 = "SELECT SUM(transaction.EDC2) as total FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            ckon.cmd = new MySqlCommand(sql5, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                try
                {
                    edc2 = ckon.myReader.GetInt32("total");
                }
                catch
                {
                    edc2 = 0;
                }
            }
            ckon.con.Close();
            int edc_total = edc + edc2;
            if (edc_total <= 0)
            { l_edc.Text = "0,00"; }
            else
            { l_edc.Text = string.Format("{0:#,###}" + ",00", edc_total); }
            //===================================================================================================
            String sql6 = "SELECT SUM(transaction.VOUCHER) as total FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' or STATUS='2')";
            ckon.cmd = new MySqlCommand(sql6, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                try
                {
                    voucher = ckon.myReader.GetInt32("total");
                    if (voucher <= 0)
                    { l_voucher.Text = "0,00"; }
                    else
                    { l_voucher.Text = string.Format("{0:#,###}" + ",00", voucher); }
                    //l_discount.Text = diskon.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
                }
                catch
                {
                    l_voucher.Text = "0,00";
                }
            }
            ckon.con.Close();
            //============MENDAPATKAN PETTY CASH YG TERSISA DARI TABEL STORE===========
            String sql7 = "Select * from store";
            ckon.cmd = new MySqlCommand(sql7, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while(ckon.myReader.Read())
            {
                int petty = ckon.myReader.GetInt32("BUDGET_TO_STORE");
                if(petty==0)
                {
                    l_petty.Text = "0,00";
                }
                else
                {
                    l_petty.Text = string.Format("{0:#,###}" + ",00", petty);
                }
            }
            ckon.con.Close();
        }
        //====================RESET==================================
        public void reset()
        {
            dgv_hold.Rows.Clear();
            dgv_purchase.Rows.Clear();
            l_transaksi.Text = "";
            t_custId.Text = "";
            //t_employId.Text = "";
            t_spgId.Text = "";
            l_diskon.Text = "0";
            l_total.Text = "0";
        }
    
    }
}

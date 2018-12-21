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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PrinterUtility;
using System.Collections;
using System.IO;

namespace try_bi
{
    public partial class UC_Trans_hist : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();

        String id_trans, article_id, id_list, date, view_date, a_date, a_jam, a_id, a_total, a_name, a_sub, a_subtotal, pay_type, rev, string_tipe, string_diskon, id_shift, id_bank1, id_bank2, bank_name1, bank_name2;
        int totall, int_subtotal, edc2;
        
        int diskon, total, cash, edc, change, tot_dis, tot_pay, get_diskon, get_voucher, get_dis_vou;
        String noref, noref2;
        //-----variable validasi tanggal
        String tgl_trans, tgl_validasi;
        int days = 7;
        //======================================================
        private static UC_Trans_hist _instance;
        public static UC_Trans_hist Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_Trans_hist(f1);
                return _instance;
            }
        }
        //=======================================================
        public UC_Trans_hist(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }

        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            DateTime mydate = DateTime.Now;

            date = mydate.ToString("yyyy-MM-dd");
            id_list = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            l_transaksi.Text = id_list;
            retreive();
            itung_total();
            get_data_id();
        }
        //=============================GET DATA ID SHIFT===============================
        public void get_id_shift()
        {
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
        }
        //MENCARI ID TRANSAKSI BERDASARKAN KOLOM PENCARIAN====
        private void t_search_trans_OnTextChange(object sender, EventArgs e)
        {
            if (t_search_trans.text == "")
            {
                String sql = "SELECT * FROM transaction WHERE DATE = '"+ d_tgl_trans.Text +"' AND (STATUS='1' or STATUS='2')";
                holding(sql);
            }
            else
            {
                String sql = "SELECT * FROM transaction WHERE TRANSACTION_ID LIKE '%" + t_search_trans.text + "%' AND DATE = '"+ d_tgl_trans.Text +"' AND (STATUS='1' or STATUS='2')";
                holding(sql);
            }
        }
        //======================LIST HOLD TRANSACTION============================================
        public void holding(String sql)
        {
            //String date2;
            //date2 = tanggal;
            dgv_hold.Rows.Clear();
            koneksi2 ckon2 = new koneksi2();
            ckon.con.Close();
            //String sql = "SELECT * FROM transaction WHERE ID_SHIFT='" + id_shift + "' AND (STATUS='1' OR STATUS = '2')";
            //String sql = "SELECT * FROM transaction WHERE DATE = '"+ date2 +"' AND (STATUS='1' OR STATUS = '2')";
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
        //=======================================================================================
        private void b_void_Click(object sender, EventArgs e)
        {
            DateTime mydate = DateTime.Now;
            
            //cek validasi tanggal, lebih dari 7 hari maka tidak bisa buka menu remark void
            DateTime dt = Convert.ToDateTime(tgl_trans);//convert tanggal transaksi
            DateTime tgl_validasi = dt.AddDays(days); //ambil tanggal 7 hari dari tanggal transaksi
            String tgl_skrng = mydate.ToString("yyyy-MM-dd"); //ambil tanggal hari ini
            DateTime tgl_skrng2 = Convert.ToDateTime(tgl_skrng); //convert tanggal skrng 
            TimeSpan ts = new TimeSpan();
            ts = tgl_skrng2.Subtract(dt);//ambil brapa jeda hari dari tgl skrng ke tanggal transaksi yg udh ditambah 7 hari
            int count_day = Convert.ToInt32(ts.Days);//ubah jeda hari kedalam variabel
                                                     //jika lebih dari 7 hari maka transaksi tidak bisa di void 
            //MessageBox.Show(count_day + "");
            if (count_day >= 7)
            {
                MessageBox.Show("Transaction Can't Be Void");
            }
            else
            {
                Void_Trans VT = new Void_Trans(f1);
                VT.get_id_trans(l_transaksi.Text);
                VT.spg_id = t_spgId.Text;
                VT.ShowDialog();
            }
            //============================

        }
        //=======================================================================================
        private void d_tgl_trans_ValueChanged(object sender, EventArgs e)
        {
            //String a = d_tgl_trans.Value.ToString("yyyy-MM-dd");
            String a = "SELECT * FROM transaction WHERE DATE = '" + d_tgl_trans.Text + "' AND (STATUS='1' OR STATUS='2')";
            holding(a);
        }

        //=============================================================================================

        //===============TAMPILKAN DATA PENJUALAN===================================================
        public void retreive()
        {
            String art_id, art_name, spg_id, size, color, qty, disc_desc, sub_total2;
            int price, sub_total;
            ckon.con.Close();
            dgv_purchase.Rows.Clear();
            String sql = "SELECT  transaction_line.ARTICLE_ID ,transaction_line.QUANTITY, transaction_line.SUBTOTAL, transaction_line.SPG_ID,transaction_line.DISCOUNT, transaction_line.DISCOUNT_DESC,article.ARTICLE_NAME, article.SIZE, article.COLOR, article.PRICE FROM transaction_line, article  WHERE article.ARTICLE_ID = transaction_line. ARTICLE_ID AND transaction_line.TRANSACTION_ID='" + l_transaksi.Text + "' ORDER BY transaction_line._id ASC";
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
        //===================================================================================

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
                if(get_voucher==0)
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
            //=======================================================================================================
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
                t_spgId.Text = ckon.myReader.GetString("SPG_ID");
                 diskon = ckon.myReader.GetInt32("DISCOUNT");
                 total = ckon.myReader.GetInt32("TOTAL");
                 cash = ckon.myReader.GetInt32("CASH");
                 edc = ckon.myReader.GetInt32("EDC");
                edc2 = ckon.myReader.GetInt32("EDC2");
                 change = ckon.myReader.GetInt32("CHANGEE");
                 noref = ckon.myReader.GetString("NO_REF");
                noref2 = ckon.myReader.GetString("NO_REF2");
                pay_type = ckon.myReader.GetString("PAYMENT_TYPE");
                tgl_trans = ckon.myReader.GetString("DATE");
                id_bank1 = ckon.myReader.GetString("BANK_NAME");
                id_bank2 = ckon.myReader.GetString("BANK_NAME2");
                
            }
            if(pay_type == "0")
            {
                string_tipe = "CASH";
                rev = "-";
            }
            if(pay_type == "1")
            {
                string_tipe = "EDC";
                rev = noref;
                //mencari bank name sesuai bank id
                get_id_bank();
            }
            if(pay_type == "2")
            {
                string_tipe = "SPLIT";
                rev = noref;
                //mencari bank name sesuai bank id
                get_id_bank();
            }
            if(pay_type == "3")
            {
                string_tipe = "SPLIT EDC";
                rev = noref;
                //mencari bank name sesuai bank id
                get_id_bank();
            }
            //MENCETAK METHOD PEMBAYARAN
            desc_method_payment();
            tot_dis = total + diskon;
            tot_pay = cash + edc;
        }
        //mencetak method pembayaran ke textbox
        public void desc_method_payment()
        {
            if(string_tipe == "CASH")
            {
                t_payment_method.Text = "Payment Method : Cash";
                t_detail_charge.Text = "";
            }
            if(string_tipe == "EDC")
            {
                t_payment_method.Text = "Payment Method : EDC";
                t_detail_charge.Text = bank_name1 + " : " + string.Format("{0:#,###}" + ",00", edc)+"-No Ref : "+noref;
            }
            if (string_tipe == "SPLIT")
            {
                t_payment_method.Text = "Payment Method : SPLIT";
                t_detail_charge.Text = "Cash : " + string.Format("{0:#,###}" + ",00", cash)+" . "+bank_name1 + " : "+ string.Format("{0:#,###}" + ",00", edc) + "-No Ref : " + noref;
            }
            if (string_tipe == "SPLIT EDC")
            {
                t_payment_method.Text = "Payment Method : SPLIT EDC";
                t_detail_charge.Text = bank_name1 + " : " + string.Format("{0:#,###}" + ",00", edc) + "-No Ref : "+ noref +". "+bank_name2+" : "+ string.Format("{0:#,###}" + ",00", edc2)+"-No Ref : "+noref2;
            }
        }
        //==================================================================================
        //mengambil id bank berdasarkan nama bank yang dipilih===
        public void get_id_bank()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM bank WHERE BANK_ID LIKE '%" + id_bank1 + "%'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                bank_name1 = ckon.myReader.GetString("BANK_NAME");
            }
            ckon.con.Close();
            String sql2 = "SELECT * FROM bank WHERE BANK_ID LIKE '%" + id_bank2 + "%'";
            ckon.cmd = new MySqlCommand(sql2, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                bank_name2 = ckon.myReader.GetString("BANK_NAME");
            }
            ckon.con.Close();
        }
        //=========================================================================
        private void b_print_Click(object sender, EventArgs e)
        {
            ckon.con.Close();
            //=============================================================================================
            NewFunctionPrinter print = new NewFunctionPrinter();
            print.get_trans_id(id_list);
            print.get_nm_store();
            print.get_currency();
            print.get_trans_header();
            print.coba_print();

            //a_date = mydate.ToString("yyyy-MM-dd");
            //a_jam = myhour.ToLocalTime().ToString("H:mm:ss");
            //PrinterUtility.EscPosEpsonCommands.EscPosEpson obj = new PrinterUtility.EscPosEpsonCommands.EscPosEpson();
            //var BytesValue = Encoding.ASCII.GetBytes(string.Empty);
            ////var BytesValue = GetLogo(@"E:\sample.bmp"); 
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Center());
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.CharSize.DoubleWidth6());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.FontSelect.FontE());
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Center());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Left());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("           3 SECOND AMBON CITY CENTER\n"));
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.CharSize.DoubleWidth4());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("           Jl. Patimura Ambon No. 13\n\n\n"));
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.CharSize.Nomarl());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Left());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Date: " + a_date + "   Time: " + a_jam + "\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   -----------------------------------------\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   TransID : " + id_list + "\n\n"));
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   SKU                    Total\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Description\n\n"));
            //String sql = "SELECT transaction_line.ARTICLE_ID, transaction_line.QUANTITY, transaction_line.SUBTOTAL, article.ARTICLE_NAME FROM transaction_line INNER JOIN article on transaction_line.ARTICLE_ID = article.ARTICLE_ID WHERE transaction_line.TRANSACTION_ID = '" + id_list + "'";
            //ckon.cmd = new MySqlCommand(sql, ckon.con);
            //ckon.con.Open();
            //ckon.myReader = ckon.cmd.ExecuteReader();
            //while (ckon.myReader.Read())
            //{
            //    a_id = ckon.myReader.GetString("ARTICLE_ID");
            //    a_total = ckon.myReader.GetString("QUANTITY");
            //    int sub_int = ckon.myReader.GetInt32("SUBTOTAL");
            //    a_name = ckon.myReader.GetString("ARTICLE_NAME");
            //    //String a_name2 = a_name.Substring(0, 30);
            //    String a_name2 = a_name;
            //    a_sub = sub_int.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //    BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   " + a_id + "   X " + a_total + "  " + a_sub + "\n"));
            //    BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   " + a_name2 + " \n"));
            //}
            //ckon.con.Close();
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("\n"));
            //String sql2 = "SELECT SUM(transaction_line.SUBTOTAL) as total FROM transaction_line WHERE TRANSACTION_ID='" + id_list + "'";
            //ckon.cmd = new MySqlCommand(sql2, ckon.con);
            //ckon.con.Open();
            //ckon.myReader = ckon.cmd.ExecuteReader();
            //while (ckon.myReader.Read())
            //{
            //    int_subtotal = ckon.myReader.GetInt32("total");
            //    a_subtotal = int_subtotal.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //    BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   SubTotal         " + a_subtotal + "\n"));
            //}
            //ckon.con.Close();
            //if(diskon==0)
            //{ string_diskon = "0,00"; }
            //else
            //{ string_diskon = diskon.ToString("C2", CultureInfo.GetCultureInfo("id-ID")); }
            //String total_bel = tot_dis.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //String from_value = tot_pay.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //String from_change = change.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Discount         " + string_diskon +"\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   -----------------------------------------\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Total            " + total_bel + "\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   =========================================\n\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Payment Type : "+ string_tipe +"\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   No. REF      : "+ rev +"\n"));

            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Payment      : " + from_value + "\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Change       : " + from_change + "\n\n"));
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Center());
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Left());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("              Return and Exchanges\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("            Within 3 Days With Receipt\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("              And Tags Attached\n\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Lf());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("             Thank You For Coming\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("           WWW.3SECOND-CLOTHING.COM\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Alignment.Left());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, CutPage());
            ////PrinterUtility.PrintExtensions.Print(BytesValue, TryPrintStruk.Properties.Settings.Default.PrinterPath);
            //if (File.Exists(".\\tmpPrint.print"))

            //    File.Delete(".\\tmpPrint.print");
            //File.WriteAllBytes(".\\tmpPrint.print", BytesValue);
            //RawPrinterHelper.SendFileToPrinter("EPSON LX-310", ".\\tmpPrint.print");
            //try
            //{
            //    File.Delete(".\\tmpPrint.print");
            //}
            //catch
            //{
            //    //File.Delete(".\\tmpPrint.print");
            //}

            //=============================================================================================
        }
        //===========================FOR PRINT STRUCK===========================
        //public byte[] CutPage()
        //{
        //    List<byte> oby = new List<byte>();
        //    oby.Add(Convert.ToByte(Convert.ToChar(0x1D)));
        //    oby.Add(Convert.ToByte('V'));
        //    oby.Add((byte)66);
        //    oby.Add((byte)3);
        //    return oby.ToArray();
        //}

        //public byte[] GetLogo(string LogoPath)
        //{
        //    List<byte> byteList = new List<byte>();
        //    if (!File.Exists(LogoPath))
        //        return null;
        //    BitmapData data = GetBitmapData(LogoPath);
        //    BitArray dots = data.Dots;
        //    byte[] width = BitConverter.GetBytes(data.Width);

        //    int offset = 0;
        //    MemoryStream stream = new MemoryStream();
        //    // BinaryWriter bw = new BinaryWriter(stream);
        //    byteList.Add(Convert.ToByte(Convert.ToChar(0x1B)));
        //    //bw.Write((char));
        //    byteList.Add(Convert.ToByte('@'));
        //    //bw.Write('@');
        //    byteList.Add(Convert.ToByte(Convert.ToChar(0x1B)));
        //    // bw.Write((char)0x1B);
        //    byteList.Add(Convert.ToByte('3'));
        //    //bw.Write('3');
        //    //bw.Write((byte)24);
        //    byteList.Add((byte)24);
        //    while (offset < data.Height)
        //    {
        //        byteList.Add(Convert.ToByte(Convert.ToChar(0x1B)));
        //        byteList.Add(Convert.ToByte('*'));
        //        //bw.Write((char)0x1B);
        //        //bw.Write('*');         // bit-image mode
        //        byteList.Add((byte)33);
        //        //bw.Write((byte)33);    // 24-dot double-density
        //        byteList.Add(width[0]);
        //        byteList.Add(width[1]);
        //        //bw.Write(width[0]);  // width low byte
        //        //bw.Write(width[1]);  // width high byte

        //        for (int x = 0; x < data.Width; ++x)
        //        {
        //            for (int k = 0; k < 3; ++k)
        //            {
        //                byte slice = 0;
        //                for (int b = 0; b < 8; ++b)
        //                {
        //                    int y = (((offset / 8) + k) * 8) + b;
        //                    // Calculate the location of the pixel we want in the bit array.
        //                    // It'll be at (y * width) + x.
        //                    int i = (y * data.Width) + x;

        //                    // If the image is shorter than 24 dots, pad with zero.
        //                    bool v = false;
        //                    if (i < dots.Length)
        //                    {
        //                        v = dots[i];
        //                    }
        //                    slice |= (byte)((v ? 1 : 0) << (7 - b));
        //                }
        //                byteList.Add(slice);
        //                //bw.Write(slice);
        //            }
        //        }
        //        offset += 24;
        //        byteList.Add(Convert.ToByte(0x0A));
        //        //bw.Write((char));
        //    }
        //    // Restore the line spacing to the default of 30 dots.
        //    byteList.Add(Convert.ToByte(0x1B));
        //    byteList.Add(Convert.ToByte('3'));
        //    //bw.Write('3');
        //    byteList.Add((byte)30);
        //    return byteList.ToArray();
        //    //bw.Flush();
        //    //byte[] bytes = stream.ToArray();
        //    //return logo + Encoding.Default.GetString(bytes);
        //}

        //public BitmapData GetBitmapData(string bmpFileName)
        //{
        //    using (var bitmap = (Bitmap)Bitmap.FromFile(bmpFileName))
        //    {
        //        var threshold = 127;
        //        var index = 0;
        //        double multiplier = 570; // this depends on your printer model. for Beiyang you should use 1000
        //        double scale = (double)(multiplier / (double)bitmap.Width);
        //        int xheight = (int)(bitmap.Height * scale);
        //        int xwidth = (int)(bitmap.Width * scale);
        //        var dimensions = xwidth * xheight;
        //        var dots = new BitArray(dimensions);

        //        for (var y = 0; y < xheight; y++)
        //        {
        //            for (var x = 0; x < xwidth; x++)
        //            {
        //                var _x = (int)(x / scale);
        //                var _y = (int)(y / scale);
        //                var color = bitmap.GetPixel(_x, _y);
        //                var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
        //                dots[index] = (luminance < threshold);
        //                index++;
        //            }
        //        }

        //        return new BitmapData()
        //        {
        //            Dots = dots,
        //            Height = (int)(bitmap.Height * scale),
        //            Width = (int)(bitmap.Width * scale)
        //        };
        //    }
        //}

        //public class BitmapData
        //{
        //    public BitArray Dots
        //    {
        //        get;
        //        set;
        //    }

        //    public int Height
        //    {
        //        get;
        //        set;
        //    }

        //    public int Width
        //    {
        //        get;
        //        set;
        //    }
        //}
        //==================================================================================
    }
}

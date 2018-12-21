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
    public partial class uc_kembalian : UserControl
    {
        String date, id_transaksi, method_payment, no_ref, change, a_date, a_id, a_total, a_name, a_jam, a_sub, a_subtotal;
        int kembali2, cash2, int_subtotal;
        double tot_aft_diskon, kembalian2;
        DateTime mydate = DateTime.Now;
        DateTime myhour = DateTime.Now;
        koneksi ckon = new koneksi();
        public static Form1 f1;



        private static uc_kembalian _instance;



        public static uc_kembalian Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new uc_kembalian(f1);
                return _instance;
            }
        }

        //==================================================================================================
        public uc_kembalian(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //=============================METHOD FOR CASH PAYMENT=============================================
        public void kembali(String cash, String kembalian, String new_id)
        {
            this.ActiveControl = t_shorcut2;
            t_shorcut2.Focus();

            //l_total.Visible = true;
            //l_total2.Visible = false;
            kembali2 = Int32.Parse(kembalian);
             cash2 = Int32.Parse(cash);
            
            id_transaksi = new_id;

            //l_kembali.Text = string.Format("{0:#,###}"+",00", kembali2);
            //l_total.Text = "Out Of " + string.Format("{0:#,###}"+",00", cash2);

            //l_kembali.Text = kembali2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //l_total.Text = "Out Of " + cash2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));

            //===coba fungsi menampilkan kedalam textbot yang transparan, arag lebih rapih
            String label_kembali;
            String label_total;
            label_total = string.Format("{0:#,###}" + ",00", cash2);
            if (kembali2 == 0)
            { label_kembali = "0,00"; }
            else
            { label_kembali = String.Format("{0:#,###}" + ",00", kembali2); }
            t_kembali_center.Text = "Change  " + label_kembali;//taro tulisan change dan kembalian di textboxt pertama
            t_detail_center.Text = "Out Of  " + label_total;

        }
        //==================================================================================================

        //==================================================================================================
        private void b_new_trans2_Click(object sender, EventArgs e)
        {
            //charge c = new charge(f1);
            //uc_coba uc_cb = new uc_coba(f1);
            date = mydate.ToString("yyyy-MM-dd");

            f1.p_kanan.Controls.Clear();
            if (!f1.p_kanan.Controls.Contains(uc_coba.Instance))
            {
                f1.p_kanan.Controls.Add(uc_coba.Instance);
                uc_coba.Instance.Dock = DockStyle.Fill;
                uc_coba.Instance.new_invoice();
                uc_coba.Instance.holding(date);
                uc_coba.Instance.delete_rows();
                //RunningNumber running = new RunningNumber();
                //running.get_data_before("1", "TR");
                uc_coba.Instance.set_running_number();
                uc_coba.Instance.l_total.Text = "0,00";
                uc_coba.Instance.l_diskon.Text = "0,00";
                uc_coba.Instance.l_voucher.Text = "0,00";
                //uc_coba.Instance.Show();
                uc_coba.Instance.BringToFront();
            }
            else
            {
                f1.p_kanan.Controls.Add(uc_coba.Instance);
                uc_coba.Instance.Dock = DockStyle.Fill;
                uc_coba.Instance.new_invoice();
                uc_coba.Instance.holding(date);
                uc_coba.Instance.delete_rows();
                //RunningNumber running = new RunningNumber();
                //running.get_data_before("1", "TR");
                uc_coba.Instance.set_running_number();
                uc_coba.Instance.l_total.Text = "0,00";
                uc_coba.Instance.l_diskon.Text = "0,00";
                uc_coba.Instance.l_voucher.Text = "0,00";
                //uc_coba.Instance.Show();
                uc_coba.Instance.BringToFront();
            }
        }
        //==========================METHOD FOR EDC PAYMENT =================================================
        public void edc(String new_id, int cash, String nama_bank)
        {
            this.ActiveControl = t_shorcut2;
            t_shorcut2.Focus();

            //l_total.Visible = true;
            //l_total2.Visible = false;
            cash2 = cash;
            id_transaksi = new_id;
            //l_kembali.Text = "0,00";
            //l_total.Text = "Payment Of EDC";
            String var_edc = string.Format("{0:#,###}" + ",00", cash);
            t_kembali_center.Text = "Change 0,00";//taro tulisan change dan kembalian di textboxt pertama
            t_detail_center.Text = "Payment Of EDC, EDC "+nama_bank+" = " + var_edc;
            //t_detail_center.Text = "Payment Of Split. Cash = " + cash + ", EDC " + nama_bank + " = " + edc;
        }
        //==================================================================================================

        //=====================================METHOD FOR SPLIT PAYMENT=====================================
        public void split(double change2, double cash3, String nm_bank, double edc2, String new_id, int cashh)
        {
            this.ActiveControl = t_shorcut2;
            t_shorcut2.Focus();

            //l_total.Visible = true;
            //l_total2.Visible = false;
            id_transaksi = new_id;
            cash2 = cashh;
            //l_kembali.Text = change2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //l_kembali.Text = string.Format("{0:#,###}" + ",-", change2);
            //l_kembali.Text = "0,00";
            //String cash = cash2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            String cash = string.Format("{0:#,###}" + ",00", cash3);
            String nama_bank = nm_bank;
            //String edc = edc2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            String edc = string.Format("{0:#,###}" + ",00", edc2);
            //l_total.Text = "Payment of Split. Cash " + cash + " ,EDC " + nama_bank + " " + edc;

            t_kembali_center.Text = "Change 0,00";
            t_detail_center.Text = "Payment Of Split. Cash = " + cash + ", EDC " + nama_bank + " = " + edc;
            
        }
        //==============================METHOD FOR SPLIT EDC=================================================
        public void split_edc(String new_id, double edc1, double edc2, String nm_bank1, String nm_bank2)
        {
            this.ActiveControl = t_shorcut2;
            t_shorcut2.Focus();

            //l_total.Visible = false;
            //l_total2.Visible = true;
            id_transaksi = new_id;
            String edc_1 = String.Format("{0:#,###}" + ",00", edc1);
            String edc_2 = String.Format("{0:#,###}" + ",00", edc2);
            //l_kembali.Text = "0,00";
            //l_total2.Text = "Payment of Split EDC. Bank Name 1 " + nm_bank1 + " = " + edc_1 + ", Bank Name 2 "+ nm_bank2 +" = "+ edc_2 +"";

            t_kembali_center.Text = "Change 0,00";
            t_detail_center.Text = "Payment of Split EDC. Bank Name 1 " + nm_bank1 + " = " + edc_1 + " , Bank Name 2 " + nm_bank2 + " = " + edc_2 + "";
        }
        //================================METHOD FOR STRUCK================================================
        public void for_struk(String jenis, String noref, Double totall, Double kembalian)
        {
            method_payment = jenis;
            tot_aft_diskon = totall;
            no_ref = noref;
            kembalian2 = kembalian;
            if(kembalian2 != 0)
            {
                change = kembalian2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            }

        }
        //=================================================================================================

        //==============================TOMBOL PRINT=======================================================
        private void b_print_Click(object sender, EventArgs e)
        {
            NewFunctionPrinter print = new NewFunctionPrinter();
            print.get_trans_id(id_transaksi);
            print.get_nm_store();
            print.get_currency();
            print.get_trans_header();
            print.coba_print();
            //=============================================================================================
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
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   TransID : " + id_transaksi + "\n\n"));
            ////BytesValue = PrintExtensions.AddBytes(BytesValue, obj.Separator());
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   SKU                    Total\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Description\n\n"));
            //String sql = "SELECT transaction_line.ARTICLE_ID, transaction_line.QUANTITY, transaction_line.SUBTOTAL, article.ARTICLE_NAME FROM transaction_line INNER JOIN article on transaction_line.ARTICLE_ID = article.ARTICLE_ID WHERE transaction_line.TRANSACTION_ID = '" + id_transaksi + "'";
            //ckon.cmd = new MySqlCommand(sql, ckon.con);
            //ckon.con.Open();
            //ckon.myReader = ckon.cmd.ExecuteReader();
            //while (ckon.myReader.Read())
            //{
            //    a_id = ckon.myReader.GetString("ARTICLE_ID");
            //    a_total = ckon.myReader.GetString("QUANTITY");
            //    int sub_int = ckon.myReader.GetInt32("SUBTOTAL");
            //    a_name = ckon.myReader.GetString("ARTICLE_NAME");
            //    String a_name2 = a_name.Substring(0, 30);
            //    a_sub = sub_int.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //    BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   " + a_id + "   X " + a_total + "  " + a_sub + "\n"));
            //    BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   " + a_name2 + " \n"));
            //}
            //ckon.con.Close();
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("\n"));
            //String sql2 = "SELECT SUM(transaction_line.SUBTOTAL) as total FROM transaction_line WHERE TRANSACTION_ID='" + id_transaksi + "'";
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
            //String from_value = tot_aft_diskon.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //String from_cash = cash2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //String from_change = kembalian2.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Discount         Rp0,00\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   -----------------------------------------\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Total            " + a_subtotal + "\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   =========================================\n\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   Payment Type : "+ method_payment+"\n"));
            //BytesValue = PrintExtensions.AddBytes(BytesValue, Encoding.ASCII.GetBytes("   No. REF      : "+ no_ref +"\n"));

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
        //===========================SHORTCUT TOMBOL=========================================
        private void t_shorcut_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode.ToString() == "P")
            {
                b_print_Click(null, null);
            }
            if (e.Control && e.KeyCode.ToString() == "N")
            {
                b_new_trans2_Click(null, null);
            }
        }
        //===================================================================================
        private void t_shorcut2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode.ToString() == "P")
            {
                b_print_Click(null, null);
            }
            if (e.Control && e.KeyCode.ToString() == "N")
            {
                b_new_trans2_Click(null, null);
            }
        }
        //===========================FOR PRINT STRUCK===========================
        ////======================================================================
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
    }
}

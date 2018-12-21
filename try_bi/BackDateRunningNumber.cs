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
    class BackDateRunningNumber
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();

        String type_trans, awal_number, store_code, BackDate, bulan_BackDate, tahun_BackDate, bulan_trans_backdate, number_trans_string, final_running_number;
        int number_trans_backdate;
        //=======METHOD UNTUK MENDAPATKAN DATA DARI FORM SEBELUMNYA==============
        public void get_data_before(String tipe, String awal)
        {
            type_trans = tipe;//MENDAPATKAN TIPE TRANSAKSI
            awal_number = awal;//MENDAPATKAN AWALAN RUNNING NUMBER (TR, RO, MT, RT, DO)

            get_id_store();
            get_year_month();
            get_running_number();
            give_id();
        }
        //=================BERGUNA UNTUK MENGAMBIL CODE STORE===============
        public void get_id_store()
        {
            String sql = "SELECT * FROM store";
            ckon.con.Open();
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                store_code = ckon.myReader.GetString("CODE");
            }
            ckon.con.Close();
        }
        //====METHOD GET MOUNT AND YEAR PRESENT=================
        public void get_year_month()
        {
            BackDate = Properties.Settings.Default.ValueBackDate;

            bulan_BackDate = BackDate.Substring(5,2);
            tahun_BackDate = BackDate.Substring(2,2);
        }
        //=========METHOD GET DATA FROM AUTO_NUMBER TABLE FOR SALES TRANSACTION
        public void get_running_number()
        {
            String sql = "SELECT * FROM auto_number_backdate WHERE Store_Code = '" + store_code + "' AND Type_Trans = '" + type_trans + "'";
            ckon.con.Open();
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    //tahun_trans = ckon.myReader.GetString("Year");
                    bulan_trans_backdate = ckon.myReader.GetString("Month");
                    number_trans_backdate = ckon.myReader.GetInt32("Number");
                }
                if (bulan_BackDate == bulan_trans_backdate)
                {
                    number_trans_backdate = number_trans_backdate + 1;
                    if (number_trans_backdate < 10)
                    { number_trans_string = "0000" + number_trans_backdate.ToString(); }
                    else if (number_trans_backdate < 100)
                    { number_trans_string = "000" + number_trans_backdate.ToString(); }
                    else if (number_trans_backdate < 1000)
                    { number_trans_string = "00" + number_trans_backdate.ToString(); }
                    else if (number_trans_backdate < 10000)
                    { number_trans_string = "0" + number_trans_backdate.ToString(); }
                    else
                    { number_trans_string = number_trans_backdate.ToString(); }
                    //==MEMBUAT STRING FINAL RUNNING NUMBER
                    final_running_number = awal_number + "/" + store_code + "-" + tahun_BackDate + "" + bulan_BackDate + "-" + number_trans_string;
                    //============UPDATE KE TABEL AUTO_NUMBER================
                    //String query = "UPDATE auto_number SET Month = '" + bulan_trans + "', Number = '" + number_trans + "' WHERE Type_Trans='" + type_trans + "'";
                    //CRUD ubah = new CRUD();
                    //ubah.ExecuteNonQuery(query);
                    //MessageBox.Show(final_running_number);
                }
                else
                {
                    number_trans_backdate = 1;
                    bulan_trans_backdate = bulan_BackDate;//MENJADIKAN BULAN TRANSAKSI = BULAN SEKARANG
                    //==MEMBUAT STRING FINAL RUNNING NUMBER
                    final_running_number = awal_number + "/" + store_code + "-" + tahun_BackDate + "" + bulan_trans_backdate + "-00001";
                    //============UPDATE KE TABEL AUTO_NUMBER================
                    //String query = "UPDATE auto_number SET Month = '" + bulan_trans + "', Number = '1' WHERE Type_Trans='" + type_trans + "'";
                    //CRUD ubah = new CRUD();
                    //ubah.ExecuteNonQuery(query);
                    //MessageBox.Show(final_running_number);
                }


            }
            else
            {
                number_trans_backdate = 1;
                bulan_trans_backdate = bulan_BackDate;//BULAN TRANSAKSI = BULAN SEKARANG
                final_running_number = awal_number + "/" + store_code + "-" + tahun_BackDate + "" + bulan_trans_backdate + "-00001";

                String query = "INSERT INTO auto_number_backdate (Store_Code,Month,Number,Type_Trans) VALUES ('" + store_code + "','" + bulan_trans_backdate + "','0','" + type_trans + "')";
                CRUD ubah = new CRUD();
                ubah.ExecuteNonQuery(query);

                //MessageBox.Show(final_running_number);
            }
            ckon.con.Close();
        }
        //=======METHOD DIGUNAKAN UNTUK MEMBERI RUNNING NUMBER KE HALAMAN TRANSAKSI SESUAI TYPE TRANSAKSI==
        public void give_id()
        {
            if (type_trans == "1")
            {
                //uc_coba.Instance.get_running_number(final_running_number, bulan_trans_backdate, number_trans_backdate, type_trans);
            }
            if (type_trans == "2")
            {
                //UC_Req_order.Instance.get_running_number(final_running_number, bulan_trans_backdate, number_trans_backdate, type_trans);
            }
            if (type_trans == "3")
            {
                //UC_Mut_order.Instance.get_running_number(final_running_number, bulan_trans_backdate, number_trans_backdate, type_trans);
            }
            if (type_trans == "4")
            {
                //UC_Ret_order.Instance.get_running_number(final_running_number, bulan_trans_backdate, number_trans_backdate, type_trans);
            }
            if (type_trans == "6")
            {
                //UC_Petty_Cash.Instance.get_running_number(final_running_number, bulan_trans_backdate, number_trans_backdate, type_trans);
            }
            if (type_trans == "7")
            {
                UC_Popup_Info.Instance.get_running_number_shift(final_running_number, bulan_trans_backdate, number_trans_backdate, type_trans);
            }
            if (type_trans == "8")
            {
                UC_Popup_Info.Instance.get_running_number_CloseStore(final_running_number, bulan_trans_backdate, number_trans_backdate, type_trans);
            }
        }
    }
}

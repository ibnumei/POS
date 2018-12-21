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
    class RunningNumber
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String store_code, bulan_now, tahun_now,bulan_trans, tahun_trans, type_trans, awal_number, number_trans_string, final_running_number;
        int number_trans;
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
            while(ckon.myReader.Read())
            {
                store_code = ckon.myReader.GetString("CODE");
            }
            ckon.con.Close();
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
            if (Properties.Settings.Default.DevCode != "null" && type_trans == "7")
            {
                sql = "SELECT * FROM auto_number WHERE Store_Code = '" + store_code + "' AND Type_Trans = '7' AND Dev_Code='" + device_code + "'";
            }
            else
            {
                sql = "SELECT * FROM auto_number WHERE Store_Code = '" + store_code + "' AND Type_Trans = '" + type_trans + "'";
            }
            //String sql = "SELECT * FROM auto_number WHERE Store_Code = '" + store_code + "' AND Type_Trans = '"+ type_trans +"'";
            ckon.con.Open();
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.myReader = ckon.cmd.ExecuteReader();
            if(ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    //tahun_trans = ckon.myReader.GetString("Year");
                    bulan_trans = ckon.myReader.GetString("Month");
                    number_trans = ckon.myReader.GetInt32("Number");
                }
                if(bulan_now == bulan_trans)
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
                    if (Properties.Settings.Default.DevCode != "null" && type_trans == "7")
                    {
                        final_running_number = awal_number+"/" + store_code + "-" + tahun_now + "" + bulan_trans + "-" + number_trans_string + "-" + Properties.Settings.Default.DevCode;
                    }
                    else
                    {
                        final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-" + number_trans_string;
                    }
                    //final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-" + number_trans_string;
                    //============UPDATE KE TABEL AUTO_NUMBER================
                }
                else
                {
                    number_trans = 1;
                    bulan_trans = bulan_now;//MENJADIKAN BULAN TRANSAKSI = BULAN SEKARANG
                    //==MEMBUAT STRING FINAL RUNNING NUMBER
                    //final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                    if (Properties.Settings.Default.DevCode != "null" && type_trans == "7")
                    {
                        final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001-"+ device_code;
                    }
                    else
                    {
                        final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                    }
                }
                
                
            }
            else
            {
                String query = "";
                number_trans = 1;
                bulan_trans = bulan_now;//BULAN TRANSAKSI = BULAN SEKARANG
                if (Properties.Settings.Default.DevCode != "null" && type_trans == "7")
                {
                    final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001-" + device_code;
                    query = "INSERT INTO auto_number (Store_Code,Month,Number,Type_Trans,Dev_Code) VALUES ('" + store_code + "','" + bulan_trans + "','0','"+type_trans+"','" + Properties.Settings.Default.DevCode + "')";
                }
                else
                {
                    final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";
                    query = "INSERT INTO auto_number (Store_Code,Month,Number,Type_Trans) VALUES ('" + store_code + "','" + bulan_trans + "','0','" + type_trans + "')";
                }
                //final_running_number = awal_number + "/" + store_code + "-" + tahun_now + "" + bulan_trans + "-00001";

                //String query = "INSERT INTO auto_number (Store_Code,Month,Number,Type_Trans) VALUES ('" + store_code + "','" + bulan_trans + "','0','" + type_trans + "')";
                CRUD ubah = new CRUD();
                ubah.ExecuteNonQuery(query);

                //MessageBox.Show(final_running_number);
            }
            ckon.con.Close();
        }
        //=======METHOD DIGUNAKAN UNTUK MEMBERI RUNNING NUMBER KE HALAMAN TRANSAKSI SESUAI TYPE TRANSAKSI==
        public void give_id()
        {
            if(type_trans=="1")
            {
                //baru dia apus buat coba ngakalin stuck di number sequence
                //uc_coba.Instance.get_running_number(final_running_number, bulan_now, number_trans,type_trans);
                //uc_coba a = new uc_coba(f1);
                //a.get_running_number(final_running_number, bulan_now, number_trans, type_trans);

            }
            else if (type_trans == "2")
            {
                //UC_Req_order.Instance.get_running_number(final_running_number, bulan_now, number_trans, type_trans);
                
            }
            else if (type_trans == "3")
            {
                //UC_Mut_order.Instance.get_running_number(final_running_number, bulan_now, number_trans, type_trans);
                //UC_Mut_order a = new UC_Mut_order(f1);
                //a.get_running_number(final_running_number, bulan_now, number_trans, type_trans);
            }
            else if (type_trans == "4")
            {
               // UC_Ret_order.Instance.get_running_number(final_running_number, bulan_now, number_trans, type_trans);
                
            }
            else if (type_trans == "6")
            {
                //UC_Petty_Cash.Instance.get_running_number(final_running_number, bulan_now, number_trans, type_trans);
                //UC_Petty_Cash a = new UC_Petty_Cash(f1);
                //a.get_running_number(final_running_number, bulan_now, number_trans, type_trans);
            }
            else if (type_trans == "7")
            {
                UC_Popup_Info.Instance.get_running_number_shift(final_running_number, bulan_now, number_trans, type_trans);
                //UC_Popup_Info a = new UC_Popup_Info(f1);
                //a.get_running_number_shift(final_running_number, bulan_now, number_trans, type_trans);
            }
            else if (type_trans == "8")
            {
                UC_Popup_Info.Instance.get_running_number_CloseStore(final_running_number, bulan_now, number_trans, type_trans);
                //UC_Popup_Info a = new UC_Popup_Info(f1);
                //a.get_running_number_CloseStore(final_running_number, bulan_now, number_trans, type_trans);
            }
            else
            {

            }
        }
    }
}

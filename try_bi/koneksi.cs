using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace try_bi
{
    //password untuk biensi "qwerty"
    //ganti disemua koneksi
    class koneksi
    {
        //public static string conString = "Server=localhost;Database=biensi_pos_db;Uid=root;Pwd=;";
        public static string conString = "Server='" + try_bi.Properties.Settings.Default.mServer + "';Database='" + try_bi.Properties.Settings.Default.mDBName + "';Uid='" + try_bi.Properties.Settings.Default.mUserDB + "';Pwd='" + try_bi.Properties.Settings.Default.mPassDB + "';";
        public MySqlConnection con = new MySqlConnection(conString);
        public MySqlCommand cmd;
        public MySqlDataAdapter adapter;
        public DataTable dt = new DataTable();
        public DataSet ds = new DataSet();
        public MySqlDataReader myReader;


    }

    class koneksi2
    {
        //public static string conString2 = "Server=localhost;Database=biensi_pos_db;Uid=root;Pwd=;";
        public static string conString2 = "Server='" + try_bi.Properties.Settings.Default.mServer + "';Database='" + try_bi.Properties.Settings.Default.mDBName + "';Uid='" + try_bi.Properties.Settings.Default.mUserDB + "';Pwd='" + try_bi.Properties.Settings.Default.mPassDB + "';";
        public MySqlConnection con2 = new MySqlConnection(conString2);
        public MySqlCommand cmd2;
        public MySqlDataAdapter adapter2;
        public DataTable dt2 = new DataTable();
        public DataSet ds2 = new DataSet();
        public MySqlDataReader myReader2;
    }

    class koneksi3
    {
        //public static string conString3 = "Server=localhost;Database=biensi_pos_db;Uid=root;Pwd=;";
        public static string conString3 = "Server='" + try_bi.Properties.Settings.Default.mServer + "';Database='" + try_bi.Properties.Settings.Default.mDBName + "';Uid='" + try_bi.Properties.Settings.Default.mUserDB + "';Pwd='" + try_bi.Properties.Settings.Default.mPassDB + "';";
        public MySqlConnection con3 = new MySqlConnection(conString3);
        public MySqlCommand cmd3;
        public MySqlDataAdapter adapter3;
        public DataTable dt3 = new DataTable();
        public DataSet ds3 = new DataSet();
        public MySqlDataReader myReader3;
    }


    class koneksi4
    {
        //public static string conString4 = "Server=localhost;Database=biensi_pos_db;Uid=root;Pwd=;";
        public static string conString4 = "Server='" + try_bi.Properties.Settings.Default.mServer + "';Database='" + try_bi.Properties.Settings.Default.mDBName + "';Uid='" + try_bi.Properties.Settings.Default.mUserDB + "';Pwd='" + try_bi.Properties.Settings.Default.mPassDB + "';";
        public MySqlConnection con4 = new MySqlConnection(conString4);
        public MySqlCommand cmd4;
        public MySqlDataAdapter adapter4;
        public DataTable dt4 = new DataTable();
        public DataSet ds4 = new DataSet();
        public MySqlDataReader myReader4;
    }

}

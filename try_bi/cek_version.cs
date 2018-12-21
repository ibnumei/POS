using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace try_bi
{
    class cek_version
    {
        koneksi ckon = new koneksi();
        String message;
        String ver_apk = Properties.Settings.Default.mVersion;
        String ver_db;
        public string cek_ver()
        {
            ckon.con.Close();
            
            String sql = "SELECT * FROM version_apk WHERE Type='POS'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while (ckon.myReader.Read())
                {
                    ver_db = ckon.myReader.GetString("Version");
                }
                ckon.con.Close();

                if (ver_apk == ver_db)
                {
                    message = "Same";
                    //message = "The Application Version Is up to date";
                }
                else
                {
                    message = "NotSame";
                    //message = "Application Version Needs To Be Updated";
                    
                }
            }
            catch (Exception ex)
            {
                message = "GetFirst";
                //message = ex.ToString();
                //message = "f";
            }
            return message;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;


namespace try_bi
{
    class CRUD
    {
        koneksi ckon = new koneksi();

        //==============METHOD INPUT, DELETE, Dan Edit============================================
        public void ExecuteNonQuery(String query)
        {
            ckon.con.Open();
            ckon.cmd = new MySqlCommand(query, ckon.con);
            ckon.cmd.ExecuteNonQuery();
            ckon.con.Close();
        }
        //=========================================================================================
    }
}

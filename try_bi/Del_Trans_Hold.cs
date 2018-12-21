using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace try_bi
{
    class Del_Trans_Hold
    {
        String  id, ID_TRANS, id_substring;
        int id_substring2;
        koneksi ckon = new koneksi();
        koneksi2 ckon2 = new koneksi2();
        public void get_data(String id2)
        {
            id = id2;
        }

        //DELETE TRANSAKSI sesuai id shift yang dikirim
        public void del_trans()
        {
            String sql = "SELECT * FROM transaction WHERE ID_SHIFT='" + id + "' AND STATUS='0'";
            ckon.con.Open();
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.myReader = ckon.cmd.ExecuteReader();
            if(ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    ID_TRANS = ckon.myReader.GetString("TRANSACTION_ID");
                    String del_line = "DELETE FROM transaction_line WHERE TRANSACTION_ID = '" + ID_TRANS + "'";
                    CRUD new_del = new CRUD();
                    new_del.ExecuteNonQuery(del_line);

                    String del_header = "DELETE FROM transaction WHERE TRANSACTION_ID='" + ID_TRANS + "'";
                    CRUD new_del2 = new CRUD();
                    new_del2.ExecuteNonQuery(del_header);

                    String del_inv_line = "DELETE FROM inventory_line WHERE TRANS_REF_ID='" + ID_TRANS + "'";
                    CRUD new_del3 = new CRUD();
                    new_del3.ExecuteNonQuery(del_inv_line);
                }
                ckon.con.Close();
            }
            else
            {

            }
        }

        //MENDAPATKAN id transaksi terakhir berdasarkan id_shift dan status 1, ubah running number ke int, lalu update ke table auto_number
        public void update_table()
        {
            ckon.con.Close();
            string sql = "SELECT SUBSTRING(TRANSACTION_ID, 13) AS inv FROM transaction ORDER BY TRANSACTION_ID DESC LIMIT 1";
            ckon.con.Open();
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.myReader = ckon.cmd.ExecuteReader();
            while(ckon.myReader.Read())
            {
                id_substring = ckon.myReader.GetString("inv");
                id_substring2 = Convert.ToInt32(id_substring);
            }
            ckon.con.Close();
            String sql2 = "UPDATE auto_number SET Number='" + id_substring2 + "' WHERE Type_Trans='1'";
            CRUD update = new CRUD();
            update.ExecuteNonQuery(sql2);
        }

        //String a = "TR/AAB-1803-10104";
        //String b = a.Substring(12);
        //int c = Convert.ToInt32(b);
        //MessageBox.Show(b+",,,,"+c);
    }
}

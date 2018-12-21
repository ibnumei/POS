 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace try_bi
{
    class Inv_Line
    {
        koneksi ckon = new koneksi();
        koneksi2 ckon2 = new koneksi2();
        koneksi3 ckon3 = new koneksi3();
        koneksi4 ckon4 = new koneksi4();
        //=============VARIABLE UNTUK TRANSAKSI ID DAN ARTICLE ID==============
        String transaksi_id, article_id;
        //=======variable for get qty from inv_line
        int qty_line, qty_total_art_line, qty_inv, qty_total_inv_fix;
        //===============VARIABLE FOR TYPE TRANSAKSION========
        String type_id, type_name, id_from_article;
        int qty_min_plus2;
        //==============VARIABLE FOR DO===================
        String do_id, art_id_do, status_inv;
        int qty_receive;
        //=================VARIABLE FOR VOID TRANSACTION==============
        String void_id, art_id, inv_id;
        int qty_trans_line;
        //===VARIABLE FOR MUTASI ORDER OUT=======
        String mutasi_id;
        //====VARIABLE FOR RETURN ORDER OUT===
        String return_id;
        //=====VARIABLE UNTUK TRANSAKSI LINE YANG DI DELETE DARI HALAMAN TRANSAKSI
        String del_id_trans;
        //=====================select do line======================
        public void get_do_line(String id_do)
        {

            do_id = id_do;
            String sql = "SELECT * FROM deliveryorder_line WHERE DELIVERY_ORDER_ID = '" + do_id + "'";
            ckon3.cmd3 = new MySqlCommand(sql, ckon3.con3);
            ckon3.con3.Open();
            ckon3.myReader3 = ckon3.cmd3.ExecuteReader();
            while (ckon3.myReader3.Read())
            {
                art_id_do = ckon3.myReader3.GetString("ARTICLE_ID");
                qty_receive = ckon3.myReader3.GetInt32("QTY_RECEIVE");
                cek_qty_inv(art_id_do);
                cek_inv_line(do_id, qty_receive);
            }

            ckon3.con3.Close();
        }
        //====================FROM VOID TRANSACTION==========================
        public void void_trans(String id)
        {
            void_id = id;
            String sql = "SELECT * FROM transaction_line WHERE TRANSACTION_ID = '" + void_id + "'";
            ckon3.cmd3 = new MySqlCommand(sql, ckon3.con3);
            ckon3.con3.Open();
            ckon3.myReader3 = ckon3.cmd3.ExecuteReader();
            while(ckon3.myReader3.Read())
            {
                art_id = ckon3.myReader3.GetString("ARTICLE_ID");
                qty_trans_line = ckon3.myReader3.GetInt32("QUANTITY");
                String sql2 = "SELECT * FROM article WHERE ARTICLE_ID = '" + art_id + "'";
                ckon4.cmd4 = new MySqlCommand(sql2, ckon4.con4);
                ckon4.con4.Open();
                ckon4.myReader4 = ckon4.cmd4.ExecuteReader();
                while(ckon4.myReader4.Read())
                {
                    inv_id = ckon4.myReader4.GetString("_id");
                    cek_qty_inv(inv_id);//mengambil good qty dari inventory sesuai id
                    cek_inv_line(void_id, qty_trans_line);
                }

                ckon4.con4.Close();
            }
            ckon3.con3.Close();
        }
        //=====FROM DELETE TRANSAKSI LINE DARI MENU TRANSAKSI======
        public void del_transaksi_line(String id)
        {
            del_id_trans = id;
        }
        //=====MUTASI ORDER OUT======
        public void mutasi_out(String id)
        {
            mutasi_id = id;
            String sql = "SELECT * FROM mutasiorder_line WHERE MUTASI_ORDER_ID = '" + mutasi_id + "'";
            ckon3.cmd3 = new MySqlCommand(sql, ckon3.con3);
            ckon3.con3.Open();
            ckon3.myReader3 = ckon3.cmd3.ExecuteReader();
            while (ckon3.myReader3.Read())
            {
                art_id = ckon3.myReader3.GetString("ARTICLE_ID");
                qty_trans_line = ckon3.myReader3.GetInt32("QUANTITY");
                String sql2 = "SELECT * FROM article WHERE ARTICLE_ID = '" + art_id + "'";
                ckon4.cmd4 = new MySqlCommand(sql2, ckon4.con4);
                ckon4.con4.Open();
                ckon4.myReader4 = ckon4.cmd4.ExecuteReader();
                while (ckon4.myReader4.Read())
                {
                    inv_id = ckon4.myReader4.GetString("_id");
                    cek_qty_inv(inv_id);
                    cek_inv_line(mutasi_id, qty_trans_line);
                }

                ckon4.con4.Close();
            }
            ckon3.con3.Close();
        }
        //=======RETURN ORDER OUT========
        public void return_order(String id)
        {
            return_id = id;
            String sql = "SELECT * FROM returnorder_line WHERE RETURN_ORDER_ID = '" + return_id + "'";
            ckon3.cmd3 = new MySqlCommand(sql, ckon3.con3);
            ckon3.con3.Open();
            ckon3.myReader3 = ckon3.cmd3.ExecuteReader();
            while (ckon3.myReader3.Read())
            {
                art_id = ckon3.myReader3.GetString("ARTICLE_ID");
                qty_trans_line = ckon3.myReader3.GetInt32("QUANTITY");
                String sql2 = "SELECT * FROM article WHERE ARTICLE_ID = '" + art_id + "'";
                ckon4.cmd4 = new MySqlCommand(sql2, ckon4.con4);
                ckon4.con4.Open();
                ckon4.myReader4 = ckon4.cmd4.ExecuteReader();
                while (ckon4.myReader4.Read())
                {
                    inv_id = ckon4.myReader4.GetString("_id");
                    cek_qty_inv(inv_id);
                    cek_inv_line(return_id, qty_trans_line);
                }

                ckon4.con4.Close();
            }
            ckon3.con3.Close();
        }
        //======================================================================
        public void get_id(String id)
        {
            ckon.con.Close();
            string sql = "SELECT * FROM article WHERE ARTICLE_ID = '" + id +"'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while(ckon.myReader.Read())
            {
                id_from_article = ckon.myReader.GetString("_id");
            }
            cek_qty_inv(id_from_article);
            ckon.con.Close();
        }
        
        //======================AMBIL TOTAL ARTICLE ID DARI INVENTORY, LALU DIKURANGI DAN DIUPDATE====================
        public void cek_qty_inv(String art_id)
        {
            //ckon.con.Close();
            ckon2.con2.Close();
            article_id = art_id;
            String sql = "SELECT * FROM inventory WHERE ARTICLE_ID = '" + article_id + "'";
            ckon2.cmd2 = new MySqlCommand(sql, ckon2.con2);
            ckon2.con2.Open();
            ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
            try
            {
                while (ckon2.myReader2.Read())
                {
                    status_inv = "YES";
                    qty_inv = ckon2.myReader2.GetInt32("GOOD_QTY");
                }
                ckon2.con2.Close();
            }
            catch
            {
                status_inv = "NO";
            }

        }
        //===================CEK TYPE TRANSACTION=====================================
        public void cek_type_trans(String type)
        {
            type_id = type;
            if(type_id=="1")
            { type_name = "SalesTrans"; }
            if(type_id=="2")
            { type_name = "DO"; }
            if(type_id=="3")
            { type_name = "MutOrder"; }
            if (type_id == "4")
            { type_name = "VoidTrans"; }
            if (type_id == "5")
            { type_name = "Return"; }
            if (type_id == "6")
            { type_name = "DelTransLine"; }
        }

        //======Cek Di inventory Line, kalo kosong input, kalo ada update==============
        public void cek_inv_line(String trans_id, int  qty)
        {
            ckon.con.Close();
            transaksi_id = trans_id;
            qty_min_plus2 = qty;
            String sql = "SELECT * FROM inventory_line WHERE TRANS_REF_ID = '" + transaksi_id + "' AND ARTICLE_ID = '" + article_id + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            int count = 0;
            while (ckon.myReader.Read())
            {
                count = count + 1;
                qty_line = ckon.myReader.GetInt32("QTY");
            }
            ckon.con.Close();
            if(count==0)
            {
                if(type_id=="1")
                {
                    int I = 1;
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '-" + I + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    qty_total_inv_fix = qty_inv - 1;
                    String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql3);
                }
                else if(type_id=="2")
                {
                    
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '" + qty_min_plus2 + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    //==========JIKA INVENTORY TELAH ADA
                    if(status_inv=="YES")
                    {
                        qty_total_inv_fix = qty_inv + qty_min_plus2;
                        String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                        CRUD UPDATE = new CRUD();
                        UPDATE.ExecuteNonQuery(sql3);
                    }
                    //=====JIKA INVENTORY BELOM ADA====
                    else
                    {
                        String input_inv = "INSERT INTO inventory (_id,ARTICLE_ID,GOOD_QTY) VALUES ('"+ article_id +"','"+ article_id +"','"+ qty_min_plus2 +"')";
                        CRUD input_baru = new CRUD();
                        input_baru.ExecuteNonQuery(input_inv);
                    }

                }
                else if(type_id=="4")
                {
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '" + qty_min_plus2 + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    qty_total_inv_fix = qty_inv + qty_min_plus2;
                    String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql3);
                }
                else if(type_id=="3")
                {
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '" + qty_min_plus2 + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    qty_total_inv_fix = qty_inv - qty_min_plus2;
                    String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql3);
                }
                else
                {
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '" + qty_min_plus2 + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    qty_total_inv_fix = qty_inv - qty_min_plus2;
                    String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql3);
                }

            }
            else
            {
                if(type_id=="1")
                {
                    qty_line = qty_line + (qty_min_plus2);
                    String sql3 = "UPDATE inventory_line SET QTY='" + qty_line + "' WHERE TRANS_REF_ID='"+ transaksi_id +"' AND ARTICLE_ID = '"+ article_id +"'";
                    CRUD update = new CRUD();
                    update.ExecuteNonQuery(sql3);

                    qty_total_inv_fix = qty_inv + (qty_min_plus2);
                    String sql4 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql4);
                }
                else if(type_id=="2")
                {
                    //qty_line = qty_line + 1;
                    String sql3 = "UPDATE inventory_line SET QTY='" + qty_line + "' WHERE TRANS_REF_ID='" + transaksi_id + "' AND ARTICLE_ID = '" + article_id + "'";
                    CRUD update = new CRUD();
                    update.ExecuteNonQuery(sql3);
                    //==========JIKA INVENTORY TELAH ADA
                    if (status_inv == "YES")
                    {
                        qty_total_inv_fix = qty_inv + qty_min_plus2;
                        String sql3A = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                        CRUD UPDATE = new CRUD();
                        UPDATE.ExecuteNonQuery(sql3A);
                    }
                    //=====JIKA INVENTORY BELOM ADA====
                    else
                    {
                        String input_inv = "INSERT INTO inventory (_id,ARTICLE_ID,GOOD_QTY) VALUES ('" + article_id + "','" + article_id + "','" + qty_min_plus2 + "')";
                        CRUD input_baru = new CRUD();
                        input_baru.ExecuteNonQuery(input_inv);
                    }
                }
                else if(type_id=="4")
                {
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '" + qty_min_plus2 + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    qty_total_inv_fix = qty_inv + qty_min_plus2;
                    String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql3);
                }
                else if(type_id=="3")
                {
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '" + qty_min_plus2 + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    qty_total_inv_fix = qty_inv - qty_min_plus2;
                    String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql3);
                }
                else
                {
                    string sql2 = "INSERT INTO inventory_line (TRANS_TYPE, TRANS_TYPE_NAME, TRANS_REF_ID, QTY,ARTICLE_ID) VALUES ('" + type_id + "','" + type_name + "', '" + transaksi_id + "', '" + qty_min_plus2 + "', '" + article_id + "')";
                    CRUD input = new CRUD();
                    input.ExecuteNonQuery(sql2);

                    qty_total_inv_fix = qty_inv - qty_min_plus2;
                    String sql3 = "UPDATE inventory SET GOOD_QTY='" + qty_total_inv_fix + "' WHERE ARTICLE_ID='" + article_id + "'";
                    CRUD UPDATE = new CRUD();
                    UPDATE.ExecuteNonQuery(sql3);
                }
            }
        }

        //======FUNGSI DELETE tabel inventory line sesuai dengan id_trans yg berasal dari tombol delete di menu transaksi
        public void del_inv_line(String id)
        {
            del_id_trans = id;
            String sql = "DELETE FROM inventory_line WHERE TRANS_REF_ID = '" + del_id_trans + "'";
            CRUD del = new CRUD();
            del.ExecuteNonQuery(sql);

            String sql2 = "DELETE FROM transaction_line WHERE TRANSACTION_ID ='" + del_id_trans + "'";
            CRUD del2 = new CRUD();
            del2.ExecuteNonQuery(sql2);
        }
    }
}

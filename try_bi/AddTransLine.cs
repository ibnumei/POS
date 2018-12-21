using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;
namespace try_bi
{
    class AddTransLine
    {
        koneksi ckon = new koneksi();
        koneksi2 ckon2 = new koneksi2();

        String code_store, customer, S_ID, disc_code, disc_type, disc_desc, t_id, id_spg;
        int subtotal, qty=1, disc, new_price, new_jumlah, new_harga, new_disc ;
        public void get_data(String code, String cust, String art_id, String spg_id)
        {
            code_store = code;
            customer = cust;
            S_ID = art_id;
            id_spg = spg_id;
        }
        //==method get data for trans_line
        public void get_data_trans_line(int price, String trans_id)
        {
            new_price = price;
            t_id = trans_id;
        }
        public async Task Post_Discount()
        {
            Transaction transaction = new Transaction();
            transaction.storeCode = code_store;
            transaction.customerId = customer;
            List<TransactionLine> transLine = new List<TransactionLine>();
            //==================================================================================
            string query = "SELECT * FROM ARTICLE WHERE ARTICLE_ID='" + S_ID + "'";
            ckon2.cmd2 = new MySqlCommand(query, ckon2.con2);
            ckon2.con2.Open();
            ckon2.myReader2 = ckon2.cmd2.ExecuteReader();
            Article articleFromDb = new Article();

            while (ckon2.myReader2.Read())
            {
                articleFromDb.articleId = ckon2.myReader2.GetString("ARTICLE_ID");
                articleFromDb.articleName = ckon2.myReader2.GetString("ARTICLE_NAME");
                articleFromDb.brand = ckon2.myReader2.GetString("BRAND");
                articleFromDb.color = ckon2.myReader2.GetString("COLOR");
                articleFromDb.department = ckon2.myReader2.GetString("DEPARTMENT");
                articleFromDb.departmentType = ckon2.myReader2.GetString("DEPARTMENT_TYPE");
                articleFromDb.gender = ckon2.myReader2.GetString("GENDER");
                articleFromDb.id = ckon2.myReader2.GetInt32("_id");
                articleFromDb.price = ckon2.myReader2.GetInt32("PRICE");
                articleFromDb.size = ckon2.myReader2.GetString("SIZE");
                articleFromDb.unit = ckon2.myReader2.GetString("UNIT");
                subtotal = ckon2.myReader2.GetInt32("PRICE");
                articleFromDb.articleIdAlias = ckon2.myReader2.GetString("ARTICLE_ID_ALIAS");
            }
            ckon2.con2.Close();
            //====================================================================
            TransactionLine t = new TransactionLine();
            t.subtotal = subtotal;
            t.quantity = qty;
            t.discount = 0;
            t.price = subtotal;

            t.article = articleFromDb;
            transLine.Add(t);

            transaction.transactionLines = transLine;
            BiensiPosDbContext.BiensiPosDbDataContext contex = new BiensiPosDbContext.BiensiPosDbDataContext();
            DiscountCalculate dc = new DiscountCalculate(contex);
            try
            {
                DiscountMaster resultData = dc.Post(transaction);
                //=================================================
                try
                {

                    foreach (var c in resultData.discountItems)
                    {
                        disc = (Int32)c.amountDiscount;
                        disc_code = c.discountCode;
                        disc_type = c.discountType;
                        disc_desc = c.discountDesc;
                        //MessageBox.Show("" + disc.ToString());
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                //MessageBox.Show(ex.ToString()+", Result Data Null");
            }
            

        }
        //=======================END OF DISCOUNT=============================================================
        //================DATAGRIDVIEW TO DATABASE =========================================
        public void cek_article2()
        {
            //new_price = int.Parse(S_price);
            string j = "1";
            //i = 0;
            string sql = "SELECT * FROM transaction_line WHERE TRANSACTION_ID ='" + t_id + "' AND ARTICLE_ID='" + S_ID + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            int count = 0;
            while (ckon.myReader.Read())
            {
                count = count + 1;
                new_jumlah = ckon.myReader.GetInt32("QUANTITY");
                new_harga = ckon.myReader.GetInt32("SUBTOTAL");
                new_disc = ckon.myReader.GetInt32("DISCOUNT");
            }
            ckon.con.Close();
            if (count == 0)
            {
                int convert_harga;//convert harga menjadi integer
                                  //jika diskon tidak ada, maka subtotal dikurangi diskon

                convert_harga = new_price;
                new_harga = convert_harga - disc;

                string sql2 = "INSERT INTO transaction_line (TRANSACTION_ID,ARTICLE_ID,QUANTITY,PRICE,DISCOUNT,SUBTOTAL, SPG_ID, DISCOUNT_CODE,DISCOUNT_TYPE,DISCOUNT_DESC) VALUES ('" + t_id + "','" + S_ID + "', '" + j + "', '" + new_price + "', '" + disc + "' ,'" + new_harga + "', '" + id_spg + "','" + disc_code + "','" + disc_type + "','" + disc_desc + "')";
                CRUD input = new CRUD();
                input.ExecuteNonQuery(sql2);
            }
            else
            {
                new_price = new_price - disc;
                new_jumlah = new_jumlah + 1;
                new_harga = new_harga + new_price;
                new_disc = new_disc + disc;
                //int subtotal_diskon = new_harga - new_disc;
                String sql3 = "UPDATE transaction_line SET QUANTITY='" + new_jumlah + "',DISCOUNT='" + new_disc + "' ,SUBTOTAL='" + new_harga + "' WHERE TRANSACTION_ID='" + t_id + "' AND ARTICLE_ID='" + S_ID + "'";
                CRUD input = new CRUD();
                input.ExecuteNonQuery(sql3);
            }
        }
    }
}

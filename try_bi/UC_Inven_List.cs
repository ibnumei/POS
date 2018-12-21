using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace try_bi
{
    public partial class UC_Inven_List : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String id, name, status, brand, good, new_status, size, color;
        //======================================================
        private static UC_Inven_List _instance;
        public static UC_Inven_List Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_Inven_List(f1);
                return _instance;
            }
        }

        

        //=======================================================
        public UC_Inven_List(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //MEMBUAT FOKUS DI TEXTBOXT SCAN ARTICLE()
        public void scan_fokus()
        {
            this.ActiveControl = t_search_trans;
            t_search_trans.Focus();

        }
        //=====menampilkan data ke datagridview====
        public void retreive(String query)
        {
            dgv_inventory.Rows.Clear();
            String sql = query;
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while(ckon.myReader.Read())
                {
                    id = ckon.myReader.GetString("ARTICLE_ID");
                    name = ckon.myReader.GetString("ARTICLE_NAME");
                    brand = ckon.myReader.GetString("BRAND");
                    size = ckon.myReader.GetString("SIZE");
                    color = ckon.myReader.GetString("COLOR");
                    status = ckon.myReader.GetString("STATUS");
                    good = ckon.myReader.GetString("GOOD_QTY");
                    if(status == "0")
                    {
                        new_status = "Available";
                    }
                    else
                    {
                        new_status = "Not Available";
                    }
                    int n = dgv_inventory.Rows.Add();

                    dgv_inventory.Rows[n].Cells[0].Value = id;
                    dgv_inventory.Rows[n].Cells[1].Value = name;
                    dgv_inventory.Rows[n].Cells[2].Value = brand;
                    dgv_inventory.Rows[n].Cells[3].Value = size;
                    dgv_inventory.Rows[n].Cells[4].Value = color;
                    dgv_inventory.Rows[n].Cells[5].Value = new_status;
                    dgv_inventory.Rows[n].Cells[6].Value = good;
                }
                ckon.con.Close();
            }
            catch
            { }
        }
        //mencari article id dengan textboxt search
        private void t_search_trans_OnValueChanged(object sender, EventArgs e)
        {
            String count_article = t_search_trans.Text;
            int count_article_int = count_article.Count();
            if (count_article_int < 5)
            {
                if (t_search_trans.Text == "")
                {

                    String sql2a = "SELECT article.ARTICLE_ID, article.ARTICLE_NAME, article.BRAND,article.SIZE, article.COLOR,inventory.STATUS, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID LIMIT 100";
                    retreive(sql2a);

                }
                if (t_search_trans.Text == "" && (combo_ktg2.Text == "Brand" || combo_ktg2.Text == "Department" || combo_ktg2.Text == "Department_Type" || combo_ktg2.Text == "Size" || combo_ktg2.Text == "Color" || combo_ktg2.Text == "Gender"))
                {
                    String sql4 = "SELECT article.ARTICLE_ID, article.ARTICLE_NAME, article.BRAND,article.SIZE, article.COLOR,inventory.STATUS, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE article." + combo_ktg2.Text + " = '" + combo_value.Text + "' LIMIT 100";
                    retreive(sql4);
                    ckon.con.Close();
                }
            }
            if (count_article_int >= 5)
            {
                if (t_search_trans.Text != "")
                {

                    String sql2 = "SELECT article.ARTICLE_ID, article.ARTICLE_NAME, article.BRAND, article.SIZE, article.COLOR,inventory.STATUS, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE  article.ARTICLE_ID LIKE '%" + t_search_trans.Text + "%' OR article.ARTICLE_NAME LIKE '%" + t_search_trans.Text + "%' LIMIT 100";
                    retreive(sql2);

                }
                if (t_search_trans.Text != "" && (combo_ktg2.Text == "Brand" || combo_ktg2.Text == "Department" || combo_ktg2.Text == "Department_Type" || combo_ktg2.Text == "Size" || combo_ktg2.Text == "Color" || combo_ktg2.Text == "Gender"))
                {

                    String sql3 = "SELECT article.ARTICLE_ID, article.ARTICLE_NAME, article.BRAND,article.SIZE, article.COLOR,inventory.STATUS, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE article." + combo_ktg2.Text + " = '" + combo_value.Text + "' AND (article.ARTICLE_ID LIKE '%" + t_search_trans.Text + "%' OR article.ARTICLE_NAME LIKE '%" + t_search_trans.Text + "%' ) LIMIT 100";
                    retreive(sql3);
                }
            }
        }
        //=========================SELECT VALUE COMBOBOX CATEGORY============================
        private void combo_ktg2_SelectedIndexChanged(object sender, EventArgs e)
        {
            scan_fokus();
            if (combo_ktg2.Text == "ALL")
            {
                combo_value.Items.Clear();
                String sql = "SELECT article.ARTICLE_ID, article.ARTICLE_NAME, article.BRAND,article.SIZE, article.COLOR,inventory.STATUS, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID LIMIT 100";
                combo_value.Text = "ALL";
                retreive(sql);
                //MENGHITUNG TOTAL, KIRIM KE METHOD HITUNG
                String sql2 = "SELECT SUM(inventory.GOOD_QTY) as total FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID ";
                itung_total(sql2);
            }
            else if(combo_ktg2.Text =="Brand")
            {
                String query = "SELECT * FROM brand";
                isi_combo_value(query);
                combo_value.Text = "MOUTLEY";
            }
            else if(combo_ktg2.Text == "Department")
            {
                String query = "SELECT * FROM departement";
                isi_combo_value(query);
                combo_value.Text = "Shirt";
            }
            else if(combo_ktg2.Text == "Department_Type")
            {
                String query = "SELECT * FROM departementtype";
                isi_combo_value(query);
                combo_value.Text = "Denim";
            }
            else if(combo_ktg2.Text == "Size")
            {
                String query = "SELECT * FROM Size";
                isi_combo_value(query);
                combo_value.Text = "L";
            }
            else if(combo_ktg2.Text == "Color")
            {
                String query = "SELECT * FROM Color";
                isi_combo_value(query);
                combo_value.Text = "Blue";
            }
            else if (combo_ktg2.Text == "Gender")
            {
                String query = "SELECT * FROM Gender";
                isi_combo_value(query);
                combo_value.Text = "Men";
            }
            else
            { }
        }
        //======================================================================================

        //========================ISI VALUE COMBO ==============================================
        public void isi_combo_value(String query)
        {
            combo_value.Items.Clear();
            String sql = query;
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.myReader = ckon.cmd.ExecuteReader();
                while(ckon.myReader.Read())
                {
                    String name = ckon.myReader.GetString("DESCRIPTION");
                    combo_value.Items.Add(name);
                }
                ckon.con.Close();

            }
            catch
            { }
        }

        private void combo_value_SelectedIndexChanged(object sender, EventArgs e)
        {
            scan_fokus();
            //MessageBox.Show(" "+ combo_value.Text);
            String sql = "SELECT article.ARTICLE_ID, article.ARTICLE_NAME, article.BRAND ,article.SIZE, article.COLOR,inventory.STATUS, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE article." + combo_ktg2.Text +" = '"+ combo_value.Text + "' LIMIT 100";
            retreive(sql);
            //MENGHITUNG TOTAL, KIRIM KE METHOD HITUNG
            String sql2 = "SELECT SUM(inventory.GOOD_QTY) as total FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE article." + combo_ktg2.Text + " = '" + combo_value.Text + "' ";
            itung_total(sql2);
        }

        private void UC_Inven_List_Load(object sender, EventArgs e)
        {

        }
        //======================================================================================
        public void itung_total(String sql2)
        {
            int total;
            //String sql2 = "SELECT SUM(inventory.GOOD_QTY) as total FROM inventory ";
            ckon.cmd = new MySqlCommand(sql2, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                try
                {
                    total = ckon.myReader.GetInt32("total");
                    l_total.Text = total.ToString();
                }
                catch
                { l_total.Text = "0"; }
            }
            ckon.con.Close();
        }
        //========MENCOBA MENAMPILKAN ARTICLE DENGAN SCAN BARCODE===============================================
        private void t_search_trans_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                String sql2 = "SELECT article.ARTICLE_ID, article.ARTICLE_NAME, article.BRAND, article.SIZE, article.COLOR,inventory.STATUS, inventory.GOOD_QTY FROM article INNER JOIN inventory ON article._id = inventory.ARTICLE_ID WHERE  article.ARTICLE_ID LIKE '%" + t_search_trans.Text + "%' OR article.ARTICLE_NAME LIKE '%" + t_search_trans.Text + "%' ";
                retreive(sql2);
            }
            else
            { }
        }

    }
}

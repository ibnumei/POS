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

namespace try_bi
{
    public partial class UC_Promotion : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String id_disc;
        String field_none = "None", field_kosong = "0";
        private static UC_Promotion _instance;
        public static UC_Promotion Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_Promotion(f1);
                return _instance;
            }
        }
        public UC_Promotion(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //===========================================================================================================
        public void holding()
        {
            ckon.con.Close();
            dgv_hold.Rows.Clear();
            String sql = "SELECT * FROM promotion";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                ckon.adapter.Fill(ckon.dt);
                foreach (DataRow row in ckon.dt.Rows)
                {
                    int n = dgv_hold.Rows.Add();
                    dgv_hold.Rows[n].Cells[0].Value = row["DISCOUNT_CODE"];
                    dgv_hold.Rows[n].Cells[1].Value = row["DISCOUNT_NAME"];
                    dgv_hold.Rows[n].Cells[2].Value = row["START_DATE"];
                    dgv_hold.Rows[n].Cells[3].Value = row["END_DATE"];
                }
                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch
            { }
        }
        //===========================================================================================================
        public void get_data_id()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM promotion WHERE DISCOUNT_CODE='" + id_disc + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                dc_name.Text = ckon.myReader.GetString("DISCOUNT_NAME");
                dc_ctg.Text = ckon.myReader.GetString("DISCOUNT_CATEGORY");
                dc_cd.Text = ckon.myReader.GetString("DISCOUNT_CODE");
                dc_desc.Text = ckon.myReader.GetString("DESCRIPTION");
            }
            ckon.con.Close();
        }
        //===========================================================================================================
        public void retreive()
        {
            ckon.con.Close();
            dgv_purchase.Rows.Clear();
            String sql = "SELECT * FROM promotion_line WHERE DISCOUNT_CODE='"+ id_disc +"'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                ckon.adapter.Fill(ckon.dt);
                foreach (DataRow row in ckon.dt.Rows)
                {
                    int n = dgv_purchase.Rows.Add();
                    dgv_purchase.Rows[n].Cells[0].Value = row["ARTICLE_ID"];
                    dgv_purchase.Rows[n].Cells[1].Value = row["ARTICLE_NAME"];
                    dgv_purchase.Rows[n].Cells[2].Value = row["BRAND"];
                    dgv_purchase.Rows[n].Cells[3].Value = row["SIZE"];
                    dgv_purchase.Rows[n].Cells[4].Value = row["COLOR"];
                    dgv_purchase.Rows[n].Cells[5].Value = row["GENDER"];
                    dgv_purchase.Rows[n].Cells[6].Value = row["DEPARTMENT"];
                    dgv_purchase.Rows[n].Cells[7].Value = row["DEPARTMENT_TYPE"];
                    dgv_purchase.Rows[n].Cells[8].Value = row["CUSTOMER_GROUP"];
                    dgv_purchase.Rows[n].Cells[9].Value = row["QTA"];
                    dgv_purchase.Rows[n].Cells[10].Value = row["AMOUNT"];
                    dgv_purchase.Rows[n].Cells[11].Value = row["BANK"];
                    dgv_purchase.Rows[n].Cells[12].Value = row["DISCOUNT_PERCENT"];
                    dgv_purchase.Rows[n].Cells[13].Value = row["DISCOUNT_PRICE"];
                    dgv_purchase.Rows[n].Cells[14].Value = row["SPESIAL_PRICE"];
                }
                dgv_purchase.Columns[10].DefaultCellStyle.Format = "#,###";
                dgv_purchase.Columns[13].DefaultCellStyle.Format = "#,###";
                dgv_purchase.Columns[14].DefaultCellStyle.Format = "#,###";
                //===================================================================================================================
                foreach (DataGridViewColumn clm in dgv_purchase.Columns)
                {

                    dgv_purchase.Columns[clm.Index].Visible = false;
                    //bool notAvailable = true;
                    bool notAvailable = false;

                    foreach (DataGridViewRow row in dgv_purchase.Rows)
                    {

                        if (row.Cells[clm.Index].Value != null)
                        {
                            // If string of value is empty
                            if (row.Cells[clm.Index].Value.ToString() != field_none)
                            {
                                if (row.Cells[clm.Index].Value.ToString() != field_kosong)
                                {
                                    if (row.Cells[clm.Index].Value.ToString() != "")
                                    {
                                        notAvailable = true;
                                        break;
                                    }
                                }
                                //notAvailable = false;

                            }
                        }
                    }

                    if (notAvailable)
                    {
                        //dgv_purchase.Columns[clm.Index].Visible = false;
                        dgv_purchase.Columns[clm.Index].Visible = true;
                        //dgv_purchase.Columns[clm.Index].Name("h") = true;
                    }
                }
                //===================================================================================================================
                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch
            { }
        }
        //===========================================================================================================
        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            id_disc = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            //dgv_purchase.Visible = true;

            get_data_id();
            retreive();
        }
        //===========================================================================================================
    }
}

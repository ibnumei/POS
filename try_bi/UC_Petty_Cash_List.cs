﻿using System;
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
    public partial class UC_Petty_Cash_List : UserControl
    {
        public static Form1 f1;
        koneksi ckon = new koneksi();
        String id_trans, exp_ctg, exp_date, exp_total, id_list;
        int amount, bg_ToStore, tot_exp;
        //======================================================
        private static UC_Petty_Cash_List _instance;
        public static UC_Petty_Cash_List Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new UC_Petty_Cash_List(f1);
                return _instance;
            }
        }



        //=======================================================
        public UC_Petty_Cash_List(Form1 form1)
        {
            f1 = form1;
            InitializeComponent();
        }
        //======================LIST HOLD TRANSACTION============================================
        public void holding(String query)
        {
            dgv_hold.Rows.Clear();

            ckon.con.Close();
            //string date = tanggal;
            String sql = query;
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();

            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    id_trans = ckon.myReader.GetString("PETTY_CASH_ID");
                    exp_ctg = ckon.myReader.GetString("EXPENSE_CATEGORY");
                    exp_date = ckon.myReader.GetString("EXPENSE_DATE");
                    exp_total = ckon.myReader.GetString("TOTAL_EXPENSE");
                    int exp_total2 = Int32.Parse(exp_total);
                    int n = dgv_hold.Rows.Add();
                    dgv_hold.Rows[n].Cells[0].Value = id_trans;
                    dgv_hold.Rows[n].Cells[1].Value = exp_ctg;
                    dgv_hold.Rows[n].Cells[2].Value = exp_total2;
                    dgv_hold.Rows[n].Cells[3].Value = exp_date;

                }
                dgv_hold.Columns[2].DefaultCellStyle.Format = "#,###";
            }
            ckon.con.Close();
        }
        //=============================================================================
        private void t_search_trans_OnTextChange(object sender, EventArgs e)
        {
            if (t_search_trans.text == "")
            {
                String sql = "SELECT * FROM pettycash WHERE  STATUS='1' ";
                holding(sql);
            }
            else
            {
                String sql = "SELECT * FROM pettycash WHERE  STATUS='1' AND PETTY_CASH_ID LIKE '%" + t_search_trans.text + "%' ";
                holding(sql);
            }
        }

        //=============================================================================================
        //===============TAMPILKAN DATA PETTY CASH ====================================================
        public void retreive()
        {

            dgv_petty.Rows.Clear();
            String sql = "SELECT * FROM pettycash_line WHERE PETTY_CASH_ID = '" + l_transaksi.Text + "'ORDER BY _id ASC";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            try
            {
                ckon.con.Open();
                ckon.adapter = new MySqlDataAdapter(ckon.cmd);
                ckon.adapter.Fill(ckon.dt);
                foreach (DataRow row in ckon.dt.Rows)
                {
                    int n = dgv_petty.Rows.Add();
                    dgv_petty.Rows[n].Cells[0].Value = row["_id"].ToString();
                    dgv_petty.Rows[n].Cells[1].Value = row["EXPENSE_NAME"].ToString();
                    dgv_petty.Rows[n].Cells[2].Value = row["QUANTITY"].ToString();
                    dgv_petty.Rows[n].Cells[3].Value = row["PRICE"];
                    dgv_petty.Rows[n].Cells[4].Value = row["TOTAL"];
                }
                dgv_petty.Columns[3].DefaultCellStyle.Format = "#,###";
                dgv_petty.Columns[4].DefaultCellStyle.Format = "#,###";
                ckon.dt.Rows.Clear();
                ckon.con.Close();
            }
            catch
            { }

        }
        //===================================================================================
        //===========================GET DATA BY ID ================================================
        public void get_data_id()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM pettycash WHERE PETTY_CASH_ID='" + id_list + "'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {
                l_exp_ctg.Text = ckon.myReader.GetString("EXPENSE_CATEGORY");
                l_exp_date.Text = ckon.myReader.GetString("EXPENSE_DATE");
                amount = ckon.myReader.GetInt32("TOTAL_EXPENSE");
                //l_desc.Text = ckon.myReader.GetString("DESCRIPTION");
            }
            l_total.Text = string.Format("{0:#,###}" + ",00", amount);
            //l_total.Text = amount.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
        }
        //==========================================================================================
        //=====================ITUNG TOTAL BUDGET===========================================
        public void get_budget()
        {
            ckon.con.Close();
            String sql = "SELECT * FROM store ";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            if (ckon.myReader.HasRows)
            {
                while (ckon.myReader.Read())
                {
                    bg_ToStore = ckon.myReader.GetInt32("BUDGET_TO_STORE");
                    //bg_ToCasir = ckon.myReader.GetInt32("BUDGET_TO_CASHIER");
                }
                l_budget.Text = String.Format("{0:#,###}" + ",00", bg_ToStore);
                //l_b_ToCashier.Text = String.Format("{0:#,###}" + ",00", bg_ToCasir);
            }
            else
            {
                l_budget.Text = "0,00";
            }
            ckon.con.Close();
        }
        //==================================================================================

        //=======================TOTAL EXPENSE==============================================
        public void get_expense()
        {
            ckon.con.Close();
            String sql = "SELECT SUM(pettycash.TOTAL_EXPENSE) as total FROM pettycash WHERE STATUS='1'";
            ckon.cmd = new MySqlCommand(sql, ckon.con);
            ckon.con.Open();
            ckon.myReader = ckon.cmd.ExecuteReader();
            while (ckon.myReader.Read())
            {

                try
                {
                    tot_exp = ckon.myReader.GetInt32("total");
                    if (tot_exp <= 0)
                    { l_expense.Text = "0,00"; }
                    else
                    { l_expense.Text = string.Format("{0:#,###}" + ",00", tot_exp); }

                    //l_total_amount.Text = amount.ToString("C2", CultureInfo.GetCultureInfo("id-ID"));
                }
                catch
                {
                    l_expense.Text = "0,00";
                }
            }
            ckon.con.Close();
        }
        //==================================================================================
        //===================================================================================
        private void dgv_hold_MouseClick(object sender, MouseEventArgs e)
        {
            String sql = "SELECT * FROM pettycash WHERE STATUS='1' ORDER BY EXPENSE_DATE ASC";
            id_list = dgv_hold.SelectedRows[0].Cells[0].Value.ToString();
            l_transaksi.Text = id_list;
            holding(sql);
            retreive();
            get_data_id();
        }
        //===================================================================================
        //======================================================================================
        private void b_back_PC_Click(object sender, EventArgs e)
        {
            f1.p_kanan.Controls.Clear();
            if (!f1.p_kanan.Controls.Contains(UC_Petty_Cash.Instance))
            {
                f1.p_kanan.Controls.Add(UC_Petty_Cash.Instance);
                UC_Petty_Cash.Instance.Dock = DockStyle.Fill;
                UC_Petty_Cash.Instance.BringToFront();
            }
            else
                UC_Petty_Cash.Instance.BringToFront();
        }
        //======================================================================================
    }
}

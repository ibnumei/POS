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
using System.Xml;
using System.Net.NetworkInformation;

namespace try_bi
{
    public partial class Form_Conenctor : Form
    {
        String replace_pass, VarBackDate, StringBackDate, OnOrOff, MasterOrChild, DeviceCode="";
        public Form_Conenctor()
        {
            InitializeComponent();
        }

        private void Form_Conenctor_Load(object sender, EventArgs e)
        {
            radioOnline.Checked = true;

            groupOnline.Visible = true;
            groupOffline.Visible = false;

            //txtServer.Text = "localhost";
            //txtNmDb.Text = "biensi_pos_db";
            //txtUser.Text = "root";
            dateTimePicker1.Visible = false;
            LabelYesNo.Text = Properties.Settings.Default.mBackDate;
            //txtServer.Text = Properties.Settings.Default.mServer;
            //txtNmDb.Text = Properties.Settings.Default.mDBName;
            //txtUser.Text = Properties.Settings.Default.mUserDB;
            txtPass.Text = Properties.Settings.Default.mPassDB;
            nm_printer.Text = Properties.Settings.Default.mPrinter;
        }

        private void b_save_Click(object sender, EventArgs e)
        {
            String link = "";

            //Get Device Code
            if (OnOrOff == "Online")
            {
                link = "http://mpos.biensicore.co.id";
            }
            else
            {
                link = txtOff.Text;
                MethodGetDevId();
            }
            

            
            if (checkboxBackDate.Checked == true)
            {
                LabelYesNo.Text = "YES";
            }
            else
            {
                LabelYesNo.Text = "NO";
            }

            replace_pass = txtPass.Text.Replace(" ", "");
            Properties.Settings.Default.mServer = "localhost";
            Properties.Settings.Default.mDBName = "biensi_pos_db";
            Properties.Settings.Default.mUserDB = "root";
            Properties.Settings.Default.mPassDB = replace_pass;
            Properties.Settings.Default.mPrinter = nm_printer.Text;
            Properties.Settings.Default.mBackDate = LabelYesNo.Text;
            Properties.Settings.Default.ValueBackDate = dateTimePicker1.Text;

            Properties.Settings.Default.OnnOrOff = OnOrOff;
            Properties.Settings.Default.MstrOrChld = MasterOrChild;
            Properties.Settings.Default.DevCode = DeviceCode;
            save_app_config();
            Properties.Settings.Default.Save();

           
            
            XmlDocument doc = new XmlDocument();
            doc.Load("xmlConn.xml");

            XmlNode node = doc.SelectSingleNode("Table/Product/pass_db[1]"); // [index of user node]
            node.InnerText = replace_pass;

            XmlNode node2 = doc.SelectSingleNode("Table/Product/link_api[1]"); // [index of user node]
            node2.InnerText = link;

            XmlNode node3 = doc.SelectSingleNode("Table/Product/code_pc[1]"); // [index of user node]
            node3.InnerText = DeviceCode;

            doc.Save("xmlConn.xml");

            MessageBox.Show("Connection Successfully Saved. Application Will Be Closed, Please Re-Open");
            Application.Restart();
           
        }

        public void save_app_config()
        {
            String connectionString = string.Format("User Id={0};Password={1};Host={2};Database={3};Persist Security Info=True", "root", replace_pass, "localhost", "biensi_pos_db");
            try
            {
                AppSetting setting = new AppSetting();
                setting.SaveConnectionString("BiensiPosDbDataContextConnectionString", connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }



        private void checkboxBackDate_CheckedChanged(object sender, EventArgs e)
        {
            if(checkboxBackDate.Checked == true)
            {
                dateTimePicker1.Visible = true;
                LabelYesNo.Text = "YES";
            }
            else
            {
                dateTimePicker1.Visible = false;
                LabelYesNo.Text = "NO";

            }
            
        }

        private void b_MacAdrs_Click(object sender, EventArgs e)
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {

                if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        mac = nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            //MessageBox.Show(mac);

            API_DeviceCode code = new API_DeviceCode();
            code.cek_storeCode();
            DeviceCode = code.GetDeviceId(mac);
            //MessageBox.Show(DeviceCode);
        }

        //=======================MEMUNCULKAN GROUP BOX ONLINE TRUE============================
        private void radioOnline_CheckedChanged(object sender, EventArgs e)
        {
            groupOnline.Visible = true;
            groupOffline.Visible = false;
            radioMaster.Checked = false;
            radioChild.Checked = false;
            OnOrOff = "Online";
            DeviceCode = "null";
        }
        //=====================MEMUNCULKAN GROUP BOX OFFLINE TRUE=============================
        private void radioOffline_CheckedChanged(object sender, EventArgs e)
        {
            groupOnline.Visible = true;
            groupOffline.Visible = true;
            radioMaster.Checked = true;
            OnOrOff = "Offline";
        }
        //===============Set to PC Master=================================
        private void radioMaster_CheckedChanged(object sender, EventArgs e)
        {
            MasterOrChild = "Master";
        }
        //===============Set to PC Child==================================
        private void radioChild_CheckedChanged(object sender, EventArgs e)
        {
            MasterOrChild = "Child";
        }
        //================================================================
        public void MethodGetDevId()
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {

                if (nic.OperationalStatus == OperationalStatus.Up && (!nic.Description.Contains("Virtual") && !nic.Description.Contains("Pseudo")))
                {
                    if (nic.GetPhysicalAddress().ToString() != "")
                    {
                        mac = nic.GetPhysicalAddress().ToString();
                    }
                }
            }
            //MessageBox.Show(mac);

            API_DeviceCode code = new API_DeviceCode();
            code.LinkGetCode(txtOff.Text);
            code.cek_storeCode();
            DeviceCode = code.GetDeviceId(mac);
            //MessageBox.Show(DeviceCode);
        }

    }
}

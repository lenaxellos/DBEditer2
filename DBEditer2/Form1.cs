﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OracleClient;
using System.Configuration;

namespace DBEditer2
{
    public partial class Form1 : Form
    {
        DBCon con;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string firstString, secondString, thirdString;
            int txt1Position = textBox1.SelectionStart;
            int txt1FullLength = textBox1.TextLength;

            firstString = textBox1.Text.Substring(1, txt1Position);
            secondString = (string)dataGridView1[e.ColumnIndex, e.RowIndex].Value;
            thirdString = textBox1.Text.Substring(txt1Position + 1, txt1FullLength - txt1Position - 1);

            textBox1.Text = firstString + secondString + thirdString;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string query = "";
            con = new DBCon();

            con.DBConnect();

            if (textBox1.SelectedText.Trim() == "")
            {
                query = textBox1.Text;
            }
            else
            {
                query = textBox1.SelectedText.Trim();
            }

            con.DBDataSet(query);
            con.DBFillData(dataGridView1);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            con.DBUpdate();            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {   
            /*
            TabControl tabC = new TabControl();
            tabC = this.tabControl1;            
            tabC.TabPages.Add("SQL" +  tabC.TabPages.Count + 1);
            ucGrid ucGridNew = new ucGrid();
             */
            CreateTabPage();
        }

        private void CreateTabPage()
        {
            TabPage tp = new TabPage("SQL"+(tabControl1.TabPages.Count + 1).ToString());
            //Form1 frm1 = new Form1();
            ucGrid ucGridNew = new ucGrid();          
            tp.Controls.Add(ucGridNew);
            tabControl1.TabPages.Add(tp);
            ucGridNew.Dock = DockStyle.Fill;
            tabControl1.TabPages[0].Select();
        }

        private void rdoTable_Click(object sender, EventArgs e)
        {
            DbShowTable();
        }

        private void txtFindTable_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DbShowTable();
            }

        }

        private void DbShowTable()
        {
            string query = "SELECT TABLE_NAME FROM USER_TABLES WHERE UPPER(TABLE_NAME) LIKE UPPER('%@TABLENAME@%') ORDER BY TABLE_NAME";
            query = query.Replace("@TABLENAME@", txtFindTable.Text.Trim());

            con = new DBCon();
            con.DBConnect();
            con.DBDataSet(query);
            con.DBFillData(dataGridView2);
        }

        private void DbShowColumn()
        {
            dataGridView3.Rows.Clear();     

            string query = "SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = '@TABLENASE@'";
            query = query.Replace("@TABLENAME@", txtFindTable.Text.Trim());

            con = new DBCon();
            con.DBConnect();
            con.DBDataSet(query);
            con.DBFillData(dataGridView3);
        }

        private void rdoView_Click(object sender, EventArgs e)
        {

        }
    }
}

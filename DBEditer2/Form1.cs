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
            CreateTabPage();
        }
/*
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
*/

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ShowData();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            con.DBUpdate();            
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {   
            CreateTabPage();
        }

        private void CreateTabPage()
        {
            TabPage tp = new TabPage("SQL"+(tabControl1.TabPages.Count + 1).ToString());

            Panel plCtr = new Panel();
            TextBox txtSqlNote = new TextBox();
            DataGridView dgv = new DataGridView();
            SplitContainer spl1 = new SplitContainer();

            //txtBox event
            txtSqlNote.PreviewKeyDown += new PreviewKeyDownEventHandler(TextBox_PreviewKeyDown);
            txtSqlNote.KeyDown += new KeyEventHandler(TextBox_KeyDown);

            //txtSqlNote Setting - style
            spl1.Controls[0].Controls.Add(txtSqlNote);
            txtSqlNote.Dock = DockStyle.Fill;
            txtSqlNote.Multiline = true;
            txtSqlNote.Name = "txtSql";
            Font font1 = new Font("돋움", 11);
            txtSqlNote.Font = font1;

            //DataGridView event
            dgv.CellDoubleClick += new DataGridViewCellEventHandler(DataGridView_CellClick);
            //dgv.CellEndEdit += new DataGridViewCellEventHandler(DataGiedView_CellEndEdit);
            //dgv.CellLeave += new DataGridViewCellEventHandler(DataGridView_CellLeave);
            dgv.CellValueChanged += new DataGridViewCellEventHandler(DataGridView_CellValueChanged);            

            //DataGridView Setting - style
            spl1.Controls[1].Controls.Add(dgv);
            dgv.Dock = DockStyle.Fill;
            dgv.Name = "dgv";
            dgv.AllowUserToDeleteRows = true;           

            spl1.Orientation = Orientation.Horizontal;
            tp.Controls.Add(spl1);
            tabControl1.TabPages.Add(tp);
            spl1.Dock = DockStyle.Fill;

            /*
            ucGrid ucGridNew = new ucGrid();
            tp.Controls.Add(ucGridNew);
            tabControl1.TabPages.Add(tp);
            ucGridNew.Dock = DockStyle.Fill;
            ucGridNew.Name = "NewPage";
            */
            tabControl1.SelectedIndex = tabControl1.TabCount - 1;
        }

        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {                        
            string firstString = "";
            string secondString = "";
            string thirdString = "";
            TextBox txtCtl = new TextBox();
            txtCtl = FindTextboxInTab(tabControl1.SelectedIndex);
            int txt1Position = txtCtl.SelectionStart;
            int txt1FullLength = txtCtl.TextLength;

            firstString = txtCtl.Text.Substring(0, txt1Position);
            secondString = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value.ToString();
            if (txt1FullLength - txt1Position > 0)
            {
                thirdString = txtCtl.Text.Substring(txt1Position, txt1FullLength - txt1Position);
            }
            else
            {
                thirdString = "";
            }

            txtCtl.Text = firstString + secondString + thirdString.TrimEnd('\r').TrimEnd('\n').ToString();
            //((DataGridView)sender)[e.ColumnIndex, e.RowIndex].Value.;                        
            
        }

        private void DataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            e.Row.RowState.ToString();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {      
            /*
            if (e.Modifiers == Keys.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        ShowData();
                        //System.Windows.Forms.MessageBox.Show("Ctrl+A");
                        break;                     
                }
            } 
             */            
            //if (e.Control && e.KeyCode == Keys.Enter)
            try
            {
                if (e.Control && e.KeyCode == Keys.Enter)
                {
                    //ShowData();
                    e.SuppressKeyPress = true;
                    e.Handled = false;                
                    ShowData();
                    //System.Windows.Forms.MessageBox.Show("0000");
                    
                    //e.SuppressKeyPress = true;
                }
            }
            catch
            {
                e.SuppressKeyPress = true;
            }
        }
          
        //컨테이너에 포함된 Textbox에 단축기가 먹게 하기위해 추가
        private void TextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                ((TextBox)sender).Copy();
            }
            if (e.Control && e.KeyCode == Keys.V)
            {
                ((TextBox)sender).Paste();
            }
            if (e.Control && e.KeyCode == Keys.X)
            {
                ((TextBox)sender).Cut();
            }
            if (e.Control && e.KeyCode == Keys.Z)
            {
                ((TextBox)sender).Undo();
            }
        }
    
        private void DelTabPage()
        {
            if (tabControl1.TabPages.Count > 0)
            {
                tabControl1.TabPages.Remove(tabControl1.TabPages[tabControl1.TabPages.Count - 1]);
            }
        }

        // 이 이름을 이용해서 tab page 내에서, 이 이름을 갖는  control 을 찾을 수 있다.
        // 이렇게 찾은 텍스트 박스에서 (찾아졌다면 TextBox 객체가 null 이 아니다) Text 를 추출하면 된다.
        private TextBox FindTextboxInTab(int tabIndex)
        {
            TextBox txt = null;
            Control[] ctrl = this.tabControl1.TabPages[tabIndex].Controls.Find("txtSql", true);
            if (ctrl != null && ctrl.Length > 0)
            {
                txt = ctrl[0] as TextBox;
            }
            return txt;
        }

        // 이 이름을 이용해서 tab page 내에서, 이 이름을 갖는  control 을 찾을 수 있다.
        // 이렇게 찾은 데이터그리드뷰에서 (찾아졌다면 DataGridView 객체가 null 이 아니다) DataGridView 를 추출하면 된다.
        private DataGridView FindDgvInTab(int tabIndex)
        {
            DataGridView dgv = null;
            Control[] ctrl = this.tabControl1.TabPages[tabIndex].Controls.Find("dgv", true);
            if (ctrl != null && ctrl.Length > 0)
            {
                dgv = ctrl[0] as DataGridView;
            }
            return dgv;
        }

        //textBox에 작성된 전체 컨트롤에서 내가 선택(커서가 깜박이는 곳)하고 있는곳의 쿼리구문을 찾는다.
        //textBox 전체 텍스트와 textBox상의 커서의 현재 position을 입력받는다.
        private string FindSelectQuery(string fullText, int curserPos)
        {
            int cnt = 0;            //Loop CountNum
            int len = 0;            
            int selectIndex = 0;    //
            string[] sqlQuery = fullText.Split(new string[] {"\r\n\r\n"}, StringSplitOptions.None);

            foreach (var r in sqlQuery)
            {
                len = len + r.Length + 2;

                if (curserPos - len <= 0)
                {
                    selectIndex = cnt;
                    break;
                }
                cnt++;
            }

            return sqlQuery[selectIndex];
        }

        //데이터를 가져와서 그리드에 보여준다.
        private void ShowData()
        {
            string query = "";
            TextBox txtCrtl = new TextBox();
            DataGridView dgv = new DataGridView();
            
            txtCrtl = FindTextboxInTab(tabControl1.SelectedIndex);
            
            dgv = FindDgvInTab(tabControl1.SelectedIndex);
           
            if (txtCrtl.SelectedText.Trim() == "")
            {
                query = FindSelectQuery(txtCrtl.Text, txtCrtl.SelectionStart);
            }
            else
            {
                query = txtCrtl.SelectedText.Trim();
            }
              
            con = new DBCon(query);
            con.DBFillData(dgv);

            //System.Windows.Forms.MessageBox.Show(con.pkCnt.ToString());           
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

            con = new DBCon(query);
            con.DBFillData(dataGridView2);
        }

        private void DbShowColumn(string tableName)
        {
            try
            {
                dataGridView3.DataSource = null;
                dataGridView3.Refresh();                
            }
            catch(System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }              

            string query = "SELECT COLUMN_NAME, DATA_TYPE, DATA_LENGTH, NULLABLE FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = '@TABLENAME@'";
            query = query.Replace("@TABLENAME@", tableName.Trim());

            con = new DBCon(query);
            //con.DBConnect();
            //con.DBDataSet(query);
            con.DBFillData(dataGridView3);
        }

        private void rdoView_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {            
            TextBox txtCrtl = new TextBox();
            DataGridView dgv = new DataGridView();
            
            txtCrtl = FindTextboxInTab(tabControl1.SelectedIndex);
            dgv = FindDgvInTab(tabControl1.SelectedIndex);

            if (dgv.ReadOnly)
            {
                dgv.ReadOnly = false;
                dgv.AllowUserToAddRows = true;
                dgv.GridColor = Color.Tomato;
                dgv.BackgroundColor = Color.Tomato;
                toolStripButton8.Text = "Editing...";
            }
            else
            {
                dgv.ReadOnly = true;
                dgv.AllowUserToAddRows = false;
                dgv.GridColor = Color.Wheat;
                dgv.BackgroundColor = Color.Wheat;
                toolStripButton8.Text = "Edit";
            }
        }

        private void rdoTable_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            DelTabPage();
        }

        /*
        private void DataGiedView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {        
            if (con.dataTableProF.Rows[e.RowIndex][e.ColumnIndex].ToString() != ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.PaleGreen;
            }
            else
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
        }
         */

        private void DataGridView_CellLeave(object sender, DataGridViewCellEventArgs e)
        {           
            if (con.dataTableProF.Rows[e.RowIndex][e.ColumnIndex].ToString() != ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.PaleGreen;
            }
            else
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }

            foreach (DataRow R in con.dataTableProF.Rows)
            {
                switch (R.RowState)
                {
                    case DataRowState.Added:
                        System.Windows.Forms.MessageBox.Show("Added");
                        break;
                    case DataRowState.Deleted:
                        System.Windows.Forms.MessageBox.Show("Deleted");
                        break;
                    case DataRowState.Detached:
                        System.Windows.Forms.MessageBox.Show("Detached");
                        break;
                    case DataRowState.Modified:
                        System.Windows.Forms.MessageBox.Show("Modified");
                        break;
                    //case DataRowState.Unchanged:
                    //    System.Windows.Forms.MessageBox.Show(con.pkCnt.ToString());
                    //    break;
                }
            }

        }

        //CellValueChanged 수정한 값이 들어온다. 그런데 dataTable과 DataGridView컨트롤 모두 바뀐값으로 들어온다.
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (con.dataTableProF.Rows[e.RowIndex][e.ColumnIndex, DataRowVersion.Original].ToString() == ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
            else
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Violet;
            }
            /*
            if (con.dataTableProF.Rows[e.RowIndex][e.ColumnIndex].ToString() != ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.PaleGreen;
            }
            else
            {
                ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
                 */                  
            /*
            foreach (DataRow R in con.dataTableProF.Rows)
            {
                switch (R.RowState)
                {
                    case DataRowState.Added:
                        System.Windows.Forms.MessageBox.Show("Added");
                        break;
                    case DataRowState.Deleted:
                        System.Windows.Forms.MessageBox.Show("Deleted");
                        break;
                    case DataRowState.Detached:
                        System.Windows.Forms.MessageBox.Show("Detached");
                        break;
                    case DataRowState.Modified:
                        System.Windows.Forms.MessageBox.Show("Modified");
                        break;
                    //case DataRowState.Unchanged:
                    //    System.Windows.Forms.MessageBox.Show(con.pkCnt.ToString());
                    //    break;
                }
            }
             */
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DbShowColumn((string)dataGridView2[e.ColumnIndex, e.RowIndex].Value);
            }
            catch (System.Exception ee)
            {
                System.Windows.Forms.MessageBox.Show(ee.Message);
            }
        }         
    }
}

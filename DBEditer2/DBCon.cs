using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;

namespace DBEditer2
{
    class DBCon
    {
        private OracleConnection conn;          //adapt의 datasource에 채울 connection으로 사용
        private OracleConnection conn2;         //OracleCommand로 직접작성쿼리로 실행할 connection으로 사용
        private OracleCommand cmd;             
        //private OracleCommand cmd2;             //conn2용 command
        private OracleDataAdapter adapter;
        private OracleCommandBuilder builder;
        private DataSet dataSet;
        private DataTable dataTable;

        //private int pkCnt;
        public int pkCnt;                    //조회한 테이블의 PK개수
        /*
        {
            get { return pkCnt; }
            private set { pkCnt = value; }
        }
        */
        public DataTable dataTableProF
        {
            get { return dataTable; }
            set { dataTable = value; }
        }

        public const string SQLPK = "SELECT COLUMN_NAME, COUNT(COLUMN_NAME) OVER () AS CNT " +
                                    "FROM USER_IND_COLUMNS " +
                                    "WHERE INDEX_NAME IN " +
                                    "(SELECT CONSTRAINT_NAME " +
                                    "FROM USER_CONSTRAINTS " +
                                    "WHERE CONSTRAINT_TYPE = 'P' " +
                                    "AND TABLE_NAME = '@TABLENAME@') " +
                                    "ORDER BY COLUMN_POSITION";

        public const string SQLPKCNT = "SELECT COUNT(COLUMN_NAME) " +
                                    "FROM USER_IND_COLUMNS " +
                                    "WHERE INDEX_NAME IN " +
                                    "(SELECT CONSTRAINT_NAME " +
                                    "FROM USER_CONSTRAINTS " +
                                    "WHERE CONSTRAINT_TYPE = 'P' " +
                                    "AND TABLE_NAME = '@TABLENAME@') ";

        public const string SQLALLTABLE = "SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME";


        public DBCon(string query)
        {
            DBConnect();
            DBDataSet(query);
            GetTbPkCnt();
        }

        public void DBConnect()
        {
            conn = new OracleConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            conn.Open();
        }

        //OracleCommand 까지 만들어서 reader나, scalar등에 사용할 수 있게 command를 넘겨줌.
        public OracleCommand CretateOraCommand(string query)
        {
            conn2 = new OracleConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            conn2.Open();
            cmd = new OracleCommand(query, conn2);

            return cmd;
        }

        public string GetOraScalarData(string sqltxt)
        {            
            OracleCommand rcvCmd = CretateOraCommand(sqltxt);
            string rtn = rcvCmd.ExecuteScalar().ToString();

            rcvCmd.Dispose();
            conn2.Close();

            return rtn;
        }

        //해당 테이블의 Pk를 구함.
        public string GetTbPkCnt()
        {
            string sqltxt;
            
            sqltxt = SQLPKCNT.Replace("@TABLENAME@", dataTable.TableName);


            /*
            OracleCommand rcvCmd = CretateOraCommand(sqltxt);
            string count = rcvCmd.ExecuteScalar().ToString();
            conn2.Close();

            return Convert.ToInt32(count);
            */
            return GetOraScalarData(sqltxt);
        }

        
        //OracleCommandBuilder로 SQL문 자동생성 기능 사용.
        public void DBDataSet(string query)
        {
            string tableName = FindTableName(query);

            adapter = new OracleDataAdapter(query, conn);
            dataTable = new DataTable(tableName);
            builder = new OracleCommandBuilder(adapter);            
        }
        
        public void DBFillData(System.Windows.Forms.DataGridView dgv)
        {
            try
            {
                adapter.Fill(dataTable);

                dgv.DataSource = dataTable;
                conn.Close();
            }
            catch(System.Exception e)
            {
                //System.Windows.Forms.MessageBox.Show(e.Message);                
            }              
        }

        public void DBUpdate()
        {
            adapter.Update(dataTable);
        }

        public void DBSetColumnKey()
        {
            using (OracleConnection conn2 = new OracleConnection(ConfigurationSettings.AppSettings["ConnectionString"]))
            {                
                int rowCnt = 0;
                string readString = "";
                string sqlText = "";
                conn2.Open();

                sqlText = SQLPK.Replace("@TABLENAME@", dataTable.TableName);

                OracleCommand cmd2 = new OracleCommand(sqlText, conn2);

                OracleDataReader reader = cmd2.ExecuteReader();
 
                DataColumn[] keys = new DataColumn[4];

                //List<DataColumn> keys2 = new List<DataColumn>;

                // 레코드 계속 가져와서 루핑
                while (reader.Read())
                {
                    readString = reader[0] as string;
                    keys[rowCnt] = dataTable.Columns[readString];                    
                    rowCnt++;
                }

                // 사용후 닫음
                reader.Close();
            }
        }

        //쿼리에서 table을 추출함.
        public string FindTableName(string query)
        {
            string tableName = "";
            string upperQuery = query.ToUpper();
            upperQuery = upperQuery.Replace('\n', ' ');
            upperQuery = upperQuery.Replace('\r', ' ');

            if (upperQuery.IndexOf("FROM") > -1)
            {               
                string[] result = upperQuery.Split(new char[] { ' ' });
                List<string> listResult = new List<string>(result);

                listResult = ArrayStringNullex(listResult);

                for (int cnt = 0; cnt < listResult.Count; cnt++)
                {
                    if (listResult[cnt] == "FROM")
                    {
                        if (listResult.Count - 1 > cnt)
                        {
                            tableName = listResult[cnt + 1];
                            return tableName;
                        }
                    }
                }
            }
            return tableName;
        }

        //기본키가 없을때 해당테이블에 ROWID가 있는지체크        
        public bool ChkNotPkIsRowid(string queryTxt)
        {
            if ( queryTxt.IndexOf("ROWID") != -1 )
            {
                return true;
            }else
            {
                return false;
            }           
        }

        //List<string>중 null값인 인덱스는 제외시킨다.
        public List<string> ArrayStringNullex(List<string> list)
        {            
            for (int i = 0 ; i < list.Count ; i++)
            {
                if (list[i] == "")
                {
                    list.RemoveAt(i);
                }
            }
            return list; 
        }

        //채워진 테이블에 Key가 없을때 adapt를 이용한 Update를 할지 아니면 rownum을 만들어서 수동업데이트를 할지를 판단하는 함수
        private void HowUpdateMtd()
        {
            
        }
        
    }
}

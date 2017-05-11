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
        OracleConnection conn;       
        OracleCommand cmd;
        OracleDataAdapter adapter;
        OracleCommandBuilder builder;
        DataSet dataSet;
        DataTable dataTable;

        public const string SQLPK = "SELECT COLUMN_NAME, COUNT(COLUMN_NAME) OVER () AS CNT " +
                                    "FROM USER_IND_COLUMNS " +
                                    "WHERE INDEX_NAME IN " +
                                    "(SELECT CONSTRAINT_NAME " +
                                    "FROM USER_CONSTRAINTS " +
                                    "WHERE CONSTRAINT_TYPE = 'P' " +
                                    "AND TABLE_NAME = '@TABLENAME@') " +
                                    "ORDER BY COLUMN_POSITION";

        public void DBConnect()
        {
            conn = new OracleConnection(ConfigurationSettings.AppSettings["ConnectionString"]);
            conn.Open();
        }

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
            }
            catch(System.Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);                
            }
            //DBSetColumnKey();
            dgv.DataSource = dataTable;
            conn.Close();
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
                    //j++;
                    rowCnt++;
                }

                // 사용후 닫음
                reader.Close();

                //keys[0] = dataTable.Columns["ACCOUNTDATE"];
                //keys[1] = dataTable.Columns["PTNO"];
                //keys[2] = dataTable.Columns["DEPTCODE"];
                //keys[3] = dataTable.Columns["JIBUFLAG"]; 
                //dataTable.PrimaryKey = new DataColumn[] { dataTable.Columns["PTNO"] }; 
                //dataTable.PrimaryKey = keys;
            }            
        }

        public void DBUpdate()
        {
            adapter.Update(dataTable);
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

        
    }
}

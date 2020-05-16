using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Ch.Common
{
    public class SqlHelper
    {
        /// <summary>
        /// 执行无记录查询
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }
        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText)
        {
            return ExecuteNonQuery(connectionString, cmdType, cmdText, null);
        }
        public static DataSet ExecuteDataset(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            sda.SelectCommand = cmd;
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                sda.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }
        public static DataSet ExecuteDataset(string connectionString, CommandType cmdType, string cmdText)
        {
            return ExecuteDataset(connectionString, cmdType, cmdText, null);
        }
        public static DataTable ExecuteDataTable(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            sda.SelectCommand = cmd;
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                sda.Fill(dt);
                cmd.Parameters.Clear();
                if (conn.State == ConnectionState.Open)
                    conn.Dispose();
                return dt;
            }
        }
        public static DataTable ExecuteDataTable(string connectionString, CommandType cmdType, string cmdText)
        {
            return ExecuteDataTable(connectionString, cmdType, cmdText, null);
        }
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            SqlDataReader rdr = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                    rdr = cmd.ExecuteReader();
                    cmd.Parameters.Clear();
                }
                catch
                {
                    cmd.Parameters.Clear();
                    conn.Close();
                    throw;
                }
                conn.Dispose();
            }
            return rdr;
        }
        public static SqlDataReader ExecuteReader(string connectionString, CommandType cmdType, string cmdText)
        {
            return ExecuteReader(connectionString, cmdType, cmdText, null);
        }
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalarWithReturn(string connectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand cmd = new SqlCommand();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                cmd.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                cmd.ExecuteNonQuery();
                object val = cmd.Parameters["RETURN_VALUE"].Value;
                cmd.Parameters.Clear();
                return val;
            }
        }
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText)
        {
            return ExecuteScalar(connectionString, cmdType, cmdText, null);
        }
        /// <summary>
        /// 构造SqlParameter,作为数组成员
        /// </summary>
        /// <param name="parm"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter MakeParame(string parm, SqlDbType type, int size, object value)
        {
            SqlParameter p = new SqlParameter(parm, type, size);
            p.Value = value;
            return p;
        }
        /// <summary>
        /// 构造SqlParameter,作为数组成员
        /// </summary>
        /// <param name="parm"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SqlParameter MakeParame(string parmname, object value)
        {
            SqlParameter p = new SqlParameter(parmname, value);
            return p;
        }

        /// <summary>
        /// 组织Command参数
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parm in cmdParms)
                {
                    if (parm != null)
                    {
                        if ((parm.Direction == ParameterDirection.InputOutput ||
                        parm.Direction == ParameterDirection.Input) &&
                        (parm.Value == null))
                        {
                            parm.Value = DBNull.Value;
                        }
                        cmd.Parameters.Add(parm);
                    }
                }
            }
        }
    }
}
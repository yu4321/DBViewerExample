using DBViewerExample.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

namespace DBViewerExample.DAO
{
    public static class MainDAO
    {
        private static SqlConnection conn;
        private static SqlDataAdapter adapter;
        private static SqlCommand cmd;
        private static string TableName;
        private static string connectionstring;

        public static bool InjectConnectionString(string str)
        {
            bool result = false;
            DataSet ds = null;
            connectionstring = str;
            conn = new SqlConnection(connectionstring);
            try
            {
                ds = GetTableList();
            }
            catch
            {
                result = false;
            }
            if (ds != null)
            {
                result = true;
                
            }
            return result;
        }

        public static void SelectTable(string _tablename)
        {
            TableName = _tablename;
        }

        public static DataSet GetTableList()
        {
            DataSet result = new DataSet();
            try
            {
                conn.Open();
                adapter = new SqlDataAdapter("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", conn);
                adapter.Fill(result);
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                try
                {
                    conn.Close();
                }
                catch
                {

                }
            }
        }

        public static bool NewItem(Collection<KeyValuePair<string, string>> values)
        {
            string lastvalue = values[0].Value;
            try
            {
                NameValueCollection NV = GetColumnsType();
                conn.Open();
                string sql = "insert into " + TableName + "(";
                for (int i = 0; i < values.Count; i++)
                {
                    sql += string.Format("@key{0:00}", i);
                    if (i != values.Count - 1) sql += ", ";
                }

                sql += ") values(";
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].Key.Contains("INDEX")) sql += "@value" + i;
                    else sql += "@value" + i + "";
                    if (i != values.Count - 1) sql += ", ";
                }

                sql += ");";
                //System.Console.WriteLine("kvp sql " + sql);
                for (int i = 0; i < values.Count; i++)
                {
                    sql = sql.Replace(string.Format("@key{0:00}", i), values[i].Key);
                }
                cmd = new SqlCommand(sql, conn);
                int index = 0;
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    //Console.WriteLine("Now Key " + kvp.Key);
                    //Console.WriteLine("Now Type " + NV[kvp.Key]);
                    if (NV[kvp.Key].Contains("int"))//(kvp.Key.Contains("INDEX") == false && kvp.Key.Contains("DIRECTION") == false)
                    {
                        //Console.WriteLine("int");
                        cmd.Parameters.AddWithValue("value" + index, int.Parse(kvp.Value));
                    }
                    else if (NV[kvp.Key].Contains("datetime"))
                    {
                        //Console.WriteLine("datetime");
                        cmd.Parameters.AddWithValue("value" + index, DateTime.Parse(kvp.Value));
                    }
                    else
                    {
                        //Console.WriteLine("not int");
                        cmd.Parameters.AddWithValue("value" + index, kvp.Value);
                        /*   SqlParameter valueparam = new SqlParameter("value" + index, SqlDbType.Int);
                           valueparam.Value = int.Parse(kvp.Value);
                           cmd.Parameters.Add(valueparam);*/
                    }
                    index++;
                }
                /* foreach (SqlParameter x in cmd.Parameters)
                 {
                     Console.WriteLine("typename and value " + x.TypeName + " " + x.Value);
                 }*/
                cmd.CommandText = "";
                cmd.CommandText = sql;
                //System.Console.WriteLine(cmd.CommandText);
                int result = cmd.ExecuteNonQuery();
                //System.Console.WriteLine("실행 횟수: " + result);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("오류 발생: " + lastvalue);
                System.Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static bool UpdateItemundercolumn(Collection<KeyValuePair<string, string>> values, string column)
        {
            try
            {
                NameValueCollection NV = GetColumnsType();
                conn.Open();
                string sql = "update " + TableName + " set ";
                object value = null;
                for (int i = 0; i < values.Count; i++)
                {
                    sql += string.Format("@key{0:00} = @value{1}", i, i);
                    if (i != values.Count - 1) sql += ", ";
                }

                sql += " where " + column + " = @code";
                System.Console.WriteLine("kvp sql " + sql);
                for (int i = 0; i < values.Count; i++)
                {
                    sql = sql.Replace(string.Format("@key{0:00}", i), values[i].Key);
                }
                cmd = new SqlCommand(sql, conn);
                int index = 0;
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    if (kvp.Key == column) value = kvp.Value;
                    if (NV[kvp.Key].Contains("int"))
                    {

                        cmd.Parameters.AddWithValue("value" + index, int.Parse(kvp.Value));
                    }
                    else if (NV[kvp.Key].Contains("datetime"))
                    {

                        cmd.Parameters.AddWithValue("value" + index, DateTime.Parse(kvp.Value));
                    }
                    else
                    {

                        cmd.Parameters.AddWithValue("value" + index, kvp.Value);

                    }
                    index++;
                }

                cmd.Parameters.AddWithValue("code", value);
                cmd.CommandText = "";
                cmd.CommandText = sql;

                int result = cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        private static NameValueCollection GetColumnsType()
        {
            DataSet result = new DataSet();
            NameValueCollection NV = new NameValueCollection();
            try
            {
                conn.Open();
                cmd = new SqlCommand("SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + TableName + "'", conn);

                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(result);
                foreach (DataRow dr in result.Tables[0].Rows)
                {
                    NV.Add(dr["COLUMN_NAME"].ToString(), dr["DATA_TYPE"].ToString());
                }
                return NV;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        

        public static bool DeleteItembyCertainColumn(string columnname, string columnvalue)
        {
            try
            {
                conn.Open();
                cmd = new SqlCommand("delete from " + TableName + " where " + columnname + "=@givenname", conn);
                cmd.Parameters.AddWithValue("givenname", columnvalue);
                int result = cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataSet GetEveryItem()
        {
            DataSet result = new DataSet();
            try
            {
                conn.Open();
                adapter = new SqlDataAdapter("SELECT * FROM " + TableName + "", conn);
                adapter.Fill(result);
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataSet GetCertainItembyCertainColumn(string columnname, string columnvalue)
        {
            DataSet result = new DataSet();
            try
            {
                conn.Open();
                cmd = new SqlCommand("select * from " + TableName + " where " + columnname + "=@givenname", conn);
                cmd.Parameters.AddWithValue("givenname", columnvalue);
                adapter = new SqlDataAdapter(cmd);
                adapter.Fill(result);
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                conn.Close();
            }
        }
        
    }
}
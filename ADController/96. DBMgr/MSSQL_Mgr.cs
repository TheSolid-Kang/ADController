using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Engine._01.DBMgr
{
    public class MSSQL_Mgr : DbMgr, IDisposable
    {

        public MSSQL_Mgr()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //Url = ConfigurationManager.ConnectionStrings["HomeDBConn"].ConnectionString;//App.config에서 작성한 DB정보

        }

        ~MSSQL_Mgr()
        {
            Dispose();
        }

        public void Dispose()
        {

        }

        #region SELECT 
        public DataTable GetDataTable(string _url, string _query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_url))
            {
                using (SqlCommand cmd = new SqlCommand(_query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        if (0 >= sqlDataAdapter.Fill(dt))
                        {
                            System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
                        }
                    }
                }
            }
            return dt;
        }

        public DataTable GetSPDataTable(string _url, string _storedProcedure, SqlParameter[] _sqlParameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_url))
            {
                using (SqlCommand cmd = new SqlCommand(_storedProcedure, conn))
                {
                    cmd.Parameters.AddRange(_sqlParameters);
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            if (0 >= sqlDataAdapter.Fill(dt))
                            {
                                System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
                            }
                        }
                    }
                    catch (Exception _e)
                    {
                        System.Diagnostics.Debug.WriteLine(_e.Message);
                    }
                }
            }
            return dt;
        }

        public DataSet GetSPDataSet(string _url, string _storedProcedure, SqlParameter[] _sqlParameters)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(_url))
            {
                using (SqlCommand cmd = new SqlCommand(_storedProcedure, conn))
                {
                    cmd.Parameters.AddRange(_sqlParameters);
                    conn.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd))
                        {
                            if (0 >= sqlDataAdapter.Fill(ds))
                            {
                                System.Diagnostics.Debug.WriteLine("DataTable에 데이터가 없습니다.");
                            }
                        }
                    }
                    catch (Exception _e)
                    {
                        System.Diagnostics.Debug.WriteLine(_e.Message);
                    }
                }
            }
            return ds;
        }
        #endregion

        #region INSERT
        public void InsertData<T>(string _connectionUrl, T data)
        {
            using (SqlConnection connection = new SqlConnection(_connectionUrl))
            {
                connection.Open();

                // 제네릭 INSERT 메서드 호출
                InsertData(connection, data);
            }
        }
        public void InsertData<T>(SqlConnection connection, T data)
        {
            // 데이터 모델 클래스의 속성 정보 가져오기
            var properties = typeof(T).GetProperties();

            // INSERT 쿼리 생성
            string query = $"INSERT INTO {typeof(T).Name} ({string.Join(", ", properties.Select(p => p.Name))}) VALUES ({string.Join(", ", properties.Select(p => $"@{p.Name}"))})";

            // SQL 커맨드 객체 생성 및 파라미터 바인딩
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)property.GetValue(data);
                        if (dateTime.Year > 1753)
                            command.Parameters.AddWithValue($"@{property.Name}", dateTime);
                        else
                            command.Parameters.AddWithValue($"@{property.Name}", "");//'1900-01-01 00:00:00.000' 으로 들어감.
                    }
                    else
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(data));
                    }
                }

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception _e)
                {
                    Debug.WriteLine(_e.Message);
                }
                // 쿼리 실행
            }
        }

        /// <summary>
        /// 
        /// 사용 예시
        ///private void SaveErpAdUsersTbl_IF()
        ///{
        ///    //1. 변수 초기화
        ///    Dictionary<string, List<Users>> mapADUsers = GetMapADUsers();
        ///
        ///    //2. Insert Query
        ///    using (var mgr = new MSSQL_Mgr())
        ///    {
        ///        foreach (var keyPairs in mapADUsers)
        ///        {
        ///            List<Users> adUsers = keyPairs.Value;
        ///            adUsers.ForEach(adUser => mgr.InsertDataByTableName<Users>(DbMgr.DB_CONNECTION.YWDEV, adUser,"yw_TADUsers_IF"));
        ///        }
        ///    }
        ///
        ///    //3. Update Query 작성
        ///}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_CON"></param>
        /// <param name="data"></param>
        /// <param name="tableName"></param>
        public void InsertDataByTableName<T>(string _connectionUrl, T data, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionUrl))
            {
                connection.Open();

                // 제네릭 INSERT 메서드 호출
                InsertDataByTableName(connection, data, tableName);
            }
        }
        public void InsertDataByTableName<T>(SqlConnection connection, T data, string tableName)
        {
            // 데이터 모델 클래스의 속성 정보 가져오기
            var properties = typeof(T).GetProperties();

            // INSERT 쿼리 생성
            string query = $"INSERT INTO {tableName} ({string.Join(", ", properties.Select(p => p.Name))}) VALUES ({string.Join(", ", properties.Select(p => $"@{p.Name}"))})";

            // SQL 커맨드 객체 생성 및 파라미터 바인딩
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)property.GetValue(data);
                        if (dateTime.Year > 1753)
                        {
                            command.Parameters.AddWithValue($"@{property.Name}", dateTime);
                        }
                        else
                        {
                            command.Parameters.AddWithValue($"@{property.Name}", "");//'1900-01-01 00:00:00.000' 으로 들어감.
                        }
                    }
                    else
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(data));
                    }
                }

                // 쿼리 실행
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception _e)
                {
                    Debug.WriteLine(_e.Message);
                }
            }
        }

        #endregion

        #region UPDATE
        public void UpdateData<T>(string connectionString, T data, string keyColumn)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 업데이트할 컬럼 및 값 구성
                var properties = typeof(T).GetProperties();
                var updateColumns = new List<string>();
                var updateValues = new List<object>();

                foreach (var prop in properties)
                {
                    if (prop.Name != keyColumn)
                    {
                        updateColumns.Add($"{prop.Name} = @{prop.Name}");
                        updateValues.Add(prop.GetValue(data));
                    }
                }

                // 쿼리 작성
                string query = $"UPDATE {typeof(T).Name} SET {string.Join(", ", updateColumns)} WHERE {keyColumn} = @{keyColumn}";

                // 파라미터 바인딩
                SqlCommand command = new SqlCommand(query, connection);
                foreach (var property in properties)
                {
                    if (property.PropertyType == typeof(DateTime))
                    {
                        DateTime dateTime = (DateTime)property.GetValue(data);
                        if (dateTime.Year > 1753)
                            command.Parameters.AddWithValue($"@{property.Name}", dateTime);
                        else
                            command.Parameters.AddWithValue($"@{property.Name}", "");//'1900-01-01 00:00:00.000' 으로 들어감.
                    }
                    else
                    {
                        command.Parameters.AddWithValue($"@{property.Name}", property.GetValue(data));
                    }
                }
                //쿼리실행
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception _e)
                {
                    Debug.WriteLine(_e.Message);
                }
            }
        }
        #endregion
    }
}

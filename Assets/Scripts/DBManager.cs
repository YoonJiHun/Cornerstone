using System;
using MySql.Data.MySqlClient;

namespace com.luna.world
{
    public class DBManager
    {
        private MySqlConnection _connection;
        public bool IsConnected => _connection?.State == System.Data.ConnectionState.Open;

        public DBManager() { }

        /// <summary>
        /// DB 연결 초기화
        /// </summary>
        public bool Initialize(DBConfig config)
        {
            try
            {
                if (config == null)
                {
                    UnityEngine.Debug.LogError("[DBManager] DB 설정이 없습니다.");
                    return false;
                }

                string connectionString = $"Server={config.host};Port={config.port};Database={config.database};Uid={config.user};Pwd={config.password};CharSet=utf8mb4;";
                _connection = new MySqlConnection(connectionString);
                _connection.Open();

                UnityEngine.Debug.Log("[DBManager] MariaDB 연결 성공!");
                return true;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[DBManager] MariaDB 연결 실패: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// DB 연결 종료
        /// </summary>
        public void Close()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
                UnityEngine.Debug.Log("[DBManager] MariaDB 연결 종료");
            }
        }

        /// <summary>
        /// SELECT 쿼리 실행
        /// </summary>
        public MySqlDataReader ExecuteReader(string query)
        {
            try
            {
                if (!IsConnected)
                {
                    UnityEngine.Debug.LogError("[DBManager] DB에 연결되어 있지 않습니다.");
                    return null;
                }

                MySqlCommand cmd = new MySqlCommand(query, _connection);
                return cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[DBManager] 쿼리 실행 실패: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// INSERT, UPDATE, DELETE 쿼리 실행
        /// </summary>
        public int ExecuteNonQuery(string query)
        {
            try
            {
                if (!IsConnected)
                {
                    UnityEngine.Debug.LogError("[DBManager] DB에 연결되어 있지 않습니다.");
                    return -1;
                }

                MySqlCommand cmd = new MySqlCommand(query, _connection);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[DBManager] 쿼리 실행 실패: {ex.Message}");
                return -1;
            }
        }

        /// <summary>
        /// 단일 값 조회 (COUNT, MAX 등)
        /// </summary>
        public object ExecuteScalar(string query)
        {
            try
            {
                if (!IsConnected)
                {
                    UnityEngine.Debug.LogError("[DBManager] DB에 연결되어 있지 않습니다.");
                    return null;
                }

                MySqlCommand cmd = new MySqlCommand(query, _connection);
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[DBManager] 쿼리 실행 실패: {ex.Message}");
                return null;
            }
        }
    }
}

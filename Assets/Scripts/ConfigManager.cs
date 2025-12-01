using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace com.luna.world
{
    [Serializable]
    public class DBConfig
    {
        public string host;
        public int port;
        public string user;
        public string password;  // 암호화된 상태로 저장
        public string database;
    }

    /// <summary>
    /// 설정 관련 유틸리티 클래스 (static)
    /// </summary>
    public static class ConfigManager
    {
        // 암호화 키 (실제 운영에서는 더 안전하게 관리)
        private static readonly string ENCRYPTION_KEY = "LunaWorld2024SecretKey32B";  // 32 bytes for AES-256
        private static readonly string CONFIG_FILE = "dbconfig.json";

        /// <summary>
        /// DB 설정 파일 로드
        /// </summary>
        public static DBConfig LoadDBConfig()
        {
            try
            {
                string path = Path.Combine(Application.streamingAssetsPath, CONFIG_FILE);
                
                if (!File.Exists(path))
                {
                    Debug.LogError($"[ConfigManager] 설정 파일이 없습니다: {path}");
                    Debug.LogError("[ConfigManager] dbconfig.template.json을 복사하여 dbconfig.json을 만들어주세요.");
                    return null;
                }

                string json = File.ReadAllText(path);
                DBConfig config = JsonUtility.FromJson<DBConfig>(json);

                // 비밀번호 복호화
                if (!string.IsNullOrEmpty(config.password))
                {
                    config.password = Decrypt(config.password);
                }

                Debug.Log("[ConfigManager] DB 설정 로드 완료");
                return config;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigManager] 설정 로드 실패: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 비밀번호 암호화 (설정 파일 생성 시 사용)
        /// </summary>
        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            byte[] key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY.PadRight(32).Substring(0, 32));
            byte[] iv = new byte[16];  // 초기화 벡터

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 비밀번호 복호화
        /// </summary>
        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                byte[] key = Encoding.UTF8.GetBytes(ENCRYPTION_KEY.PadRight(32).Substring(0, 32));
                byte[] iv = new byte[16];
                byte[] buffer = Convert.FromBase64String(cipherText);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using (MemoryStream ms = new MemoryStream(buffer))
                    {
                        using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(cs))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                // 암호화되지 않은 평문인 경우 그대로 반환
                return cipherText;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 에디터에서 암호화된 비밀번호 생성용 (메뉴에서 사용)
        /// </summary>
        [UnityEditor.MenuItem("Tools/Encrypt Password")]
        public static void EncryptPasswordMenu()
        {
            string password = "your_password_here";  // 여기에 실제 비밀번호 입력
            string encrypted = Encrypt(password);
            Debug.Log($"암호화된 비밀번호: {encrypted}");
            GUIUtility.systemCopyBuffer = encrypted;  // 클립보드에 복사
            Debug.Log("클립보드에 복사되었습니다!");
        }
#endif
    }
}

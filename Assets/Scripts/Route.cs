using UnityEngine;

namespace com.luna.world
{
    /// <summary>
    /// 프로젝트의 유일한 싱글톤 클래스
    /// 모든 주요 시스템과 매니저에 대한 접근점 역할을 합니다.
    /// </summary>
    public class Route : MonoBehaviour
    {
        private static Route _instance;
        
        public static Route Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Route>();
                    
                    if (_instance == null)
                    {
                        GameObject routeObject = new GameObject("Route");
                        _instance = routeObject.AddComponent<Route>();
                    }
                }
                return _instance;
            }
        }

        // 설정
        public DBConfig DBConfig { get; private set; }

        // 매니저들
        public DBManager DB;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            Initialize();
        }

        /// <summary>
        /// 초기화 로직을 여기에 작성합니다.
        /// </summary>
        private void Initialize()
        {
            // TODO: 필요한 초기화 로직 추가
            DB = new DBManager();
            
            // 설정 로드
            DBConfig = ConfigManager.LoadDBConfig();
                
            // DB 초기화
            if (DBConfig != null)
            {
                DB.Initialize(DBConfig);
            }
        }
    }
}

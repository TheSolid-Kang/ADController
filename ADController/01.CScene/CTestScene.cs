using Engine._01.DBMgr;
using Engine._03.CFTPMgr;
using Engine._08.CFileMgr;
using Engine._10.CActiveDirectoryMgr;
using Google.Protobuf.WellKnownTypes;
using Renci.SshNet.Sftp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ADController._01.CScene
{
    internal class CTestScene : CScene
    {
        public CTestScene()
        {

        }
        #region 멤버변수
        #endregion
        #region 디폴트 함수
        public override void Dispose()
        {
            base.Dispose();
        }

        public override void InitFunc()
        {
            base.InitFunc();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void InitPracticeFunc()
        {
            base.InitPracticeFunc();
        }

        public override void Render()
        {
            base.Render();
            _titles = new List<string>();
            _titles.Add("기능 선택");
            _titles.Add("1. Get AD 사용자 정보");
            _titles.Add("2. Get DB-HR 사용자 정보");
            _titles.Add("3. Get DB-NAC 사용자 정보");
            _titles.Add("4. Get IDCenter 사용자 정보");
            _titles.Add("5. Insert yw_TADUsers_IF ");
            _titles.Add("6. ");
            _titles.Add("7. AD사용자동기화조회_yw 화면 개발");
            _titles.Add("8. ");
            _titles.Add("9. ");
            _titles.Add("99.EXIT");
            _titles.ForEach(x => { Console.WriteLine(x); });
        }

        public override int Update(int _event = 0)
        {
            base.Update(_event);
            int result = 0;
            switch (_event)
            {
                case 1:
                    result = Print1();
                    break;
                case 2:
                    result = Print2();
                    break;
                case 3:
                    result = Print3();
                    break;
                case 4:
                    result = Print4();
                    break;
                case 5:
                    result = Print5();
                    break;
                case 6:
                    result = Print6();
                    break;
                case 7:
                    result = Print7();
                    break;
                case 8:
                    result = Print8();
                    break;

                case 99:
                    result = 99;
                    break;

                case -1:
                    Console.WriteLine("프로그램 종료");
                    result = -1;
                    break;
                default:
                    Console.WriteLine("선택지의 번호만 선택해주세요.");
                    this.Render();
                    result = Update();
                    break;
            }
            return result;
        }
        #endregion 
        #region 테스트 케이스 멤버함수
        /// <summary>
        /// 1. Get AD 사용자 정보
        /// </summary>
        /// <returns></returns>
        protected int Print1()
        {
            //string path = "LDAP://10.225.88.70";
            string path = ConfigurationManager.AppSettings["LDAP_URL"].ToString();
            string username = ConfigurationManager.AppSettings["LDAP_ID"].ToString();
            string password = ConfigurationManager.AppSettings["LDAP_PWD"].ToString();
            var ADUsers = CActiveDirectoryMgr.GetInstance().GetADUsers(path, username, password);

            Dictionary<string, List<Users>> mapADUsers = new Dictionary<string, List<Users>>();
            mapADUsers.Add("HR계정", new List<Users>());
            mapADUsers.Add("NAC계정", new List<Users>());
            foreach (var keyPairs in mapADUsers)
            {
                string key = keyPairs.Key;
                List<Users> values = keyPairs.Value;
                ADUsers.ForEach(a =>
                {
                    if (true == a.distinguishedName.Contains(key))
                        values.Add(a);
                });
            }

            /*
            Users user = ADUsers[242];
            Console.WriteLine($"user.strobjectGUID == {user.strobjectGUID}  user.userPrincipalName == {user.userPrincipalName}");
            SortedSet<long> setUsersKey = new SortedSet<long>();
            ADUsers.ForEach(a => setUsersKey.Add(a.uSNCreated));
            */

            return 1;
        }
        /// <summary>
        /// 2. Get DB-HR 사용자 정보
        /// </summary>
        /// <returns></returns>
        protected int Print2()
        {
            string query = GetHRUserQuery();
            using (var mgr = new MSSQL_Mgr())
            {
                DataTable dataTable = mgr.GetDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, query);
                Console.WriteLine("확인");
            }
            return 1;
        }
        /// <summary>
        /// 3. Get DB-NAC 사용자 정보
        /// </summary>
        /// <returns></returns>
        protected int Print3()
        {
            string query = GetHRUserQuery();
            return 1;
        }
        protected int Print4()
        {
            string query = GetIDCnterUserQuery();
            using (var mgr = new MSSQL_Mgr())
            {
                DataTable dataTable = mgr.GetDataTable(DbMgr.DB_CONNECTION.YQMS, query);
                Console.WriteLine("확인");
            }
            return 1;
        }
        /// <summary>
        /// yw_TADUsers_IF 테이블에 AD 사용자 정보 INSERT, UPDATE
        /// </summary>
        /// <returns></returns>
        public int Print5()
        {
            SaveErpAdUsersTbl_IF();
            return 1;
        }
        /// <summary>
        /// yw_TADUsers_IF 테이블 조회
        /// </summary>
        /// <returns></returns>
        public int Print6()
        {

            //사용자 동기화 1번: 사용자 등록-이동-삭제
            //1. 변수 초기화
            Dictionary<string, List<Users>> mapADUsers = GetMapADUsers();

            //2. AD NAC계정 사용자 생성
            //if DB사원명부에 있는 사원이 AD사용자에 없는 경우
            //AD NAC에 계정 생성


            //3. AD HR계정으로 사용자 이동
            //if DB사용자에 있는 사원이 mail이 있는 경우
            //AD HR계정으로 OU이동


            //4. AD 사용자 비활성화처리
            //if AD사용자가 DB사원명부에 없는 경우
            // 해당 사용자 retirement로 OU이동



            return 1;
        }
        /// <summary>
        /// AD사용자동기화조회_yw 화면 개발
        /// </summary>
        /// <returns></returns>
        public int Print7()
        {
            //AD사용자동기화조회_ywq
            //1. 변수 초기화
            //가. 현재 AD정보 가져오기
            Dictionary<string, List<Users>> mapADUsers = GetMapADUsers();
            //나. ERP의 _TADUsers_IF테이블 데이터 가져오기
            List<yw_TADUsers_IF> yw_TADUsers_IFs = GetErpAdUsersTbl_IF();
            //다. ERP의 HR 유저 정보 가져오기
            DataTable dataTable = GetErpHrUsers();
            //라. ERP의 NAC 유저 정보 가져오기

            //2. _TADUsers_IF 테이블에 데이터 INSERT, UPDATE 쿼리 작성

            //3. 


            return 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Print8()
        {



            return 1;
        }
        #endregion

        #region 기능 멤버함수
        private string GetIDCnterUserQuery()
        {
            StringBuilder strBuil = new StringBuilder();
            strBuil.AppendLine("SELECT * FROM VW_VGMP_IDCENTER");
            strBuil.AppendLine("SELECT * FROM _TDAEmp");
            return strBuil.ToString();
        }
        private string GetNACUserQuery()
        {
            StringBuilder strBuil = new StringBuilder();
            //오늘 날짜 기준 재직중인 직원 전체 가져오기
            return strBuil.ToString();
        }
        private string GetHRUserQuery()
        {
            StringBuilder strBuil = new StringBuilder();
            strBuil.AppendLine("SELECT                                                                                                                                                                              ");
            strBuil.AppendLine("A.EmpID--사번                                                                                                                                                                       ");
            strBuil.AppendLine(", A.EmpSeq--사원내부코드                                                                                                                                                            ");
            strBuil.AppendLine(", A.CellPhone--휴대폰번호                                                                                                                                                           ");
            strBuil.AppendLine(", C.PwdMailAdder--이메일                                                                                                                                                            ");
            strBuil.AppendLine(", D.EmpEngFirstName                                                                                                                                                                 ");
            strBuil.AppendLine(", D.EmpEngLastName                                                                                                                                                                  ");
            strBuil.AppendLine(", D.EmpName                                                                                                                                                                         ");
            strBuil.AppendLine(", B.DeptSeq                                                                                                                                                                         ");
            strBuil.AppendLine(", CASE WHEN CHARINDEX('@', C.PwdMailAdder) > 1 THEN SUBSTRING(C.PwdMailAdder, 1, CHARINDEX('@', C.PwdMailAdder) - 1) ELSE '' END PersonId --아이디 nvarchar(20)                     ");
            strBuil.AppendLine("FROM _TDAEmpIn AS A WITH(NOLOCK)                                                                                                                                                    ");
            strBuil.AppendLine("INNER JOIN _THRAdmOrdEmp AS B WITH(NOLOCK) ON A.CompanySeq = B.CompanySeq AND B.IsOrdDateLast = '1' AND A.EmpSeq = B.EmpSeq                                                         ");
            strBuil.AppendLine("                                                                                                                                                                                    ");
            strBuil.AppendLine("    AND B.OrdDate <= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END                        ");
            strBuil.AppendLine("                                                                                                                                                                                    ");
            strBuil.AppendLine("    AND B.OrdEndDate >= CASE WHEN ISNULL(A.RetireDate,'') > CONVERT(VARCHAR(8), GETDATE(), 112) THEN CONVERT(VARCHAR(8), GETDATE(), 112)  ELSE A.RetireDate END                     ");
            strBuil.AppendLine("INNER JOIN _TCAUser AS C WITH(NOLOCK) ON A.CompanySeq = C.CompanySeq AND A.EmpSeq = C.EmpSeq AND C.PwdMailAdder<> ''--PwdMailAdder(이메일)이 NULL이 아니어야 HR 계정이 있는 것임.   ");
            strBuil.AppendLine("INNER JOIN _TDAEmp AS D WITH(NOLOCK) ON A.CompanySeq = D.CompanySeq AND A.EmpSeq = D.EmpSeq                                                                                         ");
            strBuil.AppendLine("WHERE 1 = 1                                                                                                                                                                         ");
            strBuil.AppendLine("                                                                                                                                                                                    ");
            strBuil.AppendLine("    AND C.UserSeq NOT IN(1)--마스터 제외                                                                                                                                            ");
            strBuil.AppendLine("                                                                                                                                                                                    ");
            strBuil.AppendLine("    AND A.RetireDate >= CONVERT(VARCHAR(8), GETDATE(), 112)--현재기준 RetireDate가 없는 사원만 출력                                                                                 ");
            strBuil.AppendLine("ORDER BY A.EmpID ASC                                                                                                                                                                ");
            return strBuil.ToString();
        }
        private Dictionary<string, List<Users>> GetMapADUsers()
        {
            List<string> keyOUs = new List<string>() { "HR계정", "NAC계정" };
            return GetMapADUsers(keyOUs);
        }
        private Dictionary<string, List<Users>> GetMapADUsers(List<string> _keyOUs)
        {
            //1. Config 파일에서 LDAP 등록 정보 가져오기
            string path = ConfigurationManager.AppSettings["LDAP_URL"].ToString();
            string username = ConfigurationManager.AppSettings["LDAP_ID"].ToString();
            string password = ConfigurationManager.AppSettings["LDAP_PWD"].ToString();

            //2. AD에서 AD사용자 조회
            var ADUsers = CActiveDirectoryMgr.GetInstance().GetADUsers(path, username, password);
            Dictionary<string, List<Users>> mapADUsers = new Dictionary<string, List<Users>>();
            _keyOUs.ForEach(a => mapADUsers.Add(a, new List<Users>()));

            //3. Map에 지정 된 OU별로 AD사용자 등록
            foreach (var keyPairs in mapADUsers)
            {
                string key = keyPairs.Key;
                List<Users> values = keyPairs.Value;
                ADUsers.ForEach(a =>
                {
                    if (true == a.distinguishedName.Contains(key))
                        values.Add(a);
                });
            }

            //4. RETURN
            return mapADUsers;
        }
        private void SaveErpAdUsersTbl_IF()
        {
            //1. 변수 초기화
            Dictionary<string, List<Users>> mapADUsers = GetMapADUsers();

            //2. Insert Query
            using (var mgr = new MSSQL_Mgr())
            {
                foreach (var keyPairs in mapADUsers)
                {
                    List<Users> adUsers = keyPairs.Value;
                    adUsers.ForEach(adUser => mgr.InsertData<Users>(DbMgr.DB_CONNECTION.YWDEV, adUser));
                }
            }

            //3. Update Query 작성
        }
        private List<yw_TADUsers_IF> GetErpAdUsersTbl_IF()
        {
            List<yw_TADUsers_IF> yw_TADUsers_IFs = new List<yw_TADUsers_IF>();
            using (var mgr = new MSSQL_Mgr())
            {
                string query = "SELECT * FROM yw_TADUsers_IF";
                DataTable dataTable = mgr.GetDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, query);
                yw_TADUsers_IFs = new List<yw_TADUsers_IF>(dataTable.Rows.Count);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    yw_TADUsers_IF? obj = System.Activator.CreateInstance<yw_TADUsers_IF>();
                    foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (!object.Equals(dataRow[prop.Name], System.DBNull.Value))
                        {
                            prop.SetValue(obj, dataRow[prop.Name], null);
                        }
                    }
                    yw_TADUsers_IFs.Add(obj);
                }
            }
            return yw_TADUsers_IFs;
        }
        private DataTable GetErpHrUsers()
        {
            DataTable dataTable = new DataTable();
            using (var mgr = new MSSQL_Mgr())
            {
                string query = GetHRUserQuery();
                dataTable = mgr.GetDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, query);
            }
            return dataTable;
        }
        private DataTable GetErpNacUsers()
        {
            DataTable dataTable = new DataTable();

            return dataTable;
        }
        #endregion
    }
}

using Engine._01.DBMgr;
using Engine._03.CFTPMgr;
using Engine._08.CFileMgr;
using Engine._10.CActiveDirectoryMgr;
using Google.Protobuf.WellKnownTypes;
using Renci.SshNet.Sftp;
using System;
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
            _titles.Add("5. ");
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
        #region 내장 멤버함수
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
            Users user = ADUsers[242];
            Console.WriteLine($"user.strobjectGUID == {user.strobjectGUID}  user.userPrincipalName == {user.userPrincipalName}");
            SortedSet<long> setUsersKey = new SortedSet<long>();
            ADUsers.ForEach(a => setUsersKey.Add(a.uSNCreated));
            return 1;
        }
        /// <summary>
        /// 2. Get DB-HR 사용자 정보
        /// </summary>
        /// <returns></returns>
        protected int Print2()
        {
            string query = GetHRUserQuery();
            using(var mgr = new MSSQL_Mgr())
            {
                DataTable dataTable = mgr.GetDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, query);
                Console.WriteLine("확인");
            }
            return 1;
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

        /// <summary>
        /// 3. Get DB-NAC 사용자 정보
        /// </summary>
        /// <returns></returns>
        protected int Print3()
        {
            string query = GetHRUserQuery();
            return 1;
        }

        private string GetNACUserQuery()
        {
            StringBuilder strBuil = new StringBuilder();

            return strBuil.ToString();
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

        private string GetIDCnterUserQuery()
        {
            StringBuilder strBuil = new StringBuilder();
            strBuil.AppendLine("SELECT * FROM VW_VGMP_IDCENTER");
            strBuil.AppendLine("SELECT * FROM _TDAEmp");
            return strBuil.ToString();
        }



        #endregion
    }
}

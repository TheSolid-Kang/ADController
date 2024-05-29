using Engine._01.DBMgr;
using Engine._03.CFTPMgr;
using Engine._08.CFileMgr;
using Engine._10.CActiveDirectoryMgr;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using Renci.SshNet.Sftp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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
            _titles.Add("2. AD정보 ERP에 INSERT, UPDATE, DELETE");
            _titles.Add("3. ");
            _titles.Add("4. Get IDCenter 사용자 정보");
            _titles.Add("5. Insert yw_TADUsers_IF ");
            _titles.Add("6. ");
            _titles.Add("7. 취소_AD사용자동기화조회_yw 화면 개발");
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

            Dictionary<string, List<ADUser>> mapADUsers = new Dictionary<string, List<ADUser>>();
            mapADUsers.Add("HR계정", new List<ADUser>());
            mapADUsers.Add("NAC계정", new List<ADUser>());
            foreach (var keyPairs in mapADUsers)
            {
                string key = keyPairs.Key;
                List<ADUser> values = keyPairs.Value;
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
            //1. AD 사용자 컨테이너, ERP 사용자 컨테이너 생성
            //  가. 현재 AD 사용자 목록 컨테이너 생성 
            List<ADUser> adUsers = GetADUsers();
            List<yw_TADUsers_IF> adYw_TADUsers_IFs = new(adUsers.Count);
            adUsers.ForEach(adUser => adYw_TADUsers_IFs.Add(new(adUser)));
            adYw_TADUsers_IFs.RemoveAll(yw_TADUsers_IF => yw_TADUsers_IF.OU.Equals(""));
            adYw_TADUsers_IFs.RemoveAll(yw_TADUsers_IF => yw_TADUsers_IF.OU.Equals("Retirement"));

            //  나. 현재 ERP의 yw_TADUsers_IF의 AD 사용자 목록 컨테이너 생성
            List<yw_TADUsers_IF> erpYw_TADUsers_IFs = new();
            using (var mgr = new MSSQL_Mgr())
            {
                DataTable dataTable = mgr.GetDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, "SELECT * FROM yw_TADUsers_IF");
                erpYw_TADUsers_IFs = ConvertDataTableToList<yw_TADUsers_IF>(dataTable);
            }

            //2. _TADUsers_IF 테이블에 데이터 INSERT, DELETE, UPDATE 쿼리 작성
            //  가. INSERT: AD에 있으나 ERP(yw_TADUsers_IF 테이블)에 없는 경우 -> ERP(yw_TADUsers_IF 테이블)에 AD 사용자 정보 등록
            {
                List<yw_TADUsers_IF> insertYw_TADUsers_IFs = adYw_TADUsers_IFs.ToList();
                erpYw_TADUsers_IFs.ForEach(AdNacUser =>
                {
                    var yw_TADUsers_IF1 = insertYw_TADUsers_IFs.Find(ErpUser => ErpUser.uSNCreated == AdNacUser.uSNCreated);
                    if (null != yw_TADUsers_IF1)
                        insertYw_TADUsers_IFs.Remove(yw_TADUsers_IF1);
                });
                using (var mgr = new MSSQL_Mgr())
                    insertYw_TADUsers_IFs.ForEach(insertYw_TADUsers_IF => mgr.InsertData<yw_TADUsers_IF>(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, insertYw_TADUsers_IF));
            }
            //  나. DELETE: ERP(yw_TADUsers_IF 테이블)에 있으나 AD에 없는 경우 -> ERP(yw_TADUsers_IF 테이블)에서 해당 데이터 삭제
            {
                List<yw_TADUsers_IF> deleteYw_TADUsers_IFs = erpYw_TADUsers_IFs.ToList();
                adYw_TADUsers_IFs.ForEach(yw_TADUsers_IF => deleteYw_TADUsers_IFs.RemoveAll(deleteYw_TADUsers_IF => deleteYw_TADUsers_IF.uSNCreated == yw_TADUsers_IF.uSNCreated));
                    
                using (var mgr = new MSSQL_Mgr())
                    deleteYw_TADUsers_IFs.ForEach(deleteYw_TADUsers_IF => mgr.DeleteDataByKey<yw_TADUsers_IF>(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, deleteYw_TADUsers_IF.sAMAccountName));
            }
            //  다. UPDATE: ERP(yw_TADUsers_IF 테이블)의 ROW가 AD사용자 정보와 다른 경우 -> 최신 AD사용자 정보로 ERP(yw_TADUsers_IF 테이블) 업데이트
            {
                //1) 현재 AD 사용자 목록 컨테이너 재생성
                adUsers = GetADUsers();
                adYw_TADUsers_IFs = new(adUsers.Count);
                adUsers.ForEach(adUser => adYw_TADUsers_IFs.Add(new(adUser)));
                adYw_TADUsers_IFs.RemoveAll(yw_TADUsers_IF => yw_TADUsers_IF.OU.Equals(""));
                adYw_TADUsers_IFs.RemoveAll(yw_TADUsers_IF => yw_TADUsers_IF.OU.Equals("Retirement"));

                //2) 현재 ERP의 yw_TADUsers_IF의 AD 사용자 목록 컨테이너 재생성
                erpYw_TADUsers_IFs = new();
                using (var mgr = new MSSQL_Mgr())
                {
                    DataTable dataTable = mgr.GetDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, "SELECT * FROM yw_TADUsers_IF");
                    erpYw_TADUsers_IFs = ConvertDataTableToList<yw_TADUsers_IF>(dataTable);
                }

                //3) 최신 AD사용자 정보로 ERP의 yw_TADUsers_IF 업데이트
                List<yw_TADUsers_IF> updateYw_TADUsers_IFs = new();
                adYw_TADUsers_IFs.Sort((a1, a2) => a1.uSNCreated.CompareTo(a2.uSNCreated));
                erpYw_TADUsers_IFs.Sort((a1, a2) => a1.uSNCreated.CompareTo(a2.uSNCreated));

                ADUserComparer aDUserComparer = new ADUserComparer();
                
                for(int i = 0; i < adYw_TADUsers_IFs.Count; ++i)
                {
                    if(adYw_TADUsers_IFs[i].uSNCreated == erpYw_TADUsers_IFs[i].uSNCreated)
                        if (false == aDUserComparer.Equals(adYw_TADUsers_IFs[i], erpYw_TADUsers_IFs[i]))
                            updateYw_TADUsers_IFs.Add(adYw_TADUsers_IFs[i]);
                }
                using (var mgr = new MSSQL_Mgr())
                    updateYw_TADUsers_IFs.ForEach(updateYw_TADUsers_IF => mgr.UpdateData<yw_TADUsers_IF>(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, updateYw_TADUsers_IF, "uSNCreated"));
            }


            return 1;
        }
        /// <summary>
        /// 3. Get DB-NAC 사용자 정보
        /// </summary>
        /// <returns></returns>
        protected int Print3()
        {
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
            Dictionary<string, List<ADUser>> mapADUsers = GetMapADUsers();

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
            //AD사용자동기화조회_yw
            //1. 변수 초기화
            //가. ERP의 yw_TADUsers_IF테이블 데이터 가져오기
            List<yw_TADUsers_IF> list_yw_TADUsers_IF = GetErpAdUsersTbl_IF();
            DataTable yw_TADUsers_IF = ToDataTable<yw_TADUsers_IF>(list_yw_TADUsers_IF);
            //나. 현재 AD정보 가져오기
            Dictionary<string, List<ADUser>> mapADUsers = GetMapADUsers();
            //  1) AD의 NAC계정 유저 정보 가져오기
            DataTable adNacUsers = ToDataTable<ADUser>(mapADUsers["NAC계정"]);
            //  2) AD의 HR계정 유저 정보 가져오기
            DataTable adHrUsers = ToDataTable<ADUser>(mapADUsers["HR계정"]);
            //다. ERP의 NAC 유저 정보 가져오기
            DataTable erpNacUsers = GetErpNacUsers();
            //라. ERP의 HR 유저 정보 가져오기
            DataTable erpHrUsers = GetErpHrUsers();

            //2. _TADUsers_IF 테이블에 데이터 INSERT, DELETE, UPDATE 쿼리 작성
            //  가. INSERT 
            DataTable INSERT_yw_TADUsers_IF = yw_TADUsers_IF.Clone();
            INSERT_yw_TADUsers_IF.Rows.Clear();
            //      1) AD-NAC 등록   : ERP의 NAC에 등록 된 유저가 AD의 NAC에 없는 경우
            //      -> AD에 없는 ERP 사용자를 AD-NAC에 등록해야함.
            List<yw_TADUsers_IF> listErpNacUsers = ConvertDataTableToList<yw_TADUsers_IF>(erpNacUsers);
            List<yw_TADUsers_IF> listAdNacUsers = ConvertDataTableToList<yw_TADUsers_IF>(adNacUsers);
            listAdNacUsers.ForEach(AdNacUser =>
            {
                var yw_TADUsers_IF1 = listErpNacUsers.Find(ErpUser => ErpUser.sAMAccountName == AdNacUser.sAMAccountName);
                if (yw_TADUsers_IF1 != null)
                    listErpNacUsers.Remove(yw_TADUsers_IF1);
            });

            //      2) AD-HR 등록    : ERP의 HR에 등록 된 유저가 AD의 HR에 없는 경우
            //      -> AD에 없는 ERP 사용자를 AD-HR에 등록해야함.
            List<yw_TADUsers_IF> listErpHrUsers = ConvertDataTableToList<yw_TADUsers_IF>(erpHrUsers);
            List<yw_TADUsers_IF> listAdHrUsers = ConvertDataTableToList<yw_TADUsers_IF>(adHrUsers);
            listAdHrUsers.ForEach(AdHrUser =>
            {
                var yw_TADUsers_IF1 = listErpHrUsers.Find(ErpUser => ErpUser.sAMAccountName == AdHrUser.sAMAccountName);
                if (yw_TADUsers_IF1 != null)
                    listErpHrUsers.Remove(yw_TADUsers_IF1);
            });

            //Insert
            using (var mgr = new MSSQL_Mgr())
            {
                listErpNacUsers.ForEach(data => { data.condition = "AD NAC계정"; mgr.InsertData<yw_TADUsers_IF>(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, data); });
                listErpHrUsers.ForEach(data => { data.condition = "AD HR계정"; mgr.InsertData<yw_TADUsers_IF>(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, data); });
            }

            //  나. DELETE 
            DataTable DELETE_yw_TADUsers_IF = yw_TADUsers_IF.Clone();
            DELETE_yw_TADUsers_IF.Rows.Clear();
            listErpNacUsers = ConvertDataTableToList<yw_TADUsers_IF>(erpNacUsers);
            listErpHrUsers = ConvertDataTableToList<yw_TADUsers_IF>(erpHrUsers);

            //      3) AD-NAC 비활성 : ERP의 NAC에 없는 유저가 AD의 NAC에 있는 경우
            //      -> ERP에 없는 AD사용자를 AD-NAC에서 비활성화 처리해야함.

            //      4) AD-HR 비활성 : ERP의 HR에 없는 유저가 AD의 HR에 있는 경우
            //      -> ERP에 없는 AD사용자를 AD-HR에서 비활성화 처리해야함.

            //ADView만 고치면 됨...
            //using (var mgr = new MSSQL_Mgr())
            //{
            //    listErpNacUsers.ForEach(data => { mgr.DeleteDataByKey<yw_TADUsers_IF>(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, data.sAMAccountName); });
            //    listErpHrUsers.ForEach(data => { mgr.DeleteDataByKey<yw_TADUsers_IF>(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, data.sAMAccountName); });
            //}

            //  다. UPDATE
            DataTable UPDATE_yw_TADUsers_IF = yw_TADUsers_IF.Clone();
            UPDATE_yw_TADUsers_IF.Rows.Clear();
            //      1) AD-NAC 수정   : ERP의 NAC에 등록 된 유저가 AD의 NAC에 등록 된 유저의 정보와 다를 경우
            //      -> ERP의 yw_TADUsers_IF의 데이터도 변경해야함.
            //      2) AD-HR  수정   : ERP의 HR에 등록 된 유저가 AD의 HR에 등록 된 유저의 정보와 다를 경우
            //      -> ERP의 yw_TADUsers_IF의 데이터도 변경해야함.

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
            return strBuil.ToString();
        }
        public List<ADUser> GetADUsers()
        {
            //1. Config 파일에서 LDAP 등록 정보 가져오기
            string path = ConfigurationManager.AppSettings["LDAP_URL"].ToString();
            string username = ConfigurationManager.AppSettings["LDAP_ID"].ToString();
            string password = ConfigurationManager.AppSettings["LDAP_PWD"].ToString();
            //2. AD에서 AD사용자 조회
            var ADUsers = CActiveDirectoryMgr.GetInstance().GetADUsers(path, username, password);
            return ADUsers;
        }
        private Dictionary<string, List<ADUser>> GetMapADUsers()
        {
            List<string> keyOUs = new List<string>() { "HR계정", "NAC계정", "Retirement" };
            return GetMapADUsers(keyOUs);
        }
        private Dictionary<string, List<ADUser>> GetMapADUsers(List<string> _keyOUs)
        {
            //1. Config 파일에서 LDAP 등록 정보 가져오기
            string path = ConfigurationManager.AppSettings["LDAP_URL"].ToString();
            string username = ConfigurationManager.AppSettings["LDAP_ID"].ToString();
            string password = ConfigurationManager.AppSettings["LDAP_PWD"].ToString();

            //var dataTable = CActiveDirectoryMgr.GetInstance().GetDataTable<Users>(path, username, password);

            //2. AD에서 AD사용자 조회
            var ADUsers = CActiveDirectoryMgr.GetInstance().GetADUsers(path, username, password);
            Dictionary<string, List<ADUser>> mapADUsers = new Dictionary<string, List<ADUser>>();
            _keyOUs.ForEach(a => mapADUsers.Add(a, new List<ADUser>()));

            //3. Map에 지정 된 OU별로 AD사용자 등록
            foreach (var keyPairs in mapADUsers)
            {
                string key = keyPairs.Key;
                List<ADUser> values = keyPairs.Value;
                ADUsers.ForEach(a =>
                {
                    if (true == a.distinguishedName.Contains(key))
                        values.Add(a);
                });
            }

            //4. RETURN
            return mapADUsers;
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
                SqlParameter[] sqlParameters = new[] { new SqlParameter { ParameterName = "@OU", Direction = ParameterDirection.Input, Value = "HR계정" } };
                dataTable = mgr.GetSPDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, "_SADUserErpInfoQuery", sqlParameters);
            };
            return dataTable;
        }
        private DataTable GetErpNacUsers()
        {
            DataTable dataTable = new DataTable();
            using (var mgr = new MSSQL_Mgr())
            {
                SqlParameter[] sqlParameters = new[] { new SqlParameter { ParameterName = "@OU", Direction = ParameterDirection.Input, Value = "NAC계정" } };
                dataTable = mgr.GetSPDataTable(ConfigurationManager.ConnectionStrings["YWDEV"].ConnectionString, "_SADUserErpInfoQuery", sqlParameters);
            };

            return dataTable;
        }
        private void SaveErpAdUsersTbl_IF()
        {
            //1. 변수 초기화
            Dictionary<string, List<ADUser>> mapADUsers = GetMapADUsers();

            //2. Insert Query
            using (var mgr = new MSSQL_Mgr())
            {
                foreach (var keyPairs in mapADUsers)
                {
                    List<ADUser> adUsers = keyPairs.Value;
                    List<yw_TADUsers_IF> adYw_TADUsers_IFs = new(adUsers.Count);
                    adUsers.ForEach(adUser => adYw_TADUsers_IFs.Add(new(adUser)));

                    adYw_TADUsers_IFs.ForEach(adUser => mgr.InsertDataByTableName<yw_TADUsers_IF>(DbMgr.DB_CONNECTION.YWDEV, adUser, "yw_TADUsers_IF"));
                }
            }

            //3. Update Query 작성
        }
        public DataTable ToDataTable<T>(List<T>? items)
        {
            var tb = new DataTable(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in props)
                tb.Columns.Add(prop.Name, prop.PropertyType);
            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                    values[i] = props[i].GetValue(item, null);
                tb.Rows.Add(values);
            }
            return tb;
        }

        public List<T> ConvertDataTableToList<T>(DataTable _dataTable)
        {
            List<T> list = new List<T>(_dataTable.Rows.Count);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            try
            {

                foreach (DataRow dataRow in _dataTable.Rows)
                {
                    T obj = System.Activator.CreateInstance<T>();
                    foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        if (false == dataRow.Table.Columns.Contains(prop.Name))
                            continue;
                        if (!object.Equals(dataRow[prop.Name], System.DBNull.Value))
                        {
                            prop.SetValue(obj, dataRow[prop.Name], null);
                        }
                    }
                    list.Add(obj);
                }
            }
            catch (Exception _e)
            {
                System.Diagnostics.Debug.WriteLine(_e.Message);
            }
            return list;
        }
        #endregion
    }
}
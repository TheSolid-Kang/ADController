using ADController._99._Default;
using Engine._01.DBMgr;
using Engine._10.CActiveDirectoryMgr;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADController._01.CSyncModule
{
    internal class CADUserSyncModule : CSyncModule
    {
        public CADUserSyncModule() { }
        #region 멤버변수
        #endregion
        #region 디폴트 함수
        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Initialize()
        {
            base.Initialize();
            _titles = new List<string>();
            _titles.Add("기능 실행: AD 사용자 정보 동기화");
            _titles.Add("1. AD정보 ERP에 INSERT, UPDATE, DELETE(UPDATE)");
            _titles.Add("2. ");
            _titles.Add("3. ");
        }

        public override void Render()
        {
            base.Render();
            _titles.ForEach(x => { Console.WriteLine(x); });
        }

        public override int Update(int _event = 0)
        {
            int result = 0;
            UpdateYw_TADUsers_IF();
            return result;
        }
        #endregion
        #region 기능 멤버함수
        /// <summary>
        /// 2. AD정보 ERP에 INSERT, UPDATE, DELETE(UPDATE)
        /// 가. INSERT: AD에 있으나 ERP(yw_TADUsers_IF 테이블)에 없는 경우 -> ERP(yw_TADUsers_IF 테이블)에 AD 사용자 정보 등록
        ///  1) AD컨테이너, ERP컨테이너 비교
        ///  2) AD컨테이너에서 ERP컨테이너에 있는 사용자 삭제
        ///  3) INSERT: AD컨테이너 -> ERP(yw_TADUsers_IF 테이블)
        /// 나.DELETE(UPDATE): ERP(yw_TADUsers_IF 테이블)에 있으나 AD에 없는 경우 -> ERP(yw_TADUsers_IF 테이블)에서 해당 데이터 ERP(yw_TADUsers_IF 테이블)에서 해당 데이터의 isUse 컬럼 값 '0'으로 변경
        ///  1) AD컨테이너, ERP컨테이너 비교
        ///  2) ERP컨테이너에서 AD컨테이너에 있는 사용자 삭제
        ///  3) DELETE(UPDATE): ERP(yw_TADUsers_IF 테이블)에서 ERP컨테이너에서 해당 데이터의 isUse 컬럼 값 '0'으로 변경
        /// 다.UPDATE: ERP(yw_TADUsers_IF 테이블) 의 ROW가 AD사용자 정보와 다른 경우 -> 최신 AD사용자 정보로 ERP(yw_TADUsers_IF 테이블) 업데이트
        ///  1) AD컨테이너, ERP컨테이너 재생성
        ///  2) AD컨테이너, ERP컨테이너 uSNCreated로 ASC 정렬
        ///  3) AD컨테이너, ERP컨테이너 비교
        ///  4) UPDATE컨테이너 생성
        ///  5) AD컨테이너 중 ERP컨테이너와 다른 행이 있는 경우 UPDATE컨테이너에 ADD
        ///  6) UPDATE: UPDATE컨테이너로 ERP(yw_TADUsers_IF 테이블) 업데이트
        /// </summary>
        /// <returns></returns>
        private int UpdateYw_TADUsers_IF()
        {
            //1. AD 사용자 컨테이너, ERP 사용자 컨테이너 생성
            //  가. Config 파일에서 LDAP 등록 정보 가져오기
            string path = AppConfig.LDAP_URL;
            string username = AppConfig.LDAP_ID;
            string password = AppConfig.LDAP_PWD;

            //  나. 현재 AD 사용자 목록 컨테이너 생성 
            List<ADUser> adUsers = CActiveDirectoryMgr.GetInstance().GetADUsers(path, username, password);
            List<yw_TADUsers_IF> adYw_TADUsers_IFs = new(adUsers.Count);
            adUsers.ForEach(adUser => adYw_TADUsers_IFs.Add(new(adUser)));

            //  다. 현재 ERP의 yw_TADUsers_IF의 AD 사용자 목록 컨테이너 생성
            List<yw_TADUsers_IF> erpYw_TADUsers_IFs = new();
            using (var mgr = new MSSQL_Mgr())
            {
                DataTable dataTable = mgr.GetDataTable(AppConfig.DB_URL, "SELECT * FROM yw_TADUsers_IF");
                erpYw_TADUsers_IFs = DataTableMgr.ConvertDataTableToList<yw_TADUsers_IF>(dataTable);
                dataTable.Dispose();
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
                    insertYw_TADUsers_IFs.ForEach(insertYw_TADUsers_IF => mgr.InsertData<yw_TADUsers_IF>(AppConfig.DB_URL, insertYw_TADUsers_IF));
            }
            //  나. DELETE(UPDATE): ERP(yw_TADUsers_IF 테이블)에 있으나 AD에 없는 경우 -> ERP(yw_TADUsers_IF 테이블)에서 해당 데이터의 isUse 컬럼 값 '0'으로 변경
            {
                List<yw_TADUsers_IF> deleteYw_TADUsers_IFs = erpYw_TADUsers_IFs.ToList();
                adYw_TADUsers_IFs.ForEach(yw_TADUsers_IF => deleteYw_TADUsers_IFs.RemoveAll(deleteYw_TADUsers_IF => deleteYw_TADUsers_IF.uSNCreated == yw_TADUsers_IF.uSNCreated));
                deleteYw_TADUsers_IFs.ForEach(deleteYw_TADUsers_IF => deleteYw_TADUsers_IF.isDeleted = "1");
                using (var mgr = new MSSQL_Mgr())
                    deleteYw_TADUsers_IFs.ForEach(updateYw_TADUsers_IF => mgr.UpdateData<yw_TADUsers_IF>(AppConfig.DB_URL, updateYw_TADUsers_IF, "uSNCreated"));
            }
            //  다. UPDATE: ERP(yw_TADUsers_IF 테이블)의 ROW가 AD사용자 정보와 다른 경우 -> 최신 AD사용자 정보로 ERP(yw_TADUsers_IF 테이블) 업데이트
            {
                //1) 현재 AD 사용자 목록 컨테이너 재생성
                adUsers = CActiveDirectoryMgr.GetInstance().GetADUsers(path, username, password);
                adYw_TADUsers_IFs = new(adUsers.Count);
                adUsers.ForEach(adUser => adYw_TADUsers_IFs.Add(new(adUser)));

                //2) 현재 ERP의 yw_TADUsers_IF의 AD 사용자 목록 컨테이너 재생성
                erpYw_TADUsers_IFs = new();
                using (var mgr = new MSSQL_Mgr())
                {
                    DataTable dataTable = mgr.GetDataTable(AppConfig.DB_URL, "SELECT * FROM yw_TADUsers_IF");
                    erpYw_TADUsers_IFs = DataTableMgr.ConvertDataTableToList<yw_TADUsers_IF>(dataTable);
                    dataTable.Dispose();
                }

                //3) 최신 AD사용자 정보로 ERP의 yw_TADUsers_IF 업데이트
                List<yw_TADUsers_IF> updateYw_TADUsers_IFs = new();
                adYw_TADUsers_IFs.Sort((a1, a2) => a1.uSNCreated.CompareTo(a2.uSNCreated));
                erpYw_TADUsers_IFs.Sort((a1, a2) => a1.uSNCreated.CompareTo(a2.uSNCreated));

                yw_TADUsers_IFComparer aDUserComparer = new yw_TADUsers_IFComparer();
                for (int i = 0; i < adYw_TADUsers_IFs.Count; ++i)
                {
                    if (adYw_TADUsers_IFs[i].uSNCreated == erpYw_TADUsers_IFs[i].uSNCreated)
                        if (false == aDUserComparer.Equals(adYw_TADUsers_IFs[i], erpYw_TADUsers_IFs[i]))
                            updateYw_TADUsers_IFs.Add(adYw_TADUsers_IFs[i]);
                }
                using (var mgr = new MSSQL_Mgr())
                    updateYw_TADUsers_IFs.ForEach(updateYw_TADUsers_IF => mgr.UpdateData<yw_TADUsers_IF>(AppConfig.DB_URL, updateYw_TADUsers_IF, "uSNCreated"));
            }

            return 1;
        }




        #endregion
    }
}

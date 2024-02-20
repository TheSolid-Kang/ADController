using Engine._01.DBMgr;
using Engine._03.CFTPMgr;
using Engine._08.CFileMgr;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
            _titles.Add("1. 그룹웨어 첨부파일 중 2023년 파일 전체 다운로드 ");
            _titles.Add("2. 미완성_그룹웨어 첨부파일 중 쿼리에 맞는 파일만 다운로드 ");
            _titles.Add("3. ");
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
        /// 1. 
        /// </summary>
        /// <returns></returns>
        protected int Print1()
        {
            

            return 1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int Print2()
        {
            
            return 1;
        }

        protected int Print3()
        {

            return 1;
        }

        #endregion
    }
}

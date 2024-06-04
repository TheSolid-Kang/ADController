using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADController._01.CSyncModule
{
    internal interface ISyncModule : IDisposable
    {
        protected void Initialize();
        protected void Render();
        protected int Update(int _event = 0);
    }

    internal class CSyncModule : ISyncModule
    {
        public CSyncModule() { }


        #region 멤버변수
        protected List<string> _titles = new List<string>();
        #endregion

        #region 디폴트 함수
        public virtual void Dispose()
        {
            //throw new NotImplementedException();
        }

        public virtual void Initialize()
        {
            //throw new NotImplementedException();
        }

        public virtual void Render()
        {
            //throw new NotImplementedException();
        }

        public virtual int Update(int _event = 0)
        {
            //throw new NotImplementedException();
            return 0;
        }
        #endregion

        #region 내장 멤버함수
        public int Excute(int _num = 0)
        {
            Initialize();
            Render();
            int result = Update();


            return result;
        }

        #endregion
    }
}

using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADController._01.CScene
{
    internal interface IScene : IDisposable
    {
        protected void Initialize();
        protected void InitFunc();
        protected void InitPracticeFunc();
        protected void Render();
        protected int Update(int _event = 0);
    }

    internal class CScene : IScene
    {
        public CScene() { }


        #region 멤버변수
        protected List<string> _titles = new List<string>();
        #endregion

        #region 디폴트 함수
        public virtual void Dispose()
        {
            //throw new NotImplementedException();
        }
        public virtual void InitFunc()
        {
            //throw new NotImplementedException();
        }

        public virtual void Initialize()
        {
            //throw new NotImplementedException();
        }

        public virtual void InitPracticeFunc()
        {
            //throw new NotImplementedException();
        }

        public virtual void Render()
        {
            Console.WriteLine("아무 키나 눌러주세요.");
            Console.ReadKey();
            Console.Clear();
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
            this.Render();

            Initialize();
            Render();
            int num = CIO.AskAndReturnInteger();
            int result = Update(num);

            return result;
        }

        #endregion
    }
}

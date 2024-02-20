using ADController._01.CScene;
using Engine._98.Headers;
using ADController._02.CObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADController._97.DesignPattern
{
    internal class CSceneMgr : GENERIC_MGR<CSceneMgr>
    {
        private CScene _scene = new CTestScene();

        public void SetScene(int _sceneNum)
        {
            if (_scene != null)
                _scene.Dispose();
            switch (_sceneNum)
            {
                case 1:
                    _scene = new CTestScene();
                    break;

            }

        }
        public CScene GetScene => _scene;
        public int Excute(int _num = 0)
        {
            GetScene.Excute(_num);
            return 0;
        }
    }
}

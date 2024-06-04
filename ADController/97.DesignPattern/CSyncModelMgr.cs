using ADController._01.CSyncModule;
using Engine._98.Headers;
using ADController._02.CObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADController._97.DesignPattern
{
    internal class CSyncModuleMgr : GENERIC_MGR<CSyncModuleMgr>
    {
        private CSyncModule _SyncModule = new CTestSyncModule();

        public enum SYNC_MODEL
        {
            TEST = 0
                , AD_USER
                , END
        }

        private void SetSyncModule(int _SyncModuleNum)
        {
            if (_SyncModule != null)
                _SyncModule.Dispose();
            SYNC_MODEL sYNC_MODEL = (SYNC_MODEL)_SyncModuleNum;

            switch (sYNC_MODEL)
            {
                case SYNC_MODEL.TEST:
                    _SyncModule = new CTestSyncModule();
                    break;
                case SYNC_MODEL.AD_USER:
                    _SyncModule = new CADUserSyncModule();
                    break;
            }
        }
        private CSyncModule SyncModule => _SyncModule;
        public int Excute(int _num = 0)
        {
            SetSyncModule(_num);

            SyncModule.Excute(_num);
            return 0;
        }
    }
}

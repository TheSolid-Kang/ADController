using ADController._01.CSyncModel;
using Engine._98.Headers;
using ADController._02.CObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADController._97.DesignPattern
{
    internal class CSyncModelMgr : GENERIC_MGR<CSyncModelMgr>
    {
        private CSyncModel _SyncModel = new CTestSyncModel();

        public enum SYNC_MODEL
        {
            TEST = 0
                , AD_USER
                , END
        }

        public void SetSyncModel(int _SyncModelNum)
        {
            if (_SyncModel != null)
                _SyncModel.Dispose();
            SYNC_MODEL sYNC_MODEL = (SYNC_MODEL)_SyncModelNum;

            switch (sYNC_MODEL)
            {
                case SYNC_MODEL.TEST:
                    _SyncModel = new CTestSyncModel();
                    break;
                case SYNC_MODEL.AD_USER:
                    _SyncModel = new CADUserSyncModel();
                    break;
            }
        }
        private CSyncModel SyncModel => _SyncModel;
        public int Excute(int _num = 0)
        {
            SetSyncModel(_num);

            SyncModel.Excute(_num);
            return 0;
        }
    }
}

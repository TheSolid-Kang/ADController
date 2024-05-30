using Engine._10.CActiveDirectoryMgr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ADController
{
    internal class yw_TADUsers_IF : ADUser
    {
        public yw_TADUsers_IF() { }
        public yw_TADUsers_IF(ADUser _adUser)
        {
            this.cn = _adUser.cn;
            this.description = _adUser.description;
            this.department = _adUser.department;
            this.displayName = _adUser.displayName;
            this.distinguishedName = _adUser.distinguishedName;
            this.givenName = _adUser.givenName;
            this.homePhone = _adUser.homePhone;
            this.lastLogon = _adUser.lastLogon;
            this.otherHomePhone = _adUser.otherHomePhone;
            this.otherMobile = _adUser.otherMobile;
            this.pwdLastSet = _adUser.pwdLastSet;
            this.sn = _adUser.sn;
            this.title = _adUser.title;
            this.mail = _adUser.mail;
            this.manager = _adUser.manager;
            this.mobile = _adUser.mobile;
            this.sAMAccountName = _adUser.sAMAccountName;
            this.userPrincipalName = _adUser.userPrincipalName;
            this.uSNChanged = _adUser.uSNChanged;
            this.uSNCreated = _adUser.uSNCreated;
            this.whenChanged = _adUser.whenChanged;
            this.whenCreated = _adUser.whenCreated;
        }

        public string OU
        {
            get
            {
                if (true == string.IsNullOrEmpty(this.distinguishedName))
                    return "";
                var str = this.distinguishedName;
                int fromIdx = str.IndexOf(",OU=");
                if (fromIdx == -1)
                    return "";
                str = str.Substring(fromIdx + 1);//문자열에서 ",OU="를 제외: ,OU=HR계정... -> OU=HR계정...
                return str;
            }
            set { }
        }
        public string isDeleted { get; set; } = "0";
        public DateTime lastSyncDateTime { get; set; } = DateTime.Now;
    }

    public class yw_TADUsers_IFComparer : ADUserComparer
    {
        public bool Equals(ADUser x, ADUser y)
        {
            ADUserComparer aDUserComparer = new ADUserComparer();
            bool isTrue = aDUserComparer.Equals(x, y);

            yw_TADUsers_IF xx = (yw_TADUsers_IF)x;
            yw_TADUsers_IF yy = (yw_TADUsers_IF)y;

            return isTrue
                    && xx.OU == yy.OU;
        }
    }
}
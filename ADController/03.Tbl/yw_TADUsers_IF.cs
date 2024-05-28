using Engine._10.CActiveDirectoryMgr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
            this.sAMAccountName = _adUser.sAMAccountName;
            this.userPrincipalName = _adUser.userPrincipalName;
            this.uSNChanged = _adUser.uSNChanged;
            this.uSNCreated = _adUser.uSNCreated;
            this.whenChanged = _adUser.whenChanged;
            this.whenCreated = _adUser.whenCreated;
        }

        public string condition { get; set; } = "";
        public string OU
        {
            get
            {
                if (true == string.IsNullOrEmpty(this.distinguishedName))
                    return "";
                var str = this.distinguishedName;
                int fromIdx = str.IndexOf("OU=");
                if (fromIdx == -1)
                    return "";
                str = str.Substring(fromIdx + 3, str.Length - fromIdx - 3);//문자열에서 "OU="를 제외: OU=HR계정 -> HR계정
                int toIdx = str.IndexOf(",");
                str = str.Substring(0, toIdx);
                return str;
            }
            set { }
        }
        
    }
}

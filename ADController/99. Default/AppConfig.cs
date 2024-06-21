using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADController._99._Default
{
    public class AppConfig
    {
        public static string LDAP_URL => _LDAP_URL;
        public static string LDAP_ID => _LDAP_ID;
        public static string LDAP_PWD => _LDAP_PWD;
        public static string DB_URL => _DB_URL;

        private static string _LDAP_URL = ConfigurationManager.AppSettings["LDAP_URL"].ToString();
        private static string _LDAP_ID = ConfigurationManager.AppSettings["LDAP_ID"].ToString();
        private static string _LDAP_PWD = ConfigurationManager.AppSettings["LDAP_PWD"].ToString();
        private static string _DB_URL = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
    }
}

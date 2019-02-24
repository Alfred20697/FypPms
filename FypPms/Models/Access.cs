using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FypPms.Models
{
    public class Access
    {
        public string UserName { get; set; }
        public string UserType { get; set; }

        public Access(string username, string usertype)
        {
            UserName = username;
            UserType = usertype;
        }

        public bool IsLogin()
        {
            return !string.IsNullOrEmpty(UserName);
        }

        public bool IsAuthorize(string usertype)
        {
            return usertype.Equals(UserType);
        }
    }
}

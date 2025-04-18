using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Classes
{
    internal class RBAC
    {
        private readonly Dictionary<Role, List<string>> _rolePermissions;
        public RBAC()
        {
            _rolePermissions = new Dictionary<Role, List<string>>
            {
                {Role.Admin, new List<string>{"Read","Write","Delete"}},
                {Role.Manager, new List<string>{"Read","Write"}},
                {Role.User, new List<string>{"Read"}}
            };
        }
        public bool HasPermission(User user, string permission)
        {
            if (_rolePermissions.ContainsKey(user.Role) && _rolePermissions[user.Role].Contains(permission))
            {
                return true;
            }
            return false;
        }
    }
}

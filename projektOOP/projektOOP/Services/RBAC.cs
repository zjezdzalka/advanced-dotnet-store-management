using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projektOOP.Classes;
using projektOOP.Enums;

namespace projektOOP.Services
{
    public class RBAC
    {
        private readonly Dictionary<Role, List<Permission>> _rolePermissions;

        public RBAC()
        {
            _rolePermissions = new Dictionary<Role, List<Permission>>
        {
            { Role.Administrator, new List<Permission>
                {
                    Permission.ViewUsers,
                    Permission.ManageUsers,
                    Permission.ViewLogs,
                    Permission.ViewProducts,
                    Permission.UpdateStock,
                    Permission.PlaceOrder,
                    Permission.ViewOrderLogs,
                    Permission.ViewProfile
                }
            },
            { Role.Manager, new List<Permission>
                {
                    Permission.ViewProducts,
                    Permission.UpdateStock,
                    Permission.ViewOrderLogs,
                    Permission.ViewProfile
                }
            },
            { Role.User, new List<Permission>
                {
                    Permission.ViewProducts,
                    Permission.PlaceOrder,
                    Permission.ViewProfile
                }
            }
        };
        }

        public bool HasPermission(User user, Permission permission)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.Permissions.Contains(permission);
        }

        public void ApplyPermissions(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (_rolePermissions.TryGetValue(user.Role, out var perms))
                user.Permissions = new List<Permission>(perms);
            else
                user.Permissions = new List<Permission>();
        }
    }
}

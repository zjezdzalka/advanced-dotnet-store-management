using System;
using System.Collections.Generic;
using projektOOP.Classes;
using projektOOP.Enums;

namespace projektOOP.Services
{

    /// <summary>
    /// The RBAC (Role-Based Access Control) class is responsible for managing permissions
    /// and roles for users within the system. It defines the permissions associated with
    /// each role and provides methods to check and apply permissions for users.
    /// </summary>
    public class RBAC
    {
        /// <summary>
        /// A dictionary mapping roles to their respective list of permissions.
        /// </summary>
        private readonly Dictionary<Role, List<Permission>> _rolePermissions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RBAC"/> class and sets up
        /// the default role-to-permission mappings.
        /// </summary>
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

        /// <summary>
        /// Checks if a given user has a specific permission.
        /// </summary>
        /// <param name="user">The user whose permissions are being checked.</param>
        /// <param name="permission">The permission to check for.</param>
        /// <returns>True if the user has the specified permission; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the user is null.</exception>
        public bool HasPermission(User user, Permission permission)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return user.Permissions.Contains(permission);
        }

        /// <summary>
        /// Applies the permissions associated with the user's role to the user.
        /// </summary>
        /// <param name="user">The user whose permissions are being set.</param>
        /// <exception cref="ArgumentNullException">Thrown if the user is null.</exception>
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

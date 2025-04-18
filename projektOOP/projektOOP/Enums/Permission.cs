using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Enums
{
    [Flags]
    public enum Permission
    {
        None,
        ViewUsers,
        ManageUsers,
        ViewLogs,
        ViewOrderLogs,
        ViewProducts,
        UpdateStock,
        PlaceOrder,
        ViewProfile,
    }
}

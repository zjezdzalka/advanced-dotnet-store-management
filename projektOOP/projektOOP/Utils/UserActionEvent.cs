using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projektOOP.Utils
{
    public delegate void UserActionEventHandler(object sender, UserActionEventArgs e);

    public class UserActionEventArgs : EventArgs
    {
        public string Username { get; }
        public string Action { get; }

        public UserActionEventArgs(string username, string action)
        {
            Username = username;
            Action = action;
        }
    }
}

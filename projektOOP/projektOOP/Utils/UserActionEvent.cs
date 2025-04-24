using System;

namespace projektOOP.Utils
{
    /// <summary>
    /// Represents the method that will handle a user action event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An object that contains the event data.</param>
    public delegate void UserActionEventHandler(object sender, UserActionEventArgs e);

    /// <summary>
    /// Provides data for the user action event.
    /// </summary>
    public class UserActionEventArgs : EventArgs
    {
        public string Username { get; }

        public string Action { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserActionEventArgs"/> class.
        /// </summary>
        /// <param name="username">The username of the user who performed the action.</param>
        /// <param name="action">The description of the action performed by the user.</param>
        public UserActionEventArgs(string username, string action)
        {
            Username = username;
            Action = action;
        }
    }
}

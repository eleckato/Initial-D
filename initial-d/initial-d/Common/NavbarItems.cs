using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Common
{
    /// <summary>
    /// Object that model the Links of the Top Navbar
    /// </summary>
    public class NavbarItems
    {
        /// <summary>
        /// Controller name of the Link
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// Action of the Link
        /// </summary>
        public string ActionName { get; set; }
        /// <summary>
        /// Name to display in the View
        /// </summary>
        public string ViewName { get; set; }

        public NavbarItems()
        {
            ControllerName = string.Empty;
            ActionName = string.Empty;
            ViewName = string.Empty;
        }

        public NavbarItems(string controllerName, string actionName, string viewName)
        {
            ControllerName = controllerName;
            ActionName = actionName;
            ViewName = viewName;
        }
    }
}
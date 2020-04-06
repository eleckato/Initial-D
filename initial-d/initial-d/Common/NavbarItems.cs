using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace initial_d.Common
{
    public class NavbarItems
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
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
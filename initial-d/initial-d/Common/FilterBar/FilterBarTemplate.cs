using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace initial_d.Common.FilterBar
{
    /// <summary>
    /// Template for a FilterBar. I should *NOT* have more than 4 elements in filters
    /// </summary>
    public class FilterBarTemplate
    {
        /// <summary>
        /// Controller that the form will call
        /// </summary>
        public string formController { get; set; }
        /// <summary>
        /// Action that the form will call
        /// </summary>
        public string formAction { get; set; }
        /// <summary>
        /// List of Inputs, work nice with HtmlHelper elements
        /// </summary>
        public List<MvcHtmlString> filters { get; set; }

        public FilterBarTemplate()
        {

        }
    }
}
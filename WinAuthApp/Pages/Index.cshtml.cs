using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WinAuthApp.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            DirectorySearcher ds = new DirectorySearcher();
            var name = User.Identity.Name.Split('\\')[1];

            ds.Filter = $"(&(objectClass=user)(objectcategory=person)(sAMAccountName={name}))";
            SearchResult userProperty = ds.FindOne();
            List<string> _department = new List<string>();
            List<string> _memberof = new List<string>();
            string _name = string.Empty;
            string _mail = string.Empty;
            if (userProperty != null)
            {
                ResultPropertyValueCollection _departmentResult = userProperty.Properties["department"];
                if (_departmentResult != null)
                {
                    foreach (var de in _departmentResult)
                        _department.Add(de.ToString());
                }

                ResultPropertyValueCollection _memberofResult = userProperty.Properties["memberof"];
                if (_memberofResult != null)
                {
                    foreach (var me in _memberofResult)
                        _memberof.Add(me.ToString());
                }

                ResultPropertyValueCollection _nameResult = userProperty.Properties["name"];
                if (_nameResult != null)
                    _name = _nameResult[0].ToString();

                ResultPropertyValueCollection _mailResult = userProperty.Properties["mail"];
                if (_mailResult != null)
                    _mail = _mailResult[0].ToString();
            }

            int sdfsdf = 0;

        }
    }
}

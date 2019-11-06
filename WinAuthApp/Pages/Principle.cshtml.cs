
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;

namespace WinAuthApp.Pages
{
    public class PrincipleModel : PageModel
    {
        public UserPrincipal TheUser { get; set; }
        public List<string> Groups { get; set; }

        public List<string> AllUsers { get; set; }

        public List<string> AllGroups { get; set; }

        public void OnGet()
        {

            PrincipalContext insPrincipalContext = new PrincipalContext(ContextType.Machine);//Connecting to local computer.
            /*
            PrincipalContext insPrincipalContext = new PrincipalContext(ContextType.Domain, "MyDomain", "DC=MyDomain,DC=com"); //Connecting to Active Directory
            PrincipalContext insPrincipalContext = new PrincipalContext(ContextType.Machine, "TAMERO", "administrator", "password"); //Connecting to local computer with credentials of an user
            */

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                WindowsPrincipal principal = (WindowsPrincipal)User;
                UserPrincipal user = UserPrincipal.FindByIdentity(context, principal.Identity.Name);

                TheUser = user;
                Groups = new List<string>();
                foreach (var group in user.GetGroups())
                {
                    Groups.Add($"Group: {group.Name} | {group.Description} | {group.Guid}");
                }

                foreach (var group in user.GetAuthorizationGroups())
                {
                    Groups.Add($"Auth Group: {group.Name} | {group.Description} | {group.Guid}");
                }

                AllUsers = new List<string>();
                AllUsers.AddRange(ListUsers(context));

                AllGroups = new List<string>();
                AllGroups.AddRange(ListGroups(context));

            }
        }

        private List<string> ListUsers(PrincipalContext insPrincipalContext)
        {
            UserPrincipal insUserPrincipal = new UserPrincipal(insPrincipalContext);
            insUserPrincipal.Name = "*";
            return SearchUsers(insUserPrincipal);
        }

        private List<string> SearchUsers(UserPrincipal parUserPrincipal)
        {
            List<string> results = new List<string>();
            PrincipalSearcher insPrincipalSearcher = new PrincipalSearcher();
            insPrincipalSearcher.QueryFilter = parUserPrincipal;
            PrincipalSearchResult<Principal> result = insPrincipalSearcher.FindAll();
            foreach (Principal p in result)
            {
                if (p.Name.Contains("JoeBloogs", System.StringComparison.InvariantCultureIgnoreCase)) // for selected users get their groups too
                {
                    results.Add($"{p.Name} | {p.Guid} | Groups: {string.Join(",", p.GetGroups().Select(x => x.Name).ToArray())}");
                }
                else
                {
                    results.Add($"{p.Name} | {p.Guid}");
                }
            }
            return results.OrderBy(x => x).ToList();
        }
        
        private List<string> SearchGroups(GroupPrincipal parGroupPrincipal)
        {

            List<string> results = new List<string>();
            PrincipalSearcher insPrincipalSearcher = new PrincipalSearcher();
            insPrincipalSearcher.QueryFilter = parGroupPrincipal;
            PrincipalSearchResult<Principal> result = insPrincipalSearcher.FindAll();
            foreach (Principal p in result)
            {
                results.Add($"{p.Name} | {p.Guid} | {p.Description}");
            }
            return results.OrderBy(x => x).ToList();
        }
        
        private List<string> ListGroups(PrincipalContext insPrincipalContext)
        {
            GroupPrincipal insGroupPrincipal = new GroupPrincipal(insPrincipalContext);
            insGroupPrincipal.Name = "*";
            return SearchGroups(insGroupPrincipal);
        }
        
    }
}
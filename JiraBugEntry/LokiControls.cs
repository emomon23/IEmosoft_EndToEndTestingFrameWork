using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraBugEntry
{
    public static class LokiControls
    {
        public static class LoginPage
        {
            public static string UserNameId = "login-form-username";
            public static string PasswordId = "login-form-password";
            public static string LoginSubmitButtonId = "login-form-submit";
        }

        public static class DashboardPage
        {
            public static string CreateBugLinkId = "create_link";
            public static string ProjectDrowndownId = "browse_link";
            public static string TestProjectLinkId = "admin_main_proj_link_lnk";
            public static string IssuesDropdownId = "find_link";
            public static string MyOpenIssuesId = "filter_lnk_my_lnk";

        }

        public static class CreateIssuePage
        {
            public static string IssueTypeDrowndownId = "issuetype-field";
            public static string SummaryId = "summary";
            public static string PriorityId = "priority-field";
            public static string DueDateId = "duedate";
            public static string EnvironmentId = "environment";
            public static string DescriptionId = "description";
            public static string AttachementChooseFileButtonId = "attachment_box";
            public static string SubmitButtonId = "create-issue-submit";
        }
    }
}

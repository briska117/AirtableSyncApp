using Intuit.TSheets.Api;
using Intuit.TSheets.Model;
using Intuit.TSheets.Model.Filters;
using Navmii.AirTableSyncNetcore6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Navmii.AirtableSync
{
    public class TSheetsFetch
    {
        private readonly SyncSettings settings;
        public TSheetsFetch(SyncSettings syncSettings)
        {
            settings = syncSettings;
        }
        public Dictionary<int, Dictionary<string, string>> FetchUsers()
        {
            List<string> projectPrefixes = new List<string>();
                foreach (string pp in settings.ProjectPrefix.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    projectPrefixes.Add(pp.Trim().ToLower());
                }

                DataService apiClient = new DataService(settings.TSheetsApiToken, null);

                UserFilter filterUsers = new UserFilter
                {
                    Active = Intuit.TSheets.Model.Enums.TristateChoice.Both
                };
                (IList<User> listUsers, ResultsMeta resultsMetaUsers) = apiClient.GetUsers(filterUsers);

                HashSet<int> groupIDs = new HashSet<int>();
                HashSet<int> customFieldIDs = new HashSet<int>();
                foreach (User user in listUsers)
                {
                    if (user.GroupId.HasValue && !groupIDs.Contains(user.GroupId.Value))
                    {
                        groupIDs.Add(user.GroupId.Value);
                    }

                    foreach (KeyValuePair<string, string> pair in user.CustomFields)
                    {
                        if (int.TryParse(pair.Key, out int customFieldID) && !customFieldIDs.Contains(customFieldID))
                        {
                            customFieldIDs.Add(customFieldID);
                        }
                    }
                }

                GroupFilter filterGroups = new GroupFilter
                {
                    Active = Intuit.TSheets.Model.Enums.TristateChoice.Both,
                    Ids = groupIDs
                };
                (IList<Group> listGroups, ResultsMeta resultsMetaGroups) = apiClient.GetGroups(filterGroups);

                CustomFieldFilter filterCustomFields = new CustomFieldFilter
                {
                    AppliesTo = Intuit.TSheets.Model.Enums.AppliesToType.User,
                    Ids = customFieldIDs
                };
                (IList<CustomField> listCustomFields, ResultsMeta resultsMetaCustomFields) = apiClient.GetCustomFields(filterCustomFields);

                Dictionary<int, Group> groups = new Dictionary<int, Group>();
                foreach (Group group in listGroups)
                {
                    groups.Add(group.Id, group);
                }

                Dictionary<string, CustomField> customFields = new Dictionary<string, CustomField>();
                foreach (CustomField customField in listCustomFields)
                {
                    customFields.Add(customField.Id.ToString(), customField);
                }

                Dictionary<int, Dictionary<string, string>> users = new Dictionary<int, Dictionary<string, string>>();


                foreach (User user in listUsers)
                {
                    Dictionary<string, string> fields = new Dictionary<string, string>();

                    fields.Add("First Name (required)", user.FirstName);
                    fields.Add("Last Name (required)", user.LastName);
                    //fields.Add("User Name", user.Name);
                    //fields.Add("Email", user.Email);

                    //if (user.EmployeeNumber.HasValue && user.EmployeeNumber.Value > 0)
                    //{
                    //    fields.Add("Employee #", "" + user.EmployeeNumber.Value);
                    //}
                    //else
                    //{
                    //    fields.Add("Employee #", null);
                    //}

                    //fields.Add("Payroll ID", user.PayrollId);

                    //if (user.HireDate.HasValue)
                    //{
                    //    fields.Add("Hire Date", user.HireDate.Value.ToString("yyyy-MM-dd"));
                    //}
                    //else
                    //{
                    //    fields.Add("Hire Date", "0000-00-00");
                    //}

                    if (user.GroupId.HasValue && groups.TryGetValue(user.GroupId.Value, out Group group))
                    {
                        fields.Add("Group", group.Name);
                    }
                    else
                    {
                        fields.Add("Group", null);
                    }

                    //if (user.PayRate.HasValue && user.PayRate.Value > 0)
                    //{
                    //    fields.Add("Pay Rate ($)", "" + user.PayRate.Value);

                    //    if (user.PayInterval.HasValue)
                    //    {
                    //        fields.Add("Per", user.PayInterval.Value.ToString());
                    //    }
                    //}
                    //else
                    //{
                    //    fields.Add("Pay Rate ($)", null);
                    //    fields.Add("Per", null);
                    //}

                    //if (user.Salaried.HasValue && user.Salaried.Value)
                    //{
                    //    fields.Add("Salaried", "Y");
                    //}
                    //else
                    //{
                    //    fields.Add("Salaried", null);
                    //}

                    //if (user.Exempt.HasValue && user.Exempt.Value)
                    //{
                    //    fields.Add("Exempt", "Y");
                    //}
                    //else
                    //{
                    //    fields.Add("Exempt", null);
                    //}

                    //if (user.Permissions.ManageMyTimesheets.HasValue && user.Permissions.ManageMyTimesheets.Value)
                    //{
                    //    fields.Add("Manage their timesheets", "Y");
                    //}
                    //else
                    //{
                    //    fields.Add("Manage their timesheets", null);
                    //}

                    //if (user.Permissions.Admin.HasValue && user.Permissions.Admin.Value)
                    //{
                    //    fields.Add("Admin", "Y");
                    //}
                    //else
                    //{
                    //    fields.Add("Admin", null);
                    //}

                    //if (user.Permissions.Mobile.HasValue && user.Permissions.Mobile.Value)
                    //{
                    //    fields.Add("Mobile time entry", "Y");
                    //}
                    //else
                    //{
                    //    fields.Add("Mobile time entry", null);
                    //}

                    //fields.Add("Enabled", null); //!!!

                    //fields.Add("Phone #", user.MobileNumber);
                    //fields.Add("Kiosk time entry", null); //!!!
                    //fields.Add("Timezone", null); //!!!
                    //fields.Add("Kiosk PIN", null); //!!!

                    //if (user.Active.HasValue && user.Active.Value)
                    //{
                    //    fields.Add("Active", "active");
                    //}
                    //else
                    //{
                    //    fields.Add("Active", "archived");
                    //}

                    //if (user.Permissions.ViewCompanySchedules.HasValue && user.Permissions.ViewCompanySchedules.Value)
                    //{
                    //    fields.Add("View schedule (company, group, own)", "company");
                    //}
                    //else if (user.Permissions.ViewGroupSchedules.HasValue && user.Permissions.ViewGroupSchedules.Value)
                    //{
                    //    fields.Add("View schedule (company, group, own)", "group");
                    //}
                    //else if (user.Permissions.ViewMySchedules.HasValue && user.Permissions.ViewMySchedules.Value)
                    //{
                    //    fields.Add("View schedule (company, group, own)", "own");
                    //}
                    //else
                    //{
                    //    fields.Add("View schedule (company, group, own)", null);
                    //}

                    //if (user.Permissions.ManageCompanySchedules.HasValue && user.Permissions.ManageCompanySchedules.Value)
                    //{
                    //    fields.Add("Manage schedule (company, group, own, none)", "company");
                    //}
                    //else if (user.Permissions.ManageSchedules.HasValue && user.Permissions.ManageSchedules.Value)
                    //{
                    //    fields.Add("Manage schedule (company, group, own, none)", "group");
                    //}
                    //else if (user.Permissions.ManageMySchedule.HasValue && user.Permissions.ManageMySchedule.Value)
                    //{
                    //    fields.Add("Manage schedule (company, group, own, none)", "own");
                    //}
                    //else
                    //{
                    //    fields.Add("Manage schedule (company, group, own, none)", null);
                    //}

                    //fields.Add("Password", null); //!!!

                    string[] customFieldNames = new[] { "Job Title", "Work Email", "Work Number" }; // new[] { "Staffing Agency", "Job Title", "Country", "Work Email", "Work Number" };
                    foreach (string customFieldName in customFieldNames)
                    {
                        fields.Add("cf_" + customFieldName, null);
                    }

                    foreach (KeyValuePair<string, string> pair in user.CustomFields)
                    {
                        if (customFields.TryGetValue(pair.Key, out CustomField customField))
                        {
                            foreach (string customFieldName in customFieldNames)
                            {
                                if (customField.Name == customFieldName)
                                {
                                    fields["cf_" + customFieldName] = pair.Value;
                                }
                            }
                        }
                    }

                    if (fields.TryGetValue("cf_Job Title", out string jobRole) && jobRole != null && string.IsNullOrWhiteSpace(jobRole))
                    {
                        fields["cf_Job Title"] = null;
                    }

                    fields.Add(Synchronizer.Field_TSheetsUserID, "" + user.Id);

                    if (fields.TryGetValue("Group", out string groupValue) && groupValue != null)
                    {
                        bool prefixFound = false;
                        foreach (string prefix in projectPrefixes)
                        {
                            if (groupValue.ToLower().StartsWith(prefix))
                            {
                                prefixFound = true;
                                break;
                            }
                        }

                        if (prefixFound)
                        {
                            users.Add(user.Id, fields);
                        }
                    }
                }

                return users;
           
        }

    }
}

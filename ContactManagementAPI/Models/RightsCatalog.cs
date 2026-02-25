using System.Collections.Generic;

namespace ContactManagementAPI.Models
{
    public static class RightsCatalog
    {
        public const string ContactsView = "Contacts.View";
        public const string ContactsCreate = "Contacts.Create";
        public const string ContactsEdit = "Contacts.Edit";
        public const string ContactsDelete = "Contacts.Delete";
        public const string DocumentsManage = "Documents.Manage";
        public const string PhotosManage = "Photos.Manage";
        public const string AdminManageUsers = "Admin.ManageUsers";
        public const string AdminManageGroups = "Admin.ManageGroups";
        public const string AdminManageRights = "Admin.ManageRights";

        public static readonly IReadOnlyList<RightDefinition> All = new List<RightDefinition>
        {
            new RightDefinition(ContactsView, "Contacts", "View contacts"),
            new RightDefinition(ContactsCreate, "Contacts", "Create contacts"),
            new RightDefinition(ContactsEdit, "Contacts", "Edit contacts"),
            new RightDefinition(ContactsDelete, "Contacts", "Delete contacts"),
            new RightDefinition(PhotosManage, "Photos", "Manage photo gallery"),
            new RightDefinition(DocumentsManage, "Documents", "Manage contact documents"),
            new RightDefinition(AdminManageUsers, "Administration", "Manage users"),
            new RightDefinition(AdminManageGroups, "Administration", "Manage user groups"),
            new RightDefinition(AdminManageRights, "Administration", "Manage rights")
        };
    }

    public class RightDefinition
    {
        public RightDefinition(string key, string category, string label)
        {
            Key = key;
            Category = category;
            Label = label;
        }

        public string Key { get; }
        public string Category { get; }
        public string Label { get; }
    }
}

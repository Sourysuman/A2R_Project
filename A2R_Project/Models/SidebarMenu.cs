namespace A2R_Project.Models
{
    public class SidebarMenu
    {
        public string MenuName { get; set; } = "";
        public string Icon { get; set; } = "cog";
        public List<SubMenuItem> SubMenus { get; set; } = new();
    }

    public class SubMenuItem
    {
        public string Name { get; set; } = "";
        public string Controller { get; set; } = "";
        public string Action { get; set; } = "List";
    }
}
using System.ComponentModel.DataAnnotations;

namespace A2R_Project.Models
{
    public class AccessControl
    {
        public int AccessID { get; set; }
        public int? User_ID { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public int? Menu_ID { get; set; }
        public string? MenuName { get; set; }
        public string? MenuIcon { get; set; }
        public string? SubMenuName { get; set; }
        public string? IconClass { get; set; }
        public int? Submenu_ID { get; set; }

        public int Insert_Flag { get; set; } = 0;
        public int Update_Flag { get; set; } = 0;
        public int Delete_Flag { get; set; } = 0;
        public int Module_Flag { get; set; } = 0;

        public string? RoleName { get; set; }
        public string? SubMenuController { get; set; }
        public string? SubMenuAction { get; set; }
        public string? SubMenuChildAction { get; set; }
    }

    public class AccessControls
    {
        public List<AccessControl> accessControl { get; set; } = new List<AccessControl>();
    }

    
    
}
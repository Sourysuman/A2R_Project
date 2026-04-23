using A2R_Project.Models;

namespace A2R_Project.Models
{
    public class CompanyViewModel
    {
        public List<Company> companies { get; set; } = new List<Company>();
        public List<State> states { get; set; } = new List<State>();  // Your State model
        public List<Country> country { get; set; } = new List<Country>();  // Your Country model
    }
}
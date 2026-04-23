using A2R_Project.Models;
using System.Threading.Tasks;

namespace A2R_Project.Interface
{
    public interface IFollowUpRepository
    {
        Task<bool> UpdateFollowUp(FollowUpDetail followUpDetail);
    }
}
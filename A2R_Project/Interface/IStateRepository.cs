using A2R_Project.Models;

namespace A2R_Project.Interfaces
{
    public interface IStateRepository
    {
        Task<List<State>> GetAllStates();
        Task<string> Add(State state);
        Task<bool> Edit(State state);
        Task<State> GetById(int stateID);
        Task<bool> Delete(int stateID);
        Task<List<State>> GetAllCountries(); // For dropdown
    }
}
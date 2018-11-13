using BT.Auctions.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BT.Auctions.API.Data.Interfaces
{
    /// <summary>
    /// Display Configuration Repository Interface
    /// </summary>
    public interface IDisplayConfigurationRepository
    {
        Task<DisplayConfiguration> AddDisplayConfiguration(DisplayConfiguration displayConfiguration);
        Task<DisplayConfiguration> GetDisplayConfigurationById(int id);
        Task<DisplayConfiguration> GetDisplayConfigurationDetailsById(int displayConfigurationId);
        Task<DisplayConfiguration> UpdateDisplayConfiguration(int id, DisplayConfiguration displayConfiguration);
        Task DeleteDisplayConfiguration(int displayConfigurationId);
        Task<IEnumerable<DisplayConfiguration>> GetDisplayConfigurations();
        Task<IEnumerable<DisplayConfiguration>> GetDisplayGroupConfigurationsByDisplayGroupId(int displayGroupId);
    }
}
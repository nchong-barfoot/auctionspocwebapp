using System.Collections.Generic;
using System.Threading.Tasks;
using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;

namespace BT.Auctions.API.Services.Interfaces
{
    public interface IDisplayConfigurationService
    {
        /// <summary>
        /// Adds the lot.
        /// </summary>
        /// <param name="displayConfiguration">The Display Configuration.</param>
        /// <returns>The Added Display Configuration</returns>
        Task<DisplayConfiguration> AddDisplayConfiguration(DisplayConfigurationDto displayConfigurationDto);


        /// <summary>
        /// Gets the display configurations.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DisplayConfiguration>> GetDisplayConfigurations();

        /// <summary>
        /// Updates the Display Configuration.
        /// </summary>
        /// <param name="displayConfigurationId">The display configuration identifier.</param>
        /// <param name="displayConfigurationDto">The display configuration dto.</param>
        /// <returns>
        /// The Updated Display Configuration
        /// </returns>
        Task<DisplayConfiguration> UpdateDisplayConfiguration(int displayConfigurationId, DisplayConfigurationDto displayConfigurationDto);

        /// <summary>
        /// Gets the display configurations by display configuration identifier.
        /// </summary>
        /// <param name="displayGroupId">The display group identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<DisplayConfigurationDto>> GetDisplayGroupConfigurationsByDisplayGroupId(int displayGroupId);

        /// <summary>
        /// Deletes the display configuration.
        /// </summary>
        /// <param name="displayConfigurationId">The display configuration identifier.</param>
        /// <returns></returns>
        Task<DisplayConfiguration> DeleteDisplayConfiguration(int displayConfigurationId);
    }
}
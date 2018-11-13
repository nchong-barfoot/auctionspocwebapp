using BT.Auctions.API.Models;
using BT.Auctions.API.Models.DataTransferObjects;
using Castle.Core.Internal;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace BT.Auctions.API.Services.Helpers
{
    /// <summary>
    /// Used for managing storage access for the auctions solution
    /// </summary>
    public class StorageHelper
    {
        private readonly IOptions<ServiceSettings> _serviceSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageHelper"/> class.
        /// </summary>
        /// <param name="serviceSettings">The service settings.</param>
        public StorageHelper(IOptions<ServiceSettings> serviceSettings)
        {
            _serviceSettings = serviceSettings;
        }

        /// <summary>
        /// Gets the BLOB container.
        /// </summary>
        /// <returns></returns>
        public CloudBlobContainer GetBlobContainer(string containerName = "")
        {
            var storageAccount = new CloudStorageAccount(
                new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(
                    _serviceSettings.Value.StorageAccountName,
                    _serviceSettings.Value.StorageAccessKey), true);
            var blobClient = storageAccount.CreateCloudBlobClient();

            if (containerName.IsNullOrEmpty())
            {
                containerName = _serviceSettings.Value.StorageDefaultContainerName;
            }

            return blobClient.GetContainerReference(containerName);
        }

        /// <summary>
        /// Checks the media exists in BLOB storage.
        /// </summary>
        /// <param name="mediaReference">The media reference.</param>
        /// <returns></returns>
        public async Task<bool> CheckMediaExistsInBlobStorage(string mediaReference)
        {
            var container = GetBlobContainer();
            var blockBlob = container.GetBlockBlobReference(mediaReference);
            return await blockBlob.ExistsAsync();
        }

        /// <summary>
        /// Uploads the media to BLOB storage.
        /// </summary>
        /// <param name="media">The media.</param>
        /// <returns></returns>
        public async Task UploadMediaToBlobStorage(MediaDto media, string mediaName = "")
        {
            var container = GetBlobContainer();
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });

            if (string.IsNullOrEmpty(mediaName))
            {
                mediaName = media.Data.FileName;
            }

            var blockBlob = container.GetBlockBlobReference(mediaName);
            using (var fileStream = media.Data.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            media.FileName = mediaName;
            media.Url = blockBlob.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Deletes the media from BLOB storage.
        /// </summary>
        /// <param name="mediaReference">The media reference aka file name.</param>
        /// <returns></returns>
        public async Task DeleteMediaFromBlobStorage(string mediaReference)
        {
            var container = GetBlobContainer();
            var blockBlob = container.GetBlockBlobReference(mediaReference);
            await blockBlob.DeleteAsync();
        }
    }
}

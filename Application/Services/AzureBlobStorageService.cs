using Application.Interfaces;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;

namespace Application.Services
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly BlobContainerClient _blobContainer;
        private readonly string _publicEndpoint;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            var containerName = configuration["AzureStorage:ContainerName"];
            _publicEndpoint = configuration["AzureStorage:PublicEndpoint"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException(nameof(containerName));

            _blobContainer = new BlobContainerClient(connectionString, containerName);

            if (string.IsNullOrWhiteSpace(_publicEndpoint))
            {
                _publicEndpoint = _blobContainer.Uri.AbsoluteUri;
            }
        }

        public async Task SaveMediaAsync(Stream mediaStream, string fileName, string mimeType = null)
        {
            await _blobContainer.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            var blobClient = _blobContainer.GetBlobClient(fileName);
            await blobClient.UploadAsync(mediaStream, overwrite: true);
        }

        public async Task DeleteMediaAsync(string fileName)
        {
            var blobClient = _blobContainer.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
        }

        public string GetMediaUrl(string fileName)
        {
            return $"{_publicEndpoint}/{fileName}";
        }
    }
}

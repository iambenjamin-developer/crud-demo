using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Text;

namespace API
{
    public class AzureBlobIntegration
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobIntegration(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureBlob");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<(string BlobName, string Url)> UploadAsync(string containerName, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Archivo vacío o nulo", nameof(file));

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            var extension = Path.GetExtension(file.FileName);
            var blobName = Guid.NewGuid() + extension;

            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            var metadata = new Dictionary<string, string>
            {
                { "OriginalFileName", Convert.ToBase64String(Encoding.UTF8.GetBytes(file.FileName)) }
            };

            await blobClient.SetMetadataAsync(metadata);

            return (blobName, blobClient.Uri.ToString());
        }

        public async Task DeleteAsync(string containerName, string blobName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            var exists = await blobClient.ExistsAsync();

            if (exists)
                await blobClient.DeleteAsync();
        }
    }
}

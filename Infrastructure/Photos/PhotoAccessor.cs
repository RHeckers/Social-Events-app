using System;
using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    public class PhotoAccessor : IPhotoAccessor
    {
        // property of type Cloudinary
        private readonly Cloudinary _cloudinary;

        // Constructor to get the Cloudinary stringly types settings from user-secrets
        // Create new instance of Cloudinary 
        public PhotoAccessor(IOptions<CloudinarySettings> config)
        {
            var account = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        public PhotoUploadResult AddPhoto(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                // Using to dispose directly after it runs
                // Open file read stream to check for a file
                using(var stream = file.OpenReadStream())
                {
                    // Create a new instance op ImageUploadParams and write the File value 
                    // with the fileName and the stream aka the file
                    // Transform the image to specific dementions and focus on the face
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("Fill").Gravity("face")
                    };

                    // Write the values of the upload to the uploadResults
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            // Return the PhotoUploadResult with a public Id and image Url
            return new PhotoUploadResult
            {
                publicId = uploadResult.PublicId,
                Url = uploadResult.SecureUri.AbsoluteUri
            };
        }

        public string DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = _cloudinary.Destroy(deleteParams);

            return result.Result == "ok" ? result.Result : null;
        }
    }
}
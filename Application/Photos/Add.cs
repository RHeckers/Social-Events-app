using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos
{
    public class Add
    {
        public class Command : IRequest<Photo>
        {
            public IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Photo>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IPhotoAccessor _photoAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor, IPhotoAccessor photoAccessor)
            {
                _photoAccessor = photoAccessor;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Photo> Handle(Command request, CancellationToken cancellationToken)
            {
                // handler logic     
                // Add the picture to cloudinary with the pohotoAccessor
                var photoUploadResult = _photoAccessor.AddPhoto(request.File);
                
                // Get the current user
                var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == _userAccessor.GetCurrentUsername());   

                // Create a new photo object with the URL and publicID from Cloudinary
                var photo = new Photo
                {
                    Url = photoUploadResult.Url,
                    Id = photoUploadResult.publicId
                };

                // Check if there is a main photo for the current user, if not make this photo the main photo
                if (!user.Photos.Any(x => x.IsMain))
                    photo.IsMain = true;
                
                // Add the photo to the users photos collection
                user.Photos.Add(photo);

                // Default succes error logic               
                var success = await _context.SaveChangesAsync() > 0;

                // If save changes has succeeded, return the created image object
                if (success) return photo;

                throw new Exception("Problem saving changes");
            }
        }
    }
}
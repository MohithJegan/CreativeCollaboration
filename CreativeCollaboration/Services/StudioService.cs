
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;


namespace CreativeCollaboration.Services
{
    public class StudioService : IStudioService
    {
        private readonly ApplicationDbContext _context;

        // dependency injection of database context
        public StudioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudioDto>> ListStudios()
        {
            // all studios
            IEnumerable<Studio> Studios = await _context.Studios
                 .ToListAsync();
            List<StudioDto> StudioDtos = new List<StudioDto>();

            // foreach Studio record in database
            foreach (Studio Studio in Studios)
            {
                // create new instance of StudioDto, add to list
                StudioDto StudioDto = new StudioDto()
                {
                    StudioID = Studio.StudioID,
                    StudioName = Studio.StudioName,
                    StudioCountry = Studio.StudioCountry,
                    StudioEstablishedYear = Studio.StudioEstablishedYear,
                    StudioCEO = Studio.StudioCEO,
                    StudioHeadquarter = Studio.StudioHeadquarter,
                    HasStudioPic = Studio.HasPic
                };

                if (Studio.HasPic)
                {
                    StudioDto.StudioImagePath = $"/images/studios/{Studio.StudioID}{Studio.PicExtension}";
                }

                // create new instance of ProductDto, add to list
                StudioDtos.Add(StudioDto);

            }

            // return StudioDtos
            return StudioDtos;
        }

     
        public async Task<StudioDto?> FindStudio(int id)
        {
            // first or default async will get the first studio item matching the {id}
            var Studio = await _context.Studios.FirstOrDefaultAsync(s => s.StudioID == id);

            // no studio found
            if (Studio == null)
            {
                return null;
            }

            StudioDto StudioDto = new StudioDto()
            {
                StudioID = Studio.StudioID,
                StudioName = Studio.StudioName,
                StudioCountry = Studio.StudioCountry,
                StudioEstablishedYear = Studio.StudioEstablishedYear,
                StudioCEO = Studio.StudioCEO,
                StudioHeadquarter = Studio.StudioHeadquarter,
                HasStudioPic = Studio.HasPic
            };

            if (Studio.HasPic)
            {
                StudioDto.StudioImagePath = $"/images/studios/{Studio.StudioID}{Studio.PicExtension}";
            }

            return StudioDto;
        }

    
        public async Task<ServiceResponse> UpdateStudio(StudioDto studioDto)
        {
            ServiceResponse serviceResponse = new();

            Studio? Studio = await _context.Studios.FindAsync(studioDto.StudioID);

            if (Studio == null)
            {
                serviceResponse.Messages.Add("Studio could not be found");
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                return serviceResponse;
            }

            Studio.StudioID = studioDto.StudioID;
            Studio.StudioName = studioDto.StudioName;
            Studio.StudioCountry = studioDto.StudioCountry;
            Studio.StudioEstablishedYear = studioDto.StudioEstablishedYear;
            Studio.StudioCEO = studioDto.StudioCEO;
            Studio.StudioHeadquarter = studioDto.StudioHeadquarter;

            // flags that the object has changed
            _context.Entry(Studio).State = EntityState.Modified;
            // handled by another method
            _context.Entry(Studio).Property(s => s.HasPic).IsModified = false;
            _context.Entry(Studio).Property(s => s.PicExtension).IsModified = false;

            try
            {
                // SQL Equivalent: Update Studios set ... where StudioId={id}
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("An error occurred updating the record");
                return serviceResponse;
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Updated;
            return serviceResponse;
        }

        public async Task<ServiceResponse> AddStudio(StudioDto studioDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Studio
            Studio Studio = new Studio()
            {
                StudioID = studioDto.StudioID,
                StudioName = studioDto.StudioName,
                StudioCountry = studioDto.StudioCountry,
                StudioEstablishedYear = studioDto.StudioEstablishedYear,
                StudioCEO = studioDto.StudioCEO,
                StudioHeadquarter = studioDto.StudioHeadquarter
            };

            // SQL Equivalent: Insert into Studios (..) values (..)

            try
            {
                _context.Studios.Add(Studio);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Studio.");
                serviceResponse.Messages.Add(ex.Message);
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Studio.StudioID;
            return serviceResponse;
        
        }

        public async Task<ServiceResponse> DeleteStudio(int id)
        {
            ServiceResponse response = new();

            // Studio Item must exist in the first place
            var Studio = await _context.Studios.FindAsync(id);

            if (Studio == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Studio cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Studios.Remove(Studio);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the studio");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }

        public async Task<IEnumerable<StudioDto>> ListStudioForMovie(int id)
        {
            // join StudioMovie on studios.studioid = StudioMovie.studioid WHERE StudioMovie.movieid = {id}
            List<Movie> Movies = await _context.Movies
                .Where(m => m.MovieID == id)
                .ToListAsync();

            // empty list of data transfer object StudioDto
            List<StudioDto> StudioDtos = new List<StudioDto>();

            List<Studio> Studios = await _context.Studios
                 .Where(s => s.StudioID == Movies[0].StudioID)
                 .ToListAsync();

                // foreach Studio Item record in database
                foreach (Studio Studio in Studios)
                  {
                    // create new instance of StudioDto
                    StudioDtos.Add(new StudioDto()
                    {
                        StudioID = Studio.StudioID,
                        StudioName = Studio.StudioName,
                        StudioCountry = Studio.StudioCountry,
                        StudioEstablishedYear = Studio.StudioEstablishedYear,
                        StudioCEO = Studio.StudioCEO,
                        StudioHeadquarter = Studio.StudioHeadquarter
                    });
                  }

            // return StudioDtos
            return StudioDtos;

        }


        public async Task<ServiceResponse> UpdateStudioImage(int id, IFormFile StudioPic)
        {
            ServiceResponse response = new();

            Studio? Studio = await _context.Studios.FindAsync(id);
            if (Studio == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add($"Studio {id} not found");
                return response;
            }

            if (StudioPic.Length > 0)
            {


                // remove old picture if exists
                if (Studio.HasPic)
                {
                    string OldFileName = $"{Studio.StudioID}{Studio.PicExtension}";
                    string OldFilePath = Path.Combine("wwwroot/images/studios/", OldFileName);
                    if (File.Exists(OldFilePath))
                    {
                        File.Delete(OldFilePath);
                    }

                }


                // establish valid file types (can be changed to other file extensions if desired!)
                List<string> Extensions = new List<string> { ".jpeg", ".jpg", ".png", ".gif" };
                string StudioPicExtension = Path.GetExtension(StudioPic.FileName).ToLowerInvariant();
                if (!Extensions.Contains(StudioPicExtension))
                {
                    response.Messages.Add($"{StudioPicExtension} is not a valid file extension");
                    response.Status = ServiceResponse.ServiceStatus.Error;
                    return response;
                }

                string FileName = $"{id}{StudioPicExtension}";
                string FilePath = Path.Combine("wwwroot/images/studios/", FileName);

                using (var targetStream = File.Create(FilePath))
                {
                    StudioPic.CopyTo(targetStream);
                }

                // check if file was uploaded
                if (File.Exists(FilePath))
                {
                    Studio.PicExtension = StudioPicExtension;
                    Studio.HasPic = true;

                    _context.Entry(Studio).State = EntityState.Modified;

                    try
                    {
                        // SQL Equivalent: Update Studios set HasPic=True, PicExtension={ext} where StudioId={id}

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        response.Status = ServiceResponse.ServiceStatus.Error;
                        response.Messages.Add("An error occurred updating the record");

                        return response;
                    }
                }

            }
            else
            {
                response.Messages.Add("No File Content");
                response.Status = ServiceResponse.ServiceStatus.Error;
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Updated;



            return response;
        }

    }
}

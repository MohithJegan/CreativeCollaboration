using CreativeCollaboration.Models;

namespace CreativeCollaboration.Interfaces
{
    public interface IStudioService
    {
        // base CRUD
        Task<IEnumerable<StudioDto>> ListStudios();

        Task<StudioDto?> FindStudio(int id);

        Task<ServiceResponse> UpdateStudio(StudioDto studioDto);

        Task<ServiceResponse> AddStudio(StudioDto studioDto);

        Task<ServiceResponse> DeleteStudio(int id);

        Task<ServiceResponse> UpdateStudioImage(int id, IFormFile StudioPic);

        // related methods

        Task<IEnumerable<StudioDto>> ListStudioForMovie(int id);



    }
}

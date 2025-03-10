using CreativeCollaboration.Models;

namespace CreativeCollaboration.Interfaces
{
    public interface IMovieService
    {
        // base CRUD
        Task<IEnumerable<MovieDto>> ListMovies();

        Task<MovieDto?> FindMovie(int id);

        Task<ServiceResponse> UpdateMovie(MovieDto movieDto);

        Task<ServiceResponse> AddMovie(MovieDto movieDto);

        Task<ServiceResponse> DeleteMovie(int id);

        // related methods

        Task<IEnumerable<MovieDto>> ListMoviesForActor(int id);

        Task<IEnumerable<MovieDto>> ListMoviesForStudio(int id);

        Task<ServiceResponse> LinkMovieToActor(int movieId, int actorId);

        Task<ServiceResponse> UnlinkMovieFromActor(int movieId, int actorId);

        Task<IEnumerable<MovieDto>> ListMoviesForCustomer(int id);

        Task<ServiceResponse> LinkMovieToCustomer(int movieId, int customerId);

        Task<ServiceResponse> UnlinkMovieFromCustomer(int movieId, int customerId);
    }
}

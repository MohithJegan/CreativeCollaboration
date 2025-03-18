using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;
using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;


namespace CreativeCollaboration.Services
{
    public class MovieService : IMovieService
    {
        private readonly ApplicationDbContext _context;
        
        // dependency injection of database context
        public MovieService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MovieDto>> ListMovies()
        {
            // all movies
            IEnumerable<Movie> Movies = await _context.Movies
                .Include(m => m.Studio)
                .Include(m => m.Actors)
                .Include (m => m.Customers)
                .ToListAsync();

            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();

            // foreach Movie record in database
            foreach (Movie Movie in Movies)
            {
                // create new instance of MovieDto, add to list
                MovieDtos.Add(new MovieDto()
                {
                    MovieID = Movie.MovieID,
                    MovieTitle = Movie.MovieTitle,
                    MovieReleaseDate = Movie.MovieReleaseDate,
                    MovieDuration = Movie.MovieDuration,
                    MovieDescription = Movie.MovieDescription,
                    MovieBudget = Movie.MovieBudget,
                    MovieBoxOfficeCollection = Movie.MovieBoxOfficeCollection,
                    MovieRating = Movie.MovieRating,
                    MovieAwardNomination = Movie.MovieAwardNomination,
                    MovieAwardWin = Movie.MovieAwardWin,
                    MovieCost = Movie.MovieCost,
                    StudioID = Movie.StudioID,
                    MovieStudioName = Movie.Studio.StudioName
                });
            }

            // return MovieDtos
            return MovieDtos;
        }

        public async Task<MovieDto?> FindMovie(int id)
        {
            // first or default async will get the first movie item matching the {id}
            var Movie = await _context.Movies
                .Include(m => m.Studio)
                .Include(m => m.Actors)
                .FirstOrDefaultAsync(m => m.MovieID == id);

            // no movie found
            if (Movie == null)
            {
                return null;
            }
            MovieDto MovieDto = new MovieDto()
            {
                MovieID = Movie.MovieID,
                MovieTitle = Movie.MovieTitle,
                MovieReleaseDate = Movie.MovieReleaseDate,
                MovieDuration = Movie.MovieDuration,
                MovieDescription = Movie.MovieDescription,
                MovieBudget = Movie.MovieBoxOfficeCollection,
                MovieBoxOfficeCollection = Movie.MovieBoxOfficeCollection,
                MovieRating = Movie.MovieRating,
                MovieAwardNomination = Movie.MovieAwardNomination,
                MovieAwardWin = Movie.MovieAwardWin,
                MovieCost = Movie.MovieCost,
                MovieStudioName = Movie.Studio.StudioName
            };

            return MovieDto;
        }

        public async Task<ServiceResponse> UpdateMovie(MovieDto movieDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Movie
            Movie Movie = new Movie()
            {
                MovieID = Convert.ToInt32(movieDto.MovieID),
                MovieTitle = movieDto.MovieTitle,
                MovieReleaseDate = movieDto.MovieReleaseDate,
                MovieDuration = movieDto.MovieDuration,
                MovieDescription = movieDto.MovieDescription,
                MovieBudget = movieDto.MovieBoxOfficeCollection,
                MovieBoxOfficeCollection = movieDto.MovieBoxOfficeCollection,
                MovieRating = movieDto.MovieRating,
                MovieAwardNomination = movieDto.MovieAwardNomination,
                MovieAwardWin = movieDto.MovieAwardWin,
                MovieCost = movieDto.MovieCost,
                StudioID = movieDto.StudioID
            };

            // flags that the object has changed
            _context.Entry(Movie).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Movies set ... where MovieId={id}
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


        public async Task<ServiceResponse> AddMovie(MovieDto movieDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Movie
            Movie Movie = new Movie()
            {
                MovieID = Convert.ToInt32(movieDto.MovieID),
                MovieTitle = movieDto.MovieTitle,
                MovieReleaseDate = movieDto.MovieReleaseDate,
                MovieDuration = movieDto.MovieDuration,
                MovieDescription = movieDto.MovieDescription,
                MovieBudget = movieDto.MovieBoxOfficeCollection,
                MovieBoxOfficeCollection = movieDto.MovieBoxOfficeCollection,
                MovieRating = movieDto.MovieRating,
                MovieAwardNomination = movieDto.MovieAwardNomination,
                MovieAwardWin = movieDto.MovieAwardWin,
                MovieCost = movieDto.MovieCost,
                StudioID = movieDto.StudioID
            };
            
            // SQL Equivalent: Insert into Movies (..) values (..)

            try
            {
                _context.Movies.Add(Movie);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Movie.");
                serviceResponse.Messages.Add(ex.Message);
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Movie.MovieID;
            return serviceResponse;

        }


        public async Task<ServiceResponse> DeleteMovie(int id)
        {
            ServiceResponse response = new();

            // Movie Item must exist in the first place
            var Movie = await _context.Movies.FindAsync(id);

            if (Movie == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Movie cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Movies.Remove(Movie);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the movie");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }


        public async Task<IEnumerable<MovieDto>> ListMoviesForActor(int id)
        {
            // join MovieActor on movies.movieid = MovieActor.movieid WHERE MovieActor.actorid = {id}
            List<Movie> Movies = await _context.Movies
                .Include(m => m.Studio)
                .Where(m => m.Actors.Any(a => a.ActorId == id))
                .ToListAsync();

            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();

            // foreach Movie Item record in database
            foreach (Movie Movie in Movies)
            {
                // create new instance of MovieDto, add to list
                MovieDtos.Add(new MovieDto()
                {
                    MovieID = Movie.MovieID,
                    MovieTitle = Movie.MovieTitle,
                    MovieReleaseDate = Movie.MovieReleaseDate,
                    MovieDuration = Movie.MovieDuration,
                    MovieDescription = Movie.MovieDescription,
                    MovieBudget = Movie.MovieBoxOfficeCollection,
                    MovieBoxOfficeCollection = Movie.MovieBoxOfficeCollection,
                    MovieRating = Movie.MovieRating,
                    MovieAwardNomination = Movie.MovieAwardNomination,
                    MovieAwardWin = Movie.MovieAwardWin,
                    MovieCost = Movie.MovieCost,
                    StudioID = Movie.StudioID,
                    MovieStudioName = Movie.Studio.StudioName
                });
            }

            // return MovieDtos
            return MovieDtos;

        }


        public async Task<IEnumerable<MovieDto>> ListMoviesForStudio(int id)
        {
            List<Movie> Movies = await _context.Movies
                .Include(m => m.Studio)
                .Where(m => m.StudioID== id)
                .ToListAsync();

            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();

            // foreach Movie Item record in database
            foreach (Movie Movie in Movies)
            {
                // create new instance of MovieDto, add to list
                MovieDtos.Add(new MovieDto()
                {
                    MovieID = Movie.MovieID,
                    MovieTitle = Movie.MovieTitle,
                    MovieReleaseDate = Movie.MovieReleaseDate,
                    MovieDuration = Movie.MovieDuration,
                    MovieDescription = Movie.MovieDescription,
                    MovieBudget = Movie.MovieBoxOfficeCollection,
                    MovieBoxOfficeCollection = Movie.MovieBoxOfficeCollection,
                    MovieRating = Movie.MovieRating,
                    MovieAwardNomination = Movie.MovieAwardNomination,
                    MovieAwardWin = Movie.MovieAwardWin,
                    MovieCost = Movie.MovieCost,
                    StudioID = Movie.StudioID,
                    MovieStudioName = Movie.Studio.StudioName
                });
            }

            // return MovieDtos
            return MovieDtos;

        }


        public async Task<IEnumerable<MovieDto>> ListMoviesForCustomer(int id)
        {
            // join MovieActor on movies.movieid = MovieActor.movieid WHERE MovieActor.actorid = {id}
            List<Movie> Movies = await _context.Movies
                .Include(m => m.Studio)
                .Where(m => m.Customers.Any(c => c.CustomerId == id))
                .ToListAsync();

            // empty list of data transfer object MovieDto
            List<MovieDto> MovieDtos = new List<MovieDto>();

            // foreach Movie Item record in database
            foreach (Movie Movie in Movies)
            {
                // create new instance of MovieDto, add to list
                MovieDtos.Add(new MovieDto()
                {
                    MovieID = Movie.MovieID,
                    MovieTitle = Movie.MovieTitle,
                    MovieReleaseDate = Movie.MovieReleaseDate,
                    MovieDuration = Movie.MovieDuration,
                    MovieDescription = Movie.MovieDescription,
                    MovieBudget = Movie.MovieBoxOfficeCollection,
                    MovieBoxOfficeCollection = Movie.MovieBoxOfficeCollection,
                    MovieRating = Movie.MovieRating,
                    MovieAwardNomination = Movie.MovieAwardNomination,
                    MovieAwardWin = Movie.MovieAwardWin,
                    MovieCost = Movie.MovieCost,
                    StudioID = Movie.StudioID,
                    MovieStudioName = Movie.Studio.StudioName
                });
            }

            // return MovieDtos
            return MovieDtos;

        }


        public async Task<ServiceResponse> LinkMovieToActor(int movieId, int actorId)
        {
            ServiceResponse serviceResponse = new();

            Movie? movie = await _context.Movies
                .Include(m => m.Actors)
                .Where(m => m.MovieID == movieId)
                .FirstOrDefaultAsync();
            Actor? actor = await _context.Actors.FindAsync(actorId);

            // Data must link to a valid entity
            if (actor == null || movie == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (actor == null)
                {
                    serviceResponse.Messages.Add("Actor was not found. ");
                }
                if (movie == null)
                {
                    serviceResponse.Messages.Add("Movie was not found.");
                }
                return serviceResponse;
            }
            try
            {
                movie.Actors.Add(actor);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue linking the actor to the movie");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkMovieFromActor(int movieId, int actorId)
        {
            ServiceResponse serviceResponse = new();

            Movie? movie = await _context.Movies
                .Include(m => m.Actors)
                .Where(m => m.MovieID == movieId)
                .FirstOrDefaultAsync();
            Actor? actor = await _context.Actors.FindAsync(actorId);

            // Data must link to a valid entity
            if (actor == null || movie == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (actor == null)
                {
                    serviceResponse.Messages.Add("Actor was not found.");
                }
                if (movie == null)
                {
                    serviceResponse.Messages.Add("Movie was not found.");
                }
                return serviceResponse;
            }
            try
            {
                movie.Actors.Remove(actor);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue unlinking the actor to the movie");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }


        public async Task<ServiceResponse> LinkMovieToCustomer(int movieId, int customerId)
        {
            ServiceResponse serviceResponse = new();

            Movie? movie = await _context.Movies
                .Include(m => m.Customers)
                .Where(m => m.MovieID == movieId)
                .FirstOrDefaultAsync();
            Customer? customer = await _context.Customers.FindAsync(customerId);

            // Data must link to a valid entity
            if (customer == null || movie == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (customer == null)
                {
                    serviceResponse.Messages.Add("Customer was not found. ");
                }
                if (movie == null)
                {
                    serviceResponse.Messages.Add("Movie was not found.");
                }
                return serviceResponse;
            }
            try
            {
                movie.Customers.Add(customer);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue linking the customer to the movie");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkMovieFromCustomer(int movieId, int customerId)
        {
            ServiceResponse serviceResponse = new();

            Movie? movie = await _context.Movies
                .Include(m => m.Customers)
                .Where(m => m.MovieID == movieId)
                .FirstOrDefaultAsync();
            Customer? customer = await _context.Customers.FindAsync(customerId);

            // Data must link to a valid entity
            if (customer == null || movie == null)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.NotFound;
                if (customer == null)
                {
                    serviceResponse.Messages.Add("Customer was not found.");
                }
                if (movie == null)
                {
                    serviceResponse.Messages.Add("Movie was not found.");
                }
                return serviceResponse;
            }
            try
            {
                movie.Customers.Remove(customer);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue unlinking the customer to the movie");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }


    }
}

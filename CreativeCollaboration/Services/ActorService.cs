using Microsoft.EntityFrameworkCore;
using CreativeCollaboration.Data;
using CreativeCollaboration.Interfaces;
using CreativeCollaboration.Models;

namespace CreativeCollaboration.Services
{
    public class ActorService : IActorService
    {
        private readonly ApplicationDbContext _context;
        
        // dependency injection of database context
        public ActorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActorDto>> ListActors()
        {
            // all actors
            IEnumerable<Actor> Actors = await _context.Actors
                 .ToListAsync();

            // empty list of data transfer object ActorDto
            List<ActorDto> ActorDtos = new List<ActorDto>();

            // foreach Actor Item record in database
            foreach (Actor Actor in Actors)
            {
                // create new instance of ActorDto, add to list
                ActorDtos.Add(new ActorDto()
                {
                    ActorId = Actor.ActorId,
                    ActorName = Actor.ActorName,
                    ActorDOB = Actor.ActorDOB,
                    ActorBirthPlace = Actor.ActorBirthPlace,
                    ActorGender = Actor.ActorGender,
                    ActorNationality = Actor.ActorNationality,
                    ActorRole = Actor.ActorRole,
                    ActorAwardWon = Actor.ActorAwardWon,
                    ActorDebutYear = Actor.ActorDebutYear,
                    ActorNetWorth = Actor.ActorNetWorth
                });
            }

            // return ActorDtos
            return ActorDtos;
        }

        public async Task<ActorDto?> FindActor(int id)
        {
            // first or default async will get the first actor item matching the {id}
            var Actor = await _context.Actors.FirstOrDefaultAsync(a => a.ActorId == id);

            // no actor found
            if (Actor == null)
            {
                return null;
            }

            ActorDto ActorDto = new ActorDto()
            {
                ActorId = Actor.ActorId,
                ActorName = Actor.ActorName,
                ActorDOB = Actor.ActorDOB,
                ActorBirthPlace = Actor.ActorBirthPlace,
                ActorGender = Actor.ActorGender,
                ActorNationality = Actor.ActorNationality,
                ActorRole = Actor.ActorRole,
                ActorAwardWon = Actor.ActorAwardWon,
                ActorDebutYear = Actor.ActorDebutYear,
                ActorNetWorth = Actor.ActorNetWorth
            };
            return ActorDto;
        }


        public async Task<ServiceResponse> UpdateActor(ActorDto actorDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of an Actor
            Actor actor = new Actor()
            {
                ActorId = Convert.ToInt32(actorDto.ActorId),
                ActorName = actorDto.ActorName,
                ActorDOB = actorDto.ActorDOB,
                ActorBirthPlace = actorDto.ActorBirthPlace,
                ActorGender = actorDto.ActorGender,
                ActorNationality = actorDto.ActorNationality,
                ActorRole = actorDto.ActorRole,
                ActorAwardWon = actorDto.ActorAwardWon,
                ActorDebutYear = actorDto.ActorDebutYear,
                ActorNetWorth = actorDto.ActorNetWorth
            };

            // flags that the object has changed
            _context.Entry(actor).State = EntityState.Modified;

            try
            {
                // SQL Equivalent: Update Actors set ... where ActorId={id}
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


        public async Task<ServiceResponse> AddActor(ActorDto actorDto)
        {
            ServiceResponse serviceResponse = new();

            // Create instance of Actor
            Actor Actor = new Actor()
            {
                ActorId = Convert.ToInt32(actorDto.ActorId),
                ActorName = actorDto.ActorName,
                ActorDOB = actorDto.ActorDOB,
                ActorBirthPlace = actorDto.ActorBirthPlace,
                ActorGender = actorDto.ActorGender,
                ActorNationality = actorDto.ActorNationality,
                ActorRole = actorDto.ActorRole,
                ActorAwardWon = actorDto.ActorAwardWon,
                ActorDebutYear = actorDto.ActorDebutYear,
                ActorNetWorth = actorDto.ActorNetWorth
            };
            
            // SQL Equivalent: Insert into Actors (..) values (..)

            try
            {
                _context.Actors.Add(Actor);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Status = ServiceResponse.ServiceStatus.Error;
                serviceResponse.Messages.Add("There was an error adding the Actor.");
                serviceResponse.Messages.Add(ex.Message);
            }

            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            serviceResponse.CreatedId = Actor.ActorId;
            return serviceResponse;

        }

        public async Task<ServiceResponse> DeleteActor(int id)
        {
            ServiceResponse response = new();

            // Actor Item must exist in the first place
            var Actor = await _context.Actors.FindAsync(id);
            if (Actor == null)
            {
                response.Status = ServiceResponse.ServiceStatus.NotFound;
                response.Messages.Add("Actor cannot be deleted because it does not exist.");
                return response;
            }

            try
            {
                _context.Actors.Remove(Actor);
                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                response.Status = ServiceResponse.ServiceStatus.Error;
                response.Messages.Add("Error encountered while deleting the actor");
                return response;
            }

            response.Status = ServiceResponse.ServiceStatus.Deleted;

            return response;

        }


        public async Task<IEnumerable<ActorDto>> ListActorsForMovie(int id)
        {
            // join ActorMovie on actors.actorid = ActorMovie.actorid WHERE ActorMovie.movieid = {id}
            List<Actor> Actors = await _context.Actors
                .Where(a => a.Movies.Any(m => m.MovieID == id))
                .ToListAsync();

            // empty list of data transfer object ActorDto
            List<ActorDto> ActorDtos = new List<ActorDto>();

            // foreach Actor Item record in database
            foreach (Actor Actor in Actors)
            {
                // create new instance of ActorDto, add to list
                ActorDtos.Add(new ActorDto()
                {
                    ActorId = Actor.ActorId,
                    ActorName = Actor.ActorName,
                    ActorDOB = Actor.ActorDOB,
                    ActorBirthPlace = Actor.ActorBirthPlace,
                    ActorGender = Actor.ActorGender,
                    ActorNationality = Actor.ActorNationality,
                    ActorRole = Actor.ActorRole,
                    ActorAwardWon = Actor.ActorAwardWon,
                    ActorDebutYear = Actor.ActorDebutYear,
                    ActorNetWorth = Actor.ActorNetWorth
                });
            }

            // return ActorDtos
            return ActorDtos;

        }

        public async Task<ServiceResponse> LinkActorToMovie(int actorId, int movieId)
        {
            ServiceResponse serviceResponse = new();
           
            Actor? actor = await _context.Actors
                           .Include(a => a.Movies)
                           .Where(a => a.ActorId == actorId)
                           .FirstOrDefaultAsync();
            Movie? movie = await _context.Movies.FindAsync(movieId);

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
                actor.Movies.Add(movie);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue linking the movie to the actor");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Created;
            return serviceResponse;
        }

        public async Task<ServiceResponse> UnlinkActorFromMovie(int actorId, int movieId)
        {
            ServiceResponse serviceResponse = new();

            Actor? actor = await _context.Actors
                .Include(a => a.Movies)
                .Where(a => a.ActorId == actorId)
                .FirstOrDefaultAsync();
            Movie? movie = await _context.Movies.FindAsync(movieId);

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
                actor.Movies.Remove(movie);
                _context.SaveChanges();
            }
            catch (Exception Ex)
            {
                serviceResponse.Messages.Add("There was an issue unlinking the movie to the actor");
                serviceResponse.Messages.Add(Ex.Message);
            }


            serviceResponse.Status = ServiceResponse.ServiceStatus.Deleted;
            return serviceResponse;
        }

    }
}

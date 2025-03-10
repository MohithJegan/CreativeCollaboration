# CreativeCollaboration

## Project Overview
The Creative Collaboration Project is an innovative web application that merges two distinct domains—Movie Management System and Restaurant Management System—to demonstrate entity relationships using ASP.NET Core Web API.  

## Controllers and Views  
The application contains the following main controllers:
- **Actor** (for Movie Management)
- **Movie** (for Movie Management)
- **Studio** (for Movie Management)
- **Customer** (for Restaurant Management)
- **OrderItem** (for Restaurant Management)
- **Order** (for Restaurant Management)
- **MenuItem** (for Restaurant Management)

## Controllers and Views
The application contains three main controllers: **Actor**, **Movie**, and **Studio**. Each controller manages its respective entity and provides views for displaying data.

### **Actor Controller**
The ActorPage contains the following views:

- **ActorPage/List.cshtml**: Displays a list of all actors.
- **ActorPage/Details.cshtml**: Displays detailed information about a specific actor.
- **ActorPage/New.cshtml**: Displays a form to add a new actor.
- **ActorPage/ConfirmDelete.cshtml**: Displays a delete confirmation page where the user can either confirm the deletion of an actor or cancel to go back without performing the delete operation.
- **ActorPage/Edit.cshtml**: Displays a form to update an actor in the database.

### **Movie Controller**
The MoviePage contains the following views:

- **MoviePage/List.cshtml**: Displays a list of all movies.
- **MoviePage/Details.cshtml**: Displays detailed information about a specific movie.
- **MoviePage/New.cshtml**: Displays a form to add a new movie.
- **MoviePage/ConfirmDelete.cshtml**: Displays a delete confirmation page where the user can either confirm the deletion of a movie or cancel to go back without performing the delete operation.
- **MoviePage/Edit.cshtml**: Displays a form to update a movie in the database.

### **Studio Controller**
The StudioPage contains the following views:

- **StudioPage/List.cshtml**: Displays a list of all studios.
- **StudioPage/Details.cshtml**: Displays detailed information about a specific studio.
- **StudioPage/New.cshtml**: Displays a form to add a new studio.
- **StudioPage/ConfirmDelete.cshtml**: Displays a delete confirmation page where the user can either confirm the deletion of a studio or cancel to go back without performing the delete operation.
- **StudioPage/Edit.cshtml**: Displays a form to update a studio in the database.

## Entity Models
- **Actor.cs**: Represents actors and their attributes.
- **Movie.cs**: Represents movies and their attributes.
- **Studio.cs**: Represents studios and their attributes.

## Interfaces
- **IActorService.cs**: Defines logic for actors.
- **IMovieService.cs**: Defines logic for movies.
- **IStudioService.cs**: Defines logic for studios.

## Services
- **ActorService.cs**: Implements logic for actors, including linking/unlinking movies.
- **MovieService.cs**: Implements logic for movies, including linking/unlinking actors.
- **StudioService.cs**: Implements logic for studios.

## API Endpoints

### **Actor API**
- `GET /api/Actors/List` - Retrieves a list of all actors.
- `GET /api/Actors/Find/{id}` - Retrieves a specific actor by ID.
- `PUT /api/Actors/Update/{id}` - Updates an existing actor.
- `POST /api/Actors/Add` - Adds a new actor.
- `DELETE /api/Actors/Delete/{id}` - Deletes an actor by ID.
- `GET /api/Actors/ListActorsForMovie/{id}` - Retrieves actors associated with a specific movie.
- `POST /api/Actors/Link?actorId={id}&movieId={id}` - Links a movie to an actor.
- `DELETE /api/Actors/Unlink?actorId={id}&movieId={id}` - Unlinks a movie from an actor.

### **Movie API**
- `GET /api/Movies/List` - Retrieves a list of all movies.
- `GET /api/Movies/Find/{id}` - Retrieves a specific movie by ID.
- `PUT /api/Movies/Update/{id}` - Updates an existing movie.
- `POST /api/Movies/Add` - Adds a new movie.
- `DELETE /api/Movies/Delete/{id}` - Deletes a movie by ID.
- `GET /api/Movies/ListMoviesForActor/{id}` - Retrieves movies associated with a specific actor.
- `GET /api/Movies/ListMoviesForStudio/{id}` - Retrieves movies associated with a specific studio.
- `POST /api/Movies/Link?movieId={id}&actorId={id}` - Links an actor to a movie.
- `DELETE /api/Movies/Unlink?movieId={id}&actorId={id}` - Unlinks an actor from a movie.

### **Studio API**
- `GET /api/Studios/List` - Retrieves a list of all studios.
- `GET /api/Studios/Find/{id}` - Retrieves a specific studio by ID.
- `PUT /api/Studios/Update/{id}` - Updates an existing studio.
- `POST /api/Studios/Add` - Adds a new studio.
- `DELETE /api/Studios/Delete/{id}` - Deletes a studio by ID.
- `GET /api/Studios/ListStudioForMovie/{id}` - Retrieves the studio associated with a specific movie.



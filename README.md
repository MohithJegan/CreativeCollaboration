# Movie and Restaurant Management

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
The application contains six main controllers: **Actor**, **Movie**, **Studio**, **Customer**, **Order**, and **MenuItem**. Each controller manages its respective entity and provides views for displaying data.

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

### **Customer Controller**
- **CustomerPage/List.cshtml**: Displays a list of all customers.
- **CustomerPage/Details.cshtml**: Displays detailed information about a specific customer.
- **CustomerPage/New.cshtml**: Displays a form to add a new customer.
- **CustomerPage/ConfirmDelete.cshtml**: Displays a delete confirmation page where the user can either confirm the deletion of a customer or cancel to go back without performing the delete operation.
- **CustomerPage/Edit.cshtml**: Displays a form to update a customer's information in the database.

### **Order Controller**

- **OrderPage/List.cshtml**: Displays a list of all orders.
- **OrderPage/Details.cshtml**: Displays detailed information about a specific order, including customer details and ordered items.
- **OrderPage/New.cshtml**: Displays a form to create a new order by selecting a customer and adding items.
- **OrderPage/ConfirmDelete.cshtml**: Displays a delete confirmation page where the user can either confirm the deletion of an order or cancel to go back without performing the delete operation.
- **OrderPage/Edit.cshtml**: Displays a form to update order details, such as modifying items or customer information.

### **MenuItem Controller**
- **MenuItemPage/List.cshtml**: Displays a list of all menu items available in the restaurant.
- **MenuItemPage/Details.cshtml**: Displays detailed information about a specific menu item, including its name, description, price, and category.
- **MenuItemPage/New.cshtml**: Displays a form to add a new menu item to the system.
- **MenuItemPage/ConfirmDelete.cshtml**: Displays a delete confirmation page where the user can either confirm the deletion of a menu item or cancel to go back without performing the delete operation.
- **MenuItemPage/Edit.cshtml**: Displays a form to update an existing menu item, including modifying its price, name, or description.


## Entity Models
- **Actor.cs**: Represents actors and their attributes.
- **Movie.cs**: Represents movies and their attributes.
- **Studio.cs**: Represents studios and their attributes.
- **Order.cs**: Represents customer orders, including order details such as date, total price, and associated customer.
- **MenuItem.cs**: Represents menu items available in the restaurant, including name, price, category, and description.
- **Customer.cs**: Represents customers and their details, including name, contact information, and order history.

## Interfaces
- **IActorService.cs**: Defines logic for actors.
- **IMovieService.cs**: Defines logic for movies.
- **IStudioService.cs**: Defines logic for studios.
- **IOrderService.cs**: Defines logic for handling orders, including creation, updating, and retrieval.
- **IMenuItemService.cs**: Defines logic for managing menu items, such as adding, updating, and retrieving items.
- **ICustomerService.cs**: Defines logic for handling customers, including registration, updates, and retrieving customer details.

## Services
- **ActorService.cs**: Implements logic for actors, including linking/unlinking movies.
- **MovieService.cs**: Implements logic for movies, including linking/unlinking actors and customers.
- **StudioService.cs**: Implements logic for studios.
- **OrderService.cs**: Implements logic for orders, including processing new orders and updating order statuses.
- **MenuItemService.cs**: Implements logic for menu items, such as managing availability, pricing, and descriptions.
- **CustomerService.cs**: Implements logic for managing customers, including creating, updating, and retrieving customer information.

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
- `POST /api/Movies/LinkMovie?movieId={id}&customerId={id}` - Links a customer to a movie.
- `DELETE /api/Movies/UnlinkMovie?movieId={id}&customerId={id}` - Unlinks a customer from a movie.

### **Studio API**
- `GET /api/Studios/List` - Retrieves a list of all studios.
- `GET /api/Studios/Find/{id}` - Retrieves a specific studio by ID.
- `PUT /api/Studios/Update/{id}` - Updates an existing studio.
- `POST /api/Studios/Add` - Adds a new studio.
- `DELETE /api/Studios/Delete/{id}` - Deletes a studio by ID.
- `GET /api/Studios/ListStudioForMovie/{id}` - Retrieves the studio associated with a specific movie.

### **Customer API**
- `GET /api/Customers/List` - Retrieves a list of all customers.
- `GET /api/Customers/Find/{id}` - Retrieves a specific customer by ID.
- `PUT /api/Customers/Update/{id}` - Updates an existing customer's details.
- `POST /api/Customers/Add` - Adds a new customer.
- `DELETE /api/Customers/Delete/{id}` - Deletes a customer by ID.
- `GET /api/Customers/Orders/{id}` - Retrieves all orders placed by a specific customer.
- `POST /api/Customers/Link?customerId={id}&movieId={id}` - Links a customer to a movie.
- `DELETE /api/Customers/Unlink?customerId={id}&movieId={id}` - Unlinks a customer from a movie.

### **Order API**
- `GET /api/Orders/List` - Retrieves a list of all orders.
- `GET /api/Orders/Find/{id}` - Retrieves a specific order by ID.
- `PUT /api/Orders/Update/{id}` - Updates an existing order's details.
- `POST /api/Orders/Add` - Adds a new order.
- `DELETE /api/Orders/Delete/{id}` - Deletes an order by ID.
- `GET /api/Orders/Items/{id}` - Retrieves all menu items associated with a specific order.

### **MenuItem API**
- `GET /api/MenuItems/List` - Retrieves a list of all menu items.
- `GET /api/MenuItems/Find/{id}` - Retrieves a specific menu item by ID.
- `PUT /api/MenuItems/Update/{id}` - Updates an existing menu item.
- `POST /api/MenuItems/Add` - Adds a new menu item.
- `DELETE /api/MenuItems/Delete/{id}` - Deletes a menu item by ID.

## Contributors

### Mohith Krishnamoorthy Jeganathan
- `Customers Component` - CustomersController.cs, CustomersPageController.cs, ICustomerService.cs, Customer.cs, CustomerService.cs, CustomerPage(AddCustomer.cshtml,DeleteCustomer.cshtml,Details.cshtml,Edit.cshtml,List.cshtml)
- `Movies Component` - MovieController.cs, MoviePageController.cs, IMovieService.cs, Movie.cs, MovieService.cs, MoviePage(New.cshtml,ConsfirmDelete.cshtml,Details.cshtml,Edit.cshtml,List.cshtml)
- `Orders Component` - OrdersController.cs, OrdersPageController.cs, IOrderService.cs, Order.cs, OrderService.cs, OrderPage(Add.cshtml,Delete.cshtml,Details.cshtml,Edit.cshtml,List.cshtml)
- `MenuItems Component` - MenuItemsController.cs, MenuItemsPageController.cs, IMenuItemService.cs, MenuItemSevice.cs, MenuItemPage(Add.cshtml,Delete.cshtml,Details.cshtml,Edit.cshtml,List.cshtml)
- Feature Enhancement: Added additional search functionality, integration of Administration, Role Authorization, Login functionality, Conditional Access, Conditional Rendering and styled the application to improve the user experience.

### Srivignesh Kavle Sathyanarayanan
- `Customers Component` - CustomerPage(AddCustomer.cshtml,DeleteCustomer.cshtml,Details.cshtml,Edit.cshtml,List.cshtml)
- `MenuItems Component` - MenuItemPage(Add.cshtml,Delete.cshtml,Details.cshtml,Edit.cshtml,List.cshtml)
- `Orders Component` - OrderPage(Add.cshtml,Delete.cshtml,Details.cshtml,Edit.cshtml,List.cshtml)
- Feature Enhancement: Added additional interactive functionality, such as integration of TinyMCE rich text editor to enhance text input areas and improve user experience across the application.

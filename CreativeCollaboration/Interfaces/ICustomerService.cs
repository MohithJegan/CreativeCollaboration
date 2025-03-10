using CreativeCollaboration.Models;

namespace CreativeCollaboration.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> ListCustomers();
        Task<CustomerDto?> FindCustomer(int id);
        Task<ServiceResponse> AddCustomer(AUCustomerDto addCustomerDto);
        Task<ServiceResponse> UpdateCustomer(int id, AUCustomerDto updateCustomerDto);
        Task<ServiceResponse> DeleteCustomer(int id);

        // Related CRUD
        Task<IEnumerable<CustomerDto>> ListCustomersForMovie(int id);
      
        Task<ServiceResponse> LinkCustomerToMovie(int customerId, int movieId);

        Task<ServiceResponse> UnlinkCustomerFromMovie(int customerId, int movieId);
    }
}

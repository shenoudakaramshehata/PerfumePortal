using CRM.Repository.GenericRepo;
using CRM.Models;
using CRM.Data;
namespace CRM.Repository.UserRepo
{
    public interface IUserRepo : IGenericRepo<ApplicationUser> 
    {

        ApplicationUser? GetUserById(int Id);

        List<ApplicationUser> GetAllUsers();
    }
}

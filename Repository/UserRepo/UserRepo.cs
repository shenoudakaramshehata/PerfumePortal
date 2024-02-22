using CRM.Data;
using CRM.Repository.GenericRepo;
using System.Collections.Generic;
using CRM.Models;

namespace CRM.Repository.UserRepo
{
    public class UserRepo : GenericRepo<ApplicationUser>, IUserRepo
    {
        private readonly ApplicationDbContext _context;

        public UserRepo(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public List<ApplicationUser> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public ApplicationUser? GetUserById(int Id)
        {
            return _context.Users.Find(Id);
        }
    }
}

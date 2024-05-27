using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exemplo_Identity1
{
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }
    }
}

using Microsoft.EntityFrameworkCore;
using UserManagmentSolution.Data;

namespace UserManagmentSolution.Installers
{
    public class DbInstaller : IServiceInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }
    }
}

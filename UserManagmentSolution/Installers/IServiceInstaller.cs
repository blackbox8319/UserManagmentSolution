using Microsoft.Extensions.DependencyInjection;

namespace UserManagmentSolution.Installers
{
    public interface IServiceInstaller
    {
        void InstallServices(IServiceCollection services, IConfiguration configuration);
    }
}

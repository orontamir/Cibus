using CibusServer.DAL.SQL;

namespace CibusServer.Services
{
    public abstract class DALService
    {
        protected RepositoryBase Repository { get; }

        protected DALService(RepositoryBase repo)
        {
            Repository = repo;
        }
    }
}

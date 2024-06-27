using Zufanci.Client.Helpers;
using Zufanci.Client.Repository.IRepository;
using Zufanci.Shared;

namespace Zufanci.Client.Repository
{
    public class UnitRepositoryClient : IUnitRepository
    {
        private readonly IHttpService httpService;
        private string url = "api/units";

        public UnitRepositoryClient(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task CreateUnitAsync(Unit unit)
        {
            var response = await httpService.Post<Unit>(url, unit);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }

        public async Task<bool> DeleteUnitAsync(int Id)
        {
            var response = await httpService.Delete($"{url}/{Id}");
            if (response.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                return false;
            }
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return true;
        }

        public async Task<Unit> GetUnitAsync(int id)
        {
            var response = await httpService.Get<Unit>($"{url}/{id}");
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task<List<Unit>> GetUnitsAsync()
        {
            var response = await httpService.Get<List<Unit>>(url);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
            return response.Response;
        }

        public async Task UpdateUnitAsync(Unit unit)
        {
            var response = await httpService.Put<Unit>(url, unit);
            if (!response.Success)
            {
                throw new ApplicationException(await response.GetBody());
            }
        }
    }
}

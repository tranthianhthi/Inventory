using AutoMapper;
using OnlineInventoryAPI.Models;
using OnlineInventoryAPI.Models.DTO;
using OnlineInventoryAPI.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace OnlineInventoryAPI.Repositories
{
    public class OnlineStoreAuthenticateRepo : IOnlineStoreAuthenticateRepo<OnlineStoreAuthenticate, OnlineStoreAuthenticateDTO>
    {
        private readonly IMapper _mapper;

        private string cpoaConnectionString = "Data Source=" +
            "(" +
            "DESCRIPTION=" +
            "(" +
            "ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))" +
            ")" +
            "(" +
            "CONNECT_DATA=(SERVICE_NAME=rproods)" +
            ")" +
            ");" +
            "User Id=reportuser;Password=report;Connection Timeout=999";

        public OnlineStoreAuthenticateRepo(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<OnlineStoreAuthenticate> Get(string storeCode, string platform)
        {
            throw new NotImplementedException();
        }

        public void Update(OnlineStoreAuthenticateDTO entityToUpdate, OnlineStoreAuthenticate entity)
        {
            throw new NotImplementedException();
        }

        public void Update(OnlineStoreAuthenticate entityToUpdate, OnlineStoreAuthenticate entity)
        {
            throw new NotImplementedException();
        }
    }
}

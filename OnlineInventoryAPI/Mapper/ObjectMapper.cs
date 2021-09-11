using AutoMapper;
using OnlineInventoryAPI.Models;
using OnlineInventoryAPI.Models.DTO;

namespace OnlineInventoryLib.Mapper
{
    public class ObjectMapper : Profile
    {
        public ObjectMapper()
        {
            CreateMap<OnlineStoreAuthenticate, OnlineStoreAuthenticateDTO>().ReverseMap();
        }
    }
}

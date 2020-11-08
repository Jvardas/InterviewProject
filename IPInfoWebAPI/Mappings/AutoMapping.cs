using AutoMapper;
using IPInfoWebAPI.DataTransferObjects;
using IPInfoWebAPI.Models;

namespace IPInfoWebAPI.Mappings
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            // The map between the Model and my DTO
            CreateMap<IpDetail, IpDetailDTO>(); 
        }
    }
}

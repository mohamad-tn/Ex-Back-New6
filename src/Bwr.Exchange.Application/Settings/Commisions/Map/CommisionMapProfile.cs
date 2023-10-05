using AutoMapper;
using Bwr.Exchange.Settings.Commisions.Dto;

namespace Bwr.Exchange.Settings.Commisions.Map
{
    public class CommisionMapProfile:Profile
    {
        public CommisionMapProfile()
        {
            CreateMap<Commision, CommisionDto>();
            CreateMap<Commision, ReadCommisionDto>();
            CreateMap<Commision, CreateCommisionDto>();
            CreateMap<CreateCommisionDto, Commision>();
            CreateMap<Commision, UpdateCommisionDto>();
            CreateMap<UpdateCommisionDto, Commision>();
        }
    }
}

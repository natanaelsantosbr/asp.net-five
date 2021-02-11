using AutoMapper;
using Natanael.Contracts.V1.Requests.Queries;
using Natanael.Web.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Natanael.Web.MappingProfiles
{
    public class RequestToDomainProfile : Profile
    {
        public RequestToDomainProfile()
        {
            this.CreateMap<PaginationQuery, PaginationFilter>();
        }
    }
}

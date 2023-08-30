using AirTableDatabase.DBModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Navmii.AirTableSyncNetcore6.AirtablesModels;

namespace AirTableWebApi.AutoMapperProfile
{
    public class AirTableWebApiProfile: Profile
    {
        public AirTableWebApiProfile()
        {
            // Source -> Target
            CreateMap<ProjectForm, Project>();
            CreateMap<UserProjectRequest, UserProject>();
            CreateMap<Navmii.AirTableSyncNetcore6.AirtablesModels.Table, TableDto>();
            CreateMap<TableDto, Navmii.AirTableSyncNetcore6.AirtablesModels.Table>();
        }
    }
}

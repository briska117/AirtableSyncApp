using AirTableDatabase.DBModels;
using AutoMapper;

namespace AirTableWebApi.AutoMapperProfile
{
    public class AirTableWebApiProfile: Profile
    {
        public AirTableWebApiProfile()
        {
            // Source -> Target
            CreateMap<ProjectForm, Project>();
            CreateMap<UserProjectRequest, UserProject>();
        }
    }
}

using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ASI.Basecode.WebApp
{
    // AutoMapper configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configure auto mapper
        /// </summary>
        private void ConfigureAutoMapper()
        {
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProfileConfiguration());
            });

            this._services.AddSingleton<IMapper>(sp => mapperConfiguration.CreateMapper());
        }

        private class AutoMapperProfileConfiguration : Profile
        {
            public AutoMapperProfileConfiguration()
            {
                CreateMap<UserViewModel, User>();

                // Ticket Mappings
                CreateMap<CreateTicketDto, Ticket>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.SubmittedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.Status, opt => opt.Ignore())
                    .ForMember(dest => dest.ResolvedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.ResolvedBy, opt => opt.Ignore())
                    .ForMember(dest => dest.ResolutionDescription, opt => opt.Ignore())
                    .ForMember(dest => dest.AgentFeedback, opt => opt.Ignore());

                CreateMap<UpdateTicketDto, Ticket>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.SubmittedDate, opt => opt.Ignore())
                    .ForMember(dest => dest.SubmittedBy, opt => opt.Ignore());

                CreateMap<Ticket, TicketDto>();
            }
        }
    }
}

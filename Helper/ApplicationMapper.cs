using AutoMapper;
using SICProject.Models;

namespace SICProject.Helper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            CreateMap<Departmentmaster, DepartmentmasterVM>().ReverseMap();
            CreateMap<Registrationmaster, RegistrationmasterVM>().ReverseMap();
        }
    }
}

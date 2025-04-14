using AutoMapper;
using CleanArchitecture.Application.DTOs.Business;
using CleanArchitecture.Application.Features.Business.ExpenseCategories.Commands;
using CleanArchitecture.Application.Features.Business.ExpenseTypes.Commands;
using CleanArchitecture.Application.Features.Business.UserExpenses.Commands;
using CleanArchitecture.Domain.Entities.Business;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ExpenseType, ExpenseTypeDto>()
              .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()))
              .ForMember(dest => dest.ExpenseCategoryName, opt => opt.MapFrom(src => src.ExpenseCategory.CategoryName))
              .ForMember(dest => dest.UnitOfMeasurement, opt => opt.MapFrom(src => src.UnitOfMeasurement))
              .ForMember(dest => dest.ExpenseCategoryId, opt => opt.MapFrom(src => src.ExpenseCategory.CategoryId));

            CreateMap<CreateExpenseTypeDto, ExpenseType>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusEnum.Active));
            CreateMap<UpdateExpenseTypeDto, ExpenseType>();

            CreateMap<UpdateExpenseTypeCommand, ExpenseType>();

            CreateMap<StatusEnum, string>().ConvertUsing(src => src.ToString());




            CreateMap<ExpenseCategory, ExpenseCategoryDTO>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<UpdateExpenseCategoryCommand, ExpenseCategory>();


            CreateMap<CreateExpenseCategoryDto, ExpenseCategory>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusEnum.Active));

            CreateMap<UpdateExpenseCategoryDto, ExpenseCategory>();

            // Enum to String Mapping
            CreateMap<StatusEnum, string>().ConvertUsing(src => src.ToString());



            CreateMap<UserExpense, UserExpenseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ExpenseTypeName, opt => opt.MapFrom(src => src.ExpenseType.ExpenseName));

            CreateMap<CreateUserExpenseCommand, UserExpense>()
                .ForMember(dest => dest.ExpenseId, opt => opt.MapFrom(src => Guid.NewGuid()));

            CreateMap<UpdateUserExpenseDto, UserExpense>();

            CreateMap<StatusEnum, string>().ConvertUsing(src => src.ToString());

        }
    }
}
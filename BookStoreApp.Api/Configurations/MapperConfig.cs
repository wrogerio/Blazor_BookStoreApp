using AutoMapper;
using BookStoreApp.Api.Data;
using BookStoreApp.Api.Models.Author;
using BookStoreApp.Api.Models.Book;
using BookStoreApp.Api.Models.User;

namespace BookStoreApp.Api.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<Author, AuthorReadOnlyDto>().ReverseMap();
        CreateMap<Author, AuthorCreateDto>().ReverseMap();
        CreateMap<Author, AuthorUpdateDto>().ReverseMap();

        CreateMap<Book, BookReadOnlyDto>().ForMember(a => a.AuthorName, b => b.MapFrom(m => $"{m.Author!.FirstName} {m.Author!.LastName}")).ReverseMap();
        CreateMap<Book, BookDetailDto>().ForMember(a => a.AuthorName, b => b.MapFrom(m => $"{m.Author!.FirstName} {m.Author!.LastName}")).ReverseMap();
        CreateMap<Book, BookCreateDto>().ReverseMap();
        CreateMap<Book, BookUpdateDto>().ReverseMap();

        CreateMap<ApiUser, UserDto>().ReverseMap();
    }
}

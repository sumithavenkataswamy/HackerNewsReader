using AutoMapper;
using HackerNewsReader.Application.Models;
using HackerNewsReader.Domain.Entities;

namespace HackerNewsReader.Application.AutoMappers
{
    public class StoryProfile : Profile
    {
        public StoryProfile()
        {
            CreateMap<Story, StoryDto>().ReverseMap();
        }
    }
}

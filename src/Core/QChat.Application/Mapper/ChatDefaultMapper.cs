using AutoMapper;
using QChat.Application.Services.Chats.Commands;
using QChat.Application.Services.Chats.Queries;
using QChat.Application.Services.Users.Queries;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

namespace QChat.Application.Mapper;

public class ChatDefaultMapper : Profile
{
    public ChatDefaultMapper()
    {
        CreateMap<NewChatCommand, Chat>();

        CreateProjection<Chat, ChatBreifDto>();

        CreateMap<Message, MessageBriefDto>()
            .ForMember(e => e.Username, i => i.MapFrom(s => s.User.UserName))
            .ForMember(e => e.PostDate, i => i.MapFrom(s => s.PostDate.ToAproximateDate()));

        CreateMap<Chat, ChatDetailedDto>()
            .ForMember(e=>e.Messages,i=>i.MapAtRuntime());

        CreateMap<Chat, ChatInfoDto>()
            .ForMember(e => e.Members, i => i.MapFrom(c => c.UserChats.Select(x => x.User.UserName).ToList()));

        CreateMap<NewMessageCommand, Message>()
            .ForMember(e => e.ChatId, i => i.MapFrom(o => long.Parse(o.ChatId)))
            .ForMember(e => e.UserId, i => i.MapFrom(o => o.UserId.ToGuid()))
            .ForMember(e => e.Content, i => i.MapFrom(o => o.MessageBody));
    }

}

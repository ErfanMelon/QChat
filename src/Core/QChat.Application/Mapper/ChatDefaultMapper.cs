using AutoMapper;
using QChat.Application.Services.Chats.Commands;
using QChat.Application.Services.Chats.Queries;
using QChat.Application.Services.Users.Queries;
using QChat.Domain.Entities;

namespace QChat.Application.Mapper;

public class ChatDefaultMapper:Profile
{
	public ChatDefaultMapper()
	{
		CreateMap<NewChatCommand, Chat>();

		CreateProjection<Chat,ChatBreifDto>();

       CreateProjection<Message, MessageBriefDto>()
           .ForMember(e => e.Username, i => i.MapFrom(s => s.User.UserName));

		CreateMap<Chat, ChatDetailedDto>()
			.ForMember(d=>d.Messages,o=>o.MapFrom(s=>s.Messages.Select(e=>new MessageBriefDto
			{
				Content=e.Content,
				PostDate=e.PostDate,
				Username=e.User.UserName
			}).ToList()));
	}
}

using AutoMapper;
using QChat.Application.Services.Chats.Commands;
using QChat.Application.Services.Users.Queries;
using QChat.Domain.Entities;

namespace QChat.Application.Mapper;

public class ChatDefaultMapper:Profile
{
	public ChatDefaultMapper()
	{
		CreateMap<NewChatCommand, Chat>();
		CreateProjection<Chat,ChatBreifDto>();
	}
}

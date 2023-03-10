using AutoMapper;
using QChat.Application.Services.Users.Commands;
using QChat.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QChat.Common.Security;
using QChat.Application.Services.Users.Queries;

namespace QChat.Application.Mapper;

public class UserDefaultMapper:Profile
{
	public UserDefaultMapper()
	{
		CreateMap<CreateUserCommand, User>()
			.ForMember(e => e.UserName, i => i.MapFrom(p => p.UserName))
			.ForMember(e => e.Password, i => i.MapFrom(p => PasswordHasher.EncodePasswordMd5(p.PassWord)));

		CreateMap<CreateUserCommand, LoginUserCommand>()
				.ForMember(e => e.Username, i => i.MapFrom(p => p.UserName))
				.ForMember(e => e.Password, i => i.MapFrom(p => p.PassWord));
	}
}

using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace QChat.Application.Services.Chats.Commands;

public class NewPrivateChatCommand : IRequest<Result<long>>
{
    public NewPrivateChatCommand(string? userName, string? userId)
    {
        UserName = userName;
        UserId = userId;
    }
    public NewPrivateChatCommand()
    {

    }

    [Required(ErrorMessage = "شناسه کاربر را وارد کنید")]
    public string? UserName { get; set; }
    public string? UserId { get; set; }
    public class Validator : AbstractValidator<NewPrivateChatCommand>
    {
        private readonly IChatDbContext _context;
        public Validator(IChatDbContext context)
        {
            _context = context;
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.UserName)
                .NotEmpty().WithMessage("نام کاربری را وارد کنید")
                .MustAsync(ValidUser).WithMessage("کاربری یافت نشد");

            RuleFor(e => e.UserId)
                .NotEmpty()
                .MustAsync(ValidUserId).WithMessage("کاربری یافت نشد");
        }

        private Task<bool> ValidUserId(string arg1, CancellationToken arg2)
        {
            return _context.Users.AnyAsync(e => e.Id == arg1.ToGuid());
        }

        private Task<bool> ValidUser(string userName, CancellationToken arg2)
        {
            return _context.Users.AnyAsync(e => e.UserName.ToLower() == userName.ToLower());
        }
    }
    public class Handler : IRequestHandler<NewPrivateChatCommand, Result<long>>
    {
        private readonly IChatDbContext _context;

        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result<long>> Handle(NewPrivateChatCommand request, CancellationToken cancellationToken)
        {
            var thatUser = await _context.Users.SingleAsync(e => e.UserName.ToLower() == request.UserName.ToLower()); // Find another user by his/her username
            var thisUser = await _context.Users.SingleAsync(e => e.Id == request.UserId.ToGuid()); // get current user

            var oldChat = await _context.PrivateChats
                .SingleOrDefaultAsync(e => e.UserId1 == thisUser.Id && e.UserId2 == thatUser.Id);

            if (oldChat == null) // check for old chat between users
                oldChat = await _context.PrivateChats
                    .SingleOrDefaultAsync(e => e.UserId2 == thisUser.Id && e.UserId1 == thatUser.Id);

            if (oldChat != null) // if finds old chat return its id
                return oldChat.Id;

            var privateChat = new PrivateChat // create new PrivateChat
            {
                UserId1 = request.UserId.ToGuid(),
                UserId2 = thatUser.Id,
            };
            await _context.PrivateChats.AddAsync(privateChat); // Add PrivateChat To Context
            var user_chats = new List<UserChat>
            {
                new UserChat{Chat=privateChat,ChatId=privateChat.Id,User=thisUser,UserId=thisUser.Id},
                new UserChat{Chat=privateChat,ChatId=privateChat.Id,User=thatUser,UserId=thatUser.Id}
            }; // Create UserChats by above detail
            await _context.UserChats.AddRangeAsync(user_chats); // Add UserChats To Context
            await _context.SaveChangesAsync(cancellationToken);

            return privateChat.Id;
        }
    }
}

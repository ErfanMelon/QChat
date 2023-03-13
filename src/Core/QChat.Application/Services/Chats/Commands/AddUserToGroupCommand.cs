using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace QChat.Application.Services.Chats.Commands;

public class AddUserToGroupCommand : IRequest<Result>
{
    [Required(ErrorMessage ="شناسه کاربر را وارد کنید")]
    public string? UserName { get; set; }
    public long? ChatId { get; set; }
    public class Validator : AbstractValidator<AddUserToGroupCommand>
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

            RuleFor(e => e.ChatId)
                .NotEmpty().WithMessage("گروهی را انتخاب کنید")
                .MustAsync(ChatExist).WithMessage("گروه معتبر نیست");

            RuleFor(e => e)
                .MustAsync(NotInChat).WithMessage("کاربر قبلا عضو گروه بوده");
        }

        private async Task<bool> NotInChat(AddUserToGroupCommand command, CancellationToken arg2)
        {
            var userId = _context.Users
                .Where(e => e.UserName.ToLower() == command.UserName.ToLower())
                .Select(e => e.Id).Single();
            return !(await _context.UserChats.AnyAsync(e => e.ChatId == command.ChatId && e.UserId == userId));
        }

        private Task<bool> ChatExist(long? id, CancellationToken arg2)
        {
            return _context.Chats.AnyAsync(e => e.Id == id.Value);
        }

        private Task<bool> ValidUser(string userName, CancellationToken arg2)
        {
            return _context.Users.AnyAsync(e => e.UserName.ToLower() == userName.ToLower());
        }
    }
    public class Handler : IRequestHandler<AddUserToGroupCommand,Result>
    {
        private readonly IChatDbContext _context;
        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(AddUserToGroupCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .SingleAsync(e => e.UserName.ToLower() == request.UserName.ToLower());

            var chat = await _context.Chats
                .SingleAsync(e => e.Id == request.ChatId);

            var userChat = new UserChat
            {
                Chat = chat,
                ChatId = chat.Id,
                User = user,
                UserId = user.Id
            };

            await _context.UserChats.AddAsync(userChat);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}

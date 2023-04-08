using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

namespace QChat.Application.Services.Chats.Queries;

public class GetChatDetailQuery : IRequest<Result<ChatInfoDto>>
{
    public GetChatDetailQuery(long? chatId, string? userId)
    {
        ChatId = chatId;
        UserId = userId;
    }

    public long? ChatId { get; set; }
    public string? UserId { get; set; }
    public class Validator : AbstractValidator<GetChatDetailQuery>
    {
        private readonly IChatDbContext _context;
        public Validator(IChatDbContext context)
        {
            _context = context;

            ClassLevelCascadeMode = CascadeMode.Stop;

            RuleFor(e => e.UserId)
                .NotEmpty().WithMessage("کاربر معتبر نیست");

            RuleFor(e => e.ChatId)
                .NotEmpty().WithMessage("چت را انتخاب کنید");

            RuleFor(e => e)
                .MustAsync(UserAccessChat).WithMessage("خطا هنگام دریافت چت");
        }

        private Task<bool> UserAccessChat(GetChatDetailQuery query, CancellationToken arg2)
        {
            return _context.UserChats.AnyAsync(e => e.UserId == query.UserId.ToGuid() && e.ChatId == query.ChatId.Value);
        }
    }

    public class Handler : IRequestHandler<GetChatDetailQuery, Result<ChatInfoDto>>
    {
        private readonly IChatDbContext _context;

        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ChatInfoDto>> Handle(GetChatDetailQuery request, CancellationToken cancellationToken)
        {
            var chat = await
                _context.Chats
                .AsNoTracking()
                .Include(e => e.UserChats)
                .ThenInclude(e => e.User)
                .SingleAsync(e => e.Id == request.ChatId.Value);

            var chatInfo = (ChatInfoDto)chat;

            return chatInfo;
        }
    }
}
public class ChatInfoDto
{
    public List<string> Members { get; set; }
    public static explicit operator ChatInfoDto(Chat chat)
    {
        return new ChatInfoDto
        {
            Members = chat.UserChats.Select(e => e.User.UserName).ToList()
        };
    }
}
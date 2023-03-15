using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;

namespace QChat.Application.Services.Chats.Queries;

public class GetChatQuery : IRequest<Result<ChatDetailedDto>>
{
    public GetChatQuery(long? chatId, string? userId)
    {
        ChatId = chatId;
        UserId = userId;
    }

    public long? ChatId { get; set; }
    public string? UserId { get; set; }
    public class Validator : AbstractValidator<GetChatQuery>
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

        private Task<bool> UserAccessChat(GetChatQuery query, CancellationToken arg2)
        {
            return _context.UserChats.AnyAsync(e => e.UserId == query.UserId.ToGuid() && e.ChatId == query.ChatId.Value);
        }
    }
    public class Handler : IRequestHandler<GetChatQuery, Result<ChatDetailedDto>>
    {
        private readonly IChatDbContext _context;
        private readonly IMapper _mapper;

        public Handler(IChatDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<ChatDetailedDto>> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            var userchat = await _context.UserChats
                .AsNoTracking()
                .Include(e=>e.Chat)
                .ThenInclude(e=>e.Messages)
                .ThenInclude(e=>e.User)
                .SingleAsync(e => e.UserId == request.UserId.ToGuid() && e.ChatId == request.ChatId.Value);

            var chat = _mapper.Map<ChatDetailedDto>(userchat.Chat);
            return chat;
        }
    }
}
public class ChatDetailedDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? ImageSrc { get; set; }
    public List<MessageBriefDto>? Messages { get; set; }
}
public class MessageBriefDto
{
    public string Username { get; set; }
    public string PostDate { get; set; }
    public string Content { get; set; }
}

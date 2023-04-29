using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

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

        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ChatDetailedDto>> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            var chat = await _context.Chats
                .AsNoTracking()
                .Include(e=>e.UserChats)
                .ThenInclude(e=>e.User)
                .Include(e => e.Messages)
                .ThenInclude(e => e.User)
                .SingleAsync(e => e.Id == request.ChatId);

            ChatDetailedDto result = new ChatDetailedDto
            {
                Id = chat.Id,
                ImageSrc = chat.ImageSrc,
                Messages = chat.Messages.Select(e => (MessageBriefDto)e).ToList()
            };

            if (chat is PrivateChat)
                result.Title = chat.UserChats.First(e => e.UserId != request.UserId.ToGuid()).User.UserName;
            else
                result.Title = chat.Title;

            return result;
        }
    }
}
public class ChatDetailedDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? ImageSrc { get; set; }
    public List<MessageBriefDto>? Messages { get; set; }
    public static explicit operator ChatDetailedDto(UserChat userChat)
    {
        if (userChat.Chat is PrivateChat)
            return new ChatDetailedDto
            {
                Id = userChat.Chat.Id,
                ImageSrc = userChat.Chat.ImageSrc,
                Messages = userChat.Chat.Messages.Select(e => (MessageBriefDto)e).ToList(),
                Title = userChat.Chat
                .Messages
                .DistinctBy(e => e.UserId)
                .SingleOrDefault(e => e.UserId != userChat.UserId)?
                .User?
                .UserName
            };
        else
            return new ChatDetailedDto
            {
                Id = userChat.Chat.Id,
                ImageSrc = userChat.Chat.ImageSrc,
                Messages = userChat.Chat.Messages.Select(e => (MessageBriefDto)e).ToList(),
                Title = userChat.Chat.Title
            };
    }
}
public class MessageBriefDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string PostDate { get; set; }
    public string Content { get; set; }
    public bool IsFile { get; set; } = false;
    public string FilePath { get; set; }
    public static explicit operator MessageBriefDto(Message message)
    {
        return new MessageBriefDto
        {
            Content = message.Content,
            PostDate = message.PostDate.ToShamsi(),
            Username = message.User.UserName,
            Id=message.Id.ToString(),
            IsFile=message.FileSrc!=null,
            FilePath= message.FileSrc
        };
    }
}

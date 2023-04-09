using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Application.Services.Users.Queries;
using QChat.Common.ExtentionMethods;
using QChat.Domain.Entities;

namespace QChat.Application.Services.Chats.Queries;

public class SearchChatQuery : IRequest<Result<List<ChatBreifDto>>>
{
    public SearchChatQuery(string? searchKey, string? userId)
    {
        SearchKey = searchKey;
        UserId = userId;
    }

    public string? SearchKey { get; set; }
    public string? UserId { get; set; }
    public class Validator : AbstractValidator<SearchChatQuery>
    {
        public Validator()
        {
            RuleFor(e => e.UserId)
                .NotEmpty();

            RuleFor(e => e.SearchKey)
                .NotEmpty();
        }
    }
    public class Handler : IRequestHandler<SearchChatQuery, Result<List<ChatBreifDto>>>
    {
        private readonly IChatDbContext _context;

        public Handler(IChatDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<ChatBreifDto>>> Handle(SearchChatQuery request, CancellationToken cancellationToken)
        {
            var chatIds =await _context.UserChats
                .AsNoTracking()
                .Include(e => e.Chat)
                .Where(e => e.UserId == request.UserId.ToGuid())
                .Where(e => e.Chat.Title.ToLower().Contains(request.SearchKey.ToLower()))
                .Select(e=>e.ChatId)
                .ToListAsync();

            var chats = new List<ChatBreifDto>();
            foreach (long id in chatIds)
            {
                var chat = await _context.Chats.AsNoTracking().Include(e => e.UserChats).ThenInclude(e => e.User).SingleAsync(e => e.Id == id);
                string title = "";
                if (chat is PrivateChat)
                    title = chat.UserChats.DistinctBy(e => e.UserId).Single(e => e.UserId != request.UserId.ToGuid()).User.UserName;
                else
                    title = chat.Title;
                chats.Add(new ChatBreifDto
                {
                    Id = chat.Id,
                    Title = title,
                    ImageSrc = chat.ImageSrc
                });
            }
            return chats;
        }
    }
}

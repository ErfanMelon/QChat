using AutoMapper;
using CSharpFunctionalExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QChat.Application.Interfaces;
using QChat.Common.ExtentionMethods;

namespace QChat.Application.Services.Users.Queries;

public class GetChatsQuery : IRequest<Result<List<ChatBreifDto>>>
{
    public GetChatsQuery(string? userId)
    {
        UserId = userId;
    }

    public string? UserId { get; set; }
    public class Handler : IRequestHandler<GetChatsQuery, Result<List<ChatBreifDto>>>
    {
        private readonly IChatDbContext _context;
        private readonly IMapper _mapper;

        public Handler(IChatDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<ChatBreifDto>>> Handle(GetChatsQuery request, CancellationToken cancellationToken)
        {
            var chats = _context.UserChats
                .AsNoTracking()
                .Include(e => e.Chat)
                .Where(e => e.UserId == request.UserId.ToGuid())
                .Select(e => e.Chat);

            var result = _mapper.ProjectTo<ChatBreifDto>(chats);

            return result.ToList();
        }
    }
}
public class ChatBreifDto
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? ImageSrc { get; set; }
}

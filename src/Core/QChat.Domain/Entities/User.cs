﻿namespace QChat.Domain.Entities;

public class User:BaseEntity
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
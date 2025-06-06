﻿using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Cinemory.Models
{
    public class UserProfile
    {
        public int Id { get; set; } //primary key

        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime JoinDate { get; set; }

        public required string UserId { get; set; } //foreign key, User related 
        public AppUser? User { get; set; }  //navigation, reference to User, one to one relation

        //


    }
}

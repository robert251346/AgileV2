﻿using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TaskBoard.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        private bool seedDb = true;
        private User GuestUser { get; set; }
        private Board OpenBoard { get; set; }
        private Board InProgressBoard { get; set; }
        private Board DoneBoard { get; set; }
        private Board TestsBoard { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, bool seedDb = true)
            : base(options)
        {
            this.seedDb = seedDb;
            this.Database.EnsureCreated();
        }

        public DbSet<Board> Boards { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (seedDb)
            {
                builder
                .Entity<Task>()
                .HasOne(t => t.Board)
                .WithMany(b => b.Tasks)
                .HasForeignKey(t => t.BoardId)
                .OnDelete(DeleteBehavior.Restrict);

                SeedBoards();
                builder
                    .Entity<Board>()
                    .HasData(this.OpenBoard, this.InProgressBoard, this.DoneBoard);

                SeedUsers();
                builder.Entity<User>()
                    .HasData(this.GuestUser);

                builder
                    .Entity<Task>()
                    .HasData(new Task()
                    {
                        Id = 1,
                        Title = "Style CSS",
                        Description = "Poprawić wygląd interfejsu",
                        CreatedOn = DateTime.Now.AddDays(-200),
                        OwnerId = this.GuestUser.Id,
                        BoardId = this.OpenBoard.Id
                    },
                    new Task()
                    {
                        Id = 2,
                        Title = "Stworzyć aplikacje na androida",
                        Description = "Napisać aplikacje np w Javie i podłączyć do gotowego API",
                        CreatedOn = DateTime.Now.AddMonths(-5),
                        OwnerId = this.GuestUser.Id,
                        BoardId = this.OpenBoard.Id
                    },
                    new Task()
                    {
                        Id = 3,
                        Title = "Klient dla Windowsa",
                        Description = "Przygotować aplikacje na Windowsa np w C# albo VB",
                        CreatedOn = DateTime.Now.AddMonths(-1),
                        OwnerId = this.GuestUser.Id,
                        BoardId = this.InProgressBoard.Id
                    },
                    new Task()
                    {
                        Id = 4,
                        Title = "Dodać nowe zadania",
                        Description = "Zadania możesz przenosić między zakładkami dzięki edycji zadania",
                        CreatedOn = DateTime.Now.AddYears(-1),
                        OwnerId = this.GuestUser.Id,
                        BoardId = this.DoneBoard.Id
                    });
            }

            base.OnModelCreating(builder);
        }

        private void SeedUsers()
        {
            var hasher = new PasswordHasher<User>();

            this.GuestUser = new User()
            {
                UserName = "gosc",
                NormalizedUserName = "gosc",
                Email = "gosc@mail.com",
                NormalizedEmail = "gosc@MAIL.COM",
                FirstName = "gosc",
                LastName = "gosc",
            };

            this.GuestUser.PasswordHash = hasher.HashPassword(this.GuestUser, "gosc");
        }

        private void SeedBoards()
        {
            this.OpenBoard = new Board()
            {
                Id = 1,
                Name = "Nowe zadanie"
            };

            this.InProgressBoard = new Board()
            {
                Id = 2,
                Name = "W trakcie realizacji"
            };

            this.DoneBoard = new Board()
            {
                Id = 3,
                Name = "Gotowe"
            };

            this.TestsBoard = new Board()
            {
                Id = 3,
                Name = "Testy"
            };
        }
    }
}

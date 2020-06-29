﻿using Microsoft.EntityFrameworkCore;
using ResourceAPI.Models;

namespace ResourceAPI
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Author> Authors { get; set; }

        public static string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

    }
}
﻿using Brickweave.EventStore.SqlServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Brickweave.EventStore.SqlServer
{
    public class EventStoreDbContext : DbContext, IEventStore
    {
        public const string SCHEMA_NAME = "EventStore";

        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<EventData> Events { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            base.OnModelCreating(modelBuilder);
        }
    }
}

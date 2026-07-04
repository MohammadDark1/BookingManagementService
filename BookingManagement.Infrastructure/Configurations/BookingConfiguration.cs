using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingManagement.Infrastructure.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ResourceId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.UserId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(x => x.StartDateTime)
                   .IsRequired();

            builder.Property(x => x.EndDateTime)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .IsRequired();

            builder.Property(x => x.RowVersion)
                   .IsRowVersion();
        }
    }
}

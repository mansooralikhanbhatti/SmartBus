using Microsoft.EntityFrameworkCore;

namespace SmartBus.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<Bus> Buses { get; set; }
    public virtual DbSet<Cancellation> Cancellations { get; set; }
    public virtual DbSet<Route> Routes { get; set; }
    public virtual DbSet<Schedule> Schedules { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AEDB79ECD61");
            entity.Property(e => e.BookingDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus).HasMaxLength(20).HasDefaultValue("Paid");
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Confirmed");
            entity.Property(e => e.TotalFare).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Schedule).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK__Bookings__Schedu__440B1D61");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bookings__UserId__4316F928");
        });

        modelBuilder.Entity<Bus>(entity =>
        {
            entity.HasKey(e => e.BusId).HasName("PK__Buses__6A0F60B53369D391");
            entity.Property(e => e.BusNumber).HasMaxLength(20);
            entity.Property(e => e.BusType).HasMaxLength(50);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Cancellation>(entity =>
        {
            entity.HasKey(e => e.CancellationId).HasName("PK__Cancella__6A2D9A3A0A923776");
            entity.Property(e => e.CancellationDate).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.RefundAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RefundStatus).HasMaxLength(20).HasDefaultValue("Pending");

            entity.HasOne(d => d.Booking).WithMany(p => p.Cancellations)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__Cancellat__Booki__49C3F6B7");
        });

        modelBuilder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.RouteId).HasName("PK__Routes__80979B4D7E71CA5C");
            entity.Property(e => e.Destination).HasMaxLength(100);
            entity.Property(e => e.Distance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Source).HasMaxLength(100);
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Schedule__9C8A5B4931AAA4B6");
            entity.Property(e => e.ArrivalTime).HasColumnType("datetime");
            entity.Property(e => e.DepartureTime).HasColumnType("datetime");
            entity.Property(e => e.Fare).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Bus).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.BusId)
                .HasConstraintName("FK__Schedules__BusId__3C69FB99");

            entity.HasOne(d => d.Route).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.RouteId)
                .HasConstraintName("FK__Schedules__Route__3D5E1FD2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CBF77D153");
            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345C3F4B1E").IsUnique();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

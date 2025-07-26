using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeBookApi.DA.Models;

namespace RecipeBookaApi.DA.Models;

public partial class RecipeBookContext : IdentityDbContext<TbUser, IdentityRole<int>, int>
{
    public RecipeBookContext()
    {
    }

    public RecipeBookContext(DbContextOptions<RecipeBookContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbRecipe> TbRecipes { get; set; }

    public virtual DbSet<TbUser> TbUsers { get; set; }

    public virtual DbSet<TbRefreshToken> TbRefreshTokens { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TbRecipe>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity
                
                .ToTable("TbRecipe");

            entity.Property(e => e.CreationDate).HasColumnType("datetime");
            entity.Property(e => e.CurrentState).HasDefaultValue(1);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Ingredients).HasMaxLength(1000);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(d=>d.tbRecipes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbRecipe_TbUser");
        });

        modelBuilder.Entity<TbRefreshToken>(entity => {
        entity.HasKey(t => t.RefreshID);
        
        });

        modelBuilder.Entity<TbUser>(entity =>
        {
            entity.ToTable("TbUser");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255);
               
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.UserName)
                .HasMaxLength(50);
               
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

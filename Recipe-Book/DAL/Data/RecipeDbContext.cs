using DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class RecipeDbContext : IdentityDbContext<ApplicationUser>
{
    public RecipeDbContext(DbContextOptions<RecipeDbContext> options)
        : base(options)
    {
    }

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Allergen> Allergens => Set<Allergen>();
    public DbSet<RecipeCategory> RecipeCategories => Set<RecipeCategory>();
    public DbSet<IngredientAllergen> IngredientAllergens => Set<IngredientAllergen>();
    public DbSet<UserAllergenProfile> UserAllergenProfiles => Set<UserAllergenProfile>();
    public DbSet<FavoriteRecipe> FavoriteRecipes => Set<FavoriteRecipe>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureRecipe(modelBuilder);
        ConfigureIngredient(modelBuilder);
        ConfigureRecipeIngredient(modelBuilder);
        ConfigureCategory(modelBuilder);
        ConfigureAllergen(modelBuilder);
        ConfigureJoinTables(modelBuilder);
        ConfigureUserTables(modelBuilder);
    }

    private void ConfigureRecipe(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasFilter("IsDeleted = 0");

            entity.Property(e => e.Description)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("datetime('now')");

            entity.Property(e => e.ModifiedAt)
                .HasDefaultValueSql("datetime('now')");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    private void ConfigureIngredient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Name)
                .IsUnique();

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Ingredient_VeganIsVegetarian",
                "IsVegan = 0 OR IsVegetarian = 1"
            ));
        });
    }

    private void ConfigureRecipeIngredient(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Ingredient)
                .WithMany(i => i.RecipeIngredients)
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.RecipeId, e.IngredientId })
                .IsUnique();

            entity.Property(e => e.Amount)
                .HasPrecision(10, 3);
        });
    }

    private void ConfigureCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.Name)
                .IsUnique();
        });
    }

    private void ConfigureAllergen(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Allergen>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(e => e.Name)
                .IsUnique();
        });
    }

    private void ConfigureJoinTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecipeCategory>(entity =>
        {
            entity.HasKey(e => new { e.RecipeId, e.CategoryId });

            entity.HasOne(e => e.Recipe)
                .WithMany(r => r.RecipeCategories)
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.RecipeCategories)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<IngredientAllergen>(entity =>
        {
            entity.HasKey(e => new { e.IngredientId, e.AllergenId });

            entity.HasOne(e => e.Ingredient)
                .WithMany(i => i.IngredientAllergens)
                .HasForeignKey(e => e.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Allergen)
                .WithMany(a => a.IngredientAllergens)
                .HasForeignKey(e => e.AllergenId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureUserTables(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAllergenProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                .WithMany(u => u.UserAllergenProfiles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Allergen)
                .WithMany()
                .HasForeignKey(e => e.AllergenId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.UserId, e.AllergenId })
                .IsUnique();
        });

        modelBuilder.Entity<FavoriteRecipe>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.User)
                .WithMany(u => u.FavoriteRecipes)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Recipe)
                .WithMany()
                .HasForeignKey(e => e.RecipeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            entity.HasIndex(e => new { e.UserId, e.RecipeId })
                .IsUnique();
        });
    }
}

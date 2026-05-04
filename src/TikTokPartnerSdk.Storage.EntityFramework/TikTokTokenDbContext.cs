using Microsoft.EntityFrameworkCore;

namespace TikTokPartnerSdk.Storage.EntityFramework;

public sealed class TikTokTokenDbContext(DbContextOptions<TikTokTokenDbContext> options) : DbContext(options)
{
    public DbSet<TikTokTokenEntity> TikTokTokens => Set<TikTokTokenEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TikTokTokenEntity>(entity =>
        {
            entity.HasKey(token => token.Id);
            entity.Property(token => token.AppKey).HasMaxLength(128).IsRequired();
            entity.Property(token => token.ShopCipher).HasMaxLength(512);
            entity.Property(token => token.AccessToken).IsRequired();
            entity.Property(token => token.RefreshToken).IsRequired();
            entity.HasIndex(token => new { token.AccessTokenKind, token.AppKey, token.ShopCipher }).IsUnique();
        });
    }
}

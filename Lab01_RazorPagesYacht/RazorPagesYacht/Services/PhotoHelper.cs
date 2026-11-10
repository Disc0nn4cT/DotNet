namespace RazorPagesYacht.Services;

public static class PhotoHelper
{
    private static readonly string[] Permitted = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxSize = 2 * 1024 * 1024; // 2 MB

    // Returns an error message, or null if the file is OK (or no file was chosen)
    public static string? Validate(IFormFile? file)
    {
        if (file == null || file.Length == 0) return null;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!Permitted.Contains(ext)) return "Allowed formats: jpg, jpeg, png, webp.";
        if (file.Length > MaxSize) return "The file is larger than 2 MB.";
        return null;
    }

    // Saves the file to wwwroot/uploads/yachts/{id}/ and returns its web path
    public static async Task<string> SaveAsync(IFormFile file, int yachtId, string webRoot)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var dir = Path.Combine(webRoot, "uploads", "yachts", yachtId.ToString());
        Directory.CreateDirectory(dir);

        var fileName = $"photo-{Guid.NewGuid():N}{ext}";
        await using var stream = File.Create(Path.Combine(dir, fileName));
        await file.CopyToAsync(stream);

        return $"/uploads/yachts/{yachtId}/{fileName}";
    }

    public static void TryDelete(string? webPath, string webRoot)
    {
        // Delete only files uploaded by admins, never the bundled seed photos
        if (string.IsNullOrWhiteSpace(webPath) || !webPath.StartsWith("/uploads/")) return;
        var fullPath = Path.Combine(webRoot, webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        try { if (File.Exists(fullPath)) File.Delete(fullPath); } catch { /* ignore */ }
    }

    public static void TryDeleteFolder(int yachtId, string webRoot)
    {
        var dir = Path.Combine(webRoot, "uploads", "yachts", yachtId.ToString());
        try { if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true); } catch { /* ignore */ }
    }
}
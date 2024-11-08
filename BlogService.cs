using System.Text;

public class BlogService
{
    private readonly HttpClient _httpClient;
    
    public BlogService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string? AudioUrl { get; private set; }
    public async Task GenerateAudio(string text)
    {
        var queryUrl = $"https://voice.jirigalang.com/audio_query?speaker=61&text={Uri.EscapeDataString(text)}";
        var queryResponse = await _httpClient.PostAsync(queryUrl, null); // 没有请求体
        queryResponse.EnsureSuccessStatusCode();

        var queryJson = await queryResponse.Content.ReadAsStringAsync();

        // 使用从 audio_query 获得的 JSON 作为 synthesis 的请求体
        var synthesisContent = new StringContent(queryJson, Encoding.UTF8, "application/json");

        var synthesisResponse = await _httpClient.PostAsync("https://voice.jirigalang.com/synthesis?speaker=61", synthesisContent);
        synthesisResponse.EnsureSuccessStatusCode();

        var audioData = await synthesisResponse.Content.ReadAsByteArrayAsync();
        // 将二进制音频数据转换为 Base64 字符串
        var audioBase64 = Convert.ToBase64String(audioData); 
        AudioUrl = $"data:audio/wav;base64,{audioBase64}";
    }
    public async Task<List<BlogDto>> GetBlogsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("https://api.jirigalang.com/api/blog");

            response.EnsureSuccessStatusCode(); // 确保 HTTP 响应成功

            var content = await response.Content.ReadAsStringAsync(); // 读取响应内容

            return await response.Content.ReadFromJsonAsync<List<BlogDto>>() ?? [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"获取博客文章时出错：{ex.Message}\r\n");
            return [];
        }
    }
    // 根据分类 ID 获取博客
    public async Task<List<BlogDto>?> GetBlogsByCategoryAsync(int categoryId)
    {
        return await _httpClient.GetFromJsonAsync<List<BlogDto>>($"api/Blog/category/{categoryId}");
    }
}
public class Blog
{
    public int Id { get; set; } // 主键
    public string? Title { get; set; } // 文章标题
    public string? CoverImage { get; set; } // 封面图像的路径或 URL
    public string? Content { get; set; } // 文章内容
    public DateTime CreatedAt { get; set; } // 创建时间
    public DateTime UpdatedAt { get; set; } // 更新时间
    public int CategoryId { get; set; } // 外键，指向分区
    public Category? Category { get; set; } // 导航属性
}
public class Category
{
    public int Id { get; set; } // 主键
    public string? Name { get; set; } // 分区名称
    public string? CoverImage { get; set; }// 分区封面图像的路径或 URL 
}
/// <summary>
/// 用于返回博客文章的 DTO
/// </summary>
public class BlogDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? CoverImage { get; set; } // 封面图像的路径或 URL
    public DateTime CreatedAt { get; set; } // 创建时间
    public DateTime UpdatedAt { get; set; } // 更新时间
    public int CategoryId { get; set; } // 外键，指向分区
    public Category? Category { get; set; } // 导航属性
}
using System.Text.Json;
using System.Text.RegularExpressions;
using SongModels;

namespace MS.wyyyy
{
    public class WyySongList
    {
        private readonly HttpClient? _httpClient;
        public string songlist = "歌曲,专辑,别名,翻译名,艺术家\r\n";
        public bool isSucceed = false;
        public string buttonContent = "生成";

        public WyySongList(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        /// <summary>
        /// 获取歌单导出csv文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task? GetSongList(string url)
        {
            string pattern = @"[?&]id=(\d+)";
            Match match = Regex.Match(url, pattern);

            if (match.Success)
            {
                // 提取到的 id 
                string id = match.Groups[1].Value;
                // 根据id进行请求
                var queryUrl = $"https://music.163.com/api/playlist/detail?id={id}";
                
                // 重试五次
                int maxRetries = 5;
                int retryCount = 0;
                bool success = false;

                while (retryCount < maxRetries && !success)
                {
                    try
                    {
                        // 发送请求
                        var queryResponse = await _httpClient?.GetAsync(queryUrl);

                        var queryJson = await queryResponse.Content.ReadAsStringAsync();

                        var result = JsonSerializer.Deserialize<JS>(queryJson);

                        // 检查返回的 JSON 是否表示服务器忙碌
                        if (result?.code == -447)
                        {
                            retryCount++;
                            buttonContent = ($"服务器忙碌，正在重试 ({retryCount}/{maxRetries})...");
                            await Task.Delay(2000); // 等待 2 秒再重试
                        }
                        else
                        {
                            // 如果返回结果正常，执行后续操作后退出循环
                            success = true;
                            buttonContent = ("请求成功！");
                            foreach (Track tracks in result.result.tracks)
                            {
                                songlist += $"{tracks.name},{tracks.album.name},{tracks.subType},{tracks.transName},";
                                foreach (Artist artist in tracks?.album.artists)
                                {
                                    songlist += artist.name + ",";
                                }
                                songlist += "\r\n";
                            }
                            isSucceed = true;
                        }

                    }
                    catch (Exception ex)
                    {
                        buttonContent = ($"请求发生错误：{ex.Message}");
                        break; // 如果发生其他异常，退出循环
                    }
                }

                if (!success)
                {
                    buttonContent = ("请求失败，最大重试次数已达到。");
                }
            }
            else
            {
                buttonContent = ("请输入正确的链接");
            }
        }
    }
}
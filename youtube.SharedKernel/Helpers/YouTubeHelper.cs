using System;
using System.Text.RegularExpressions;

namespace youtube.SharedKernel.Helpers
{
    public static class YouTubeHelper
    {
        private static readonly Regex YouTubeRegex = new Regex(
            @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public static bool IsYouTubeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return YouTubeRegex.IsMatch(url.Trim());
        }

        public static string ExtractVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var match = YouTubeRegex.Match(url.Trim());
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }

            if (url.Trim().Length == 11 && Regex.IsMatch(url.Trim(), @"^[a-zA-Z0-9_-]{11}$"))
            {
                return url.Trim();
            }

            return null;
        }

        public static string GetWatchUrl(string videoUrlOrId)
        {
            var videoId = ExtractVideoId(videoUrlOrId);
            if (string.IsNullOrEmpty(videoId)) return videoUrlOrId;
            return $"https://www.youtube.com/watch?v={videoId}";
        }

        public static string GetEmbedUrl(string videoUrlOrId, bool autoplay = true)
        {
            var videoId = ExtractVideoId(videoUrlOrId);
            if (string.IsNullOrEmpty(videoId)) return null;

            var autoplayParam = autoplay ? "1" : "0";
            return $"https://www.youtube-nocookie.com/embed/{videoId}?autoplay={autoplayParam}&rel=0&modestbranding=1&enablejsapi=1";
        }

        public static string GetThumbnailUrl(string videoUrlOrId)
        {
            var videoId = ExtractVideoId(videoUrlOrId);
            if (string.IsNullOrEmpty(videoId)) return null;
            return $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
        }
    }
}

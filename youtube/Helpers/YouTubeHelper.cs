using System;
using System.Text.RegularExpressions;

namespace youtube.Helpers
{
    public static class YouTubeHelper
    {
        private static readonly Regex YouTubeRegex = new Regex(
            @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|\S*?[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        /// <summary>
        /// Checks if a given string contains a valid YouTube URL or Video ID.
        /// </summary>
        public static bool IsYouTubeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return YouTubeRegex.IsMatch(url.Trim());
        }

        /// <summary>
        /// Extracts the 11-character YouTube video ID.
        /// </summary>
        public static string ExtractVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            var match = YouTubeRegex.Match(url.Trim());
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }

            // If it's already an 11-character ID
            if (url.Trim().Length == 11 && Regex.IsMatch(url.Trim(), @"^[a-zA-Z0-9_-]{11}$"))
            {
                return url.Trim();
            }

            return null;
        }

        /// <summary>
        /// Gets the clean YouTube Watch URL that opens the video on YouTube.
        /// </summary>
        public static string GetWatchUrl(string videoUrlOrId)
        {
            var videoId = ExtractVideoId(videoUrlOrId);
            if (string.IsNullOrEmpty(videoId)) return videoUrlOrId;
            return $"https://www.youtube.com/watch?v={videoId}";
        }

        /// <summary>
        /// Gets the responsive YouTube embed URL (cookie-free domain).
        /// </summary>
        public static string GetEmbedUrl(string videoUrlOrId, bool autoplay = true)
        {
            var videoId = ExtractVideoId(videoUrlOrId);
            if (string.IsNullOrEmpty(videoId)) return null;

            var autoplayParam = autoplay ? "1" : "0";
            return $"https://www.youtube-nocookie.com/embed/{videoId}?autoplay={autoplayParam}&rel=0&modestbranding=1&enablejsapi=1";
        }

        /// <summary>
        /// Gets high-quality YouTube thumbnail.
        /// </summary>
        public static string GetThumbnailUrl(string videoUrlOrId)
        {
            var videoId = ExtractVideoId(videoUrlOrId);
            if (string.IsNullOrEmpty(videoId)) return null;
            return $"https://img.youtube.com/vi/{videoId}/hqdefault.jpg";
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShalicoLib.Utility
{
    public static class ShareUtility
    {
        public static void Share(string text, IEnumerable<string> tags = null)
        {
            var tweetUrl = $"https://twitter.com/intent/tweet?text={Uri.EscapeUriString(text)}";
            if (tags != null)
            {
                var tagText = string.Join(",", tags);

                if (!string.IsNullOrEmpty(tagText)) tweetUrl += "&hashtags=" + tagText;
            }

            Application.OpenURL(tweetUrl);
        }
    }
}
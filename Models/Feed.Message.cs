﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Coolapk_UWP.Models
{
    public class MessageRawStructBase
    {
        // 可能为空
        [JsonIgnore]
        public Feed ParentFeed { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> OtherField { get; set; }
        public string Type { get; set; }

        public MessageRawStructBase AutoCast()
        {
            var _s = JsonConvert.SerializeObject(this);
            switch (Type)
            {
                case "text":
                    return JsonConvert.DeserializeObject<MessageRawStructText>(_s);
                case "image":
                    return JsonConvert.DeserializeObject<MessageRawStructImage>(_s);
                default:
                    return JsonConvert.DeserializeObject<MessageRawStructUnsupport>(_s);
            }
        }
    }

    public class MessageRawStructText : MessageRawStructBase
    {
        [JsonIgnore]
        public string _cachedMessageWithEmojiJoined = "";
        [JsonIgnore]
        public string MessageWithEmoji
        {
            get
            {
                if (_cachedMessageWithEmojiJoined.Equals(String.Empty))
                {
                    var msg = Message;
                    Other.EmojisUtil.Emojis?.ForEach(emoji =>
                    {
                        msg = msg.Replace(emoji, "<emoji src=\"" + Other.EmojisUtil.GetEmojiUriFor(emoji) + "\" />");
                    });
                    return msg;
                }
                return _cachedMessageWithEmojiJoined;
            }
        }
        public string Message { get; set; }
    }

    public class MessageRawStructImage : MessageRawStructBase
    {
        public string Url { get; set; }
        [JsonIgnore]
        public string SmallPic
        {
            get
            {
                return Url.EndsWith(".gif") ? Url : (Url + ".s.jpg");
            }
        }
        public string Description { get; set; }
    }

    public class MessageRawStructUnsupport : MessageRawStructBase { }
}

﻿
using static MagicVilla_Utility.SD;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        public ApiType Apitype { get; set; }
        public string Url { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
    }
}

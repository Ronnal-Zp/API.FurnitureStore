﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.FurnitureStore.Shared.Auth
{
    public class AuthResult
    {
        public string Token {  get; set; }
        public string Result{ get; set; }
        public List<string> Errors { get; set; }
    }
}

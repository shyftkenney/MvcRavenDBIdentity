﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MvcRavenDBIdentity.Models
{
    public class RegisterModel
    {
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}

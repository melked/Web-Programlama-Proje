﻿using System;
namespace KuaforUygulamasi.Models
{
    public class Kullanici
    {
        public int ID { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public string Rol { get; set; } // "Admin" veya "User"
    }
}
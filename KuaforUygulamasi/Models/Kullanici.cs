using Microsoft.AspNetCore.Identity;

public class Kullanici : IdentityUser
{
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string Rol { get; set; }

}
namespace blink.Common.Models;


//TODO
public class User
{
    public string user_id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public string created_at { get; set; }
}
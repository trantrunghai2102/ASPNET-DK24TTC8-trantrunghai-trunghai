using System.Text.Json;

namespace BanCaPheNuocGiaiKhat.Helpers;

public class SessionCartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public static class SessionCartHelper
{
    private const string CartKey = "cart";

    public static List<SessionCartItem> GetCart(ISession session)
    {
        var json = session.GetString(CartKey);
        if (string.IsNullOrEmpty(json)) return new();
        return JsonSerializer.Deserialize<List<SessionCartItem>>(json) ?? new();
    }

    public static void SaveCart(ISession session, List<SessionCartItem> items)
    {
        session.SetString(CartKey, JsonSerializer.Serialize(items));
    }

    public static void ClearCart(ISession session)
    {
        session.Remove(CartKey);
    }
}
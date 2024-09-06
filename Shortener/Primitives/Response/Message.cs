namespace Shortener.Primitives.Response;

public class Message
{
    public Message(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text, nameof(text));
        Text = text;
    }
    public string Text { get; private set; }
}
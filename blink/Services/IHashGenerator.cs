namespace blink.Services;

public interface IHashGenerator
{
    string GenerateShortCode(string url);
}

public class HashGenerator : IHashGenerator
{
    public string GenerateShortCode(string url)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(url));
        var base64Hash = Convert.ToBase64String(hashBytes);
        var cleanedHash = Regex.Replace(base64Hash, "[+/=]", string.Empty);

        return cleanedHash[..6];
        //return SegmentAndBuildHashCode(cleanedHash);
    }

    /// <summary>
    /// Segments the given hash code into fixed-length parts and constructs a shortened hash 
    /// by extracting the first character from segments longer than the defined length.
    /// </summary>
    /// <param name="hashCode">The input hash string to be processed.</param>
    /// <returns>A shortened hash code constructed from selected segments.</returns>
    private string SegmentAndBuildHashCode(string hashCode)
    {
        if (string.IsNullOrEmpty(hashCode))
            return string.Empty;

        var segments = new List<string>();
        var sb = new StringBuilder();
        const int hashPart = 6;

        for (var i = 0; i < hashCode.Length; i += hashPart)
        {
            var segment = i + hashPart <= hashCode.Length
                ? hashCode.Substring(i, hashPart)
                : hashCode.Substring(i);

            segments.Add(segment);
        }

        foreach (var segment in segments)
        {
            if (segment.Length > 6)
            {
                sb.Append(segment[0]);
            }
        }

        return sb.ToString();
    }
}
using System.Text;
using System.Text.RegularExpressions;
using FishyFlip.Models;

namespace Up.Bsky.PostBot.Util.FishyFlip;

public static partial class PostExtensions
{
    [GeneratedRegex(@"#(\w+)")]
    private static partial Regex HashtagsToLinksRegex();
    
    public static string? GetRichText(this Post self)
    {
        if (self.Text == null)
        {
            return null;
        }

        var bytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(self.Text));
        var sb = new StringBuilder();

        var idx = 0;
        if (self.Facets != null)
        {
            foreach (var facet in self.Facets)
            {
                if (facet.Index == null || facet.Features == null)
                {
                    continue;
                }

                // Append the text before the facet
                sb.Append(Encoding.UTF8.GetString(bytes[idx..facet.Index.ByteStart]));

                var segment = Encoding.UTF8.GetString(bytes[facet.Index.ByteStart..facet.Index.ByteEnd]);

                foreach (var feature in facet.Features)
                {
                    switch (feature.Type)
                    {
                        case null: continue;
                        case "app.bsky.richtext.facet#link":
                            var uri = feature.Uri ?? throw new ArgumentException($"no URI provided for {segment}");
                            if (uri == segment && (segment.StartsWith("http://") || segment.StartsWith("https://")))
                            {
                                sb.Append(segment);
                            }
                            else
                            {
                                sb.Append('[').Append(segment).Append("](").Append(uri).Append(')');
                            }
                            break;
                        case "app.bsky.richtext.facet#mention":
                            var did = feature.Did ?? throw new ArgumentException($"no Target provided for {segment}");
                            sb.Append('[').Append(segment).Append("](").Append(did.ToBskyUri()).Append(')');
                            break;
                        default:
                            // TODO log unknown facet feature type
                            sb.Append('`').Append(segment).Append('`');
                            break;
                    }
                    idx = facet.Index.ByteEnd;
                }
            }
        }

        // Append text after the last facet
        sb.Append(Encoding.UTF8.GetString(bytes[idx..]));

        var result = HashtagsToLinksRegex().Replace(sb.ToString(), "[$0](https://bsky.app/search?q=%23$1)");
        
        return result;
    }
}
using System;
using System.Diagnostics;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PerrijosGatijos.iOS
{
    internal static class FontExtensions
    {
        public static UIFont ToUIFont(this IFontElement element)
        {
            return ToUIFont(element.FontFamily, (float)element.FontSize, element.FontAttributes);
        }

        public static UIFont ToUIFont(string family, float size, FontAttributes attributes)
        {
            UIFont result;
            var bold = (attributes & FontAttributes.Bold) != 0;
            var italic = (attributes & FontAttributes.Italic) != 0;

            if (family != null)
            {
                try
                {
                    if (UIFont.FamilyNames.Contains(family))
                    {
                        var descriptor = new UIFontDescriptor().CreateWithFamily(family);

                        if (bold || italic)
                        {
                            result = FromDescriptor(descriptor, size, bold, italic);
                            if (result != null)
                                return result;
                        }
                    }

                    result = UIFont.FromName(family, size);
                    if (result != null)
                        return result;
                }
                catch
                {
                    Debug.WriteLine("Could not load font named: {0}", family);
                }
            }

            var defaultFont = UIFont.SystemFontOfSize(size);

            if (bold || italic)
            {
                result = FromDescriptor(defaultFont.FontDescriptor, size, bold, italic);
                if (result != null)
                    return result;
            }

            return defaultFont;
        }

        private static UIFont FromDescriptor(UIFontDescriptor descriptor, float size, bool bold, bool italic)
        {
            var traits = (UIFontDescriptorSymbolicTraits)0;
            if (bold)
                traits = traits | UIFontDescriptorSymbolicTraits.Bold;
            if (italic)
                traits = traits | UIFontDescriptorSymbolicTraits.Italic;

            descriptor = descriptor.CreateWithTraits(traits);
            return UIFont.FromDescriptor(descriptor, size);
        }
    }
}


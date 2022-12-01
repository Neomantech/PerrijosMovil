using System;
using System.Diagnostics;
using Android.Content.Res;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace PerrijosGatijos.Droid
{
    public static class FontExtensions
    {
        public static Typeface ToTypeFace(this string fontFamily, FontAttributes attr, AssetManager manager)
        {
            fontFamily = fontFamily ?? string.Empty;
            var result = fontFamily.TryGetFromAssets(manager);

            if (!result.success)
            {
                var style = Xamarin.Forms.Platform.Android.FontExtensions.ToTypefaceStyle(attr);
                return Typeface.Create(fontFamily, style);
            }

            return result.typeface;
        }

        private static (bool success, Typeface typeface) TryGetFromAssets(this string fontName, AssetManager manager)
        {
            //First check Alias
            var (hasFontAlias, fontPostScriptName) = FontRegistrar.HasFont(fontName);
            if (hasFontAlias)
            {
                return (true, Typeface.CreateFromFile(fontPostScriptName));
            }

            var isAssetFont = IsAssetFontFamily(fontName);

            if (!isAssetFont)
            {
                var folders = new[] { "", "Fonts/", "fonts/", };

                var fontFile = FontFile.FromString(fontName);

                if (!string.IsNullOrWhiteSpace(fontFile.Extension))
                {
                    var (hasFont, fontPath) = FontRegistrar.HasFont(fontFile.FileNameWithExtension());
                    if (hasFont)
                    {
                        return (true, Typeface.CreateFromFile(fontPath));
                    }
                }
                else
                {
                    foreach (var ext in FontFile.Extensions)
                    {
                        var formatted = fontFile.FileNameWithExtension(ext);
                        var (hasFont, fontPath) = FontRegistrar.HasFont(formatted);
                        if (hasFont)
                        {
                            return (true, Typeface.CreateFromFile(fontPath));
                        }

                        foreach (var folder in folders)
                        {
                            formatted = $"{folder}{fontFile.FileNameWithExtension()}#{fontFile.PostScriptName}";
                            var result = LoadTypefaceFromAsset(formatted, manager);
                            if (result.success)
                            {
                                return result;
                            }
                        }
                    }
                }

                return (false, null);
            }

            return LoadTypefaceFromAsset(fontName, manager);
        }

        private static (bool success, Typeface typeface) LoadTypefaceFromAsset(string fontFamily, AssetManager manager)
        {
            try
            {
                var result = Typeface.CreateFromAsset(manager, FontNameToFontFile(fontFamily));
                return (true, result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return (false, null);
            }
        }

        private static string FontNameToFontFile(string fontFamily)
        {
            fontFamily = fontFamily ?? String.Empty;
            var hashtagIndex = fontFamily.IndexOf('#');
            if (hashtagIndex >= 0)
            {
                return fontFamily.Substring(0, hashtagIndex);
            }

            throw new InvalidOperationException($"Can't parse the {nameof(fontFamily)} {fontFamily}");
        }

        private static bool IsAssetFontFamily(string name)
        {
            return name != null && (name.Contains(".ttf#") || name.Contains(".otf#"));
        }
    }
}


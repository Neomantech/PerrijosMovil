using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace PerrijosGatijos.Views.Controls
{
	public class IconControl: SKCanvasView
    {
        SkiaSharp.Extended.Svg.SKSvg svg;

        static Assembly mainPclAssembly;

        public static Assembly MainPclAssembly
        {
            get
            {
                if (mainPclAssembly == null)
                {
                    if (Application.Current == null)
                        throw new InvalidOperationException();
                    mainPclAssembly = Application.Current.GetType().GetTypeInfo().Assembly;
                }
                return mainPclAssembly;
            }
            set
            {
                mainPclAssembly = value;
            }
        }

        /// <summary>
        /// Cache for SVG icons. Each SVG is kept in memory only once even if it's used multple places in application.
        /// </summary>
        static readonly IDictionary<string, SkiaSharp.Extended.Svg.SKSvg> SvgCache = new Dictionary<string, SkiaSharp.Extended.Svg.SKSvg>();

        /// <summary>
        /// Global prefix for resource ids. Enabled writing cleaner XAML.
        /// </summary>
        public static string ResourceIdsPrefix { get; set; } = "PerrijosGatijos.Resources.Icons.";

        #region Bindable properties

        public static readonly BindableProperty ResourceIdProperty = BindableProperty.Create(nameof(ResourceId), typeof(string), typeof(IconControl), default(string));
        public string ResourceId
        {
            get { return (string)GetValue(ResourceIdProperty); }
            set { SetValue(ResourceIdProperty, value); }
        }

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(IconControl), Color.Black);
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        #endregion

        void LoadSvgImage()
        {
            if (svg != null)
                return;

            if (!SvgCache.TryGetValue(ResourceId, out svg))
            {
                //});

                string fullKey = $"{ResourceIdsPrefix}{ResourceId}.svg";
                using (Stream stream = MainPclAssembly.GetManifestResourceStream(fullKey))
                {
                    if (stream == null)
                        throw new FileNotFoundException($"SvgIcon : could not load SVG file {fullKey} in assembly {MainPclAssembly}. Make sure the ID is correct, the file is there and it is set to Embedded Resource build action.");
                    svg = new SkiaSharp.Extended.Svg.SKSvg();
                    svg.Load(stream);
                    SvgCache.Add(ResourceId, svg);
                }
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var canvas = e.Surface.Canvas;
            canvas.Clear();

            LoadSvgImage();

            using (var paint = new SKPaint())
            {
                if (Color != Color.Transparent)
                    paint.ColorFilter = SKColorFilter.CreateBlendMode(Color.ToSKColor(), SKBlendMode.SrcIn);
                paint.StrokeWidth = 1;

                var info = e.Info;
                canvas.Translate(info.Width / 2f, info.Height / 2f);

                SKRect bounds = svg.ViewBox;
                float xRatio = info.Width / bounds.Width;
                float yRatio = info.Height / bounds.Height;
                float ratio = Math.Min(xRatio, yRatio);

                canvas.Scale(ratio);
                canvas.Translate(-bounds.MidX, -bounds.MidY);
                canvas.DrawPicture(svg.Picture, paint);
            }
        }



        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == ResourceIdProperty.PropertyName)
                svg = null;

            if (propertyName == ResourceIdProperty.PropertyName
             || propertyName == ColorProperty.PropertyName)
                InvalidateSurface();
        }
    }
}


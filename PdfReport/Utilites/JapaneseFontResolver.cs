using PdfSharp.Fonts;
using System;
using System.IO;
using System.Reflection;

namespace PdfReport.Utilites
{
    /// <summary>
    /// 日本語フォントのためのフォントリゾルバー
    /// </summary>
    class JapaneseFontResolver : IFontResolver
    {
        /// <summary>
        /// 源真ゴシック（ http://jikasei.me/font/genshin/）
        /// </summary>
        private static readonly string GEN_SHIN_GOTHIC_MEDIUM_TTF = "PdfReport.assets.GenShinGothic-Monospace-Medium.ttf";

        /// <summary>
        /// フォントデータ取得
        /// </summary>
        /// <param name="faceName">フォントファイル名</param>
        /// <returns>フォントデータ</returns>
        public byte[] GetFont(string faceName)
        {
            switch (faceName)
            {
                case "GenShinGothic#Medium":
                    return LoadFontData(GEN_SHIN_GOTHIC_MEDIUM_TTF);
            }
            throw new ArgumentException("No faceName with name " + faceName);
        }

        /// <summary>
        /// フォント取得
        /// </summary>
        /// <param name="familyName">フォント名</param>
        /// <param name="isBold">太字か否か</param>
        /// <param name="isItalic">斜体か否か</param>
        /// <returns>フォント情報</returns>
        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            var fontName = familyName.ToLower();

            switch (fontName)
            {
                case "gen shin gothic":
                    return new FontResolverInfo("GenShinGothic#Medium");
            }

            // デフォルトのフォント
            var defaultFont = PlatformFontResolver.ResolveTypeface("Verdana", isBold, isItalic);
            if(defaultFont is null)
                throw new ArgumentException("No familyName with name " + familyName);

            return defaultFont;
        }

        /// <summary>
        /// 埋め込みリソースからフォントファイルを読み込む
        /// </summary>
        /// <param name="resourceName">埋め込みリソース名</param>
        /// <returns>フォントファイルデータ</returns>
        private byte[] LoadFontData(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream is null)
                    throw new ArgumentException("No resource with name " + resourceName);

                int count = (int)stream.Length;
                byte[] data = new byte[count];
                stream.Read(data, 0, count);
                return data;
            }
        }
    }
}
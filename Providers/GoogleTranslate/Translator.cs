// Copyright (c) 2015 Ravi Bhavnani
// License: Code Project Open License
// http://www.codeproject.com/info/cpol10.aspx

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace GoogleTranslate
{
    /// <summary>
    /// Translates text using Google's online language tools.
    /// </summary>
    public class Translator
    {
        #region Properties

        /// <summary>
        /// Gets the supported languages.
        /// </summary>
        public static IEnumerable<string> Languages
        {
            get
            {
                EnsureInitialized();
                return languageModeMap.Keys.OrderBy(p => p);
            }
        }

        /// <summary>
        /// Gets the time taken to perform the translation.
        /// </summary>
        public TimeSpan TranslationTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the url used to speak the translation.
        /// </summary>
        /// <value>The url used to speak the translation.</value>
        public string TranslationSpeechUrl
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public Exception Error
        {
            get;
            private set;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Translates the specified source text.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target language.</param>
        /// <returns>The translation.</returns>
        public string Translate
            (string sourceText,
             string sourceLanguage,
             string targetLanguage)
        {
            // Initialize
            Error = null;
            TranslationSpeechUrl = null;
            TranslationTime = TimeSpan.Zero;
            DateTime tmStart = DateTime.Now;
            string translation = string.Empty;

            try
            {
                // Download translation
                string url = string.Format("https://translate.google.com/translate_a/single?client=t&sl={0}&tl={1}&hl=en&dt=bd&dt=ex&dt=ld&dt=md&dt=qc&dt=rw&dt=rm&dt=ss&dt=t&dt=at&ie=UTF-8&oe=UTF-8&source=btn&ssel=0&tsel=0&kc=0&q={2}",
                                            LanguageEnumToIdentifier(sourceLanguage),
                                            LanguageEnumToIdentifier(targetLanguage),
                                            HttpUtility.UrlEncode(sourceText));
                string outputFile = Path.GetTempFileName();
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                    wc.DownloadFile(url, outputFile);
                }

                // Get translated text
                if (File.Exists(outputFile))
                {

                    // Get phrase collection
                    string text = File.ReadAllText(outputFile);
                    int index = text.IndexOf(string.Format(",,\"{0}\"", LanguageEnumToIdentifier(sourceLanguage)));
                    if (index == -1)
                    {
                        // Translation of single word
                        int startQuote = text.IndexOf('\"');
                        if (startQuote != -1)
                        {
                            int endQuote = text.IndexOf('\"', startQuote + 1);
                            if (endQuote != -1)
                            {
                                translation = text.Substring(startQuote + 1, endQuote - startQuote - 1);
                            }
                        }
                    }
                    else
                    {
                        // Translation of phrase
                        text = text.Substring(0, index);
                        text = text.Replace("],[", ",");
                        text = text.Replace("]", string.Empty);
                        text = text.Replace("[", string.Empty);
                        text = text.Replace("\",\"", "\"");

                        // Get translated phrases
                        string[] phrases = text.Split(new[] { '\"' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; (i < phrases.Count()); i += 2)
                        {
                            translation += phrases[i] + "  ";
                        }
                    }

                    // Fix up translation
                    translation = translation.Trim();
                    translation = translation.Replace(" ?", "?");
                    translation = translation.Replace(" !", "!");
                    translation = translation.Replace(" ,", ",");
                    translation = translation.Replace(" .", ".");
                    translation = translation.Replace(" ;", ";");

                    // And translation speech URL
                    TranslationSpeechUrl = string.Format("https://translate.google.com/translate_tts?ie=UTF-8&q={0}&tl={1}&total=1&idx=0&textlen={2}&client=t",
                                                               HttpUtility.UrlEncode(translation), LanguageEnumToIdentifier(targetLanguage), translation.Length);
                }
            }
            catch (Exception ex)
            {
                Error = ex;
            }

            // Return result
            TranslationTime = DateTime.Now - tmStart;
            return translation;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Converts a language to its identifier.
        /// </summary>
        /// <param name="language">The language."</param>
        /// <returns>The identifier or <see cref="string.Empty"/> if none.</returns>
        private static string LanguageEnumToIdentifier
            (string language)
        {
            string mode = string.Empty;
            EnsureInitialized();
            languageModeMap.TryGetValue(language, out mode);
            return mode;
        }

        /// <summary>
        /// Ensures the translator has been initialized.
        /// </summary>
        private static void EnsureInitialized()
        {
            if (languageModeMap == null)
            {
                languageModeMap = new Dictionary<string, string>
                {
                    {"Afrikaans", "af"},
                    {"Albanian", "sq"},
                    {"Arabic", "ar"},
                    {"Armenian", "hy"},
                    {"Azerbaijani", "az"},
                    {"Basque", "eu"},
                    {"Belarusian", "be"},
                    {"Bengali", "bn"},
                    {"Bulgarian", "bg"},
                    {"Catalan", "ca"},
                    {"Chinese", "zh-CN"},
                    {"Croatian", "hr"},
                    {"Czech", "cs"},
                    {"Danish", "da"},
                    {"Dutch", "nl"},
                    {"English", "en"},
                    {"Esperanto", "eo"},
                    {"Estonian", "et"},
                    {"Filipino", "tl"},
                    {"Finnish", "fi"},
                    {"French", "fr"},
                    {"Galician", "gl"},
                    {"German", "de"},
                    {"Georgian", "ka"},
                    {"Greek", "el"},
                    {"Haitian Creole", "ht"},
                    {"Hebrew", "iw"},
                    {"Hindi", "hi"},
                    {"Hungarian", "hu"},
                    {"Icelandic", "is"},
                    {"Indonesian", "id"},
                    {"Irish", "ga"},
                    {"Italian", "it"},
                    {"Japanese", "ja"},
                    {"Korean", "ko"},
                    {"Lao", "lo"},
                    {"Latin", "la"},
                    {"Latvian", "lv"},
                    {"Lithuanian", "lt"},
                    {"Macedonian", "mk"},
                    {"Malay", "ms"},
                    {"Maltese", "mt"},
                    {"Norwegian", "no"},
                    {"Persian", "fa"},
                    {"Polish", "pl"},
                    {"Portuguese", "pt"},
                    {"Romanian", "ro"},
                    {"Russian", "ru"},
                    {"Serbian", "sr"},
                    {"Slovak", "sk"},
                    {"Slovenian", "sl"},
                    {"Spanish", "es"},
                    {"Swahili", "sw"},
                    {"Swedish", "sv"},
                    {"Tamil", "ta"},
                    {"Telugu", "te"},
                    {"Thai", "th"},
                    {"Turkish", "tr"},
                    {"Ukrainian", "uk"},
                    {"Urdu", "ur"},
                    {"Vietnamese", "vi"},
                    {"Welsh", "cy"},
                    {"Yiddish", "yi"}
                };
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// The language to translation mode map.
        /// </summary>
        private static Dictionary<string, string> languageModeMap;

        #endregion
    }
}

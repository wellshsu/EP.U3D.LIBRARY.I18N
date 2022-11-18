using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EP.U3D.LIBRARY.I18N
{
    /// <summary>
    /// I18N Language interface.
    /// </summary>
    public interface ILanguage
    {
        string EN();
        string AR();
        string DE();
        string FR();
        string ES();
        string JP();
        string KO();
        string SCN();
        string TCN();
        ILanguage Read(object keyvalue, string keyname = "ID", bool check = false);
    }

    /// <summary>
    /// I18N Language code enum.
    /// </summary>
    public enum LanguageCode
    {
        /// <summary>
        /// English
        /// </summary>
        EN = 0,
        /// <summary>
        /// Arabic
        /// </summary>
        AR = 1,
        /// <summary>
        /// German
        /// </summary>
        DE = 2,
        /// <summary>
        /// French
        /// </summary>
        FR = 3,
        /// <summary>
        /// Spanish
        /// </summary>
        ES = 4,
        /// <summary>
        /// Japanese
        /// </summary>
        JP = 5,
        /// <summary>
        /// Korean
        /// </summary>
        KO = 6,
        /// <summary>
        /// Simple Chinese
        /// </summary>
        SCN = 7,
        /// <summary>
        /// Traditional Chinese
        /// </summary>
        TCN = 8,
    }

    /// <summary>
    /// Internationalization component.
    /// Use getValue() to translate text.
    /// Use setLanguage() to change current application language.
    /// All translations are in _langs variable.
    /// </summary>
    public class I18N : MonoBehaviour
    {
        #region STATIC

        /// <summary>
        /// Default language.
        /// </summary>
        private static LanguageCode mDefaultLang = LanguageCode.EN;
        private static I18N mInstance = null;

        /// <summary>
        /// I18N components instance.
        /// </summary>
        public static I18N Instance
        {
            get
            {
                if (!mInstance)
                {
                    mInstance = FindObjectOfType<I18N>();
                    if (mInstance) mInstance.Init();
                }
                return mInstance;
            }
        }

        #endregion

        #region EVENTS

        public delegate void LanguageChange(LanguageCode newLanguage);
        public static event LanguageChange OnLanguageChanged;

        public delegate void FontChange(I18NFonts newFont);
        public static event FontChange OnFontChanged;

        #endregion

        #region CONST

        private const string GAME_LANG = "game_language";

        #endregion

        #region PRIVATE VARS
        /// <summary>
        /// Current game language. Using getValue() will translate to that language.
        /// </summary>
        [SerializeField]
        private LanguageCode _gameLang = LanguageCode.EN;
        /// <summary>
        /// Returned text when there is no translation.
        /// </summary>
        private string _noTranslationText = "Translation missing for null";
        /// <summary>
        /// When true, I18NText controls will change font for different languages.
        /// Fonts will be selected from _langFonts list. When there is no custom
        /// font set fot language, I18N controls will use default font.
        /// </summary>
        [SerializeField]
        private bool _useCustomFonts = false;
        /// <summary>
        /// Current custom font.
        /// </summary>
        private I18NFonts _currentCustomFont;
        /// <summary>
        /// Custom fonts list for different languages.
        /// </summary>
        [SerializeField]
        private List<I18NFonts> _langFonts;
        /// <summary>
        /// Current list language.
        /// </summary>
        private List<ILanguage> _languages;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Current game language
        /// </summary>
        public LanguageCode GameLang
        {
            get
            {
                return _gameLang;
            }
        }

        /// <summary>
        /// True, when I18N is using custom fonts
        /// </summary>
        public bool UseCustomFonts
        {
            get
            {
                return _useCustomFonts;
            }
        }

        /// <summary>
        /// Return current custom font or null, when I18N 
        /// is not currently using custom fonts.
        /// </summary>
        public I18NFonts CustomFont
        {
            get
            {
                if (_useCustomFonts)
                    return _currentCustomFont;
                return null;
            }
        }
        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Change current language.
        /// Set default language if not initialized or recognized.
        /// </summary>
        /// <param name="lang">Language code</param>
        public void SetLanguage(string lang) { SetLanguage((LanguageCode)Enum.Parse(typeof(LanguageCode), lang)); }

        /// <summary>
        /// Change current language.
        /// Set default if language not initialized or recognized.
        /// </summary>
        /// <param name="lang">Language code</param>
        public void SetLanguage(LanguageCode lang)
        {
            _gameLang = lang;
            PlayerPrefs.SetString(GAME_LANG, _gameLang.ToString());

            if (OnLanguageChanged != null)
                OnLanguageChanged(_gameLang);

            if (_useCustomFonts)
            {
                I18NFonts newFont = null;
                _currentCustomFont = null;
                if (_langFonts != null && _langFonts.Count > 0)
                {
                    foreach (I18NFonts f in _langFonts)
                    {
                        if (f.lang == _gameLang)
                        {
                            newFont = f;
                            _currentCustomFont = f;
                            break;
                        }
                    }
                }

                if (OnFontChanged != null)
                    OnFontChanged(newFont);
            }
            else
            {
                _currentCustomFont = null;
            }
        }

        /// <summary>
        /// Register language data source.
        /// </summary>
        /// <param name="source">data source</param>
        public static void RegLanguage(ILanguage source)
        {
            if (source == null) return;
            if (Instance._languages == null) Instance._languages = new List<ILanguage>();
            Instance._languages.Add(source);
        }

        /// <summary>
        /// Unregister language data source.
        /// </summary>
        /// <param name="source">data source</param>
        public static void UnregLanguage(ILanguage source)
        {
            if (source == null || Instance._languages == null) return;
            Instance._languages.Remove(source);
        }

        /// <summary>
        /// Get key value in current language.
        /// </summary>
        /// <param name="key">Translation key. String should start with '^' character</param>
        /// <returns>Translation value</returns>
        public static string Localize(string key) { return Instance.Localize(key, null); }

        /// <summary>
        /// Get key value in current language with additional params. 
        /// Currently not working.
        /// </summary>
        /// <param name="key">Translation key.</param>
        /// <param name="parameters">Additional parameters.</param>
        /// <returns>Translation value</returns>
        public string Localize(string key, string[] parameters)
        {
            string val = GetValue(key);
            if (val == null || val.Length == 0)
            {
                if (key == "")
                    return "";
                return string.Format(_noTranslationText, val);
            }

            if (parameters != null && parameters.Length > 0)
            {
                return string.Format(val.Replace("\\n", Environment.NewLine), parameters).Replace("#", "\u3000");
            }
            return val.Replace("\\n", Environment.NewLine).Replace("#", "\u3000");
        }

        /// <summary>
        /// Get key value in current language with additional params. 
        /// Currently not working.
        /// </summary>
        /// <param name="ILanguage">class</param>
        /// <returns>Translation value</returns>
        public static string Localize(ILanguage language)
        {
            if (language != null)
            {
                switch (Instance.GameLang)
                {
                    case LanguageCode.EN: return language.EN().Replace("#", "\u3000");
                    case LanguageCode.AR: return language.AR();
                    case LanguageCode.DE: return language.DE();
                    case LanguageCode.FR: return language.FR();
                    case LanguageCode.ES: return language.ES();
                    case LanguageCode.JP: return language.JP();
                    case LanguageCode.KO: return language.KO();
                    case LanguageCode.SCN: return language.SCN();
                    case LanguageCode.TCN: return language.TCN();
                }
            }
            return "";
        }
        #endregion

        #region PRIVATE METHODS
        /// <summary>
        /// Initialize component
        /// </summary>
        private void Init()
        {
            string lang = null;
            if (!PlayerPrefs.HasKey(GAME_LANG))
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        lang = "EN";
                        break;
                    case SystemLanguage.Arabic:
                        lang = "AR";
                        break;
                    case SystemLanguage.German:
                        lang = "DE";
                        break;
                    case SystemLanguage.French:
                        lang = "FR";
                        break;
                    case SystemLanguage.Spanish:
                        lang = "ES";
                        break;
                    case SystemLanguage.Japanese:
                        lang = "JP";
                        break;
                    case SystemLanguage.Korean:
                        lang = "KO";
                        break;
                    case SystemLanguage.ChineseSimplified:
                        lang = "SCN";
                        break;
                    case SystemLanguage.ChineseTraditional:
                        lang = "TCN";
                        break;
                }
            }
            else
            {
                lang = PlayerPrefs.GetString(GAME_LANG);
            }
            try
            {
                SetLanguage(lang);
            }
            catch
            {
                SetLanguage(mDefaultLang);
            }
        }

        private string GetValue(string key)
        {
            if (_languages != null)
            {
                for (int i = 0, Count = _languages.Count; i < Count; i++)
                {
                    ILanguage vL = _languages[i];
                    if (vL != null)
                    {
                        ILanguage tL = vL.Read(key, "key", false);
                        if (tL != null)
                        {
                            switch (GameLang)
                            {
                                case LanguageCode.EN: return tL.EN();
                                case LanguageCode.AR: return tL.AR();
                                case LanguageCode.DE: return tL.DE();
                                case LanguageCode.FR: return tL.FR();
                                case LanguageCode.ES: return tL.ES();
                                case LanguageCode.JP: return tL.JP();
                                case LanguageCode.KO: return tL.KO();
                                case LanguageCode.SCN: return tL.SCN();
                                case LanguageCode.TCN: return tL.TCN();
                            }
                        }
                    }
                }
            }
            return "*N*";
        }
        #endregion
    }

    #region HELPER CLASSES
    /// <summary>
    /// Helper class, containing font parameters.
    /// </summary>
    [Serializable]
    public class I18NFonts
    {
        #region PUBLIC VARS

        /// <summary>
        /// Font language code.
        /// </summary>
        public LanguageCode lang;
        /// <summary>
        /// Font
        /// </summary>
        public Font font;
        /// <summary>
        /// TMP_FontAsset
        /// </summary>
        public TMP_FontAsset fontAsset;
        /// <summary>
        /// True, when components should use custom line spacing.
        /// </summary>
        public bool customLineSpacing = false;
        /// <summary>
        /// Custom line spacing value.
        /// </summary>
        public float lineSpacing = 1.0f;
        /// <summary>
        /// True, when components should use custom font size.
        /// </summary>
        public bool customFontSizeOffset = false;
        /// <summary>
        /// Custom font size offset in percents.
        /// e.g. 55, -10
        /// </summary>
        public int fontSizeOffsetPercent = 0;
        /// <summary>
        /// True, when components should use custom alignment.
        /// </summary>
        public bool customAlignment = false;
        /// <summary>
        /// Custom alignment value.
        /// </summary>
        public TextAlignment alignment = TextAlignment.Left;
        /// <summary>
        /// Custom alignmentOptions value.
        /// </summary>
        public TextAlignmentOptions alignmentOptions = TextAlignmentOptions.Left;

        #endregion
    }

    /// <summary>
    /// Helper class, containing sprite parameters.
    /// </summary>
    [Serializable]
    public class I18NSprites
    {
        #region PUBLIC VARS

        /// <summary>
        /// Sprite lang code.
        /// </summary>
        public LanguageCode language;
        /// <summary>
        /// Sprite.
        /// </summary>
        public Sprite image;

        #endregion
    }

    /// <summary>
    /// Helper class, containing sound parameters.
    /// </summary>
    [Serializable]
    public class I18NSounds
    {
        #region PUBLIC VARS

        /// <summary>
        /// Sound language code.
        /// </summary>
        public LanguageCode language;
        /// <summary>
        /// Audio clip.
        /// </summary>
        public AudioClip clip;

        #endregion
    }
    #endregion
}
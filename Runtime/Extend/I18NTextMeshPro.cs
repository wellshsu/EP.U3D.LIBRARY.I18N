using TMPro;
using UnityEngine;

namespace EP.U3D.LIBRARY.I18N
{
    public class I18NTextMeshPro : MonoBehaviour
    {
        [SerializeField]
        private string _key = "";
        private TextMeshProUGUI _text;
        private bool _initialized = false;
        private TMP_FontAsset _defaultFont;
        private float _defaultLineSpacing;
        private float _defaultFontSize;
        private TextAlignmentOptions _defaultAlignment;

        [SerializeField]
        private string[] _params;

        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (string.IsNullOrEmpty(_key)) _key = value;
            }
        }

        void OnEnable()
        {
            if (!_initialized)
                _init();

            updateTranslation();
        }

        void OnDestroy()
        {
            if (_initialized)
            {
                I18N.OnLanguageChanged -= _onLanguageChanged;
                I18N.OnFontChanged -= _onFontChanged;
            }
        }
        /// <summary>
        /// Change text in Text component.
        /// </summary>
        private void _updateTranslation()
        {
            if (_text)
            {
                string val = I18N.Instance.Localize(_key, _params);
                if (!string.IsNullOrEmpty(val) && val != "*N*") _text.text = val.Replace("#", "\u3000");
            }
        }

        /// <summary>
        /// Update translation text.
        /// </summary>
        /// <param name="invalidateKey">Force to invalidate current translation key</param>
        public void updateTranslation(bool invalidateKey = false)
        {
            _updateTranslation();
        }

        /// <summary>
        /// Init component.
        /// </summary>
        private void _init()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _defaultFont = _text.font;
            _defaultLineSpacing = _text.lineSpacing;
            _defaultFontSize = _text.fontSize;
            _defaultAlignment = _text.alignment;
            _initialized = true;

            if (I18N.Instance.UseCustomFonts)
            {
                _changeFont(I18N.Instance.CustomFont);
            }

            I18N.OnLanguageChanged += _onLanguageChanged;
            I18N.OnFontChanged += _onFontChanged;

            if (!_text)
            {
                Debug.LogWarning(string.Format("{0}: Text component was not found!", this));
            }
        }

        private void _onLanguageChanged(LanguageCode newLang)
        {
            _updateTranslation();
        }

        private void _onFontChanged(I18NFonts newFont)
        {
            _changeFont(newFont);
        }

        private void _changeFont(I18NFonts f)
        {

            if (f != null)
            {
                if (f.font)
                {
                    _text.font = f.fontAsset;
                }
                else
                {
                    _text.font = _defaultFont;
                }
                if (f.customLineSpacing)
                {
                    _text.lineSpacing = f.lineSpacing;
                }
                if (f.customFontSizeOffset)
                {
                    _text.fontSize = (int)(_defaultFontSize + (_defaultFontSize * f.fontSizeOffsetPercent / 100));
                }
                if (f.customAlignment)
                {
                    _text.alignment = _getAnchorFromAlignment(f.alignmentOptions);
                }
            }
            else
            {
                _text.font = _defaultFont;
                _text.lineSpacing = _defaultLineSpacing;
                _text.fontSize = _defaultFontSize;
                _text.alignment = _defaultAlignment;
            }
        }

        private TextAlignmentOptions _getAnchorFromAlignment(TextAlignmentOptions alignmentOptions)
        {
            switch (_defaultAlignment)
            {
                case TextAlignmentOptions.CaplineLeft:
                //case TextAlignmentOptions.UpperCenter:
                case TextAlignmentOptions.CaplineRight:
                    if (alignmentOptions == TextAlignmentOptions.Left)
                        return TextAlignmentOptions.CaplineLeft;
                    else if (alignmentOptions == TextAlignmentOptions.Right)
                        return TextAlignmentOptions.CaplineRight;
                    break;
                case TextAlignmentOptions.MidlineLeft:
                //case TextAlignmentOptions.MiddleCenter:
                case TextAlignmentOptions.MidlineRight:
                    if (alignmentOptions == TextAlignmentOptions.Left)
                        return TextAlignmentOptions.MidlineLeft;
                    else if (alignmentOptions == TextAlignmentOptions.Right)
                        return TextAlignmentOptions.MidlineRight;
                    break;
                case TextAlignmentOptions.BottomLeft:
                //case TextAlignmentOptions.LowerCenter:
                case TextAlignmentOptions.BottomRight:
                    if (alignmentOptions == TextAlignmentOptions.Left)
                        return TextAlignmentOptions.BottomLeft;
                    else if (alignmentOptions == TextAlignmentOptions.Right)
                        return TextAlignmentOptions.BottomRight;
                    break;
            }

            return _defaultAlignment;
        }
    }
}

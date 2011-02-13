using System;
using System.Windows;

namespace DisplayUpdates
{
    public partial class TwitterAuth : Window
    {
        public TwitterAuth()
        {
            InitializeComponent();
        }

        #region DP AuthUrl

        public static readonly DependencyProperty AuthUrlProperty = DependencyProperty.Register("AuthUrl", typeof(Uri), typeof(TwitterAuth), new UIPropertyMetadata(null, new PropertyChangedCallback(OnAuthUrlChanged), new CoerceValueCallback(OnCoerceAuthUrl)));

        private static object OnCoerceAuthUrl(DependencyObject o, object value)
        {
            TwitterAuth twitterAuth = o as TwitterAuth;
            if (twitterAuth != null)
                return twitterAuth.OnCoerceAuthUrl((Uri)value);
            else
                return value;
        }

        private static void OnAuthUrlChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TwitterAuth twitterAuth = o as TwitterAuth;
            if (twitterAuth != null)
                twitterAuth.OnAuthUrlChanged((Uri)e.OldValue, (Uri)e.NewValue);
        }

        protected virtual Uri OnCoerceAuthUrl(Uri value)
        {
            return value;
        }

        protected virtual void OnAuthUrlChanged(Uri oldValue, Uri newValue)
        {
            this.web.Navigate(newValue);
        }

        public Uri AuthUrl
        {
            get
            {
                return (Uri)GetValue(AuthUrlProperty);
            }
            set
            {
                SetValue(AuthUrlProperty, value);
            }
        }
        #endregion

        #region DP Token string
        public static readonly DependencyProperty TokenProperty = DependencyProperty.Register("Token", typeof(string), typeof(TwitterAuth), new UIPropertyMetadata(string.Empty, new PropertyChangedCallback(OnTokenChanged), new CoerceValueCallback(OnCoerceToken)));

        private static object OnCoerceToken(DependencyObject o, object value)
        {
            TwitterAuth twitterAuth = o as TwitterAuth;
            if (twitterAuth != null)
                return twitterAuth.OnCoerceToken((string)value);
            else
                return value;
        }

        private static void OnTokenChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TwitterAuth twitterAuth = o as TwitterAuth;
            if (twitterAuth != null)
                twitterAuth.OnTokenChanged((string)e.OldValue, (string)e.NewValue);
        }

        protected virtual string OnCoerceToken(string value)
        {
            return value.Trim();
        }

        protected virtual void OnTokenChanged(string oldValue, string newValue)
        {
        }

        public string Token
        {
            get
            {
                return (string)GetValue(TokenProperty);
            }
            set
            {
                SetValue(TokenProperty, value);
            }
        }
        
        #endregion
        

    }
}

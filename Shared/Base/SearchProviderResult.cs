using System;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Media;
using Shared.Helpers;
using Color = System.Windows.Media.Color;

namespace Shared.Base
{




    public class KeyboardActionArgs
    {

        public object Sender;
        public SearchProviderResult Result;
        public KeyEventArgs KeyEvent;

    }




    public class SearchProviderLazyResult : SearchProviderResult
    {
        public readonly string SearchString;
        private readonly Func<SearchProviderLazyResult, string> lazyDescription;


        public SearchProviderLazyResult(SearchProvider provider, string searchString, Func<SearchProviderLazyResult, string> lazyDescription, int priority = PriorityNormal)
            : base(provider)
        {
            Priority = priority;
            DisplayName = searchString;
            SearchString = searchString;
            this.lazyDescription = lazyDescription;
        }

        public override string Description
        {
            get { return lazyDescription(this); }
        }
    }



    public class SearchProviderResult : AppQuickId
    {


        public const int PriorityStep = 100;
        public const int PriorityAlwaysLast = Int32.MinValue;
        public const int PriorityLast = Int32.MinValue + 1;
        public const int PriorityUltraLow = -1200;
        public const int PriorityBelowLow = PriorityUltraLow + PriorityStep;
        public const int PriorityLow = PriorityBelowLow + PriorityStep;
        public const int PriorityNormal = 0;
        public const int PriorityHigh = 1200;
        public const int PriorityAboveHigh = PriorityHigh + PriorityStep;
        public const int PriorityUltraHigh = PriorityAboveHigh + PriorityStep;
        public const int PriorityExactMatch = Int32.MaxValue - PriorityStep;
        public const int PriorityTopMost = Int32.MaxValue;




        public virtual System.Windows.Media.Brush BackgroundColor
        {
            get
            {
                return new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        public virtual System.Windows.Media.Brush ForegroundColor
        {
            get
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }


        public object Tag;
        public object ProviderMetadata;
        public int Priority { get; set; }

        private Func<KeyboardActionArgs, bool> keyboardAction;
        private Func<KeyboardActionArgs, bool> enterKeyAction;

        public event EventHandler<Exception> OnException;


        public SearchProviderResult SetEnterKeyAction(Func<KeyboardActionArgs, bool> newKeyboardAction)
        {
            enterKeyAction = newKeyboardAction;
            return this;

        }


        public SearchProviderResult SetKeyboardAction(Func<KeyboardActionArgs, bool> newKeyboardAction)
        {
            keyboardAction = newKeyboardAction;
            return this;

        }


        public string ProviderName
        {
            get { return Provider.GetName(); }
        }

        public override string ToString()
        {
            return DisplayName;

        }

        public SearchProvider Provider { get; set; }

        public readonly Color DisplayColor;
        private readonly Action<object, KeyEventArgs, SearchProviderResult, string> execRoutine;


        public SearchProviderResult(SearchProvider provider)
        {
            this.Provider = provider;
            DisplayColor = Color.FromRgb(0, 0, 0);

        }

        public SearchProviderResult()
        {
            DisplayColor = Color.FromRgb(0, 0, 0);

        }
        public bool GotKeyboardEvent(object sender, KeyEventArgs keyEventArgs)
        {

            try
            {
                var keyboardActionArgs = new KeyboardActionArgs
                {
                    KeyEvent = keyEventArgs,
                    Result = this,
                    Sender = sender
                };

                if (enterKeyAction != null && keyEventArgs.Key == Key.Enter)
                {
                    return enterKeyAction(keyboardActionArgs);
                }

                if (keyboardAction != null)
                {
                    return keyboardAction(keyboardActionArgs);
                }


            }
            catch (Exception e)
            {
                DoOnException(e);
            }
            return false;
        }

        protected virtual void DoOnException(Exception e)
        {
            var handler = OnException;
            if (handler != null) handler(this, e);
        }
    }
}
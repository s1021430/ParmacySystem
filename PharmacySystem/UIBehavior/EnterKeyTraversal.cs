using System.Windows;
using System.Windows.Input;

namespace PharmacySystem.UIBehavior
{
    public class EnterKeyTraversal
    {
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        static void UIElement_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || e.OriginalSource is not FrameworkElement uiElement) return;
            e.Handled = true;
            uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private static void UIElement_Unloaded(object sender, RoutedEventArgs e)
        {
            var ue = sender as FrameworkElement;
            if (ue == null) return;

            ue.Unloaded -= UIElement_Unloaded;
            ue.PreviewKeyDown -= UIElement_PreviewKeyDown;
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),
                typeof(EnterKeyTraversal), new UIPropertyMetadata(false, IsEnabledChanged));

        static void IsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement uiElement) return;

            if ((bool)e.NewValue)
            {
                uiElement.Unloaded += UIElement_Unloaded;
                uiElement.PreviewKeyDown += UIElement_PreviewKeyDown;
            }
            else
            {
                uiElement.PreviewKeyDown -= UIElement_PreviewKeyDown;
            }
        }
    }
}

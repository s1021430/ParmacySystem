using System;
using System.Windows;
using GalaSoft.MvvmLight;

namespace PharmacySystem.Class
{
    public class CommonSettingItem : ViewModelBase
    {
        private readonly Type contentType;
        private readonly object? dataContext;

        private object? _content;

        public CommonSettingItem(string name, Type contentType, object? dataContext = null)
        {
            Name = name;
            this.contentType = contentType;
            this.dataContext = dataContext;
        }

        public string Name { get; }

        public object? Content => _content ??= CreateContent();

        private object? CreateContent()
        {
            var content = Activator.CreateInstance(contentType);
            if (dataContext != null && content is FrameworkElement element)
            {
                element.DataContext = dataContext;
            }

            return content;
        }
    }
}

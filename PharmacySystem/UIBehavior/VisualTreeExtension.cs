using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PharmacySystem.UIBehavior
{
    public class VisualTreeExtension
    {

        private static void FocusCheckBox(DataGridCell cell)
        {
            var checkBox = FindVisualChild<CheckBox>(cell);
            checkBox?.Focus();
        }

        private static void FocusTextBox(DataGridCell cell)
        {
            var textBox = FindVisualChild<TextBox>(cell);
            textBox?.Focus();
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T)
                    return (T)child;
                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }
            return null;
        }

        public static void FocusSpecificCheckBoxCell(DataGrid dg, int row, int column,Type control)
        {
            var cell = GetCell(dg, row, column);
            if (control == typeof(TextBox))
                FocusTextBox(cell);
            else if(control == typeof(CheckBox))
                FocusCheckBox(cell);
        }

        public static DataGridCell GetCell(DataGrid dg, int row, int column)
        {
            var rowContainer = GetRow(dg, row);
            if (rowContainer == null) return null;
            var presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
            if (presenter == null) return null;
            var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
            if (cell != null) return cell;
            dg.ScrollIntoView(rowContainer, dg.Columns[column]);
            cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
            return cell;
        }

        private static DataGridRow GetRow(DataGrid dg, int rowIndex)
        {
            return (DataGridRow) dg?.ItemContainerGenerator.ContainerFromIndex(rowIndex);
        }
    }
}

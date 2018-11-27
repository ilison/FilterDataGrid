using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace DataGridExtensions
{
    public class DataGridFilterColumnControl : Control
    {
        protected DataGridFilterHost _filterHost { get; private set; }
        protected DataGrid _dataGrid { get; private set; }
        protected DataGridColumnHeader _columnHeader{get;private set;}
        public DataGridFilterColumnControl()
        {
            this.Loaded += (s, e) =>
            {
                _columnHeader = this.GetParent<DataGridColumnHeader>();
                if (_columnHeader == null)
                    throw new Exception();
                _dataGrid = _columnHeader.GetParent<DataGrid>();
                if (_dataGrid == null)
                    throw new Exception();
              
                 _filterHost = _dataGrid.GetFilterHost();

                var template = _columnHeader?.Column?.GetType();
                if (template == typeof(DataGridTextColumn))
                {
                    this.Template = this.TryFindResource("DataGridTextColumn") as ControlTemplate;
                    _filterHost.AddColumn(this);
                }
                else if (template == typeof(DataGridCheckBoxColumn))
                {
                    this.Template = this.TryFindResource("DataGridCheckBoxColumn") as ControlTemplate;
                    _filterHost.AddColumn(this);
                }
                else
                {
                    this.Template = null;
                }
            };
        }

        #region FilterMethod
               
        private object _filterValue { get; set; }

        internal bool Matches(object item)
        {
            if (_filterHost == null)
            {
                return true;
            }

            return IsMatches(GetColletion(item));
        }

        internal bool IsMatches(object item)
        {
            if (item == null)
                return false;
            if (_filterValue == null || string.IsNullOrWhiteSpace(_filterValue.ToString()))
            {
                return true;
            }

            return item.ToString().IndexOf(_filterValue.ToString(), StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        /// <summary>
        /// Identifies the CellValue dependency property, a private helper property used to evaluate the property path for the list items.
        /// </summary>
        private static readonly DependencyProperty _cellValueProperty =
            DependencyProperty.Register("_cellValue", typeof(object), typeof(DataGridFilterColumnControl));
        protected object GetColletion(object item)
        {
            var propertyPath = _columnHeader?.Column?.SortMemberPath;

            if (string.IsNullOrEmpty(propertyPath))
                return null;

            // Since already the name "SortMemberPath" implies that this might be not only a simple property name but a full property path
            // we use binding for evaluation; this will properly handle even complex property paths like e.g. "SubItems[0].Name"
            BindingOperations.SetBinding(this, _cellValueProperty, new Binding(propertyPath) { Source = item });
            var propertyValue = GetValue(_cellValueProperty);
            BindingOperations.ClearBinding(this, _cellValueProperty);

            return propertyValue;
        }
        #endregion

        #region FilterValue
        public static object GetFilterValue(DependencyObject obj)
        {
            return (object)obj.GetValue(FilterValueProperty);
        }

        public static void SetFilterValue(DependencyObject obj, object value)
        {
            obj.SetValue(FilterValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for FilterValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterValueProperty =
            DependencyProperty.RegisterAttached("FilterValue", typeof(object), typeof(DataGridFilterColumnControl), new PropertyMetadata(null, (s, e) => {
                var parent = s.GetParent<DataGridFilterColumnControl>();
                parent.OnFilterValue_Changed(e.NewValue);
             }));

        private void OnFilterValue_Changed(object value)
        {
            _filterValue = value;
            if (_filterHost == null)
                _filterHost = _dataGrid.GetFilterHost();
            _filterHost.Filter();
        }
        #endregion
    }

    public static class Ext
    {
        internal static T GetParent<T>(this DependencyObject obj) where T : class
        {
            while (obj!=null)
            {
                var target = obj as T;
                if (target != null)
                    return target;

                obj = LogicalTreeHelper.GetParent(obj) ?? VisualTreeHelper.GetParent(obj); ;
            }
            return null;
        }
    }
}

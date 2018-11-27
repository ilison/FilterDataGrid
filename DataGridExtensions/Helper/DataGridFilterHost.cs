using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DataGridExtensions
{
    public sealed class DataGridFilterHost
    {
        private DataGrid _dataGrid { get; set; }
        //Columns中的所有DataGridFilterColumnControl控件
        private readonly List<DataGridFilterColumnControl> _columnControls = new List<DataGridFilterColumnControl>();
        private bool _filterEnabled;
        public DataGridFilterHost(DataGrid dataGrid)
        {
            _dataGrid = dataGrid;
        }

        internal void AddColumn(DataGridFilterColumnControl control)
        {
            control.Visibility = _filterEnabled ? Visibility.Visible : Visibility.Hidden;
            _columnControls.Add(control);
        }

        internal void OnEnabled(bool value)
        {
            _filterEnabled = value;
            //过滤、筛选
            _columnControls.ForEach(contorl => contorl.Visibility = value ? Visibility.Visible : Visibility.Hidden);

            Filter();
        }

        internal void Filter()
        {
            var controls = _columnControls.Where(control => control.Visibility == Visibility.Visible);
            _dataGrid.Items.Filter = GetPredicate(controls?.ToList());
        }

        internal Predicate<object> GetPredicate(List<DataGridFilterColumnControl> list)
        {
            if (list.Any() != true)
                return item => list.All(filter => true);
            return item => list.All(filter => filter.Matches(item));
        }
    }
}

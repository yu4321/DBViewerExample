using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace DBViewerExample.Model
{
    public class TreeNode : ObservableObject

    {
        private string _Key;

        public string Key
        {
            get
            {
                return _Key;
            }

            set
            {
                Set(nameof(Key), ref _Key, value);
            }
        }

        private string _Value;

        public string Value
        {
            get
            {
                return _Value;
            }

            set
            {
                Set(nameof(Value), ref _Value, value);
            }
        }

        private int _Count;

        public int Count
        {
            get
            {
                return _Count;
            }

            set
            {
                Set(nameof(Count), ref _Count, value);
            }
        }

        private ObservableCollection<TreeNode> _Children;

        public ObservableCollection<TreeNode> Children
        {
            get
            {
                return _Children;
            }

            set
            {
                Set(nameof(Children), ref _Children, value);
            }
        }

        public TreeNode()
        {
            Key = "";
            Value = "";
            Children = new ObservableCollection<TreeNode>();
        }

        public void setCount()
        {
            Count = Children.Count;
        }
    }
}
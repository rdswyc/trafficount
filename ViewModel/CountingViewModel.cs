
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;

using Trafficount.Model;

namespace Trafficount.ViewModel
{
    public class CountingViewModel : BaseAdapter<CountItem>
    {
        private int cornerPosition;
        private Activity context;
        private List<CountItem> _items;

        private const string timerString = "timer_key";
        private Preferences PreferencesProvider;

        public CountingViewModel(Activity Context, int Position) : base()
        {
            cornerPosition = Position;
            context = Context;
            PreferencesProvider = new Preferences(context);
        }

        public long StopTime
        {
            get { return PreferencesProvider.GetLong(timerString); }
            set { PreferencesProvider.SetLong(timerString, value); }
        }

        public List<CountItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<CountItem>();

                    foreach (string dir in Constants.Directions)
                        _items.Add(new CountItem(dir));

                    _items.AddRange(DataProvider.GetCountItemsFromFile(cornerPosition));
                }

                return _items;
            }
        }

        public void SaveValues()
        {
            List<CountItem> items = new List<CountItem>();
            items.AddRange(_items);

            for (int i = 0; i < Constants.Directions.Length; i++)
                items.RemoveAt(0);

            DataProvider.SaveCountValuesInFile(cornerPosition, items);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            CountItem item = Items[position];

            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.GridItem, null);
            }

            ImageView itemImage = view.FindViewById<ImageView>(Resource.Id.itemImage);
            itemImage.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, (int)(context.Resources.DisplayMetrics.HeightPixels * 0.1));
            itemImage.SetBackgroundColor(item.Color);
            itemImage.SetImageResource(item.Icon);

            TextView itemText = view.FindViewById<TextView>(Resource.Id.itemText);
            itemText.Text = item.Value.ToString();
            itemText.Visibility = position < 3 ? ViewStates.Gone : ViewStates.Visible;

            return view;
        }

        public override CountItem this[int position]
        {
            get { return Items[position]; }
        }

        public override int Count
        {
            get { return Items.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
    }
}
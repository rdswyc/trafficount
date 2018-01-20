
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Text;
using Android.Views;
using Android.Widget;

using Trafficount.Model;

namespace Trafficount.ViewModel
{
    public class MainViewModel : BaseAdapter<Corner>
    {
        private Activity context;
        private List<Corner> _items;

        public event EventHandler ListChanged;

        public MainViewModel(Activity Context) : base()
        {
            context = Context;
        }

        public List<Corner> Items
        {
            get
            {
                if (_items == null)
                    _items = DataProvider.CornersInFile;

                return _items;
            }
        }


        public bool Exists(string Name)
        {
            return Items.Where(p => p.Name == Name).Count() > 0;
        }

        public void Remove(int position)
        {
            DataProvider.RemoveCornerFromFile(position);
            _items.RemoveAt(position);

            NotifyDataSetChanged();
        }

        public void Edit(int position, string name, string description)
        {
            DataProvider.EditCornerInFile(position, name, description);

            _items.ElementAt(position).Name = name;
            _items.ElementAt(position).Description = description;

            NotifyDataSetChanged();
        }

        public void Add(string name, string description = "")
        {
            DataProvider.AddCornerToFile(name, description);

            _items.Add(new Corner(name, description));

            NotifyDataSetChanged();
        }


        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();

            if (ListChanged != null)
                ListChanged(this, null);
        }


        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            Corner item = Items[position];

            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.ListRow, null);
            }

            view.FindViewById<TextView>(Resource.Id.itemHeader).Text = item.Name;
            view.FindViewById<TextView>(Resource.Id.itemSubtle).Text = item.Description;

            return view;
        }

        public override Corner this[int position]
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


        public ISpanned HtmlMessage
        {
            get
            {
                string emailTemplate;
                string emailBody = DataProvider.RootElement.ToString().Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");

                System.IO.Stream email = context.Assets.Open("EmailTemplate.html");

                using (System.IO.StreamReader reader = new System.IO.StreamReader(email))
                    emailTemplate = reader.ReadToEnd();

                emailBody = string.Format(emailTemplate, emailBody);

                return Html.FromHtml(emailBody);
            }
        }
    }
}
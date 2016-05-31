
using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.OS;

using Trafficount.ViewModel;

namespace Trafficount
{
    [Activity(Name = "rdswyc.Trafficount.MainActivity", Label = "@string/AppName", Icon = "@drawable/AppIcon", UiOptions = UiOptions.SplitActionBarWhenNarrow)]
    public class MainActivity : Activity
    {
        public const string Param = "position";
        public static MainViewModel Corners;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            SetEmptyLayout();

            Corners = new MainViewModel(this);
            Corners.ListChanged += ListChanged;

            ListView ListCorners = FindViewById<ListView>(Resource.Id.ListCorners);
            ListCorners.Adapter = Corners;
            ListCorners.ItemClick += OnItemClick;

            RegisterForContextMenu(ListCorners);

            Corners.NotifyDataSetChanged();
        }

        #region OptionsMenu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent intent;

            switch (item.ItemId)
            {
                case Resource.Id.menuAdd:

                    intent = new Intent(this, typeof(Details));
                    StartActivity(intent);

                    break;

                case Resource.Id.menuSettings:

                    intent = new Intent(this, typeof(Settings));
                    intent.PutExtra(Param, 1);
                    StartActivity(intent);

                    break;

                case Resource.Id.menuExport:

                    if (Corners.Count != 0)
                    {
                        intent = new Intent(Intent.ActionSend);
                        intent.SetType("message/rfc822");
                        intent.PutExtra(Intent.ExtraSubject, Resources.GetString(Resource.String.emailSubject));
                        intent.PutExtra(Intent.ExtraText, Corners.HtmlMessage);

                        StartActivity(Intent.CreateChooser(intent, Resources.GetString(Resource.String.emailChoose)));
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.noFile, ToastLength.Short).Show();
                    }

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region ContextMenu

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            if (v.Id == Resource.Id.ListCorners)
            {
                int position = ((AdapterView.AdapterContextMenuInfo)menuInfo).Position;
                menu.SetHeaderTitle(Corners[position].Name);

                menu.Add(Menu.None, 0, 0, Resource.String.edit);
                menu.GetItem(0).SetIcon(Resource.Drawable.edit);

                menu.Add(Menu.None, 1, 1, Resource.String.remove);
                menu.GetItem(1).SetIcon(Resource.Drawable.remove);
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            int position = ((AdapterView.AdapterContextMenuInfo)item.MenuInfo).Position;

            switch (item.ItemId)
            {
                case 0: //edit

                    Intent intent = new Intent(this, typeof(Details));
                    intent.PutExtra(Param, position);
                    StartActivity(intent);
                    break;

                case 1: //remove

                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle(Resource.String.removeConfirm);
                    alert.SetMessage(string.Format(GetString(Resource.String.removeMessage), Corners[position].Name));

                    alert.SetNegativeButton(Resource.String.no, (object s, DialogClickEventArgs e) => { });
                    alert.SetPositiveButton(Resource.String.yes, (object s, DialogClickEventArgs e) =>
                    {
                        Corners.Remove(position);
                        Toast.MakeText(this, Resource.String.removed, ToastLength.Short).Show();
                    });
                    alert.Show();

                    break;
            }

            return true;
        }

        #endregion

        #region CustomEvents

        private void ListChanged(object sender, EventArgs e)
        {
            bool hasItems = FindViewById<ListView>(Resource.Id.ListCorners).Count > 0;
            FindViewById<TextView>(Resource.Id.TextEmpty).Visibility = hasItems ? ViewStates.Gone : ViewStates.Visible;
        }

        public void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent intent = new Intent(this, typeof(Counting));
            intent.PutExtra(Param, e.Position);
            StartActivity(intent);
        }

        #endregion

        public void SetEmptyLayout()
        {
            TextView TextEmpty = FindViewById<TextView>(Resource.Id.TextEmpty);
            DisplayMetrics metrics = Resources.DisplayMetrics;

            LinearLayout.LayoutParams layout = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent);
            layout.Gravity = GravityFlags.Center;
            layout.TopMargin = (int)(metrics.HeightPixels * 0.4);

            TextEmpty.LayoutParameters = layout;
        }
    }
}
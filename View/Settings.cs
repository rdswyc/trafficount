
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Trafficount.ViewModel;

namespace Trafficount
{
    [Activity(Name = "rdswyc.Trafficount.Settings", Label = "@string/AppName", Icon = "@drawable/AppIcon", MainLauncher = true)]
    public class Settings : Activity
    {
        public TextView TextKeyLabel;
        public EditText TextKey;
        public LinearLayout LayoutAbout;
        public Button ButtonUpdate;
        public IMenuItem menuDone;
        public SettingsViewModel Activation;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Settings);

            TextKeyLabel = FindViewById<TextView>(Resource.Id.TextKeyLabel);
            TextKey = FindViewById<EditText>(Resource.Id.TextKey);
            LayoutAbout = FindViewById<LinearLayout>(Resource.Id.LayoutAbout);
            ButtonUpdate = FindViewById<Button>(Resource.Id.ButtonUpdate);

            ButtonUpdate.Click += ButtonUpdate_Click;

            Activation = new SettingsViewModel(this);

            Window.SetSoftInputMode(SoftInput.StateVisible);

            CheckActivation();
        }

        #region OptionsMenu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.SettingsMenu, menu);

            menuDone = menu.FindItem(Resource.Id.menuDone);

            if (Intent.GetIntExtra(MainActivity.Param, -1) != -1 && Activation.IsAppActive)
                menuDone.SetVisible(false);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuDone:

                    string key = FindViewById<EditText>(Resource.Id.TextKey).Text;

                    if (Activation.ValidateKey(key))
                    {
                        Activation.Key = key;
                        CheckActivation();
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.wrongKey, ToastLength.Short).Show();
                    }

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region CustomEvents

        private void ButtonUpdate_Click(object sender, EventArgs e)
        {
            TextKeyLabel.Text = Resources.GetString(Resource.String.insertKey);
            TextKey.Visibility = ViewStates.Visible;

            menuDone.SetVisible(true);
            ButtonUpdate.Visibility = ViewStates.Gone;

            Window.SetSoftInputMode(SoftInput.StateVisible);
        }

        #endregion

        public void CheckActivation()
        {
            if (Activation.IsAppActive)
            {
                if (Intent.GetIntExtra(MainActivity.Param, -1) == -1)
                {
                    Toast.MakeText(this, string.Format(Resources.GetString(Resource.String.appActivated), Activation.GoodThru), ToastLength.Long).Show();

                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);

                    Finish();
                }
                else
                {
                    SetNavigatedLayout();
                }
            }
            else
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.appNotActivated), ToastLength.Long).Show();
            }
        }

        public void SetNavigatedLayout()
        {
            Title = Resources.GetString(Resource.String.settings);
            ActionBar.SetIcon(Resource.Drawable.settings);

            FindViewById<TextView>(Resource.Id.TextVersion).Text = string.Format(Resources.GetString(Resource.String.AppVerion), PackageManager.GetPackageInfo(PackageName, 0).VersionName);
            TextKeyLabel.Text = string.Format(Resources.GetString(Resource.String.goodThru), Activation.GoodThru);

            TextKey.Visibility = ViewStates.Gone;
            LayoutAbout.Visibility = ViewStates.Visible;
            ButtonUpdate.Visibility = ViewStates.Visible;

            if (menuDone != null)
                menuDone.SetVisible(false);

            Window.SetSoftInputMode(SoftInput.StateHidden);
        }
    }
}

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Trafficount
{
    [Activity(Name = "rdswyc.Trafficount.Details", Label = "@string/details", ParentActivity = typeof(MainActivity))]
    public class Details : Activity
    {
        private int position;
        private string cornerName;
        private string cornerDetail;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Details);

            position = Intent.HasExtra(MainActivity.Param) ? Intent.GetIntExtra(MainActivity.Param, -1) : -1;

            if (position != -1)
            {
                FindViewById<EditText>(Resource.Id.TextName).Text = MainActivity.Corners[position].Name;
                FindViewById<EditText>(Resource.Id.TextDetails).Text = MainActivity.Corners[position].Description;
            }

            Window.SetSoftInputMode(SoftInput.StateVisible);
        }

        #region OptionsMenu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DetailsMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuDone:

                    cornerName = FindViewById<EditText>(Resource.Id.TextName).Text;
                    cornerDetail = FindViewById<EditText>(Resource.Id.TextDetails).Text;

                    if (cornerName == "")
                    {
                        Toast.MakeText(this, Resource.String.warningNull, ToastLength.Short).Show();
                    }
                    else
                    {
                        ManageCorner();
                    }

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        public void ManageCorner()
        {
            if (position == -1)
            {
                if (MainActivity.Corners.Exists(cornerName))
                {
                    Toast.MakeText(this, Resource.String.warningDuplicate, ToastLength.Short).Show();
                    return;
                }

                MainActivity.Corners.Add(cornerName, cornerDetail);

                Intent intent = new Intent(this, typeof(Counting));
                intent.PutExtra(MainActivity.Param, MainActivity.Corners.Count - 1);
                StartActivity(intent);
            }
            else
            {
                if (MainActivity.Corners.Exists(cornerName) && MainActivity.Corners[position].Name != cornerName)
                {
                    Toast.MakeText(this, Resource.String.warningDuplicate, ToastLength.Short).Show();
                    return;
                }

                MainActivity.Corners.Edit(position, cornerName, cornerDetail);
            }

            Finish();
        }
    }
}
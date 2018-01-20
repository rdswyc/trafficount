
using sysTimer = System.Timers;
using jarUtil = Java.Util;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Trafficount.ViewModel;

namespace Trafficount
{
    [Activity(Name = "rdswyc.Trafficount.Counting", ParentActivity = typeof(MainActivity))]
    public class Counting : Activity
    {
        private const long timerSpan = 900000;

        private int position;
        private string title;
        private long stopTime;
        private bool remove;
        private bool[] clicked;

        public sysTimer.Timer timer;
        public IMenuItem timerOnMenu;
        public IMenuItem timerOffMenu;
        public CountingViewModel Values;

        #region PageEvents

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Counting);

            position = Intent.GetIntExtra(MainActivity.Param, -1);
            Values = new CountingViewModel(this, position);

            GridView GridItems = FindViewById<GridView>(Resource.Id.GridItems);
            GridItems.Adapter = Values;
            GridItems.ItemClick += OnItemClick;
            GridItems.SetHorizontalSpacing((int)(Resources.DisplayMetrics.WidthPixels * 0.05));
            GridItems.SetVerticalSpacing((int)(Resources.DisplayMetrics.HeightPixels * 0.02));

            if (timer == null)
            {
                timer = new sysTimer.Timer(1000);
                timer.Elapsed += Timer_Elapsed;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            title = string.Format("{0}{1}{2}", MainActivity.Corners[position].Name, (MainActivity.Corners[position].Description == "" ? "" : " - "), MainActivity.Corners[position].Description);
            ActionBar.Title = title;

            remove = false;
            clicked = new bool[] { false, false, false };

            if (timer == null)
            {
                timer = new sysTimer.Timer(1000);
                timer.Elapsed += Timer_Elapsed;
            }

            long now = jarUtil.Calendar.GetInstance(jarUtil.Locale.Default).TimeInMillis;
            stopTime = Values.StopTime;

            if (stopTime > now)
                timer.Enabled = true;
        }

        protected override void OnPause()
        {
            base.OnPause();

            if (!remove)
                Values.SaveValues();

            if (timer != null)
            {
                if (timer.Enabled)
                    Values.StopTime = stopTime;

                timer.Stop();
            }
        }

        #endregion

        #region OptionsMenu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.CountingMenu, menu);

            timerOnMenu = menu.FindItem(Resource.Id.menuStartTimer);
            timerOffMenu = menu.FindItem(Resource.Id.menuStopTimer);

            timerOnMenu.SetVisible(!timer.Enabled);
            timerOffMenu.SetVisible(timer.Enabled);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menuStartTimer:

                    long now = jarUtil.Calendar.GetInstance(jarUtil.Locale.Default).TimeInMillis;
                    stopTime = now + timerSpan;

                    timer.Enabled = true;

                    ActionBar.Title = string.Format("({0}:00) {1}", (timerSpan / 60000L).ToString(), title);
                    timerOnMenu.SetVisible(false);
                    timerOffMenu.SetVisible(true);

                    break;

                case Resource.Id.menuStopTimer:

                        StopTimer();

                    break;

                case Resource.Id.menuEdit:

                    Intent intent = new Intent(this, typeof(Details));
                    intent.PutExtra(MainActivity.Param, position);
                    StartActivity(intent);

                    break;

                case Resource.Id.menuRemove:

                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle(Resource.String.removeConfirm);
                    alert.SetMessage(string.Format(GetString(Resource.String.removeMessage), MainActivity.Corners[position].Name));

                    alert.SetNegativeButton(Resource.String.no, (object s, DialogClickEventArgs e) => { });
                    alert.SetPositiveButton(Resource.String.yes, (object s, DialogClickEventArgs e) =>
                    {
                        remove = true;
                        MainActivity.Corners.Remove(position);
                        Toast.MakeText(this, Resource.String.removed, ToastLength.Short).Show();
                        Finish();
                    });
                    alert.Show();

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region CustomEvents

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            LinearLayout layout = ((LinearLayout)e.View);
            layout.Alpha = 0.3f;
            layout.Animate().SetDuration(100).Alpha(1f);

            if (e.Position < 3)
            {
                ImageView itemImage = layout.FindViewById<ImageView>(Resource.Id.itemImage);

                clicked[e.Position] = !clicked[e.Position];

                switch (e.Position)
                {
                    case 0:
                        itemImage.SetImageResource(clicked[e.Position] ? Resource.Drawable.arrow_left_up : Resource.Drawable.arrow_left);
                        break;

                    case 1:
                        itemImage.SetImageResource(clicked[e.Position] ? 0 : Resource.Drawable.arrow_up);
                        break;

                    case 2:
                        itemImage.SetImageResource(clicked[e.Position] ? Resource.Drawable.arrow_right_up : Resource.Drawable.arrow_right);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                TextView itemText = layout.FindViewById<TextView>(Resource.Id.itemText);

                itemText.Text = (int.Parse(itemText.Text) + 1).ToString();
                Values[e.Position].Value = int.Parse(itemText.Text);
            }
        }


        private void Timer_Elapsed(object sender, sysTimer.ElapsedEventArgs e)
        {
            long now = jarUtil.Calendar.GetInstance(jarUtil.Locale.Default).TimeInMillis;
            jarUtil.Calendar counter = jarUtil.Calendar.Instance;

            counter.TimeInMillis = stopTime - now;

            if (now > stopTime)
            {
                StopTimer();
            }
            else
            {
                RunOnUiThread(() =>
                    ActionBar.Title = string.Format("({0}:{1}) {2}", counter.Get(jarUtil.CalendarField.Minute).ToString("00"), counter.Get(jarUtil.CalendarField.Second).ToString("00"), title)
                );
            }
        }

        #endregion

        private void StopTimer()
        {
            timer.Stop();
            Values.StopTime = 0;

            RunOnUiThread(() =>
            {
                ActionBar.Title = title;
                timerOnMenu.SetVisible(true);
                timerOffMenu.SetVisible(false);

                Toast.MakeText(this, Resource.String.timerOver, ToastLength.Long).Show();
            });
        }
    }
}

using System;

using Java.Util;

using Android.App;

using Trafficount.Model;

namespace Trafficount.ViewModel
{
    public class SettingsViewModel
    {
        private const string prefString = "activation_key";
        private const int refDay = 1;
        private const int maxYear = 2026;
        private int[] _pi = { 14, 15, 92, 65, 35, 89, 79, 32, 38, 46, 26, 43 };

        private string _key;
        private string _goodThru;

        public Preferences Provider;


        public SettingsViewModel(Activity context)
        {
            Provider = new Preferences(context);
        }

        public string Key
        {
            get
            {
                if (_key == null)
                    _key = Provider.GetString(prefString);

                return _key;
            }
            set
            {
                Provider.SetString(prefString, value);

                _key = value;
                _goodThru = null;
            }
        }

        public string GoodThru
        {
            get
            {
                if (_goodThru == null)
                {
                    Calendar expire = DateFromKey(Key);

                    string month = expire.GetDisplayName(2, 1, Locale.Default);
                    int year = expire.Get(CalendarField.Year);

                    _goodThru = string.Format("{0}/{1}", month, year);
                }

                return _goodThru;
            }
        }

        public bool IsAppActive
        {
            get
            {
                if (Provider.Contains(prefString))
                {
                    Calendar today = Calendar.GetInstance(Locale.Default);
                    return today.Before(DateFromKey(Key));
                }

                return false;
            }
        }

        public bool ValidateKey(string key)
        {
            long value;

            if (long.TryParse(key, System.Globalization.NumberStyles.HexNumber, null, out value))
            {
                for (int i = 0; i < _pi.Length; i++)
                {
                    if (_pi[i] == Math.Truncate(value / 10000D))
                    {
                        if (value % 10000 < maxYear)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Calendar DateFromKey(string key)
        {
            int value = Convert.ToInt32(key, 16);

            int month = 0; int year;

            for (int i = 0; i < _pi.Length; i++)
                if (_pi[i] == Math.Truncate(value / 10000D))
                    month = i + 1;

            year = value % 10000;

            GregorianCalendar cal = new GregorianCalendar(year, month, refDay, 0, 0, 0);
            cal.Add(CalendarField.DayOfMonth, -1);

            return cal;
        }

        public string KeyFromDate(Calendar date)
        {
            int value = date.Get(CalendarField.Year) + 10000 * _pi[date.Get(CalendarField.Month) - 1];
            return value.ToString("X");
        }
    }
}
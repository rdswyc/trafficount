
using Android.App;
using Android.Content;

namespace Trafficount.Model
{
    public class Preferences
    {
        private ISharedPreferences instance;

        public Preferences(Activity context)
        {
            instance = context.GetPreferences(FileCreationMode.Private);
        }

        public bool Contains(string key)
        {
            return instance.Contains(key);
        }

        public string GetString(string key)
        {
            return instance.GetString(key, null);
        }

        public void SetString(string key, string value)
        {
            ISharedPreferencesEditor editor = instance.Edit();
            editor.PutString(key, value);
            editor.Commit();
        }

        public long GetLong(string key)
        {
            return instance.GetLong(key, 0);
        }

        public void SetLong(string key, long value)
        {
            ISharedPreferencesEditor editor = instance.Edit();
            editor.PutLong(key, value);
            editor.Commit();
        }
    }
}
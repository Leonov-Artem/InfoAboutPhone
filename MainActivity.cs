using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Telephony;
using Android.Content;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using System;
using System.Threading.Tasks;

namespace InfoAboutPhone
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button buttonOK;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            buttonOK = FindViewById<Button>(Resource.Id.buttonOK);
            buttonOK.Click += OnClickButtonOk;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void OnClickButtonOk(object o, EventArgs e)
        {
            var IMEI = await GetIMEI();

            buttonOK.Text = $"Модель: {DeviceInfo.Model}\n" +
                        $"Производитель: {DeviceInfo.Manufacturer}\n" +
                        $"Название: {DeviceInfo.Name}\n" +
                        $"Версия андроид: {DeviceInfo.VersionString}\n" +
                        $"Язык интерфейса: {GetInterfaceLanguage()}\n" +
                        $"ОЗУ: {GetTotalRAM()}ГБ";

            if (IMEI != null)
                buttonOK.Text += $"\nIMEI: {IMEI}";
        }

        private async Task<PermissionStatus> GetPermissionStatusAsync()
        {
            // Подтверждение ограничения
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Phone);
            if (status != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Phone);

                if (results.ContainsKey(Permission.Phone))
                    status = results[Permission.Phone];
            }

            return status;
        }

        private async Task<string> GetIMEI()
        {
            var status = await GetPermissionStatusAsync();
            if (status == PermissionStatus.Granted)
            {
                var manager = GetSystemService(Context.TelephonyService) as TelephonyManager;
                return manager.Imei;
            }
            else
                return null;
        }

        private string GetInterfaceLanguage()
            => Java.Util.Locale.Default.ToString();

        private double GetTotalRAM()
        {
            var activityManager = GetSystemService(Activity.ActivityService) as ActivityManager;
            var memoryInfo = new ActivityManager.MemoryInfo();
            activityManager.GetMemoryInfo(memoryInfo);

            return ConvertToGB(memoryInfo.TotalMem);
        }

        private double ConvertToGB(double bytes)
            => Math.Round(bytes / (double)(1024 * 1024 * 1024), 2);
    }
}
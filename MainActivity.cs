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
using Java.Lang;
using Android.Hardware;
using Android.Util;
using Android.Net.Wifi;

namespace InfoAboutPhone
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TextView textView;
        private Info info;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var telephonyManager = GetSystemService(Context.TelephonyService) as TelephonyManager;
            var activityManager = GetSystemService(Activity.ActivityService) as ActivityManager;
            var wifiManager = GetSystemService(Context.WifiService) as WifiManager;
            info = new Info(telephonyManager, activityManager, wifiManager);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            textView = FindViewById<TextView>(Resource.Id.infoDisplay);
            textView.Click += OnClickButtonOk;
            textView.TextSize = 20;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void OnClickButtonOk(object o, EventArgs e)
        {
            var IMEI = await info.IMEI();

            textView.Text = $"Модель: {info.Model}\n" +
                            $"Производитель: {info.Manufacturer}\n" +
                            $"Бренд: {info.Brand}\n" +
                            $"Название: {info.Name}\n" +
                            $"Версия андроид: {info.Version}\n" +
                            $"Язык интерфейса: {info.InterfaceLanguage}\n" +
                            $"ОЗУ: {info.TotalRAM}ГБ\n" +
                            $"Слотов для сим-карт: {info.SimCardsCount}\n" +
                            $"Уровень API: {info.API}\n" +
                            $"Время с момента включения: {info.TimeFromStart}ч.\n" +
                            $"Серийный номер: {info.SerialNumber}\n" +
                            $"CPU: \n============\n{info.CPUInfo}\n============";
                            //$"MAC: {info.GetMACAdress()}";     
            
            if (IMEI != null)
                textView.Text += $"\nIMEI: {IMEI}";
        }
    }
}
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

namespace InfoAboutPhone
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button buttonOK;
        private TextView textView;
        private Info info;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var telephonyManager = GetSystemService(Context.TelephonyService) as TelephonyManager;
            var activityManager = GetSystemService(Activity.ActivityService) as ActivityManager;
            info = new Info(telephonyManager, activityManager);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //buttonOK = FindViewById<Button>(Resource.Id.buttonOK);
            textView = FindViewById<TextView>(Resource.Id.infoDisplay);
            textView.Click += OnClickButtonOk;
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

            //buttonOK.TextAlignment = Android.Views.TextAlignment.ViewStart;
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
                            $"CPU: \n{info.CPUInfo}\n";


            if (IMEI != null)
                textView.Text += $"\nIMEI: {IMEI}";
        }
    }
}
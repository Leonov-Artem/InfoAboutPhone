using Android.App;
using Android.Telephony;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.OS;
using Java.Lang;

namespace InfoAboutPhone
{
    public class Info
    {
        TelephonyManager telephonyManager;
        ActivityManager activityManager;

        public string Model
            => Build.Model;

        public string Brand
            => Build.Brand;

        public string Manufacturer
            => Build.Manufacturer;

        public string Name
            => DeviceInfo.Name;

        public string Version
            => DeviceInfo.VersionString;

        public string InterfaceLanguage
            => Java.Util.Locale.Default.ToString();

        public double TotalRAM
            => GetTotalRAM();

        public int SimCardsCount
            => telephonyManager.PhoneCount;

        public string API
            => Build.VERSION.Sdk;

        public double TimeFromStart
            =>System.Math.Round(SystemClock.ElapsedRealtime() * 2.78e-7, 2);

        public string SerialNumber
            => Build.Serial;

        public string CPUInfo
            => GetCPUInfo();

        public Info(TelephonyManager telephonyManager, ActivityManager activityManager)
        {
            this.telephonyManager = telephonyManager;
            this.activityManager = activityManager;           
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

        public async Task<string> IMEI()
        {
            var status = await GetPermissionStatusAsync();

            if (status == PermissionStatus.Granted)
                return telephonyManager.Imei;
            else
                return null;
        }

        private double GetTotalRAM()
        {
            var memoryInfo = new ActivityManager.MemoryInfo();
            activityManager.GetMemoryInfo(memoryInfo);

            return ConvertToGB(memoryInfo.TotalMem);
        }

        private double ConvertToGB(double bytes)
            => System.Math.Ceiling(bytes / (1024 * 1024 * 1024));

        public string GetCPUInfo()
        {
            string[] DATA = { "/system/bin/cat", "/proc/cpuinfo" };
            var processBuilder = new ProcessBuilder(DATA);

            Java.Lang.Process process = processBuilder.Start();

            System.IO.Stream inputStream = process.InputStream;
            byte[] byteArry = new byte[1024];
            string output = "";

            inputStream.Read(byteArry);
            output += new Java.Lang.String(byteArry);

            inputStream.Close();

            return output;
        }
    }
}
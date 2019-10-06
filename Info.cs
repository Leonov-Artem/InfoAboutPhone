using Android.App;
using Android.Telephony;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Essentials;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfoAboutPhone
{
    public class Info
    {
        TelephonyManager telephonyManager;
        ActivityManager activityManager;

        public string Model
            => DeviceInfo.Model;

        public string Manufacturer
            => DeviceInfo.Manufacturer;

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
            => Android.OS.Build.VERSION.Sdk;

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
            => Math.Ceiling(bytes / (1024 * 1024 * 1024));
    }
}
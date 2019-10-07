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
using Android.Net.Wifi;
using System.Net.NetworkInformation;
using System.Linq;

namespace InfoAboutPhone
{
    public class Info
    {
        private TelephonyManager telephonyManager;
        private ActivityManager activityManager;
        private WifiManager wifiManager;

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
        {
            get
            {
                var memoryInfo = new ActivityManager.MemoryInfo();
                activityManager.GetMemoryInfo(memoryInfo);

                return ConvertToGB(memoryInfo.TotalMem);
            }
        }

        public int SimCardsCount
            => telephonyManager.PhoneCount;

        public string API
            => Build.VERSION.Sdk;

        public double TimeFromStart
            =>System.Math.Round(SystemClock.ElapsedRealtime() * 2.78e-7, 2);

        public string SerialNumber
            => Build.Serial;

        public string CPUInfo
        {
            get
            {
                string[] DATA = { "/system/bin/cat", "/proc/cpuinfo" };
                var processBuilder = new ProcessBuilder(DATA);

                Java.Lang.Process process = processBuilder.Start();

                System.IO.Stream inputStream = process.InputStream;
                var byteArry = new byte[1024];
                string output = "";

                inputStream.Read(byteArry);
                output += new Java.Lang.String(byteArry);

                inputStream.Close();

                return output;
            }
        }

        public Info(TelephonyManager telephonyManager, ActivityManager activityManager, WifiManager wifiManager)
        {
            this.telephonyManager = telephonyManager;
            this.activityManager = activityManager;
            this.wifiManager = wifiManager;
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

        private double ConvertToGB(double bytes)
            => System.Math.Ceiling(bytes / (1024 * 1024 * 1024));

        public string GetMACAdress()
        {
            //WifiInfo wInfo = wifiManager.ConnectionInfo;
            //return wInfo.MacAddress;
            var ni = NetworkInterface.GetAllNetworkInterfaces()
                   .OrderBy(intf => intf.NetworkInterfaceType)
                   .FirstOrDefault(intf => intf.OperationalStatus == OperationalStatus.Up
                         && (intf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                             || intf.NetworkInterfaceType == NetworkInterfaceType.Ethernet));
            var hw = ni.GetPhysicalAddress();
            return string.Join(":", (from ma in hw.GetAddressBytes() select ma.ToString("X2")).ToArray());
        }
    }
}
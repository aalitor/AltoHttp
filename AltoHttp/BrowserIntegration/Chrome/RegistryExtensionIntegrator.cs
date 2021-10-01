using Microsoft.Win32;
using System.Security.Principal;
using System.Linq;
using System.Collections.Generic;
namespace AltoHttp.BrowserIntegration.Chrome
{
    class RegistryExtensionIntegrator
    {
        public static void Complete(string hostPath, string extname)
        {
            var regNatMsgPath = @"SOFTWARE\Google\Chrome\NativeMessagingHosts\";
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var natmsgkey = hklm.OpenSubKey(regNatMsgPath, true);
            var key = natmsgkey.CreateSubKey(extname);

            key.SetValue("", hostPath);
        }
        public static bool HasAdminRights()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }
        public static void Remove(string extname)
        {
            var regNatMsgPath = @"SOFTWARE\Google\Chrome\NativeMessagingHosts\";
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var natmsgkey = hklm.OpenSubKey(regNatMsgPath, true);
            var key = natmsgkey.CreateSubKey(extname);

            key.SetValue("", "");
        }
        public static bool CheckHost(string extname, string hostPath)
        {
            var regNatMsgPath = @"SOFTWARE\Google\Chrome\NativeMessagingHosts\";
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var natmsgkey = hklm.OpenSubKey(regNatMsgPath, false);
            var key = natmsgkey.GetSubKeyNames().FirstOrDefault(x=> x== extname);
            if(string.IsNullOrEmpty(key))
                return false;

            var key2 = natmsgkey.OpenSubKey(extname);
            if(key2 == null)
                return false;

            return key2.GetValue("").ToString() == hostPath;
        }

        public static bool CheckExtension(string extId)
        {
            var regExtPath = @"SOFTWARE\Google\Chrome\Extensions\";
            var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            var natmsgkey = hklm.OpenSubKey(regExtPath, false);
            var key = natmsgkey.GetSubKeyNames().FirstOrDefault(x => x == extId);
            if (string.IsNullOrEmpty(key))
                return false;

            return true;
        }
        public static bool CheckIntegration(string extname, string hostPath, string extId)
        {
            return CheckHost(extname, hostPath) && CheckExtension(extId);
        }
    }
}

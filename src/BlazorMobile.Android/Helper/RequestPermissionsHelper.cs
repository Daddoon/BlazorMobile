

using Android.OS;
using Android.Support.Design.Widget;
using Android.Views.Accessibility;
using BlazorMobile.Droid.Services;
using System.Linq;

namespace BlazorMobile.Droid.Helper
{
    internal static class RequestPermissionHelper
    {
        public static int BLAZORMOBILE_REQUESTCODE = 8755;

        /// <summary>
        /// Check for a permission available in <seealso cref="Android.Manifest.Permission"/> namespace
        /// </summary>
        /// <param name="permission"></param>
        /// 
        public static void CheckForPermission(string[] permission, System.Action onGranted, System.Action onDenied)
        {
            string[] needAdditionalPermissions = permission.Where(p => BlazorWebViewService.GetCurrentActivity().CheckSelfPermission(p) != Android.Content.PM.Permission.Granted).ToArray();

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && needAdditionalPermissions != null && needAdditionalPermissions.Length > 0)
            {
                BlazorWebViewService.GetCurrentActivity().RequestPermissions(needAdditionalPermissions, BLAZORMOBILE_REQUESTCODE);

                //TODO: We must forward event directly from an async result OR at least show a toast in order to ask to repeat the action again

                //We must dismiss the result, and the user have to retry after validating the permission
                onDenied?.Invoke();
            }
            else
            {
                //We already have the permission
                onGranted?.Invoke();
            }
        }

        /// <summary>
        /// Check for a permission available in <seealso cref="Android.Manifest.Permission"/> namespace
        /// </summary>
        /// <param name="permission"></param>
        /// 
        public static void CheckForPermission(string permission, System.Action onGranted, System.Action onDenied)
        {
            CheckForPermission(new string[] { permission }, onGranted, onDenied);
        }
    }
}
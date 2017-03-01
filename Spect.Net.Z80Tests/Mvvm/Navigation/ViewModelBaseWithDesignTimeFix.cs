using System;
using System.Linq;
using System.Reflection;
using GalaSoft.MvvmLight;

namespace Spect.Net.Z80Tests.Mvvm.Navigation
{
    /// <summary>
    /// This class fixes the design time issue with MvvmLibs
    /// </summary>
    public abstract class ViewModelBaseWithDesignTimeFix : ViewModelBase
    {
        private static bool? s_IsInDesignMode;
        private const string WPD_IDENTITY = "Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
        private const string WINDOWS_RUNTIME_IDENTITY = "Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime";

        private enum DesignerPlatformLibrary
        {
            Unknown,
            Net,
            WinRt
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public new static bool IsInDesignModeStatic
        {
            get
            {
                if (!s_IsInDesignMode.HasValue)
                    s_IsInDesignMode = IsInDesignModePortable();
                return s_IsInDesignMode.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public new bool IsInDesignMode => IsInDesignModeStatic;

        /// <summary>
        /// Determines the current platform
        /// </summary>
        /// <returns></returns>
        private static DesignerPlatformLibrary GetCurrentPlatform()
        {
            if (Type.GetType($"System.ComponentModel.DesignerProperties, PresentationFramework, {WPD_IDENTITY}") != null)
                return DesignerPlatformLibrary.Net;
            return Type.GetType(WINDOWS_RUNTIME_IDENTITY) != null
                ? DesignerPlatformLibrary.WinRt
                : DesignerPlatformLibrary.Unknown;
        }

        /// <summary>
        /// Checks if design mode is in a portable library
        /// </summary>
        /// <returns></returns>
        private static bool IsInDesignModePortable()
        {
            switch (GetCurrentPlatform())
            {
                case DesignerPlatformLibrary.WinRt:
                    return IsInDesignModeWinRt();
                case DesignerPlatformLibrary.Net:
                    return IsInDesignModeNet();
                default:
                    return false;
            }
        }

        private static bool IsInDesignModeWinRt()
        {
            try
            {
                return (bool)Type.GetType(WINDOWS_RUNTIME_IDENTITY)
                    .GetTypeInfo()
                    .GetDeclaredProperty("DesignModeEnabled")
                    .GetValue(null, null);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInDesignModeNet()
        {
            try
            {
                var dm = Type.GetType($"System.ComponentModel.DesignerProperties, PresentationFramework, {WPD_IDENTITY}");
                if (dm == null)
                {
                    return false;
                }

                var dmp = dm.GetTypeInfo().GetDeclaredField("IsInDesignModeProperty").GetValue(null);

                var dpd = Type.GetType($"System.ComponentModel.DependencyPropertyDescriptor, WindowsBase, {WPD_IDENTITY}");
                var typeFe = Type.GetType($"System.Windows.FrameworkElement, PresentationFramework, {WPD_IDENTITY}");

                if (dpd == null || typeFe == null)
                {
                    return false;
                }

                var fromPropertys = dpd
                    .GetTypeInfo()
                    .GetDeclaredMethods("FromProperty")
                    .ToList();

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (fromPropertys == null || fromPropertys.Count == 0)
                {
                    return false;
                }

                var fromProperty = fromPropertys
                    .FirstOrDefault(mi => mi.IsPublic && mi.IsStatic && mi.GetParameters().Length == 2);

                var descriptor = fromProperty?.Invoke(null, new[] { dmp, typeFe });
                if (descriptor == null)
                {
                    return false;
                }

                var metaProp = dpd.GetTypeInfo().GetDeclaredProperty("Metadata");

                if (metaProp == null)
                {
                    return false;
                }

                var metadata = metaProp.GetValue(descriptor, null);
                var tPropMeta = Type.GetType($"System.Windows.PropertyMetadata, WindowsBase, {WPD_IDENTITY}");

                if (metadata == null || tPropMeta == null)
                {
                    return false;
                }

                var dvProp = tPropMeta.GetTypeInfo().GetDeclaredProperty("DefaultValue");

                if (dvProp == null)
                {
                    return false;
                }

                var dv = (bool)dvProp.GetValue(metadata, null);
                return dv;
            }
            catch
            {
                return false;
            }
        }
    }
}
using System;

namespace Spect.Net.Wpf.Mvvm
{
    /// <summary>
    /// Helper class for platform detection.
    /// </summary>
    internal static class DesignerLibrary
    {
        internal static DesignerPlatformLibrary DetectedDesignerLibrary
        {
            get
            {
                if (s_DetectedDesignerPlatformLibrary == null)
                {
                    s_DetectedDesignerPlatformLibrary = GetCurrentPlatform();
                }
                return s_DetectedDesignerPlatformLibrary.Value;
            }
        }

        private static DesignerPlatformLibrary GetCurrentPlatform()
        {
            // We check Silverlight first because when in the VS designer, the .NET libraries will resolve
            // If we can resolve the SL libs, then we're in SL or WP
            // Then we check .NET because .NET will load the WinRT library (even though it can't really run it)
            // When running in WinRT, it will not load the PresentationFramework lib

            // Check Silverlight
            var dm = Type.GetType("System.ComponentModel.DesignerProperties, System.Windows, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e");
            if (dm != null)
            {
                return DesignerPlatformLibrary.Silverlight;
            }

            // Check .NET 
            var cmdm = Type.GetType("System.ComponentModel.DesignerProperties, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
            if (cmdm != null) // loaded the assembly, could be .net 
            {
                return DesignerPlatformLibrary.Net;
            }

            // check WinRT next
            var wadm = Type.GetType("Windows.ApplicationModel.DesignMode, Windows, ContentType=WindowsRuntime");
            if (wadm != null)
            {
                return DesignerPlatformLibrary.WinRt;
            }

            return DesignerPlatformLibrary.Unknown;
        }


        private static DesignerPlatformLibrary? s_DetectedDesignerPlatformLibrary;

        private static bool? s_IsInDesignMode;

        public static bool IsInDesignMode
        {
            get
            {
                if (s_IsInDesignMode.HasValue) return s_IsInDesignMode.Value;
                var prop = System.ComponentModel.DesignerProperties.IsInDesignModeProperty;
                s_IsInDesignMode
                    = (bool)System.ComponentModel.DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(System.Windows.FrameworkElement))
                        .Metadata.DefaultValue;
                return s_IsInDesignMode.Value;
            }
        }

    }

    internal enum DesignerPlatformLibrary
    {
        Unknown,
        Net,
        WinRt,
        Silverlight
    }
}

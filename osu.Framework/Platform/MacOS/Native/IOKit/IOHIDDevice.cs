// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Runtime.InteropServices;
using osu.Framework.Platform.MacOS.Native.CoreFoundation;

// ReSharper disable InconsistentNaming

namespace osu.Framework.Platform.MacOS.Native.IOKit
{
    public class IOHIDDevice : NativeObject
    {
        public const string kIOHIDTransportKey = "Transport";
        public const string kIOHIDVendorIDKey = "VendorID";
        public const string kIOHIDVendorIDSourceKey = "VendorIDSource";
        public const string kIOHIDProductIDKey = "ProductID";
        public const string kIOHIDVersionNumberKey = "VersionNumber";
        public const string kIOHIDManufacturerKey = "Manufacturer";
        public const string kIOHIDProductKey = "Product";
        public const string kIOHIDSerialNumberKey = "SerialNumber";
        public const string kIOHIDCountryCodeKey = "CountryCode";
        public const string kIOHIDStandardTypeKey = "StandardType";
        public const string kIOHIDLocationIDKey = "LocationID";
        public const string kIOHIDDeviceUsageKey = "DeviceUsage";
        public const string kIOHIDDeviceUsagePageKey = "DeviceUsagePage";
        public const string kIOHIDDeviceUsagePairsKey = "DeviceUsagePairs";
        public const string kIOHIDPrimaryUsageKey = "PrimaryUsage";
        public const string kIOHIDPrimaryUsagePageKey = "PrimaryUsagePage";
        public const string kIOHIDMaxInputReportSizeKey = "MaxInputReportSize";
        public const string kIOHIDMaxOutputReportSizeKey = "MaxOutputReportSize";
        public const string kIOHIDMaxFeatureReportSizeKey = "MaxFeatureReportSize";
        public const string kIOHIDReportIntervalKey = "ReportInterval";
        public const string kIOHIDReportDescriptorKey = "ReportDescriptor";

        public string Transport => GetProperty(kIOHIDTransportKey);
        public string VendorID => GetProperty(kIOHIDVendorIDKey);
        public string VendorIDSource => GetProperty(kIOHIDVendorIDSourceKey);
        public string ProductID => GetProperty(kIOHIDProductIDKey);
        public string VersionNumber => GetProperty(kIOHIDVersionNumberKey);
        public string Manufacturer => GetProperty(kIOHIDManufacturerKey);
        public string Product => GetProperty(kIOHIDProductKey);
        public string SerialNumber => GetProperty(kIOHIDSerialNumberKey);
        public string CountryCode => GetProperty(kIOHIDCountryCodeKey);
        public string StandardType => GetProperty(kIOHIDStandardTypeKey);
        public string LocationID => GetProperty(kIOHIDLocationIDKey);
        public string DeviceUsage => GetProperty(kIOHIDDeviceUsageKey);
        public string DeviceUsagePage => GetProperty(kIOHIDDeviceUsagePageKey);
        public string DeviceUsagePairs => GetProperty(kIOHIDDeviceUsagePairsKey);
        public string PrimaryUsage => GetProperty(kIOHIDPrimaryUsageKey);
        public string PrimaryUsagePage => GetProperty(kIOHIDPrimaryUsagePageKey);
        public string MaxInputReportSize => GetProperty(kIOHIDMaxInputReportSizeKey);
        public string MaxOutputReportSize => GetProperty(kIOHIDMaxOutputReportSizeKey);
        public string MaxFeatureReportSize => GetProperty(kIOHIDMaxFeatureReportSizeKey);
        public string ReportInterval => GetProperty(kIOHIDReportIntervalKey);
        public string ReportDescriptor => GetProperty(kIOHIDReportDescriptorKey);

        internal IOHIDDevice(IntPtr handle)
            : base(handle)
        {
        }

        public static IOHIDDevice Create(IntPtr allocator, IntPtr service)
        {
            var handle = IOKitNative.IOHIDDeviceCreate(allocator, service);
            return handle == IntPtr.Zero ? null : new IOHIDDevice(handle);
        }

        public string GetProperty(string name)
        {
            var cfName = new CFString(name);
            IntPtr ptr = IOKitNative.IOHIDDeviceGetProperty(Handle, cfName.Handle);
            return new CFString(ptr).ToString();
        }
    }
}

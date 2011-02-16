namespace RXDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class UsbTEMPer : IDisposable
    {
        private double _compensator;
        private int _deviceId;
        private object _lck = new object();
        public UsbTEMPer(int device)
        {
            _deviceId = device;
            Initalize();
        }

        private void Initalize()
        {
            HIDFT.EMySetCurrentDev(_deviceId);
            Thread.Sleep(100);
            var ret = HIDFT.EMyInitConfig(true);
            if (!ret)
            {
                throw new InvalidOperationException("the type is not temper");
            }

            byte[] upd = new byte[6];
            HIDFT.EMyReadEP(ref upd[0], ref upd[1], ref upd[2], ref upd[3], ref upd[4], ref upd[5]);
            if (upd[1] != 0x58)
            {
                throw new InvalidOperationException("the type is not temper");
            }

            _compensator = (upd[2] >= 10) & (upd[2] <= 0x1d) ? Convert.ToDouble(String.Format("{0}.{1}", upd[2], upd[3])) - 20.0 : 0.0;
        }

        public double GetTemperature()
        {
            return GetTempReadings()
                .Take(10)
                .Where(tmp => tmp != 999 && tmp != 888 )
                .FirstOrDefault() + _compensator;
        }

        public IEnumerable<double> GetTempReadings()
        {
            lock (_lck)
            {
                double tmp = HIDFT.EMyReadTemp(true);
                yield return tmp; 
            }
        }

        internal static IEnumerable<string> FindDevices()
        {
            return FindDevices(IntPtr.Zero);
        }

        public static IEnumerable<string> FindDevices(IntPtr hndl)
        {
            int devCount = HIDFT.EMyDetectDevice((long)hndl); 
            return Enumerable.Range(0, devCount)
                .Select(ii => String.Format("HID Device {0}", ii));
        }

        public void Dispose()
        {
            HIDFT.EMyCloseDevice();
        }

        public static double CelsiustoFarenheight(double celsius)
        {
            return (9 * celsius / 5 + 32);
        }

        #region DllImports
        internal sealed class HIDFT
        {
            // Methods
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern void EMyCloseDevice();
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern int EMyDetectDevice(long myhwnd);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool EMyInitConfig(bool dOrc);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool EMyReadEP(ref byte up1, ref byte up2, ref byte up3, ref byte up4, ref byte up5, ref byte up6);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool EMyReadHid([MarshalAs(UnmanagedType.AnsiBStr)] ref string pcBuffer, byte btUrlIndex, int btUrlLen);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern double EMyReadTemp(bool flag);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern void EMySetCurrentDev(int nCurDev);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool EMyWriteEP(ref byte fd1, ref byte fd2, ref byte fd3, ref byte fd4, ref byte fd5);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool EMyWriteHid(ref char[] pcBuffer, byte btUrlIndex, int btUrlLen);
            [DllImport("HidFTDll.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
            public static extern bool EMyWriteTempText(bool flag);
        }
        #endregion
    }
}

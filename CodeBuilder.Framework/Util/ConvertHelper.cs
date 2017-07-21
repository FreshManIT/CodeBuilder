using System;

// ReSharper disable once CheckNamespace
namespace CodeBuilder.Util
{
    public static class ConvertHelper
    {
        public static bool GetBoolean(int value)
        {
            return value > 0;
        }

        public static Int64 GetInt64(String str)
        {
            Int64 data;
            Int64.TryParse(str,out data);
            return data;
        }

        public static int GetInt32(String str)
        {
            Int32 data;
            Int32.TryParse(str, out data);
            return data;
        }

        public static short GetInt16(String str)
        {
            Int16 data;
            Int16.TryParse(str, out data);
            return data;
        }

        public static Byte GetByte(String str)
        {
            Byte data;
            Byte.TryParse(str, out data);
            return data;
        }

        public static float GetFloat(String str)
        {
            Single data;
            Single.TryParse(str, out data);
            return data;
        }

        public static double GetDouble(String str)
        {
            Double data;
            Double.TryParse(str, out data);
            return data;
        }

        public static decimal GetDecimal(String str)
        {
            Decimal data;
            Decimal.TryParse(str, out data);
            return data;
        }

        public static Single GetSingle(String str)
        {
            Single data;
            Single.TryParse(str, out data);
            return data;
        }

        public static bool GetBoolean(String str)
        {
            Boolean data;
            Boolean.TryParse(str, out data);
            return data;
        }

        public static byte[] GetBytes(String str)
        {
            if (String.IsNullOrEmpty(str) ||
               str.Trim().Length == 0) return null;

            return System.Text.Encoding.Unicode.GetBytes(str);
        }

        public static Guid GetGuid(String str)
        {
            if (String.IsNullOrEmpty(str) || 
                str.Trim().Length == 0) return Guid.Empty;

            Guid data = new Guid(str);
            return data;
        }

        public static DateTime GetDateTime(String str)
        {
            if (String.IsNullOrEmpty(str) || 
                str.Trim().Length == 0) return DateTime.Now;

            DateTime data;
            DateTime.TryParse(str, out data);
            return data;
        }
    }
}

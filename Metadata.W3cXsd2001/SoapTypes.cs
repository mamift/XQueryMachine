using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Metadata.W3cXsd2001
{
    [ComVisible(true)]
    [Serializable]
    public sealed class SoapAnyUri : ISoapXsd
    {
        private string _value;

        public static string XsdType => "anyURI";

        public string GetXsdType() => SoapAnyUri.XsdType;

        public SoapAnyUri() { }

        public SoapAnyUri(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => this._value;

        public static SoapAnyUri Parse(string value) => new SoapAnyUri(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapBase64Binary : ISoapXsd
    {
        private byte[] _value;

        public static string XsdType => "base64Binary";

        public string GetXsdType() => SoapBase64Binary.XsdType;

        public SoapBase64Binary() { }

        public SoapBase64Binary(byte[] value) => this._value = value;

        public byte[] Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() =>
            this._value == null ? (string)null : SoapType.LineFeedsBin64(Convert.ToBase64String(this._value));

        public static SoapBase64Binary Parse(string value)
        {
            switch (value) {
                case "":
                case null:
                    return new SoapBase64Binary(new byte[0]);
                default:
                    byte[] numArray;
                    try {
                        numArray = Convert.FromBase64String(SoapType.FilterBin64(value));
                    }
                    catch (Exception ex) {
                        throw new Exception("Remoting_SOAPInteropxsdInvalid" + "base64Binary");
                    }

                    return new SoapBase64Binary(numArray);
            }
        }
    }

    internal static class SoapType
    {
        internal static Type typeofSoapTime = typeof(SoapTime);
        internal static Type typeofSoapDate = typeof(SoapDate);
        internal static Type typeofSoapYearMonth = typeof(SoapYearMonth);
        internal static Type typeofSoapYear = typeof(SoapYear);
        internal static Type typeofSoapMonthDay = typeof(SoapMonthDay);
        internal static Type typeofSoapDay = typeof(SoapDay);
        internal static Type typeofSoapMonth = typeof(SoapMonth);
        internal static Type typeofSoapHexBinary = typeof(SoapHexBinary);
        internal static Type typeofSoapBase64Binary = typeof(SoapBase64Binary);
        internal static Type typeofSoapInteger = typeof(SoapInteger);
        internal static Type typeofSoapPositiveInteger = typeof(SoapPositiveInteger);
        internal static Type typeofSoapNonPositiveInteger = typeof(SoapNonPositiveInteger);
        internal static Type typeofSoapNonNegativeInteger = typeof(SoapNonNegativeInteger);
        internal static Type typeofSoapNegativeInteger = typeof(SoapNegativeInteger);
        internal static Type typeofSoapAnyUri = typeof(SoapAnyUri);
        internal static Type typeofSoapQName = typeof(SoapQName);
        internal static Type typeofSoapNotation = typeof(SoapNotation);
        internal static Type typeofSoapNormalizedString = typeof(SoapNormalizedString);
        internal static Type typeofSoapToken = typeof(SoapToken);
        internal static Type typeofSoapLanguage = typeof(SoapLanguage);
        internal static Type typeofSoapName = typeof(SoapName);
        internal static Type typeofSoapIdrefs = typeof(SoapIdrefs);
        internal static Type typeofSoapEntities = typeof(SoapEntities);
        internal static Type typeofSoapNmtoken = typeof(SoapNmtoken);
        internal static Type typeofSoapNmtokens = typeof(SoapNmtokens);
        internal static Type typeofSoapNcName = typeof(SoapNcName);
        internal static Type typeofSoapId = typeof(SoapId);
        internal static Type typeofSoapIdref = typeof(SoapIdref);
        internal static Type typeofSoapEntity = typeof(SoapEntity);
        internal static Type typeofISoapXsd = typeof(ISoapXsd);

        internal static string FilterBin64(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < value.Length; ++index) {
                if (value[index] != ' ' && value[index] != '\n' && value[index] != '\r')
                    stringBuilder.Append(value[index]);
            }

            return stringBuilder.ToString();
        }

        internal static string LineFeedsBin64(string value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < value.Length; ++index) {
                if (index % 76 == 0)
                    stringBuilder.Append('\n');
                stringBuilder.Append(value[index]);
            }

            return stringBuilder.ToString();
        }

        internal static string Escape(string value)
        {
            switch (value) {
                case "":
                case null:
                    return value;
                default:
                    StringBuilder stringBuilder = new StringBuilder();
                    int startIndex1 = value.IndexOf('&');
                    if (startIndex1 > -1) {
                        stringBuilder.Append(value);
                        stringBuilder.Replace("&", "&#38;", startIndex1, stringBuilder.Length - startIndex1);
                    }

                    int startIndex2 = value.IndexOf('"');
                    if (startIndex2 > -1) {
                        if (stringBuilder.Length == 0)
                            stringBuilder.Append(value);
                        stringBuilder.Replace("\"", "&#34;", startIndex2, stringBuilder.Length - startIndex2);
                    }

                    int startIndex3 = value.IndexOf('\'');
                    if (startIndex3 > -1) {
                        if (stringBuilder.Length == 0)
                            stringBuilder.Append(value);
                        stringBuilder.Replace("'", "&#39;", startIndex3, stringBuilder.Length - startIndex3);
                    }

                    int startIndex4 = value.IndexOf('<');
                    if (startIndex4 > -1) {
                        if (stringBuilder.Length == 0)
                            stringBuilder.Append(value);
                        stringBuilder.Replace("<", "&#60;", startIndex4, stringBuilder.Length - startIndex4);
                    }

                    int startIndex5 = value.IndexOf('>');
                    if (startIndex5 > -1) {
                        if (stringBuilder.Length == 0)
                            stringBuilder.Append(value);
                        stringBuilder.Replace(">", "&#62;", startIndex5, stringBuilder.Length - startIndex5);
                    }

                    int startIndex6 = value.IndexOf(char.MinValue);
                    if (startIndex6 > -1) {
                        if (stringBuilder.Length == 0)
                            stringBuilder.Append(value);
                        stringBuilder.Replace(char.MinValue.ToString(), "&#0;", startIndex6,
                            stringBuilder.Length - startIndex6);
                    }

                    return stringBuilder.Length <= 0 ? value : stringBuilder.ToString();
            }
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapDate : ISoapXsd
    {
        private DateTime _value = DateTime.MinValue.Date;
        private int _sign;

        private static string[] formats = new string[6] {
            "yyyy-MM-dd",
            "'+'yyyy-MM-dd",
            "'-'yyyy-MM-dd",
            "yyyy-MM-ddzzz",
            "'+'yyyy-MM-ddzzz",
            "'-'yyyy-MM-ddzzz"
        };

        public static string XsdType => "date";

        public string GetXsdType() => SoapDate.XsdType;

        public SoapDate() { }

        public SoapDate(DateTime value) => this._value = value;

        public SoapDate(DateTime value, int sign)
        {
            this._value = value;
            this._sign = sign;
        }

        public DateTime Value
        {
            get => this._value;
            set => this._value = value.Date;
        }

        public int Sign
        {
            get => this._sign;
            set => this._sign = value;
        }

        public override string ToString() => this._sign < 0 ?
            this._value.ToString("'-'yyyy-MM-dd", (IFormatProvider)CultureInfo.InvariantCulture) :
            this._value.ToString("yyyy-MM-dd", (IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapDate Parse(string value)
        {
            int sign = 0;
            if (value[0] == '-')
                sign = -1;
            return new SoapDate(
                DateTime.ParseExact(value, SoapDate.formats, (IFormatProvider)CultureInfo.InvariantCulture,
                    DateTimeStyles.None), sign);
        }
    }

    [ComVisible(true)]
    public sealed class SoapDateTime
    {
        private static string[] formats = new string[22] {
            "yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.ffff",
            "yyyy-MM-dd'T'HH:mm:ss.ffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.fff",
            "yyyy-MM-dd'T'HH:mm:ss.fffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.ff",
            "yyyy-MM-dd'T'HH:mm:ss.ffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.f",
            "yyyy-MM-dd'T'HH:mm:ss.fzzz",
            "yyyy-MM-dd'T'HH:mm:ss",
            "yyyy-MM-dd'T'HH:mm:sszzz",
            "yyyy-MM-dd'T'HH:mm:ss.fffff",
            "yyyy-MM-dd'T'HH:mm:ss.fffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.ffffff",
            "yyyy-MM-dd'T'HH:mm:ss.ffffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.fffffff",
            "yyyy-MM-dd'T'HH:mm:ss.ffffffff",
            "yyyy-MM-dd'T'HH:mm:ss.ffffffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.fffffffff",
            "yyyy-MM-dd'T'HH:mm:ss.fffffffffzzz",
            "yyyy-MM-dd'T'HH:mm:ss.ffffffffff",
            "yyyy-MM-dd'T'HH:mm:ss.ffffffffffzzz"
        };

        public static string XsdType => "dateTime";

        public static string ToString(DateTime value) => value.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffzzz",
            (IFormatProvider)CultureInfo.InvariantCulture);

        public static DateTime Parse(string value)
        {
            DateTime dateTime;
            try {
                if (value == null) {
                    dateTime = DateTime.MinValue;
                }
                else {
                    string s = value;
                    if (value.EndsWith("Z", StringComparison.Ordinal))
                        s = value.Substring(0, value.Length - 1) + "-00:00";
                    dateTime = DateTime.ParseExact(s, SoapDateTime.formats, (IFormatProvider)CultureInfo.InvariantCulture,
                        DateTimeStyles.None);
                }
            }
            catch (Exception ex) {
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:dateTime");
            }

            return dateTime;
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapDay : ISoapXsd
    {
        private DateTime _value = DateTime.MinValue;

        private static string[] formats = new string[2] {
            "---dd",
            "---ddzzz"
        };

        public static string XsdType => "gDay";

        public string GetXsdType() => SoapDay.XsdType;

        public SoapDay() { }

        public SoapDay(DateTime value) => this._value = value;

        public DateTime Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => this._value.ToString("---dd", (IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapDay Parse(string value) => new SoapDay(DateTime.ParseExact(value, SoapDay.formats,
            (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [ComVisible(true)]
    public sealed class SoapDuration
    {
        public static string XsdType => "duration";

        private static void CarryOver(int inDays, out int years, out int months, out int days)
        {
            years = inDays / 360;
            int num1 = years * 360;
            months = Math.Max(0, inDays - num1) / 30;
            int num2 = months * 30;
            days = Math.Max(0, inDays - (num1 + num2));
            days = inDays % 30;
        }

        [SecuritySafeCritical]
        public static string ToString(TimeSpan timeSpan)
        {
            StringBuilder stringBuilder = new StringBuilder(10);
            stringBuilder.Length = 0;
            if (TimeSpan.Compare(timeSpan, TimeSpan.Zero) < 1)
                stringBuilder.Append('-');
            int years = 0;
            int months = 0;
            int days = 0;
            SoapDuration.CarryOver(Math.Abs(timeSpan.Days), out years, out months, out days);
            stringBuilder.Append('P');
            stringBuilder.Append(years);
            stringBuilder.Append('Y');
            stringBuilder.Append(months);
            stringBuilder.Append('M');
            stringBuilder.Append(days);
            stringBuilder.Append("DT");
            stringBuilder.Append(Math.Abs(timeSpan.Hours));
            stringBuilder.Append('H');
            stringBuilder.Append(Math.Abs(timeSpan.Minutes));
            stringBuilder.Append('M');
            stringBuilder.Append(Math.Abs(timeSpan.Seconds));
            int l = (int)(Math.Abs(timeSpan.Ticks % 864000000000L) % 10000000L);
            if (l != 0) {
                string str = ParseNumbers.IntToString(l, 10, 7, '0', 0);
                stringBuilder.Append('.');
                stringBuilder.Append(str);
            }

            stringBuilder.Append('S');
            return stringBuilder.ToString();
        }

        public static TimeSpan Parse(string value)
        {
            int num = 1;
            try {
                if (value == null)
                    return TimeSpan.Zero;
                if (value[0] == '-')
                    num = -1;
                char[] charArray = value.ToCharArray();
                int[] numArray = new int[7];
                string s1 = "0";
                string s2 = "0";
                string s3 = "0";
                string s4 = "0";
                string s5 = "0";
                string str1 = "0";
                string str2 = "0";
                bool flag1 = false;
                bool flag2 = false;
                int startIndex = 0;
                for (int index = 0; index < charArray.Length; ++index) {
                    switch (charArray[index]) {
                        case '.':
                            flag2 = true;
                            str1 = new string(charArray, startIndex, index - startIndex);
                            startIndex = index + 1;
                            break;
                        case 'D':
                            s3 = new string(charArray, startIndex, index - startIndex);
                            startIndex = index + 1;
                            break;
                        case 'H':
                            s4 = new string(charArray, startIndex, index - startIndex);
                            startIndex = index + 1;
                            break;
                        case 'M':
                            if (flag1)
                                s5 = new string(charArray, startIndex, index - startIndex);
                            else
                                s2 = new string(charArray, startIndex, index - startIndex);
                            startIndex = index + 1;
                            break;
                        case 'P':
                            startIndex = index + 1;
                            break;
                        case 'S':
                            if (!flag2) {
                                str1 = new string(charArray, startIndex, index - startIndex);
                                break;
                            }

                            str2 = new string(charArray, startIndex, index - startIndex);
                            break;
                        case 'T':
                            flag1 = true;
                            startIndex = index + 1;
                            break;
                        case 'Y':
                            s1 = new string(charArray, startIndex, index - startIndex);
                            startIndex = index + 1;
                            break;
                    }
                }

                return new TimeSpan((long)num *
                                    ((long.Parse(s1, (IFormatProvider)CultureInfo.InvariantCulture) * 360L +
                                      long.Parse(s2, (IFormatProvider)CultureInfo.InvariantCulture) * 30L +
                                      long.Parse(s3, (IFormatProvider)CultureInfo.InvariantCulture)) * 864000000000L +
                                     long.Parse(s4, (IFormatProvider)CultureInfo.InvariantCulture) * 36000000000L +
                                     long.Parse(s5, (IFormatProvider)CultureInfo.InvariantCulture) * 600000000L +
                                     Convert.ToInt64(double.Parse(str1 + "." + str2,
                                         (IFormatProvider)CultureInfo.InvariantCulture) * 10000000.0)));
            }
            catch (Exception ex) {
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:duration");
            }
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapEntities : ISoapXsd
    {
        private string _value;

        public static string XsdType => "ENTITIES";

        public string GetXsdType() => SoapEntities.XsdType;

        public SoapEntities() { }

        public SoapEntities(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapEntities Parse(string value) => new SoapEntities(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapEntity : ISoapXsd
    {
        private string _value;

        public static string XsdType => "ENTITY";

        public string GetXsdType() => SoapEntity.XsdType;

        public SoapEntity() { }

        public SoapEntity(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapEntity Parse(string value) => new SoapEntity(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapHexBinary : ISoapXsd
    {
        private byte[] _value;
        private StringBuilder sb = new StringBuilder(100);

        public static string XsdType => "hexBinary";

        public string GetXsdType() => SoapHexBinary.XsdType;

        public SoapHexBinary() { }

        public SoapHexBinary(byte[] value) => this._value = value;

        public byte[] Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString()
        {
            this.sb.Length = 0;
            for (int index = 0; index < this._value.Length; ++index) {
                string str = this._value[index].ToString("X", (IFormatProvider)CultureInfo.InvariantCulture);
                if (str.Length == 1)
                    this.sb.Append('0');
                this.sb.Append(str);
            }

            return this.sb.ToString();
        }

        public static SoapHexBinary Parse(string value) =>
            new SoapHexBinary(SoapHexBinary.ToByteArray(SoapType.FilterBin64(value)));

        private static byte[] ToByteArray(string value)
        {
            char[] charArray = value.ToCharArray();
            byte[] byteArray = charArray.Length % 2 == 0 ?
                new byte[charArray.Length / 2] :
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:hexBinary");
            for (int index = 0; index < charArray.Length / 2; ++index)
                byteArray[index] = (byte)((uint)SoapHexBinary.ToByte(charArray[index * 2], value) * 16U +
                                          (uint)SoapHexBinary.ToByte(charArray[index * 2 + 1], value));
            return byteArray;
        }

        private static byte ToByte(char c, string value)
        {
            c.ToString();
            try {
                return byte.Parse(c.ToString(), NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture);
            }
            catch (Exception ex) {
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:hexBinary");
            }
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapId : ISoapXsd
    {
        private string _value;

        public static string XsdType => "ID";

        public string GetXsdType() => SoapId.XsdType;

        public SoapId() { }

        public SoapId(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapId Parse(string value) => new SoapId(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapIdref : ISoapXsd
    {
        private string _value;

        public static string XsdType => "IDREF";

        public string GetXsdType() => SoapIdref.XsdType;

        public SoapIdref() { }

        public SoapIdref(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapIdref Parse(string value) => new SoapIdref(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapIdrefs : ISoapXsd
    {
        private string _value;

        public static string XsdType => "IDREFS";

        public string GetXsdType() => SoapIdrefs.XsdType;

        public SoapIdrefs() { }

        public SoapIdrefs(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapIdrefs Parse(string value) => new SoapIdrefs(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapInteger : ISoapXsd
    {
        private Decimal _value;

        public static string XsdType => "integer";

        public string GetXsdType() => SoapInteger.XsdType;

        public SoapInteger() { }

        public SoapInteger(Decimal value) => this._value = Decimal.Truncate(value);

        public Decimal Value
        {
            get => this._value;
            set => this._value = Decimal.Truncate(value);
        }

        public override string ToString() => this._value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapInteger Parse(string value) =>
            new SoapInteger(Decimal.Parse(value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture));
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapLanguage : ISoapXsd
    {
        private string _value;

        public static string XsdType => "language";

        public string GetXsdType() => SoapLanguage.XsdType;

        public SoapLanguage() { }

        public SoapLanguage(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapLanguage Parse(string value) => new SoapLanguage(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapMonth : ISoapXsd
    {
        private DateTime _value = DateTime.MinValue;

        private static string[] formats = new string[2] {
            "--MM--",
            "--MM--zzz"
        };

        public static string XsdType => "gMonth";

        public string GetXsdType() => SoapMonth.XsdType;

        public SoapMonth() { }

        public SoapMonth(DateTime value) => this._value = value;

        public DateTime Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => this._value.ToString("--MM--", (IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapMonth Parse(string value) => new SoapMonth(DateTime.ParseExact(value, SoapMonth.formats,
            (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapMonthDay : ISoapXsd
    {
        private DateTime _value = DateTime.MinValue;

        private static string[] formats = new string[2] {
            "--MM-dd",
            "--MM-ddzzz"
        };

        public static string XsdType => "gMonthDay";

        public string GetXsdType() => SoapMonthDay.XsdType;

        public SoapMonthDay() { }

        public SoapMonthDay(DateTime value) => this._value = value;

        public DateTime Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => this._value.ToString("'--'MM'-'dd", (IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapMonthDay Parse(string value) => new SoapMonthDay(DateTime.ParseExact(value, SoapMonthDay.formats,
            (IFormatProvider)CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapName : ISoapXsd
    {
        private string _value;

        public static string XsdType => "Name";

        public string GetXsdType() => SoapName.XsdType;

        public SoapName() { }

        public SoapName(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapName Parse(string value) => new SoapName(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNcName : ISoapXsd
    {
        private string _value;

        public static string XsdType => "NCName";

        public string GetXsdType() => SoapNcName.XsdType;

        public SoapNcName() { }

        public SoapNcName(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapNcName Parse(string value) => new SoapNcName(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNegativeInteger : ISoapXsd
    {
        private Decimal _value;

        public static string XsdType => "negativeInteger";

        public string GetXsdType() => SoapNegativeInteger.XsdType;

        public SoapNegativeInteger() { }

        public SoapNegativeInteger(Decimal value)
        {
            this._value = Decimal.Truncate(value);
            if (value > -1M)
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:negativeInteger");
        }

        public Decimal Value
        {
            get => this._value;
            set {
                this._value = Decimal.Truncate(value);
                if (this._value > -1M)
                    throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:negativeInteger");
            }
        }

        public override string ToString() => this._value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapNegativeInteger Parse(string value) =>
            new SoapNegativeInteger(Decimal.Parse(value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture));
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNmtoken : ISoapXsd
    {
        private string _value;

        public static string XsdType => "NMTOKEN";

        public string GetXsdType() => SoapNmtoken.XsdType;

        public SoapNmtoken() { }

        public SoapNmtoken(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapNmtoken Parse(string value) => new SoapNmtoken(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNmtokens : ISoapXsd
    {
        private string _value;

        public static string XsdType => "NMTOKENS";

        public string GetXsdType() => SoapNmtokens.XsdType;

        public SoapNmtokens() { }

        public SoapNmtokens(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapNmtokens Parse(string value) => new SoapNmtokens(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNonNegativeInteger : ISoapXsd
    {
        private Decimal _value;

        public static string XsdType => "nonNegativeInteger";

        public string GetXsdType() => SoapNonNegativeInteger.XsdType;

        public SoapNonNegativeInteger() { }

        public SoapNonNegativeInteger(Decimal value)
        {
            this._value = Decimal.Truncate(value);
            if (this._value < 0M)
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:nonNegativeInteger");
        }

        public Decimal Value
        {
            get => this._value;
            set {
                this._value = Decimal.Truncate(value);
                if (this._value < 0M)
                    throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:nonNegativeInteger");
            }
        }

        public override string ToString() => this._value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapNonNegativeInteger Parse(string value) =>
            new SoapNonNegativeInteger(
                Decimal.Parse(value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture));
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNonPositiveInteger : ISoapXsd
    {
        private Decimal _value;

        public static string XsdType => "nonPositiveInteger";

        public string GetXsdType() => SoapNonPositiveInteger.XsdType;

        public SoapNonPositiveInteger() { }

        public SoapNonPositiveInteger(Decimal value)
        {
            this._value = Decimal.Truncate(value);
            if (this._value > 0M)
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:nonPositiveInteger");
        }

        public Decimal Value
        {
            get => this._value;
            set {
                this._value = Decimal.Truncate(value);
                if (this._value > 0M)
                    throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:nonPositiveInteger");
            }
        }

        public override string ToString() => this._value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapNonPositiveInteger Parse(string value) =>
            new SoapNonPositiveInteger(
                Decimal.Parse(value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture));
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNormalizedString : ISoapXsd
    {
        private string _value;

        public static string XsdType => "normalizedString";

        public string GetXsdType() => SoapNormalizedString.XsdType;

        public SoapNormalizedString() { }

        public SoapNormalizedString(string value) => this._value = this.Validate(value);

        public string Value
        {
            get => this._value;
            set => this._value = this.Validate(value);
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapNormalizedString Parse(string value) => new SoapNormalizedString(value);

        private string Validate(string value)
        {
            switch (value) {
                case "":
                case null:
                    return value;
                default:
                    char[] anyOf = new char[3] { '\r', '\n', '\t' };
                    return value.LastIndexOfAny(anyOf) <= -1 ?
                        value :
                        throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:normalizedString");
            }
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapNotation : ISoapXsd
    {
        private string _value;

        public static string XsdType => "NOTATION";

        public string GetXsdType() => SoapNotation.XsdType;

        public SoapNotation() { }

        public SoapNotation(string value) => this._value = value;

        public string Value
        {
            get => this._value;
            set => this._value = value;
        }

        public override string ToString() => this._value;

        public static SoapNotation Parse(string value) => new SoapNotation(value);
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapPositiveInteger : ISoapXsd
    {
        private Decimal _value;

        public static string XsdType => "positiveInteger";

        public string GetXsdType() => SoapPositiveInteger.XsdType;

        public SoapPositiveInteger() { }

        public SoapPositiveInteger(Decimal value)
        {
            this._value = Decimal.Truncate(value);
            if (this._value < 1M)
                throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:positiveInteger");
        }

        public Decimal Value
        {
            get => this._value;
            set {
                this._value = Decimal.Truncate(value);
                if (this._value < 1M)
                    throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:positiveInteger");
            }
        }

        public override string ToString() => this._value.ToString((IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapPositiveInteger Parse(string value) =>
            new SoapPositiveInteger(Decimal.Parse(value, NumberStyles.Integer, (IFormatProvider)CultureInfo.InvariantCulture));
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapQName : ISoapXsd
    {
        private string _name;
        private string _namespace;
        private string _key;

        public static string XsdType => "QName";

        public string GetXsdType() => SoapQName.XsdType;

        public SoapQName() { }

        public SoapQName(string value) => this._name = value;

        public SoapQName(string key, string name)
        {
            this._name = name;
            this._key = key;
        }

        public SoapQName(string key, string name, string namespaceValue)
        {
            this._name = name;
            this._namespace = namespaceValue;
            this._key = key;
        }

        public string Name
        {
            get => this._name;
            set => this._name = value;
        }

        public string Namespace
        {
            get => this._namespace;
            set => this._namespace = value;
        }

        public string Key
        {
            get => this._key;
            set => this._key = value;
        }

        public override string ToString() =>
            this._key == null || this._key.Length == 0 ? this._name : this._key + ":" + this._name;

        public static SoapQName Parse(string value)
        {
            if (value == null)
                return new SoapQName();
            string key = "";
            string name = value;
            int length = value.IndexOf(':');
            if (length > 0) {
                key = value.Substring(0, length);
                name = value.Substring(length + 1);
            }

            return new SoapQName(key, name);
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapTime : ISoapXsd
    {
        private DateTime _value = DateTime.MinValue;

        private static string[] formats = new string[22] {
            "HH:mm:ss.fffffffzzz",
            "HH:mm:ss.ffff",
            "HH:mm:ss.ffffzzz",
            "HH:mm:ss.fff",
            "HH:mm:ss.fffzzz",
            "HH:mm:ss.ff",
            "HH:mm:ss.ffzzz",
            "HH:mm:ss.f",
            "HH:mm:ss.fzzz",
            "HH:mm:ss",
            "HH:mm:sszzz",
            "HH:mm:ss.fffff",
            "HH:mm:ss.fffffzzz",
            "HH:mm:ss.ffffff",
            "HH:mm:ss.ffffffzzz",
            "HH:mm:ss.fffffff",
            "HH:mm:ss.ffffffff",
            "HH:mm:ss.ffffffffzzz",
            "HH:mm:ss.fffffffff",
            "HH:mm:ss.fffffffffzzz",
            "HH:mm:ss.fffffffff",
            "HH:mm:ss.fffffffffzzz"
        };

        public static string XsdType => "time";

        public string GetXsdType() => SoapTime.XsdType;

        public SoapTime() { }

        public SoapTime(DateTime value) => this._value = value;

        public DateTime Value
        {
            get => this._value;
            set => this._value = new DateTime(1, 1, 1, value.Hour, value.Minute, value.Second, value.Millisecond);
        }

        public override string ToString() =>
            this._value.ToString("HH:mm:ss.fffffffzzz", (IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapTime Parse(string value)
        {
            string s = value;
            if (value.EndsWith("Z", StringComparison.Ordinal))
                s = value.Substring(0, value.Length - 1) + "-00:00";
            return new SoapTime(DateTime.ParseExact(s, SoapTime.formats, (IFormatProvider)CultureInfo.InvariantCulture,
                DateTimeStyles.None));
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapToken : ISoapXsd
    {
        private string _value;

        public static string XsdType => "token";

        public string GetXsdType() => SoapToken.XsdType;

        public SoapToken() { }

        public SoapToken(string value) => this._value = this.Validate(value);

        public string Value
        {
            get => this._value;
            set => this._value = this.Validate(value);
        }

        public override string ToString() => SoapType.Escape(this._value);

        public static SoapToken Parse(string value) => new SoapToken(value);

        private string Validate(string value)
        {
            switch (value) {
                case "":
                case null:
                    return value;
                default:
                    char[] anyOf = new char[2] { '\r', '\t' };
                    if (value.LastIndexOfAny(anyOf) > -1)
                        throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:token");
                    if (value.Length > 0 && (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1])))
                        throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:token");
                    return value.IndexOf("  ") <= -1 ?
                        value :
                        throw new Exception("Remoting_SOAPInteropxsdInvalid" + "xsd:token");
            }
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapYear : ISoapXsd
    {
        private DateTime _value = DateTime.MinValue;
        private int _sign;

        private static string[] formats = new string[6] {
            "yyyy",
            "'+'yyyy",
            "'-'yyyy",
            "yyyyzzz",
            "'+'yyyyzzz",
            "'-'yyyyzzz"
        };

        public static string XsdType => "gYear";

        public string GetXsdType() => SoapYear.XsdType;

        public SoapYear() { }

        public SoapYear(DateTime value) => this._value = value;

        public SoapYear(DateTime value, int sign)
        {
            this._value = value;
            this._sign = sign;
        }

        public DateTime Value
        {
            get => this._value;
            set => this._value = value;
        }

        public int Sign
        {
            get => this._sign;
            set => this._sign = value;
        }

        public override string ToString() => this._sign < 0 ?
            this._value.ToString("'-'yyyy", (IFormatProvider)CultureInfo.InvariantCulture) :
            this._value.ToString("yyyy", (IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapYear Parse(string value)
        {
            int sign = 0;
            if (value[0] == '-')
                sign = -1;
            return new SoapYear(
                DateTime.ParseExact(value, SoapYear.formats, (IFormatProvider)CultureInfo.InvariantCulture,
                    DateTimeStyles.None), sign);
        }
    }

    [ComVisible(true)]
    [Serializable]
    public sealed class SoapYearMonth : ISoapXsd
    {
        private DateTime _value = DateTime.MinValue;
        private int _sign;

        private static string[] formats = new string[6] {
            "yyyy-MM",
            "'+'yyyy-MM",
            "'-'yyyy-MM",
            "yyyy-MMzzz",
            "'+'yyyy-MMzzz",
            "'-'yyyy-MMzzz"
        };

        public static string XsdType => "gYearMonth";

        public string GetXsdType() => SoapYearMonth.XsdType;

        public SoapYearMonth() { }

        public SoapYearMonth(DateTime value) => this._value = value;

        public SoapYearMonth(DateTime value, int sign)
        {
            this._value = value;
            this._sign = sign;
        }

        public DateTime Value
        {
            get => this._value;
            set => this._value = value;
        }

        public int Sign
        {
            get => this._sign;
            set => this._sign = value;
        }

        public override string ToString() => this._sign < 0 ?
            this._value.ToString("'-'yyyy-MM", (IFormatProvider)CultureInfo.InvariantCulture) :
            this._value.ToString("yyyy-MM", (IFormatProvider)CultureInfo.InvariantCulture);

        public static SoapYearMonth Parse(string value)
        {
            int sign = 0;
            if (value[0] == '-')
                sign = -1;
            return new SoapYearMonth(
                DateTime.ParseExact(value, SoapYearMonth.formats, (IFormatProvider)CultureInfo.InvariantCulture,
                    DateTimeStyles.None), sign);
        }
    }
}
//        Copyright (c) 2009-2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using DataEngine.CoreServices;

namespace DataEngine.XQuery.Util
{
    public class DayTimeDurationValue: DurationValue, IComparable
    {
        new public const int ProxyValueCode = 12;

        public DayTimeDurationValue(TimeSpan value)
            : base(TimeSpan.Zero, value)
        {
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            DayTimeDurationValue other = obj as DayTimeDurationValue;
            if (other == null)
                throw new ArgumentException("obj");
            return LowPartValue.CompareTo(other.LowPartValue);
        }

        #endregion

        public static DayTimeDurationValue Multiply(DayTimeDurationValue a, double b)
        {
            if (Double.IsNaN(b) || Double.IsNegativeInfinity(b) || Double.IsPositiveInfinity(b))
                throw new XQueryException(Properties.Resources.FOCA0005);
            long timespan = (long)(a.LowPartValue.Ticks * b);
            return new DayTimeDurationValue(new TimeSpan(timespan));
        }

        public static DayTimeDurationValue Divide(DayTimeDurationValue a, double b)
        {
            if (b == 0.0)
                throw new XQueryException(Properties.Resources.FOAR0001);
            if (Double.IsNaN(b))
                throw new XQueryException(Properties.Resources.FOCA0005);
            long timespan = (long)(a.LowPartValue.Ticks / b);
            return new DayTimeDurationValue(new TimeSpan(timespan));
        }

        private static decimal Divide(DayTimeDurationValue a, DayTimeDurationValue b)
        {
            if (b.LowPartValue == TimeSpan.Zero)
                throw new XQueryException(Properties.Resources.FOAR0001);
            return (decimal)a.LowPartValue.Ticks / (decimal)b.LowPartValue.Ticks;
        }

        public static DayTimeDurationValue operator -(DayTimeDurationValue d)
        {
            return new DayTimeDurationValue(-d.LowPartValue);
        }

        new internal class ProxyFactory : ValueProxyFactory
        {
            public override ValueProxy Create(object value)
            {
                return new Proxy((DayTimeDurationValue)value);
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
            }

            public override bool IsNumeric
            {
                get { return false; }
            }

            public override Type GetValueType()
            {
                return typeof(DayTimeDurationValue);
            }

            public override int Compare(ValueProxyFactory other)
            {
                if (other.IsNumeric)
                    return 1;
                return 0;
            }
        }


        new internal class Proxy : ValueProxy
        {
            private DayTimeDurationValue _value;

            public Proxy(DayTimeDurationValue value)
            {
                _value = value;
            }

            public override int GetValueCode()
            {
                return ProxyValueCode;
            }

            public override object Value
            {
                get 
                {
                    return _value;
                }
            }

            protected override bool Eq(ValueProxy val)
            {
                return _value.Equals(val.Value);
            }

            protected override bool Gt(ValueProxy val)
            {
                return ((IComparable)_value).CompareTo(val.Value) > 0;
            }

            protected override bool TryGt(ValueProxy val, out bool res)
            {
                res = false;
                if (val.GetValueCode() != DayTimeDurationValue.ProxyValueCode)
                    return false;
                res = ((IComparable)_value).CompareTo(val.Value) > 0;
                return true;
            }

            protected override ValueProxy Promote(ValueProxy val)
            {
                if (val.IsNumeric())
                    return new ShadowProxy(val);
                if (val.GetValueCode() == DurationValue.ProxyValueCode)
                {
                    DurationValue duration = (DurationValue)val.Value;
                    return new Proxy(new DayTimeDurationValue(duration.LowPartValue));
                }
                throw new InvalidCastException();
            }

            protected override ValueProxy Neg()
            {
                throw new OperatorMismatchException(Funcs.Neg, _value, null);
            }

            protected override ValueProxy Add(ValueProxy value)
            {
                switch (value.GetValueCode())
                {
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(new DayTimeDurationValue(_value.LowPartValue + ((DayTimeDurationValue)value.Value).LowPartValue));
                    case DateTimeValue.ProxyValueCode:
                        return new DateTimeValue.Proxy(DateTimeValue.Add((DateTimeValue)value.Value, _value));
                    case DateValue.ProxyValueCode:
                        return new DateValue.Proxy(DateValue.Add((DateValue)value.Value, _value));
                    case TimeValue.ProxyValueCode:
                        return new TimeValue.Proxy(TimeValue.Add((TimeValue)value.Value, _value));
                    default:
                        throw new OperatorMismatchException(Funcs.Add, _value, value.Value);
                }
            }

            protected override ValueProxy Sub(ValueProxy value)
            {
                switch (value.GetValueCode())
                {
                    case DayTimeDurationValue.ProxyValueCode:
                        return new Proxy(new DayTimeDurationValue(_value.LowPartValue - ((DayTimeDurationValue)value.Value).LowPartValue));
                    default:
                        throw new OperatorMismatchException(Funcs.Sub, _value, value.Value);
                }
            }

            protected override ValueProxy Mul(ValueProxy value)
            {
                if (value.IsNumeric())
                    return new Proxy(DayTimeDurationValue.Multiply(_value, Convert.ToDouble(value)));
                throw new OperatorMismatchException(Funcs.Mul, _value, value.Value);
            }

            protected override ValueProxy Div(ValueProxy value)
            {
                if (value.IsNumeric())
                    return new Proxy(DayTimeDurationValue.Divide(_value, Convert.ToDouble(value)));
                else if (value.GetValueCode() == DayTimeDurationValue.ProxyValueCode)
                    return new DataEngine.CoreServices.Proxy.DecimalProxy(
                        DayTimeDurationValue.Divide(_value, (DayTimeDurationValue)value.Value));
                else
                    throw new OperatorMismatchException(Funcs.Div, _value, value.Value);
            }

            protected override Integer IDiv(ValueProxy value)
            {
                throw new OperatorMismatchException(Funcs.IDiv, _value, value.Value);
            }

            protected override ValueProxy Mod(ValueProxy value)
            {
                throw new OperatorMismatchException(Funcs.Div, _value, value.Value);
            }
        }
    }
}

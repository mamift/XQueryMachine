﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace DataEngine.CoreServices
{
    public class OperatorMismatchException : Exception
    {
        public OperatorMismatchException(object id, object arg1, object arg2)
        {
            ID = id;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public OperatorMismatchException(object id, object arg)
            : this(id, arg, null)
        {
        }

        public object ID { get; private set; }

        public object Arg1 { get; private set; }

        public object Arg2 { get; private set; }
    }

    public abstract class TypeProxy
    {
        public abstract bool Eq(object arg1, object arg2);

        public abstract bool Gt(object arg1, object arg2);

        public abstract object Promote(object arg1);

        public abstract object Neg(object arg1);

        public abstract object Add(object arg1, object arg2);

        public abstract object Sub(object arg1, object arg2);
        
        public abstract object Mul(object arg1, object arg2);

        public abstract object Div(object arg1, object arg2);

        public abstract Integer IDiv(object arg1, object arg2);
        
        public abstract object Mod(object arg1, object arg2);
    }

    public class Int16Proxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) == Convert.ToInt16(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) > Convert.ToInt16(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToInt16(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToInt16(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) + Convert.ToInt16(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) - Convert.ToInt16(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) * Convert.ToInt16(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Convert.ToInt16(arg1) / Convert.ToInt16(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToInt16(arg1) % Convert.ToInt16(arg2);
        }
    }

    public class Int32Proxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) == Convert.ToInt32(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) > Convert.ToInt32(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToInt32(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToInt32(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) + Convert.ToInt32(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) - Convert.ToInt32(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) * Convert.ToInt32(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Convert.ToInt32(arg1) / Convert.ToInt32(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToInt32(arg1) % Convert.ToInt32(arg2);
        }
    }

    public class Int64Proxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) == Convert.ToInt64(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) > Convert.ToInt64(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToInt64(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToInt64(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) + Convert.ToInt64(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) - Convert.ToInt64(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) * Convert.ToInt64(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Convert.ToInt64(arg1) / Convert.ToInt64(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToInt64(arg1) % Convert.ToInt64(arg2);
        }
    }

    public class IntegerProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) == Integer.ToInteger(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) > Integer.ToInteger(arg2);
        }

        public override object Promote(object arg1)
        {
            return Integer.ToInteger(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Integer.ToInteger(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) + Integer.ToInteger(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) - Integer.ToInteger(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) * Integer.ToInteger(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) / Integer.ToInteger(arg2);
        }

        public override object Mod(object arg1, object arg2)
        {
            return Integer.ToInteger(arg1) % Integer.ToInteger(arg2);
        }
    }

    public class SingleProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) == Convert.ToSingle(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) > Convert.ToSingle(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToSingle(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToSingle(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) + Convert.ToSingle(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) - Convert.ToSingle(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) * Convert.ToSingle(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) / Convert.ToSingle(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Math.Truncate(Convert.ToSingle(arg1) / Convert.ToSingle(arg2)));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToSingle(arg1) % Convert.ToSingle(arg2);
        }
    }

    public class DoubleProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) == Convert.ToDouble(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) > Convert.ToDouble(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToDouble(arg1);
        }

        public override object Neg(object arg1)
        {
            return -Convert.ToDouble(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) + Convert.ToDouble(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) - Convert.ToDouble(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) * Convert.ToDouble(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) / Convert.ToDouble(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Convert.ToDecimal(Math.Truncate(Convert.ToDouble(arg1) / Convert.ToDouble(arg2)));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToDouble(arg1) % Convert.ToDouble(arg2);
        }
    }

    public class DecimalProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) == Convert.ToDecimal(arg2);
        }

        public override bool Gt(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) > Convert.ToDecimal(arg2);
        }

        public override object Promote(object arg1)
        {
            return Convert.ToDecimal(arg1);
        }

        public override object Neg(object arg1)
        {
            return 0 - Convert.ToDecimal(arg1);
        }

        public override object Add(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) + Convert.ToDecimal(arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) - Convert.ToDecimal(arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) * Convert.ToDecimal(arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            return (Integer)Math.Truncate(Convert.ToDecimal(arg1) / Convert.ToDecimal(arg2));
        }

        public override object Mod(object arg1, object arg2)
        {
            return Convert.ToDecimal(arg1) % Convert.ToDecimal(arg2);
        }
    }

    public class StringProxy : TypeProxy
    {
        public override bool Eq(object arg1, object arg2)
        {
            return String.CompareOrdinal(arg1.ToString(), arg2.ToString()) == 0;
        }

        public override bool Gt(object arg1, object arg2)
        {
            return String.CompareOrdinal(arg1.ToString(), arg2.ToString()) > 0;
        }

        public override object Promote(object arg1)
        {
            return arg1.ToString();
        }

        public override object Neg(object arg1)
        {
            throw new OperatorMismatchException(Funcs.Neg, arg1, null);
        }

        public override object Add(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Add, arg1, arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Sub, arg1, arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mul, arg1, arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.IDiv, arg1, arg2);
        }

        public override object Mod(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mod, arg1, arg2);
        }
    }

    public class CultureStringProxy : TypeProxy
    {
        private CultureInfo _cultureInfo;

        public CultureStringProxy(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        public override bool Eq(object arg1, object arg2)
        {
            return String.Compare(arg1.ToString(), arg2.ToString(), false, _cultureInfo) == 0;
        }

        public override bool Gt(object arg1, object arg2)
        {
            return String.Compare(arg1.ToString(), arg2.ToString(), false, _cultureInfo) > 0;
        }

        public override object Promote(object arg1)
        {
            return arg1.ToString();
        }

        public override object Neg(object arg1)
        {
            throw new OperatorMismatchException(Funcs.Neg, arg1, null);
        }

        public override object Add(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Add, arg1, arg2);
        }

        public override object Sub(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Sub, arg1, arg2);
        }

        public override object Mul(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mul, arg1, arg2);
        }

        public override object Div(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Div, arg1, arg2);
        }

        public override Integer IDiv(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.IDiv, arg1, arg2);
        }

        public override object Mod(object arg1, object arg2)
        {
            throw new OperatorMismatchException(Funcs.Mod, arg1, arg2);
        }
    }

    public class OperatorManager
    {
        private class Key
        {
            public Type type1;
            public Type type2;

            public Key()
            {
            }

            public Key(Type t1, Type t2)
            {
                type1 = t1;
                type2 = t2;
            }

            public override bool Equals(object obj)
            {
                Key other = obj as Key;
                if (other != null)
                    return type1 == other.type1 &&
                        type2 == other.type2;
                return false;
            }

            public override int GetHashCode()
            {
                return type1.GetHashCode() ^ (type2.GetHashCode() << 16);
            }

#if DEBUG
            public override string ToString()
            {
                return String.Format("Key({0},{1})", type1, type2);
            }
#endif
        }

        private Dictionary<Key, TypeProxy> _op = new Dictionary<Key, TypeProxy>();
        private Key _key = new Key();

        public OperatorManager()
        {
        }

        private TypeProxy Get(object arg1, object arg2)
        {
            _key.type1 = arg1.GetType();
            _key.type2 = arg2.GetType();
            TypeProxy oper;
            if (_op.TryGetValue(_key, out oper))
                return oper;
            return null;
        }

        public void DefineProxy(Type type1, Type type2, TypeProxy proxy)
        {
            _op.Add(new Key(type1, type2), proxy);
        }

        public void DefineProxy(Type type1, Type[] type2, TypeProxy proxy)
        {
            _op[new Key(type1, type1)] = proxy;
            for (int k = 0; k < type2.Length; k++)
            {
                _op[new Key(type1, type2[k])] = proxy;
                _op[new Key(type2[k], type1)] = proxy;
            }
        }

        protected virtual void ThrowUnsupportedException(object id, object arg1, object arg2)
        {
            throw new OperatorMismatchException(id, arg1, arg2);
        }

        public bool Eq(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Eq, arg1, arg2);
            return oper.Eq(arg1, arg2);
        }

        public bool Eq(object arg1, object arg2, out object res)
        {
            res = null;
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return false;
            if (oper.Eq(arg1, arg2))
                res = true;
            return true;
        }

        public bool Gt(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Gt, arg1, arg2);
            return oper.Gt(arg1, arg2);
        }

        public bool Gt(object arg1, object arg2, out object res)
        {
            res = null;
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                return false;
            if (oper.Gt(arg1, arg2))
                res = true;
            return true;
        }

        public object Promote(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                throw new InvalidCastException();
            return oper.Promote(arg1);
        }

        public object Neg(object arg1)
        {
            TypeProxy oper = Get(arg1, arg1);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Neg, arg1, null);
            return oper.Neg(arg1);
        }

        public object Add(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Add, arg1, arg2);
            return oper.Add(arg1, arg2);
        }

        public object Sub(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Sub, arg1, arg2);
            return oper.Sub(arg1, arg2);
        }

        public object Mul(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Mul, arg1, arg2);
            return oper.Mul(arg1, arg2);
        }

        public object Div(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Div, arg1, arg2);
            return oper.Div(arg1, arg2);
        }

        public Integer IDiv(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.IDiv, arg1, arg2);
            return oper.IDiv(arg1, arg2);
        }

        public object Mod(object arg1, object arg2)
        {
            TypeProxy oper = Get(arg1, arg2);
            if (oper == null)
                ThrowUnsupportedException(Funcs.Mod, arg1, arg2);
            return oper.Mod(arg1, arg2);
        }

    }
}

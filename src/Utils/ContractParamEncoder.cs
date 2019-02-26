using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Math;
using ThorClient.Core.Model.Clients.Base;
using ThorClient.Core.Model.Exception;

// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace ThorClient.Utils
{
    public class ContractParamEncoder
    {
        public const int MAX_BIT_LENGTH = 256;
        public const int MAX_BYTE_LENGTH = MAX_BIT_LENGTH / 8;
        public const int DEFAULT_BIT_LENGTH = MAX_BIT_LENGTH >> 1;


        public static bool IsDynamic(int index, object param, string abiType)
        {
            bool isDynamic = false;
            switch (abiType)
            {
                case "string":
                    if (!(param is string)) {
                    throw new InvalidArgumentException("Parameter format is not match! index:" + index + " expect: "
                                                       + abiType + ",but get " + param.GetType().Name);
                }
                    isDynamic = true;
                    break;
                case "bytes":
                    if (!(param is  byte[]))
                    {
                        throw new ArgumentException("Parameter format is not match! index:" + index + " expect: "
                                                           + abiType + ",but get " + param.GetType().Name);
                    }
                    isDynamic = true;
                    break;
                default:
                    if (abiType.Contains("[]"))
                    {
                        if (!param.GetType().IsArray)
                        {
                            throw new ArgumentException("Parameter format is not match! index:" + index + " expect: "
                                                               + abiType + ",but get " + param.GetType().Name);
                        }
                        isDynamic = true;
                    }
                    break;
            }
            return isDynamic;
        }

        public static int GetLength(List<AbiDefinition.NamedType> inputs, params object[] parameters)
        {
            int count = 0;
            var pattern = new Regex("\\w+\\[\\d+\\]*");;
            for (int i = 0; i < parameters.Length; i++) {
                var matches = pattern.Matches(inputs[i].Type);
                if (matches.Count > 0)
                {
                    if (parameters[i].GetType().IsArray) {
                        count += (parameters[i] as Array).Length;
                    } else if (parameters[i] is IList) {
                        count += ((IList) parameters[i]).Count;
                    } else {
                        throw new InvalidArgumentException(
                            inputs[i].Type + " " + parameters[i].GetType().Name);
                    }
                } else {
                    count++;
                }
            }
            return count;
        }

        public static string Encode(string abiType, object param)
        {
            try
            {
                if (abiType.Contains("[]"))
                {
                    var list = new List<object>();
                    string type = abiType.Substring(0, abiType.IndexOf("["));
                    var array = (Array)param;
                    foreach (var t in array)
                    {
                        list.Add(t);
                    }
                    return EncodeDynamicArray(type, list);
                }
                else if (abiType.Contains("["))
                {
                    var list = new List<object>();
                    string type = abiType.Substring(0, abiType.IndexOf("["));
                    if (param is IList) {
                        list.AddRange((List<object>)param);
                    } else
                    {
                        var array = (Array)param;

                        foreach (var t in array)
                        {
                            list.Add(t);
                        }
                    }
                    return EncodeArrayValues(type, list);
                }
                else if (abiType.StartsWith("int"))
                {
                    return EncodeNumeric((BigInteger)param, false);
                }
                else if (abiType.StartsWith("uint"))
                {
                    return EncodeNumeric((BigInteger)param, true);
                }
                else if (abiType.Equals("fixed"))
                {
                    return EncodeNumeric((BigInteger)param, false);
                }
                else if (abiType.Equals("ufixed"))
                {
                    return EncodeNumeric((BigInteger)param, true);
                }
                else if (abiType.Equals("address"))
                {
                    return EncodeAddress((string)param);
                }
                else if (abiType.Equals("bool"))
                {
                    return EncodeBool((bool)param);
                }
                else if (abiType.Equals("bytes"))
                {
                    return EncodeDynamicBytes((byte[])param);
                }
                else if (abiType.StartsWith("bytes"))
                {
                    return EncodeBytes((byte[])param);
                }
                else if (abiType.Equals("string"))
                {
                    return EncodeString((string)param);
                }
                else
                {
                    throw new InvalidOperationException("Type cannot be encoded: " + abiType);
                }
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(
                    "Type cannot be encoded: " + abiType + " paramClass:" + param.GetType().Name, e);
            }
        }

        private static string EncodeString(string value)
        {
            var utfEncoded = Encoding.UTF8.GetBytes(value);
            return EncodeDynamicBytes(utfEncoded);
        }

        private static string EncodeBytes(byte[] value)
        {
            int length = value.Length;
            int mod = length % MAX_BYTE_LENGTH;

            byte[] dest;
            if (mod != 0)
            {
                int padding = MAX_BYTE_LENGTH - mod;
                dest = new byte[length + padding];
                Array.Copy(value, 0, dest, 0, length);
            }
            else
            {
                dest = value;
            }
            return BytesUtils.ToHexString(dest, null);
        }

        private static string EncodeDynamicBytes(byte[] dynamicBytes)
        {
            int size = dynamicBytes.Length;
            string encodedLength = Encode("uint", BigInteger.ValueOf(size));
            string encodedValue = EncodeBytes(dynamicBytes);

            var result = new StringBuilder();
            result.Append(encodedLength);
            result.Append(encodedValue);
            return result.ToString();
        }

        private static string EncodeBool(bool value)
        {
            var rawValue = new byte[MAX_BYTE_LENGTH];
            if (value)
            {
                rawValue[rawValue.Length - 1] = 1;
            }
            return BytesUtils.ToHexString(rawValue, null);
        }

        private static string EncodeAddress(string address)
        {
            if (!StringUtils.IsHex(address))
            {
                throw new InvalidArgumentException("Parameter format is not hex string");
            }
            var paramBytes = BytesUtils.ToByteArray(address);
            if (paramBytes == null || paramBytes.Length > MAX_BYTE_LENGTH)
            {
                throw new InvalidArgumentException("Parameter format is hex string size too large, or null");
            }
            if (paramBytes.Length < MAX_BYTE_LENGTH)
            {
                var fillingZero = new byte[MAX_BYTE_LENGTH];
                Array.Copy(paramBytes, 0, fillingZero, MAX_BYTE_LENGTH - paramBytes.Length, paramBytes.Length);
                return BytesUtils.ToHexString(fillingZero, null);
            }
            else
            {
                return BytesUtils.CleanHexPrefix(address);
            }
        }

        public static string EncodeNumeric(BigInteger numericType, bool isSigned)
        {
            var rawValue = ToByteArray(numericType, isSigned);
            byte paddingValue = GetPaddingValue(numericType);
            var paddedRawValue = new byte[MAX_BYTE_LENGTH];
            if (paddingValue != 0)
            {
                for (int i = 0; i < paddedRawValue.Length; i++)
                {
                    paddedRawValue[i] = paddingValue;
                }
            }

            Array.Copy(rawValue, 0, paddedRawValue, MAX_BYTE_LENGTH - rawValue.Length, rawValue.Length);
            return BytesUtils.ToHexString(paddedRawValue, null);
        }

        private static byte GetPaddingValue(BigInteger numericType)
        {
            if (numericType.SignValue == -1)
            {
                return (byte)0xff;
            }
            else
            {
                return 0;
            }
        }

        private static byte[] ToByteArray(BigInteger value, bool isSigned)
        {
            if (isSigned)
            {
                if (value.BitLength == MAX_BIT_LENGTH)
                {
                    // As BigInteger is signed, if we have a 256 bit value, the resultant byte array
                    // will contain a sign byte in it's MSB, which we should ignore for this
                    // unsigned
                    // integer type.
                    var byteArray = new byte[MAX_BYTE_LENGTH];
                    Array.Copy(value.ToByteArray(), 1, byteArray, 0, MAX_BYTE_LENGTH);
                    return byteArray;
                }
            }
            return value.ToByteArray();
        }

        private static string EncodeArrayValues(string type, List<object> value)
        {
            var result = new StringBuilder();
            foreach (var o in value)
            {
                result.Append(Encode(type, o));
            }
            return result.ToString();
        }

        private static string EncodeDynamicArray(string type, List<object> value)
        {
            int size = value.Count;
            string encodedLength = Encode("uint", BigInteger.ValueOf(size));
            string encodedValues = EncodeArrayValues(type, value);

            var result = new StringBuilder();
            result.Append(encodedLength);
            result.Append(encodedValues);
            return result.ToString();
        }

        public static BigInteger ConvertFixed(BigInteger m, BigInteger n)
        {
            return ConvertFixed(DEFAULT_BIT_LENGTH, DEFAULT_BIT_LENGTH, m, n);
        }

        public static BigInteger ConvertFixed(int mBitSize, int nBitSize, BigInteger m, BigInteger n)
        {
            var mPadded = m.ShiftLeft(nBitSize);
            int nBitLength = n.BitLength;

            // find next multiple of 4
            int shift = (nBitLength + 3) & ~0x03;
            return mPadded.Or(n.ShiftLeft(nBitSize - shift));
        }
    }
}

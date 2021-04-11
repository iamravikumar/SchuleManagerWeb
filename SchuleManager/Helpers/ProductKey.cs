using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;

namespace SchuleManager.Helpers
{
    public class ProductKey
    {
        public DateTime BaseDate;

        public struct SerialInf
        {
            public int CompanyID;
            public int LicensePeriod;
            public ushort OptVal;
        }

        private SerialInf Serial;
        private string SecretKey;
        private string SecretSalt;

        public ProductKey()
        {
            Serial = new SerialInf();
            Serial.LicensePeriod = 0;
            Serial.CompanyID = 0;
            Serial.OptVal = Conversions.ToUShort(0);
            SecretSalt = "";
            SecretKey = GenKey(SecretSalt, 256);
        }

        public int LicensePeriod
        {
            get
            {
                return Serial.LicensePeriod;
            }
            set
            {
                Serial.LicensePeriod = value;
            }
        }

        public int CompanyID
        {
            get
            {
                return Serial.CompanyID;
            }
            set
            {
                Serial.CompanyID = value;
            }
        }

        public ushort OptValue
        {
            get
            {
                return Serial.OptVal;
            }
            set
            {
                Serial.OptVal = value;
            }
        }

        public string KeyCode
        {
            get
            {
                return CryptSerial();
            }
            set
            {
                Serial = DecryptSerial(value);
            }
        }

        public string Salt
        {
            get
            {
                return SecretSalt;
            }
            set
            {
                SecretSalt = value;
                SecretKey = GenKey(SecretSalt, 256);
            }
        }

        public DateTime ExpDate
        {
            get
            {
                //return DateAdd[DateInterval.Month, Serial.LicensePeriod, BaseDate];
                return BaseDate.AddMonths(Serial.LicensePeriod);
            }
            set
            {
                //int Months = DateDiff[DateInterval.Month, BaseDate, value];
                var months = (int) (BaseDate.Subtract(value).Days / (365.25 / 12));

                if (months <= 1)
                    Serial.LicensePeriod = months;
                else
                {
                    var A = new ArgumentException("Date out of range");
                    throw A;
                }
            }
        }

        public bool OptionEnabled(int OptNo)
        {
            if (OptNo > 0 & OptNo <= 16)
            {
                ushort Opt = Conversions.ToUShort(Serial.OptVal >> OptNo - 1 & 1);

                if (Opt > 0)
                    return true;
                else
                    return false;
            }

            return default(bool);
        }

        public void SetOption(int OptNo)
        {
            if (OptNo > 0 & OptNo <= 16)
            {
                ushort OptBit = Conversions.ToUShort(1 << OptNo - 1);
                Serial.OptVal = (ushort) (Serial.OptVal | OptBit);
            }
        }

        public void UnsetOption(int OptNo)
        {
            if (OptNo > 0 & OptNo <= 16)
            {
                ushort OptBit = Conversions.ToUShort(1 << OptNo - 1);
                //OptBit = !OptBit;
                Serial.OptVal = (ushort) (Serial.OptVal & OptBit);
            }
        }

        private string CryptSerial()
        {
            try
            {
                var BASerial = SerialToBA(Serial);

                var Sym = new Encryption.Symmetric(Encryption.Symmetric.Provider.TripleDES);
                var Key = new Encryption.Data(SecretKey);
                Encryption.Data CryptSer = Sym.Encrypt(new Encryption.Data(BASerial), Key);

                //string B32Str = CryptSer.ToBase32;
                string B32Str = CryptSer.Base32;

                return B32Str;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private SerialInf DecryptSerial(string KeyCode)
        {
            var Ret = new SerialInf();
            Ret.OptVal = Conversions.ToUShort(0);
            Ret.CompanyID = 0;
            Ret.LicensePeriod = 0;

            byte[] SettingsBA;

            try
            {
                var sym = new Encryption.Symmetric(Encryption.Symmetric.Provider.TripleDES);
                var CryptDat = new Encryption.Data();
                CryptDat.Base32 = KeyCode;

                Debug.Print(CryptDat.Hex);

                var key = new Encryption.Data(SecretKey);
                Encryption.Data DecryptedData = sym.Decrypt(CryptDat, key);

                // Get the raw decrypted bytes
                SettingsBA = DecryptedData.Bytes;

                return BAToSerial(SettingsBA);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Ret;
            }
        }

        private string GenKey(string InVal, int Bits)
        {
            // Converts any arbritrary string to a 
            // length suitable for use as a crypto key
            string Ret = "";
            int InLen = InVal.Length;
            int ByteCount = Conversions.ToInteger(Bits / (double)8);

            if (InLen >= ByteCount)
                //Ret = Left[InVal, ByteCount];
                Ret = InVal.Substring(0, ByteCount);
            else
            {
                Ret = InVal;
                int Cur = 0;
                int Rand = 64;
                var loopTo = ByteCount - InLen;
                for (Cur = 1; Cur <= loopTo; Cur++)
                {
                    Rand = Rand + 1;
                    if (Rand > Strings.Asc("z"))
                        Rand = 65;
                    var PadChar = Strings.Chr(Rand);
                    //Ret = Ret + PadChar.Trim();
                    Ret = Ret + PadChar;
                }
            }

            return Ret;
        }

        private byte[] SerialToBA(SerialInf SI)
        {
            byte[] BA;
            var Ptr = Marshal.AllocHGlobal(Marshal.SizeOf(SI));
            BA = new byte[Marshal.SizeOf(SI) - 1 + 1];
            Marshal.StructureToPtr(SI, Ptr, false);
            Marshal.Copy(Ptr, BA, 0, Marshal.SizeOf(SI));
            Marshal.FreeHGlobal(Ptr);
            return BA;
        }

        private SerialInf BAToSerial(byte[] BA)
        {
            var SI = default(SerialInf);
            var GC = GCHandle.Alloc(BA, GCHandleType.Pinned);
            var Obj = Marshal.PtrToStructure(GC.AddrOfPinnedObject(), SI.GetType());
            GC.Free();
            return (SerialInf) Obj;
        }
    }

}
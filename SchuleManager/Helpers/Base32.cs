using System;
using System.Collections.Generic;

namespace SchuleManager.Helpers
{
    public static class Base32
    {
        const string cBase32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        const string cBase32Pad = "=";

        public static string ToBase32String(byte[] Data, bool IncludePadding = true)
        {
            string RetVal = "";
            List<long> Segments = new List<long>();
            // Divide the input data into 5 byte segments  
            int Index = 0;

            while (Index < Data.Length)
            {
                long CurrentSegment = 0;
                int SegmentSize = 0;

                while (Index < Data.Length & SegmentSize < 5)
                {
                    CurrentSegment <<= 8;
                    CurrentSegment += Data[Index];
                    Index += 1;
                    SegmentSize += 1;
                }

                // If the size of the last segment was less than 5 bytes, pad with zeros  
                CurrentSegment <<= (8 * (5 - SegmentSize));
                Segments.Add(CurrentSegment);
            }

            // Convert each 5 byte segment into 8 character strings  

            foreach (long CurrentSegment in Segments)
            {
                for (int x = 0; x <= 7; x++)
                    RetVal += cBase32Alphabet[(int) ((CurrentSegment >> (7 - x) * 5) & 0x1F)];
                
            }

            // Correct the end of the string (where the input wasn't a multiple of 5 bytes)  

            var LastSegmentUsefulDataLength = Math.Ceiling((Data.Length % 5) * 8 / (double)5);
            RetVal = RetVal.Substring(0, (int) (RetVal.Length - (8 - LastSegmentUsefulDataLength)));

            // Add the padding characters   
            if (IncludePadding)
                RetVal += new string(Convert.ToChar(cBase32Pad), (int) (8 - LastSegmentUsefulDataLength));

            return RetVal;
        }


        public static byte[] FromBase32String(string Data)
        {
            List<byte> RetVal = new List<byte>();
            List<long> Segments = new List<long>();
            int x;

            // Remove any trailing padding  
            Data = Data.TrimEnd(new char[] { Convert.ToChar(cBase32Pad) });

            // Process the string 8 characters at a time  
            int Index = 0;

            while (Index < Data.Length)
            {
                long CurrentSegment = 0;
                int SegmentSize = 0;

                while (Index < Data.Length & SegmentSize < 8)
                {
                    CurrentSegment <<= 5;
                    CurrentSegment = CurrentSegment | cBase32Alphabet.IndexOf(Data[Index]);
                    Index += 1;
                    SegmentSize += 1;
                }

                // If the size of the last segment was less than 40 bits, pad it  
                CurrentSegment <<= (5 * (8 - SegmentSize));
                Segments.Add(CurrentSegment);
            }

            // Break the 5 byte segments back down into individual bytes  

            foreach (long CurrentSegment in Segments)
            {
                for (x = 0; x <= 4; x++)
                    RetVal.Add((byte) ((CurrentSegment >> (4 - x) * 8) & 0xFF));
            }

            // Remove any bytes of padding from the output  

            int BytesToRemove = 4;
            // Dim BytesToRemove As Integer = 5 - (Math.Ceiling(Math.Ceiling(3 * 8 / 5) / 2))
            RetVal.RemoveRange(RetVal.Count - BytesToRemove, BytesToRemove);
            return RetVal.ToArray();
        }
    }
}
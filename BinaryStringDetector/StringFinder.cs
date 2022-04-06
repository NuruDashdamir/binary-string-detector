using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryStringDetector
{
    class StringFinder
    {
        public static List<String> getValidStringsFromFile(String fileLocation, int minimalStrLen = 7, bool isUnicodeOnlyNeeded = false, bool isNullTerminationNeeded = false)
        {
            byte[] fileContent;
            List<String> validStringList = new List<String>();

            // handle file
            try { fileContent = System.IO.File.ReadAllBytes(fileLocation); }
            catch { return validStringList; }

            String validString = "";
            int validStrLenCounter = 0;
            long startingAdress = 0;

            // we subtract 1 because reading 2 characters in unicode mode will cause indexOutOfBounds
            for (int i = 0; i < fileContent.Length - System.Convert.ToInt32(isUnicodeOnlyNeeded); i++)
            {
                byte firstByte = fileContent[i];

                // this logic works because of by unicode I mean ascii unicode: 0x0000-0x00FF
                // ascii or unicode logic in next 2 lines
                byte secondByte = 0;
                if (isUnicodeOnlyNeeded) secondByte = fileContent[++i];

                if (secondByte == 0
                     && ((firstByte < 127 && firstByte > 31)
                     || (firstByte == 9 || firstByte == 10 || firstByte == 13))
                    )
                {
                    validString += System.Convert.ToChar(firstByte);
                    validStrLenCounter++;
                }
                else
                {
                    if (validStrLenCounter > minimalStrLen && (!isNullTerminationNeeded || firstByte == 0))
                    {
                        // print with hexadecimal location of found string
                        validStringList.Add(startingAdress.ToString("X2").PadLeft(8, '0') + " - " + validString);
                    }
                    validStrLenCounter = 0;
                    validString = "";
                    startingAdress = i;
                }
            }

            // in case EOF reached and non-null terminated strings required (otherwise if last character is printable it wont appear)
            if (!isNullTerminationNeeded && validString.Length != 0) validStringList.Add(startingAdress.ToString("X2").PadLeft(8, '0') + " - " + validString);

            return validStringList;
        }
    }
}

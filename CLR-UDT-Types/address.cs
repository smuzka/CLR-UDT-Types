using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 1000)]
public struct Address: INullable, IBinarySerialize
{
    private bool _null { get; set; }
    public string City { get; set; }
    public string StreetName { get; set; }
    public int HouseNumber { get; set; }
    public string ZipCode { get; set; }
    public Address(string city, string streetname, int housenumber, string zipcode) : this()
    {
        this.City = city;
        this.StreetName = streetname;
        this.HouseNumber = housenumber;
        this.ZipCode = zipcode;
        this._null = false;
    }
    public override string ToString()
    {
        return "City: " + City +
                ", street name: " + StreetName +
                ", HouseNumber: " + HouseNumber +
                ", ZipCode: " + ZipCode;
    }
    
    public bool IsNull
    {
        get
        {
            return _null;
        }
    }
    
    public static Address Null
    {
        get
        {
            Address h = new Address();
            h._null = true;
            return h;
        }
    }
    
    public static Address Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        string[] data = s.ToString().Split(',');

        string city = data[0];
        string streetname = data[1];
        int housenumber = Int32.Parse(data[2]);
        string zipcode = data[3];

        return new Address(city, streetname, housenumber, zipcode);
    }

    // method to deserialize the object
    public void Read(BinaryReader r)
    {
        int maxStringSize = 120;
        char[] chars;
        char[] firstChars = new char[30];
        char[] secondChars = new char[30];
        char[] thirdChars = new char[30];
        char[] fourthChars = new char[30];
        int firstStringEnd;
        int secondStringEnd;
        int thirdStringEnd;
        int fourthStringEnd;
        string firstStringValue;
        string secondStringValue;
        string thirdStringValue;
        string fourthStringValue;

        // Read the characters from the binary stream.
        chars = r.ReadChars(maxStringSize);

        // Find the start of the null character padding.
        Array.Copy(chars, 0, firstChars, 0, 30);
        firstStringEnd = Array.IndexOf(firstChars, '\0');

        if (firstStringEnd == 0)
        {
            firstStringValue = null;
            return;
        }

        // Build the string from the array of characters.
        firstStringValue = new String(firstChars, 0, firstStringEnd);
        // Set the object's properties equal to the values.
        this.City = firstStringValue;

        // Read the characters from the binary stream.
        Array.Copy(chars, 30, secondChars, 0, 30);
        secondStringEnd = Array.IndexOf(secondChars, '\0');
        // Build the string from the array of characters.
        secondStringValue = new String(secondChars, 0, secondStringEnd);
        // Set the object's properties equal to the values.
        this.StreetName = secondStringValue;

        // Read the characters from the binary stream.
        Array.Copy(chars, 60, thirdChars, 0, 30);
        thirdStringEnd = Array.IndexOf(thirdChars, '\0');
        // Build the string from the array of characters.
        thirdStringValue = new String(thirdChars, 0, thirdStringEnd);
        // Set the object's properties equal to the values.
        this.HouseNumber = Int32.Parse(thirdStringValue);

        // Read the characters from the binary stream.
        Array.Copy(chars, 90, fourthChars, 0, 30);
        fourthStringEnd = Array.IndexOf(fourthChars, '\0');
        // Build the string from the array of characters.
        fourthStringValue = new String(fourthChars, 0, fourthStringEnd);
        // Set the object's properties equal to the values.
        this.ZipCode = fourthStringValue;
    }

    public Address ReadFromCharsArray(char[] stringToRead)
    {
        char[] firstChars = new char[30];
        char[] secondChars = new char[30];
        char[] thirdChars = new char[30];
        char[] fourthChars = new char[30];
        int firstStringEnd;
        int secondStringEnd;
        int thirdStringEnd;
        int fourthStringEnd;
        string firstStringValue;
        string secondStringValue;
        string thirdStringValue;
        string fourthStringValue;

        Array.Copy(stringToRead, 0, firstChars, 0, 30);
        firstStringEnd = Array.IndexOf(firstChars, '\0');

        Address newAddress = new Address();

        // Build the string from the array of characters.
        firstStringValue = new String(firstChars, 0, firstStringEnd);
        // Set the object's properties equal to the values.
        newAddress.City = firstStringValue;

        // Read the characters from the binary stream.
        Array.Copy(stringToRead, 30, secondChars, 0, 30);
        secondStringEnd = Array.IndexOf(secondChars, '\0');
        // Build the string from the array of characters.
        secondStringValue = new String(secondChars, 0, secondStringEnd);
        // Set the object's properties equal to the values.
        newAddress.StreetName = secondStringValue;

        // Read the characters from the binary stream.
        Array.Copy(stringToRead, 60, thirdChars, 0, 30);
        thirdStringEnd = Array.IndexOf(thirdChars, '\0');
        // Build the string from the array of characters.
        thirdStringValue = new String(thirdChars, 0, thirdStringEnd);
        // Set the object's properties equal to the values.
        newAddress.HouseNumber = Int32.Parse(thirdStringValue);

        // Read the characters from the binary stream.
        Array.Copy(stringToRead, 90, fourthChars, 0, 30);
        fourthStringEnd = Array.IndexOf(fourthChars, '\0');
        // Build the string from the array of characters.
        fourthStringValue = new String(fourthChars, 0, fourthStringEnd);
        // Set the object's properties equal to the values.
        newAddress.ZipCode = fourthStringValue;

        return newAddress;
    }

    // method serializing the object
    public void Write(BinaryWriter w)
    {
        int maxStringSize = 30;
        // Pad the string from the right with null characters.
        string cityString = City.PadRight(maxStringSize, '\0');
        string streetString = StreetName.PadRight(maxStringSize, '\0');
        string numberString = HouseNumber.ToString().PadRight(maxStringSize, '\0');
        string codeString = ZipCode.PadRight(maxStringSize, '\0');

        // Write the string value one byte at a time.
        for (int i = 0; i < cityString.Length; i++)
        {
            w.Write(cityString[i]);
        }
        for (int i = 0; i < streetString.Length; i++)
        {
            w.Write(streetString[i]);
        }
        for (int i = 0; i < numberString.Length; i++)
        {
            w.Write(numberString[i]);
        }
        for (int i = 0; i < codeString.Length; i++)
        {
            w.Write(codeString[i]);
        }
    }

    public static void WriteFromObject(BinaryWriter w, Address address)
    {
        int maxStringSize = 30;
        // Pad the string from the right with null characters.
        string cityString = address.City.PadRight(maxStringSize, '\0');
        string streetString = address.StreetName.PadRight(maxStringSize, '\0');
        string numberString = address.HouseNumber.ToString().PadRight(maxStringSize, '\0');
        string codeString = address.ZipCode.PadRight(maxStringSize, '\0');

        // Write the string value one byte at a time.
        for (int i = 0; i < cityString.Length; i++)
        {
            w.Write(cityString[i]);
        }
        for (int i = 0; i < streetString.Length; i++)
        {
            w.Write(streetString[i]);
        }
        for (int i = 0; i < numberString.Length; i++)
        {
            w.Write(numberString[i]);
        }
        for (int i = 0; i < codeString.Length; i++)
        {
            w.Write(codeString[i]);
        }
    }
}
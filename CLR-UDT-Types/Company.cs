using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 1000)]
public struct Company: INullable, IBinarySerialize
{
    private bool _null { get; set; }
    public string Name { get; set; }
    public string NIP { get; set; }
    public string REGON { get; set; }
    public int AddressId { get; set; }
    public Company(string name, string nip, string regon, int addressId) : this()
    {
        this.Name = name;
        this.NIP = nip;
        this.REGON = regon;
        this.AddressId = addressId;
        this._null = false;
    }
    public override string ToString()
    {
        return "Name: " + Name +
        ", NIP: " + NIP +
        ", REGON: " + REGON +
        ", AddressId: " + AddressId;
    }
    
    public bool IsNull
    {
        get
        {
            return _null;
        }
    }
    
    public static Company Null
    {
        get
        {
            Company h = new Company();
            h._null = true;
            return h;
        }
    }

    // method to parse the string from the SQL command to create a new object
    public static Company Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        string[] data = s.ToString().Split(',');

        string name = data[0];
        string nip = data[1];
        string regon = data[2];
        int addressId = Int32.Parse(data[3]);
        
        return new Company(name, nip, regon, addressId);
    }

    // method to deserialize the object
    public void Read(BinaryReader r)
    {
        int maxStringSize = 90;
        char[] chars;
        char[] firstChars = new char[30];
        char[] secondChars = new char[30];
        char[] thirdChars = new char[30];
        int firstStringEnd;
        int secondStringEnd;
        int thirdStringEnd;
        string firstStringValue;
        string secondStringValue;
        string thirdStringValue;

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
        this.Name = firstStringValue;

        // Read the characters from the binary stream.
        Array.Copy(chars, 30, secondChars, 0, 30);
        secondStringEnd = Array.IndexOf(secondChars, '\0');
        // Build the string from the array of characters.
        secondStringValue = new String(secondChars, 0, secondStringEnd);
        // Set the object's properties equal to the values.
        this.NIP = secondStringValue;

        // Read the characters from the binary stream.
        Array.Copy(chars, 60, thirdChars, 0, 30);
        thirdStringEnd = Array.IndexOf(thirdChars, '\0');
        // Build the string from the array of characters.
        thirdStringValue = new String(thirdChars, 0, thirdStringEnd);
        // Set the object's properties equal to the values.
        this.REGON = thirdStringValue;

        this.AddressId = r.ReadInt32();
    }

    // method serializing the object
    public void Write(BinaryWriter w)
    {
        int maxStringSize = 30;
        // Pad the string from the right with null characters.
        string nameString = Name.PadRight(maxStringSize, '\0');
        string nipString = NIP.PadRight(maxStringSize, '\0');
        string regonString = REGON.PadRight(maxStringSize, '\0');


        // Write the string value one byte at a time.
        for (int i = 0; i < nameString.Length; i++)
        {
            w.Write(nameString[i]);
        }
        for (int i = 0; i < nipString.Length; i++)
        {
            w.Write(nipString[i]);
        }
        for (int i = 0; i < regonString.Length; i++)
        {
            w.Write(regonString[i]);
        }
        w.Write(AddressId);

        //Address.WriteFromObject(w, Address);
    }
}
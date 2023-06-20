using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;
using System.IO;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 1000)]
public struct Product: INullable, IBinarySerialize
{
    private bool _null { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string Description{ get; set; }
    public Product(string name, double price, string description)
    {
        this.Name = name;
        this.Price = price;
        this.Description = description;
        this._null = false;
    }
    public override string ToString()
    {
        return "Name: " + Name +
        ", Price: " + Price +
        ", Description: " + Description;
    }
    
    public bool IsNull
    {
        get
        {
            return _null;
        }
    }
    
    public static Product Null
    {
        get
        {
            Product h = new Product();
            h._null = true;
            return h;
        }
    }

    // method to parse the string from the SQL command to create a new object
    public static Product Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        string[] data = s.ToString().Split(',');

        string name = data[0];
        double price = Convert.ToDouble(data[1]);
        string description = data[2];

        return new Product(name, price, description);
    }

    // method to deserialize the object
    public void Read(BinaryReader r)
    {
        int maxStringSize = 60;
        char[] chars;
        char[] firstChars = new char[30];
        char[] secondChars = new char[30];
        int firstStringEnd;
        int secondStringEnd;
        string firstStringValue;
        string secondStringValue;

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
        this.Description = secondStringValue;

        this.Price = r.ReadDouble();
    }

    // method serializing the object
    public void Write(BinaryWriter w)
    {
        int maxStringSize = 30;
        string first = Name;
        string second = Description;
        // Pad the string from the right with null characters.
        string firstString = first.PadRight(maxStringSize, '\0');
        string secondString = second.PadRight(maxStringSize, '\0');

        // Write the string value one byte at a time.
        for (int i = 0; i < firstString.Length; i++)
        {
            w.Write(firstString[i]);
        }
        for (int i = 0; i < secondString.Length; i++)
        {
            w.Write(secondString[i]);
        }
        w.Write(Price);
    }
}
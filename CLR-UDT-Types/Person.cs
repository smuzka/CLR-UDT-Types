using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 1000)]
public struct Person : INullable, IBinarySerialize
{
    private bool _null { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }

    public Person(string firstName, string lastName, DateTime birthDate) : this()
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.BirthDate = birthDate;
        this._null = false;
    }

    public override string ToString()
    {
        return "First name: " + FirstName +
        ", last name: " + LastName +
        ", birth date: " + BirthDate;
    }

    public bool IsNull
    {
        get
        {
            return _null;
        }
    }

    public static Person Null
    {
        get
        {
            Person h = new Person();
            h._null = true;
            return h;
        }
    }

    // method to parse the string from the SQL command to create a new object
    public static Person Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        string[] data = s.ToString().Split(',');

        string firstname = data[0];
        string lastname = data[1];
        DateTime birthdate = DateTime.Parse(data[2]);
        return new Person(firstname, lastname, birthdate);
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
        this.FirstName = firstStringValue;

        // Read the characters from the binary stream.
        Array.Copy(chars, 30, secondChars, 0, 30);
        secondStringEnd = Array.IndexOf(secondChars, '\0');
        // Build the string from the array of characters.
        secondStringValue = new String(secondChars, 0, secondStringEnd);
        // Set the object's properties equal to the values.
        this.LastName = secondStringValue;

        // Read the characters from the binary stream.
        Array.Copy(chars, 60, thirdChars, 0, 30);
        thirdStringEnd = Array.IndexOf(thirdChars, '\0');
        // Build the string from the array of characters.
        thirdStringValue = new String(thirdChars, 0, thirdStringEnd);
        // Set the object's properties equal to the values.
        this.BirthDate = DateTime.Parse(thirdStringValue);
    }

    // method serializing the object
    public void Write(BinaryWriter w)
    {
        int maxStringSize = 30;
        string first = FirstName;
        string last = LastName;
        string date = BirthDate.ToString();
        // Pad the string from the right with null characters.
        string firstString = first.PadRight(maxStringSize, '\0');
        string lastString = last.PadRight(maxStringSize, '\0');
        string dateString = date.PadRight(maxStringSize, '\0');


        // Write the string value one byte at a time.
        for (int i = 0; i < firstString.Length; i++)
        {
            w.Write(firstString[i]);
        }
        for (int i = 0; i < lastString.Length; i++)
        {
            w.Write(lastString[i]);
        }
        for (int i = 0; i < dateString.Length; i++)
        {
            w.Write(dateString[i]);
        }
    }
}
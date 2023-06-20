using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;


[Serializable]
[Microsoft.SqlServer.Server.SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 1000)]
public struct BankAccount: INullable, IBinarySerialize
{
    private bool _null { get; set; }
    public string AccountNumber { get; set; }
    public double Saldo { get; set; }
    public int PersonId { get; set; }
    public BankAccount (string accountNumber, double saldo, int personId)
    {
        this.AccountNumber = accountNumber;
        this.Saldo = saldo;
        this.PersonId = personId;
        this._null = false;
    }
    public override string ToString()
    {
        return "Account Number: " + AccountNumber +
        ", Saldo: " + Saldo +
        ", Person ID: " + PersonId;
    }
    
    public bool IsNull
    {
        get
        {
            return _null;
        }
    }
    
    public static BankAccount Null
    {
        get
        {
            BankAccount h = new BankAccount();
            h._null = true;
            return h;
        }
    }

    // method to parse the string from the SQL command to create a new object
    public static BankAccount Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        string[] data = s.ToString().Split(',');

        string accountNumber = data[0];
        double saldo = Convert.ToDouble(data[1]);
        int personid = Int32.Parse(data[2]);

        return new BankAccount(accountNumber, saldo, personid);
    }

    // method to deserialize the object
    public void Read(BinaryReader r)
    {
        int maxStringSize = 30;
        char[] chars;
        char[] firstChars = new char[30];
        int firstStringEnd;
        string firstStringValue;

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
        this.AccountNumber = firstStringValue;

        this.Saldo = r.ReadDouble();
        this.PersonId = r.ReadInt32();
    }

    // method serializing the object
    public void Write(BinaryWriter w)
    {
        int maxStringSize = 30;
        string first = AccountNumber;
        // Pad the string from the right with null characters.
        string firstString = first.PadRight(maxStringSize, '\0');

        // Write the string value one byte at a time.
        for (int i = 0; i < firstString.Length; i++)
        {
            w.Write(firstString[i]);
        }
        w.Write(Saldo);
        w.Write(PersonId);
    }
}
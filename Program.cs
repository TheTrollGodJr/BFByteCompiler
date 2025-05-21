using System;
using System.IO;

static class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0) return;
        bool status;

        switch (args[0])
        {
            case "-c":
            case "--compile":
                Console.WriteLine("Compiling");
                status = Compile(args[1], args[2]); // Parameters: bf path, output path
                break;
            case "-e":
            case "--execute":
                status = ExecuteBytecode(args[1]);
                break;
        }
    }

    static bool ExecuteBytecode(string inFile)
    {
        byte[] data = File.ReadAllBytes(inFile);

        foreach (byte b in data)
        {
            int high = (b & 0xF0) >> 4;
            int low = b & 0x0F;

            Console.Write($"{FromHex[high]}{FromHex[low]}");
        }
        return true;
    }

    static string? GetData(string inFile)
    {
        try
        {
            StreamReader sr = new StreamReader(inFile);
            string? line = sr.ReadLine();
            string data = "";
            while (line != null)
            {
                data += line;
                line = sr.ReadLine();
            }
            Console.WriteLine("Got data");
            return data;
        }
        catch
        {
            Console.WriteLine("Data is Null");
            return null;
        }
    }

    static bool Compile(string inFile, string outFile)
    {
        string? data = GetData(inFile);
        if (data == null) return false;

        List<byte> output = new List<byte>();

        for (int i = 0; i < data.Length; i += 2)
        {
            // (ToHex.ContainsKey(data[i]))
            //{
            int high = ToHex[data[i]];
            int low = i + 1 < data.Length ? ToHex[data[i + 1]] : 0x0;
            output.Add((byte)((high << 4) | low));
            //}
            Console.WriteLine("Got byte");
        }

        File.WriteAllBytes(outFile, output.ToArray());
        Console.WriteLine("Finished");
        return true;
    }

    public static Dictionary<char, int> ToHex = new Dictionary<char, int>
    {
        ['N'] = 0x0, // null value
        ['.'] = 0x1,
        [','] = 0x2,
        ['['] = 0x3,
        [']'] = 0x4,
        ['+'] = 0x5,
        ['-'] = 0x6,
        ['<'] = 0x7,
        ['>'] = 0x8
    };

    public static Dictionary<int, char> FromHex = new Dictionary<int, char>
    {
        [0x0] = 'N',
        [0x1] = '.',
        [0x2] = ',',
        [0x3] = '[',
        [0x4] = ']',
        [0x5] = '+',
        [0x6] = '-',
        [0x7] = '<',
        [0x8] = '>'
    };
}

//. , [ ] + - < > s

/// 0000    null/stop
/// 0001    .
/// 0010    ,
/// 0011    [
/// 0100    ]
/// 0101    +
/// 0110    -
/// 0111    < 
/// 1000    >
/// 1001    
/// 1010
/// 1011
/// 1100
/// 1101
/// 1110
/// 1111
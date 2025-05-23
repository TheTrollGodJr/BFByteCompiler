using System;
using System.IO;

static class Program
{
    static int[] arr = {};
    static int ptr = 0;
    static Stack<int?> st = new Stack<int?>();

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
        string code = "";

        foreach (byte b in data)
        {
            int high = (b & 0xF0) >> 4;
            int low = b & 0x0F;

            //Console.Write($"{FromHex[high]}{FromHex[low]}");
            code += $"{FromHex[high]}{FromHex[low]}";
        }

        RunCode(code);

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
            //Console.WriteLine("Got data");
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
            //Console.WriteLine("Got byte");
        }

        File.WriteAllBytes(outFile, output.ToArray());
        //Console.WriteLine("Finished");
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


    static void RunCode(string code)
    {
        Console.Write("\n\n");
        for (int i = 0; i < code.Length; i++)
        {
            switch (code[i])
            {
                case '>':
                    ptr++;
                    while (ptr > arr.Length) arr.Append(0);
                    break;
                case '<':
                    if (ptr != 0) ptr--;
                    break;
                case '[':
                    if (code[ptr] > 0) st.Push(ptr);
                    break;
                case ']':
                    int? loopFront = st.Peek();
                    if (loopFront == null) continue;
                    ptr = (int)(loopFront - 1);
                    i = (int)(loopFront - 1);
                    break;
                case '+':
                    if (arr[i] >= 255) arr[i] = 0;
                    else arr[i]++;
                    break;
                case '-':
                    if (arr[i] <= 0) arr[i] = 255;
                    else arr[i]--;
                    break;
                case ',':
                    char key = Console.ReadKey(true).KeyChar;
                    arr[i] = key;
                    break;
                case '.':
                    Console.Write((char)arr[i]);
                    break;
            }
        }
    }

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
/// 



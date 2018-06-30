using System;
using System.IO;

namespace AsdaImageToTex
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("result");
            string[] extensions = { "png", "jfif", "bmp", "dds", "tga" };

            for (int j = 0; j < extensions.Length; j++)
            {
                string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*." + extensions[j]);

                for (int i = 0; i < files.Length; i++)
                {
                    byte[] imageFile = File.ReadAllBytes(files[i]);
                    getImageSize(imageFile, out ushort width, out ushort height, extensions[j]);
                    byte[] texFile = new byte[36 + imageFile.Length];
                    Array.Copy(BitConverter.GetBytes(537134595), 0, texFile, 0, 4);
                    Array.Copy(BitConverter.GetBytes(imageFile.Length + 28), 0, texFile, 4, 4);
                    Array.Copy(BitConverter.GetBytes(width), 0, texFile, 8, 2);
                    Array.Copy(BitConverter.GetBytes(height), 0, texFile, 12, 2);
                    texFile[16] = 1;
                    texFile[28] = 3;
                    texFile[20] = 1;
                    texFile[24] = 21;

                    switch (extensions[j])
                    {
                        case "bmp":
                            texFile[32] = 0x00;
                            break;
                        case "jfif":
                            texFile[32] = 0x01;
                            break;
                        case "tga":
                            texFile[32] = 0x02;
                            break;
                        case "png":
                            texFile[32] = 0x03;
                            break;
                        case "dds":
                            Array.Copy(imageFile, 84, texFile, 24, 4);
                            texFile[20] = 10;
                            texFile[27] = (byte)(texFile[27] == 35 ? 32 : texFile[27]);
                            texFile[32] = 0x04;
                            break;
                    }

                    Array.Copy(imageFile, 0, texFile, 0x24, imageFile.Length);
                    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "result\\" + Path.GetFileName(files[i]).Substring(0, Path.GetFileName(files[i]).Length - extensions[j].Length) + "tex", texFile);
                }
            }
        }

        static void getImageSize(byte[] data, out ushort width, out ushort height, string extension)
        {
            switch (extension)
            {
                case "bmp":
                    width = (ushort)(data[19] * 256 + data[18]);
                    height = (ushort)(data[23] * 256 + data[22]);
                    break;
                case "jfif":
                    int index = ByteSearch(data, new byte[] { 0xFF, 0xC0 });
                    width = (ushort)(data[index + 7] * 256 + data[index + 8]);
                    height = (ushort)(data[index + 5] * 256 + data[index + 6]);
                    break;
                case "tga":
                    width = (ushort)(data[13] * 256 + data[12]);
                    height = (ushort)(data[15] * 256 + data[14]);
                    break;
                case "png":
                    width = (ushort)(data[18] * 256 + data[19]);
                    height = (ushort)(data[22] * 256 + data[23]);
                    break;
                case "dds":
                    width = (ushort)(data[17] * 256 + data[16]);
                    height = (ushort)(data[13] * 256 + data[12]);
                    break;
                default:
                    width = 0;
                    height = 0;
                    break;
            }
        }

        private static int ByteSearch(byte[] searchIn, byte[] searchBytes, int start = 0)
        {
            int found = -1;
            bool matched = false;

            //only look at this if we have a populated search array and search bytes with a sensible start
            if (searchIn.Length > 0 && searchBytes.Length > 0 && start <= (searchIn.Length - searchBytes.Length) && searchIn.Length >= searchBytes.Length)
            {
                //iterate through the array to be searched
                for (int i = start; i <= searchIn.Length - searchBytes.Length; i++)
                {
                    //multiple bytes to be searched we have to compare byte by byte
                    matched = true;

                    for (int y = 0; y <= searchBytes.Length - 1; y++)
                    {
                        if (searchIn[i + y] != searchBytes[y])
                        {
                            matched = false;
                            break;
                        }
                    }

                    //everything matched up
                    if (matched)
                    {
                        found = i;
                        break;
                    }
                }
            }

            return found;
        }
    }
}
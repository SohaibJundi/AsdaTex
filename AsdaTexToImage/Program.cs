using System;
using System.IO;

namespace AsdaTexToImage
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.tex");
            Directory.CreateDirectory("result");

            for (int i = 0; i < files.Length; i++)
            {
                byte[] texFile = File.ReadAllBytes(files[i]);
                string str4 = files[i].Substring(0, files[i].Length - 3);

                switch (texFile[0x24])
                {
                    case 0x89:
                        str4 += "png";
                        break;

                    case 0xff:
                        str4 += "jfif";
                        break;

                    case 0x42:
                        str4 += "bmp";
                        break;

                    case 0x44:
                        str4 += "dds";
                        break;

                    case 0:
                        str4 += "tga";
                        break;
                }

                byte[] imageFile = new byte[texFile.Length - 36];
                Array.Copy(texFile, 36, imageFile, 0, texFile.Length - 36);
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "result\\" + Path.GetFileName(str4), imageFile);
            }
        }
    }
}


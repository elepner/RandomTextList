using System;
using System.Linq;
using System.Text;
using RandomTextList.Models;

namespace RandomTextList.Code
{
    public class RandomRecordsGenerator : IDatagenerator<Record>
    {
        private static readonly Random Random = new Random();
        private static readonly int avgWordLength = 4, maxWordLength = 20;
        private static uint _mZ = (uint)Random.Next(), _mW = (uint)Random.Next();

        private static uint GetUint()
        {
            _mZ = 36969 * (_mZ & 65535) + (_mZ >> 16);
            _mW = 18000 * (_mW & 65535) + (_mW >> 16);
            return (_mZ << 16) + _mW;
        }
        //Magic Unifor distribution generator from http://www.codeproject.com/Articles/25172/Simple-Random-Number-Generation
        public static double GetUniform()
        {
            // 0 <= u < 2^32
            uint u = GetUint();
            // The magic number below is 1/(2^32 + 2).
            // The result is strictly between 0 and 1.
            return (u + 1.0) * 2.328306435454494e-10;
        }

        public static string CreateWord()
        {
            
            double rnd = GetUniform();
            int wordLength;
            if (rnd < 0.5)
            {
                wordLength = (int) Math.Ceiling(rnd*2*avgWordLength);
            }
            else
            {
                wordLength = avgWordLength + (int) Math.Ceiling(2*(rnd - 0.5)*(maxWordLength - avgWordLength));
            }

            return CreateWord(wordLength);
        }


        public static string CreateWord(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        public static string CreateString(int desiredLength)
        {
            StringBuilder sb = new StringBuilder(desiredLength);
            
            while (sb.Length < desiredLength)
            {
                sb.Append(CreateWord());
                sb.Append(" ");
            }
            return sb.ToString();
        }

        public Record[] GetRecords(int count)
        {

            return Enumerable.Range(0, count)
                .Select(idx => new Record
                {
                    Header = CreateWord(Random.Next(4, 8)),
                    Text = CreateString(Random.Next(100,3000))
                }).ToArray();

        }
    }
}
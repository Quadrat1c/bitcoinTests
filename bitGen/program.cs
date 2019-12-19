using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinGeneration
{
    class Program
    {
        private static Random random = new Random();
        static void Main(string[] args)
        {
            /*
            for (int i = 0; i < 999999999; i++)
            {
                string HexHash = RandomHex(130);
                Console.WriteLine("HexHash: " + HexHash);
                string Address = TestAddress(HexHash);
                Console.WriteLine(Address);
                if (Address == "1rundZJCMJhUiWQNFS5uT3BvisBuLxkAp")
                {
                    Console.WriteLine("FOUND PRIVATE KEY");
                    Console.ReadLine();
                }
            }*/
            
            int AddressGenCount = 0;

            for (int i = 0; i < 9999; i++)
            {
                AddressGenCount++;
                string HexHash = RandomHex(130);
                //Console.WriteLine(HexHash.Length);
                Console.WriteLine("HexHash: " + HexHash);
                string Address = TestAddress(HexHash);
                Console.WriteLine(Address);
                WebCrawler.CrawlSite("https://blockchain.info/q/getreceivedbyaddress/" + Address);
                //WebCrawler.CrawlSite("https://api.blockcypher.com/v1/btc/main/addrs/" + TestAddress(HexHash.ToString()));
                //WebCrawler.CrawlSite("https://api.blockcypher.com/v1/btc/main/addrs/" + "1Q9mKX164PMLeY7QdGA78oMF4kivQGmmmt");
                System.Threading.Thread.Sleep(2000);
            }

            Console.WriteLine("Address Count: " + AddressGenCount);
            Console.ReadLine();
            
        }

        public static string RandomHex(int length)
        {
            const string chars = "ABCDEF0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string TestAddress(string HexHash)
        {
            //string HexHash = "0450863AD64A87AE8A2FE83C1AF1A8403CB53F73E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6";
            byte[] PubKey = HexToByte(HexHash);
            //Console.WriteLine("Public Key:" + BitConverter.ToString(PubKey)); //Step 1
            byte[] PubKeySha = Sha256(PubKey);
            //Console.WriteLine("Sha Public Key:" + BitConverter.ToString(PubKeySha)); //Step 2
            byte[] PubKeyShaRIPE = RipeMD160(PubKeySha);
            //Console.WriteLine("Ripe Sha Public Key:" + BitConverter.ToString(PubKeyShaRIPE)); // Step 3
            byte[] PreHashWNetwork = AppendBitcoinNetwork(PubKeyShaRIPE, 0); // Step4
            byte[] PublicHash = Sha256(PreHashWNetwork);
            //Console.WriteLine("Public Hash:" + BitConverter.ToString(PublicHash)); // Step5
            byte[] PublicHashHash = Sha256(PublicHash);
            //Console.WriteLine("Public HashHash:" + BitConverter.ToString(PublicHashHash));  //Step 6
            //Console.WriteLine("Checksum:" + BitConverter.ToString(PublicHashHash).Substring(0, 4));  //Step 7
            byte[] Address = ConcatAddress(PreHashWNetwork, PublicHashHash);
            //Console.WriteLine("Address:" + BitConverter.ToString(Address)); //Step 8
            //Console.WriteLine("BTC Address:" + Base58Encode(Address));    //Step 9
            //System.Windows.Forms.Clipboard.SetText(Base58Encode(Address));
            return Base58Encode(Address);
        }

        public static string CreateAddress(string PublicKey)
        {
            byte[] PreHashQ = AppendBitcoinNetwork(RipeMD160(Sha256(HexToByte(PublicKey))), 0);
            return Base58Encode(ConcatAddress(PreHashQ, Sha256(Sha256(PreHashQ))));
        }

        public static byte[] HexToByte(string HexString)
        {
            if (HexString.Length % 2 != 0)
                throw new Exception("Invalid HEX");
            byte[] retArray = new byte[HexString.Length / 2];
            for (int i = 0; i < retArray.Length; ++i)
            {
                retArray[i] = byte.Parse(HexString.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return retArray;
        }

        public static byte[] Sha256(byte[] array)
        {
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(array);
        }

        public static byte[] RipeMD160(byte[] array)
        {
            RIPEMD160Managed hashstring = new RIPEMD160Managed();
            return hashstring.ComputeHash(array);
        }

        public static byte[] AppendBitcoinNetwork(byte[] RipeHash, byte Network)
        {
            byte[] extended = new byte[RipeHash.Length + 1];
            extended[0] = (byte)Network;
            Array.Copy(RipeHash, 0, extended, 1, RipeHash.Length);
            return extended;
        }

        public static byte[] ConcatAddress(byte[] RipeHash, byte[] Checksum)
        {
            byte[] ret = new byte[RipeHash.Length + 4];
            Array.Copy(RipeHash, ret, RipeHash.Length);
            Array.Copy(Checksum, 0, ret, RipeHash.Length, 4);
            return ret;
        }

        public static string Base58Encode(byte[] array)
        {
            const string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            string retString = string.Empty;
            BigInteger encodeSize = ALPHABET.Length;
            BigInteger arrayToInt = 0;
            for (int i = 0; i < array.Length; ++i)
            {
                arrayToInt = arrayToInt * 256 + array[i];
            }
            while (arrayToInt > 0)
            {
                int rem = (int)(arrayToInt % encodeSize);
                arrayToInt /= encodeSize;
                retString = ALPHABET[rem] + retString;
            }
            for (int i = 0; i < array.Length && array[i] == 0; ++i)
                retString = ALPHABET[0] + retString;

            return retString;
        }
    }
}

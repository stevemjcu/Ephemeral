using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Ephemeral.App.Client
{
	public static class Crypto
	{
		public static byte[] GenerateKey(int size = 256)
		{
			var generator = new CipherKeyGenerator();
			generator.Init(new KeyGenerationParameters(new(), size));
			return generator.GenerateKeyParameter().GetKey();
		}

		public static byte[] Encrypt(byte[] input, byte[] key)
		{
			var cipher = new PaddedBufferedBlockCipher(
				new EcbBlockCipher(new AesEngine()),
				new Pkcs7Padding());

			cipher.Init(true, new KeyParameter(key));

			var output1 = new byte[cipher.GetOutputSize(input.Length)];
			int length1 = cipher.ProcessBytes(input, 0, input.Length, output1, 0);
			int length2 = cipher.DoFinal(output1, length1);
			var output2 = new byte[output1.Length - (cipher.GetBlockSize() - length2)];

			Array.Copy(output1, 0, output2, 0, output2.Length);
			return output2;
		}

		public static byte[] Decrypt(byte[] output, byte[] parameters)
		{
			return default;
		}
	}
}

using System;
using System.IO;
using System.Security.Cryptography;
public class CustomDecryptor
{
    private const int BLOCK_SIZE = 256;
    private readonly uint[] _keyState = new uint[2];

    public CustomDecryptor(uint keyPart1, uint keyPart2)
    {
        _keyState[0] = keyPart1;
        _keyState[1] = keyPart2;
    }
    public byte[] Decrypt(byte[] encryptedData)
    {
        // 验证数据4字节对齐
        if (encryptedData.Length % 4 != 0)
            throw new ArgumentException("Data length must be multiple of 4");
        //using var ms = new MemoryStream();
        var ms = new MemoryStream();
        uint state = GenerateInitialVector();
        byte[] workBuffer = new byte[BLOCK_SIZE];
        int offset = 0;
        while (offset < encryptedData.Length)
        {
            int blockSize = Math.Min(BLOCK_SIZE, encryptedData.Length - offset);

            // 生成动态S盒
            GenerateSBoxes(out byte[] sBox, out byte[] inverseSBox);

            // 流层解密
            for (int i = 0; i < blockSize; i += 4)
            {
                uint cipherWord = BitConverter.ToUInt32(encryptedData, offset + i);
                uint plainWord = state ^ cipherWord;
                state = cipherWord; // 状态更新为当前密文

                byte[] decrypted = BitConverter.GetBytes(plainWord);
                Array.Copy(decrypted, 0, workBuffer, i, 4);
            }
            // 非线性层逆变换
            for (int i = 0; i < blockSize; i++)
            {
                workBuffer[i] = inverseSBox[workBuffer[i]];
                workBuffer[i] = (byte)((workBuffer[i] + i) & 0xFF); // 逆位置偏移
            }
            ms.Write(workBuffer, 0, blockSize);
            offset += blockSize;
        }

        return ms.ToArray();
    }
    private uint GenerateInitialVector()
    {
        float part1 = GenerateRandom(65536.0f);
        float part2 = GenerateRandom(65536.0f);
        return ((uint)part1 << 16) | ((uint)part2 & 0xFFFF);
    }
    private float GenerateRandom(float max)
    {
        // PRNG复制原始算法行为
        uint u1 = _keyState[0];
        uint u2 = u1 * 0x1000001;
        uint u3 = (u2 < u1 ? 1u : 0u) + _keyState[1] +
                  ((u1 >> 8) | (_keyState[1] << 24));
        uint u4 = u1 * 0x100 + ((u3 << 16) | (u2 >> 16));

        // 更新密钥状态（维持算法连续性）
        _keyState[0] = u4;
        _keyState[1] = (u4 < u1 * 0x100 ? 1u : 0u) +
                       ((u2 >> 24) | (u3 << 8)) +
                       (u3 >> 16);
        // IEEE 754标准化处理
        uint fraction = u4 & 0x7FFFFF;
        uint floatVal = fraction | 0x3F800000;
        float result = BitConverter.ToSingle(BitConverter.GetBytes(floatVal), 0) - 1.0f;
        return result * max;
    }
    private void GenerateSBoxes(out byte[] sBox, out byte[] inverseSBox)
    {
        sBox = new byte[256];
        inverseSBox = new byte[256];
        byte[] tmp = new byte[256];

        // 初始化顺序数组
        for (int i = 0; i < 256; i++)
            tmp[i] = (byte)i;
        // Fisher-Yates洗牌
        uint count = 256;
        for (int i = 0; i < 256; i++)
        {
            float rand = GenerateRandom(count);
            int index = (int)rand;
            byte value = tmp[index];

            // 交换位置
            tmp[index] = tmp[count - 1];
            tmp[count - 1] = value;

            // 构建正逆S盒
            sBox[i] = value;
            inverseSBox[value] = (byte)i;
            count--;
        }
    }
}
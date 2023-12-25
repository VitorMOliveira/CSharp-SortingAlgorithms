using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace SortingAlgorithms.old
{
    internal class TestingSortSound
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;
        private const short NUM_CHANNELS = 1;

        private static void PlaySound(float _frequency)
        {
            short[] wave = new short[SAMPLE_RATE];
            byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)]; // * 2

            float frequency = _frequency;
            short amplitude = short.MaxValue;

            //ChooseWave(wave, amplitude, frequency);

            int waveDuration = wave.Length / 1;

            Buffer.BlockCopy(wave, 0, binaryWave, 0, waveDuration * sizeof(short)); // split short into 2 bytes

            // create wav file
            using (MemoryStream memoryStream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                short blockAlign = BITS_PER_SAMPLE / 8;
                int subChunk2Size = SAMPLE_RATE * NUM_CHANNELS * blockAlign;
                binaryWriter.Write(new[] { 'R', 'I', 'F', 'F' });
                binaryWriter.Write(36 + subChunk2Size);
                binaryWriter.Write(new[] { 'W', 'A', 'V', 'E' });
                binaryWriter.Write(new[] { 'f', 'm', 't', ' ' });
                binaryWriter.Write(16); // PCM
                binaryWriter.Write((short)1); // no compression
                binaryWriter.Write(NUM_CHANNELS);
                binaryWriter.Write(SAMPLE_RATE);
                binaryWriter.Write(SAMPLE_RATE * blockAlign);
                binaryWriter.Write(blockAlign);
                binaryWriter.Write(BITS_PER_SAMPLE);
                binaryWriter.Write(new[] { 'd', 'a', 't', 'a' });
                binaryWriter.Write(subChunk2Size);
                binaryWriter.Write(binaryWave);

                memoryStream.Position = 0;

                SoundPlayer soundPlayer = new SoundPlayer(memoryStream);

                soundPlayer.Play();
            }
        }




    }
}

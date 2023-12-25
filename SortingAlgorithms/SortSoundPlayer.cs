using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SortingAlgorithms
{
    internal class SortSoundPlayer
    {
        private const int SAMPLE_RATE = 44100;
        private const short BITS_PER_SAMPLE = 16;
        private const short NUM_CHANNELS = 1;

        private static float FREQUENCY = 0f;

        private static bool keepPlaying = false;

        private static short[] wave = new short[SAMPLE_RATE];
        private static byte[] binaryWave = new byte[SAMPLE_RATE * sizeof(short)]; // * 2
        private static short amplitude = short.MaxValue;

        private static int waveDuration = wave.Length / 4;

        private static int _maxRectVal;

        private const float freqMultiplier = 1.0f;
        private const float _maxFreq = 1212f * freqMultiplier;
        private const float _minFreq = 120f * freqMultiplier;

        private const int _minRectValue = 0;

        private static SoundPlayer? player;

        private static byte[]? sound;

        // ADSR parameters
        static readonly double attackTime = 0.1; // in seconds
        static readonly double decayTime = 0.2;  // in seconds
        static readonly double sustainLevel = 0.5;
        static readonly double releaseTime = 0.3; // in seconds

        // Number of samples for each phase
        static readonly int attackSamples = (int)(attackTime * SAMPLE_RATE);
        static readonly int decaySamples = (int)(decayTime * SAMPLE_RATE);
        static readonly int releaseSamples = (int)(releaseTime * SAMPLE_RATE);
        static readonly int totalSamples = attackSamples + decaySamples + releaseSamples;

        public static byte[] GetWave(float _frequency)
        {
            ChooseWave(wave, amplitude, _frequency);

            double[] adsrEnvelope = GenerateAdsrEnvelope(attackSamples, decaySamples, sustainLevel, releaseSamples);

            // Multiply the wave by the envelope
            for (int i = 0; i < totalSamples; i++)
            {
                wave[i] = (short) ( wave[i] * adsrEnvelope[i]);
            }
            
            Buffer.BlockCopy(wave, 0, binaryWave, 0, waveDuration * sizeof(short)); // split short into 2 bytes

            // Append data to wav file
            MemoryStream memoryStream;
            using (memoryStream = new MemoryStream())
            using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
            {
                binaryWriter.Write(sound!);
                binaryWriter.Write(binaryWave);
            }

            return memoryStream.ToArray();
        }

        private static double[] GenerateAdsrEnvelope(int attackSamples, int decaySamples, double sustainLevel, int releaseSamples)
        {
            double[] envelope = new double[attackSamples + decaySamples + releaseSamples];
            int totalSamples = envelope.Length;

            // Attack phase
            for (int i = 0; i < attackSamples; i++)
            {
                envelope[i] = i / (double)attackSamples;
            }

            // Decay phase
            for (int i = attackSamples; i < attackSamples + decaySamples; i++)
            {
                envelope[i] = 1.0 - (1.0 - sustainLevel) * (i - attackSamples) / decaySamples;
            }

            // Sustain phase
            for (int i = attackSamples + decaySamples; i < totalSamples - releaseSamples; i++)
            {
                envelope[i] = sustainLevel;
            }

            // Release phase
            for (int i = totalSamples - releaseSamples; i < totalSamples; i++)
            {
                envelope[i] = sustainLevel - sustainLevel * (i - (totalSamples - releaseSamples)) / releaseSamples;
            }

            return envelope;
        }

        private static void ConstructWav()
        {
            // create wav file
            MemoryStream memoryStream;
            using (memoryStream = new MemoryStream())
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
            }

            sound = memoryStream.ToArray();
        }
        
        public static void PlaySoundLoopTask(int maxRectVal, CancellationToken _token)
        {
            ConstructWav();
            _maxRectVal = maxRectVal;

            keepPlaying = true;
            player = new SoundPlayer();

            _ = Task.Run(() =>
            {
                
                while (keepPlaying)
                {
                    
                    //Trace.WriteLine($"{FREQUENCY}");
                    byte[] sound = SortSoundPlayer.GetWave(FREQUENCY);
                    player.Stream = new MemoryStream(sound);
                    player.Play();

                    if (_token.IsCancellationRequested)
                    {
                        player.Stop();
                        _token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(40);
                }
            });
        }

        public static void StopSoundLoopTask()
        {
            player?.Stop();
            keepPlaying = false;
        }

        public static void AdjustFrequency(int value)
        {
            FREQUENCY = MapValueToFrequency(value);
        }

        private static float MapValueToFrequency(int value)
        {
            float output = (value - _minRectValue) * (_maxFreq - _minFreq) / (_maxRectVal - _minRectValue) + _minFreq;
            return output;
        }

        static float Lerp(float firstFloat, float secondFloat, float by)
        {
            return firstFloat * (1 - by) + secondFloat * by;
        }

        private static void ChooseWave(short[] wave, short amplitude, float frequency)
        {
            //ConvertToSineWave(wave, amplitude, frequency);
            //ConvertToTriangleWave(wave, frequency);
            ConvertToSquareWave(wave, amplitude, frequency);
            //ConvertToSawWave(wave, frequency);
            //ConvertToNoiseWave(wave, new Random());

        }

        private static void ConvertToSineWave(short[] wave, short amplitude, float frequency, int volumeAdjust = 150)
        {
            short amplitudeAdjusted = (short)(amplitude / volumeAdjust);
            for (int i = 0; i < SAMPLE_RATE; i++)
            {
                wave[i] = Convert.ToInt16(amplitudeAdjusted * Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i));
            }
        }

        private static void ConvertToSquareWave(short[] wave, short amplitude, float frequency, int volumeAdjust = 400)
        {
            short amplitudeAdjusted = (short)(amplitude / volumeAdjust);
            for (int i = 0; i < SAMPLE_RATE; i++)
            {
                wave[i] = Convert.ToInt16(amplitudeAdjusted * Math.Sign(Math.Sin(((Math.PI * 2 * frequency) / SAMPLE_RATE) * i)));
            }
        }

        private static void ConvertToSawWave(short[] wave, float frequency, int volumeAdjust = 250)
        {
            short tempSample;
            int samplesPerWaveLength = (int)(SAMPLE_RATE / frequency);
            short ampStep = (short)(((short.MaxValue * 2) / samplesPerWaveLength) / volumeAdjust);

            for (int i = 0; i < SAMPLE_RATE; i++)
            {
                tempSample = -short.MaxValue;

                for (int j = 0; i < SAMPLE_RATE && j < samplesPerWaveLength; j++)
                {
                    tempSample += ampStep;
                    wave[i++] = Convert.ToInt16(tempSample);
                }
                i--;
            }
        }

        private static void ConvertToTriangleWave(short[] wave, float frequency, int volumeAdjust = 140)
        {
            short tempSample = -short.MaxValue;
            int samplesPerWaveLength = (int)(SAMPLE_RATE / frequency);
            short ampStep = (short)((short.MaxValue * 2) / samplesPerWaveLength);

            for (int i = 0; i < SAMPLE_RATE; i++)
            {
                if (Math.Abs(tempSample + ampStep) > short.MaxValue)
                {
                    ampStep = (short)-ampStep;
                }
                tempSample += ampStep;
                wave[i] = Convert.ToInt16(tempSample / volumeAdjust);
            }
        }

        private static void ConvertToNoiseWave(short[] wave, Random random, int volumeAdjust = 80)
        {
            for (int i = 0; i < SAMPLE_RATE; i++)
            {
                wave[i] = (short)(random.Next(-short.MaxValue, short.MaxValue) / volumeAdjust);
            }
        }


    }
}

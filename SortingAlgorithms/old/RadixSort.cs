using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Diagnostics;
using System.Reflection;

namespace SortingAlgorithms.old
{
    public class RadixSort
    {
        private double[] _sortingArray;
        private readonly Canvas _canvas;
        private readonly int _maxVal;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        public RadixSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray,
                                    CancellationToken token)
        {
            _sortingArray = sortingArray;
            _canvas = sortingCanvas;
            _maxVal = (int)sortingCanvas.ActualHeight + SortParameters.RECT_DRAW_OFFSET_LAST;
            _token = token;
            _mainWindow = mainWindow;
        }

        public async Task DoSorting()
        {
            SortSoundPlayer.PlaySoundLoopTask(_maxVal, _token);

            // Find the maximum number to know number of digits
            double m = GetMax(_sortingArray, _sortingArray.Length);

            // Do counting sort for every digit. Note that
            // instead of passing digit number, exp is passed.
            // exp is 10^i where i is current digit number
            for (int exp = 1; m / exp > 0; exp *= 10)
            {
                await CountSort(_sortingArray, _sortingArray.Length, exp);
            }
        }

        // A function to do counting sort of arr[] according to
        // the digit represented by exp.
        private async Task CountSort(double[] arr, int n, int exp)
        {
            double[] output = new double[n]; // output array
            int i;
            int[] count = new int[10];

            // initializing all elements of count to 0
            for (i = 0; i < 10; i++)
                count[i] = 0;

            // Store count of occurrences in count[]
            for (i = 0; i < n; i++)
                count[(int)(arr[i] / exp % 10)]++;

            // Change count[i] so that count[i] now contains
            // actual
            //  position of this digit in output[]
            for (i = 1; i < 10; i++)
                count[i] += count[i - 1];

            // Build the output array
            for (i = n - 1; i >= 0; i--)
            {
                output[count[(int)(arr[i] / exp % 10)] - 1] = arr[i];
                count[(int)(arr[i] / exp % 10)]--;
            }

            // Copy the output array to arr[], so that arr[] now
            // contains sorted numbers according to current digit
            for (i = 0; i < n; i++)
            {
                arr[i] = output[i];

                await _mainWindow.Dispatcher.InvokeAsync(() =>
                                SortEngine.UpdateOneElementInCanvas(_canvas, arr, i));

                SortSoundPlayer.AdjustFrequency(i);

                if (_token.IsCancellationRequested)
                {
                    _token.ThrowIfCancellationRequested();
                }

                Thread.Sleep(1);
            }
        }

        private static double GetMax(double[] arr, int n)
        {
            double mx = arr[0];
            for (int i = 1; i < n; i++)
                if (arr[i] > mx)
                    mx = arr[i];
            return mx;
        }

    }
}

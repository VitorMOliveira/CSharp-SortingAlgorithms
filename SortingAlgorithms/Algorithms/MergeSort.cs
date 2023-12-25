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
using System.Runtime.CompilerServices;
using System.Windows.Media.Media3D;

namespace SortingAlgorithms.Algorithms
{
    public class MergeSort : ISortAlgorithms
    {
        private double[] _sortingArray;
        private readonly int _maxVal;
        private readonly Canvas _canvas;
        private readonly CancellationToken _token;
        private readonly MainWindow _mainWindow;

        private readonly int _leftIndex, _rightIndex;

        public MergeSort(MainWindow mainWindow, Canvas sortingCanvas, double[] sortingArray, int leftIndex, int rightIndex,
                                    CancellationToken token)
        {
            _sortingArray = sortingArray;
            _maxVal = (int)sortingCanvas.ActualHeight + SortParameters.RECT_DRAW_OFFSET_LAST;
            _canvas = sortingCanvas;
            _token = token;
            _mainWindow = mainWindow;
            _leftIndex = leftIndex; // low
            _rightIndex = rightIndex; // high
        }

        public async Task DoSorting()
        {
            SortSoundPlayer.PlaySoundLoopTask(_maxVal, _token);
            await SortArray(_sortingArray, _leftIndex, _rightIndex);
        }

        private async Task SortArray(double[] array, int leftIndex, int rightIndex)
        {
            if (leftIndex < rightIndex)
            {
                int middle = leftIndex + (rightIndex - leftIndex) / 2;
                await SortArray(array, leftIndex, middle);
                await SortArray(array, middle + 1, rightIndex);
                await MergeArray(array, leftIndex, middle, rightIndex);
            }
        }


        private async Task MergeArray(double[] arr, int start, int mid,
                      int end)
        {
            int start2 = mid + 1;

            // If the direct merge is already sorted
            if (arr[mid] <= arr[start2])
            {
                return;
            }

            // Two pointers to maintain start of both arrays to merge
            while (start <= mid && start2 <= end)
            {

                // If element 1 is in right place
                if (arr[start] <= arr[start2])
                {
                    start++;
                }
                else
                {
                    double value = arr[start2];
                    int index = start2;

                    // Shift all the elements between element 1 element 2, right by 1.
                    while (index != start)
                    {
                        arr[index] = arr[index - 1];

                        await _mainWindow.Dispatcher.InvokeAsync(() =>
                                SortEngine.UpdateOneElementInCanvas(_canvas, _sortingArray, index));

                        SortSoundPlayer.AdjustFrequency(index);

                        if (_token.IsCancellationRequested)
                        {
                            _token.ThrowIfCancellationRequested();
                        }

                        Thread.Sleep(1);

                        index--;
                    }

                    arr[start] = value;

                    await _mainWindow.Dispatcher.InvokeAsync(() =>
                            SortEngine.UpdateOneElementInCanvas(_canvas, _sortingArray, start));

                    SortSoundPlayer.AdjustFrequency(start);

                    if (_token.IsCancellationRequested)
                    {
                        _token.ThrowIfCancellationRequested();
                    }

                    Thread.Sleep(1);

                    // Update all the pointers
                    start++;
                    mid++;
                    start2++;
                }
            }
        }

    }
}

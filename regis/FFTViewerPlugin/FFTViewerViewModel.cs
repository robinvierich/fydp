using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Regis.Base.ViewModels;
using System.ComponentModel;
using Regis.Plugins.Models;
using System.ComponentModel.Composition;
using Regis.Plugins.Interfaces;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace FFTViewerPlugin
{
    [Export]
    public class FFTViewerViewModel : BaseViewModel
    {
        [Import]
        private IFFTSource _fftSource = null;

        [Import]
        private INoteDetectionSource _noteSource = null;

        private DispatcherTimer _fftUpdateTimer = new DispatcherTimer();

        private double _maxPower = 0;
        private Queue<double> _averageMaxPower = new Queue<double>();

        public void StartReadingFFT() {
            if (_fftUpdateTimer.IsEnabled)
                return;

            _fftUpdateTimer.Interval = TimeSpan.FromMilliseconds(1);
            _fftUpdateTimer.Tick += new EventHandler(timer_Tick);
            _fftUpdateTimer.Start();
        }

        public void StopReadingFFT() {
            if (!_fftUpdateTimer.IsEnabled)
                return;

            _fftUpdateTimer.Stop();
        }

        void timer_Tick(object sender, EventArgs e) {
            FFTPower fftCalc;
            if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                return;

            // copy array (otherwise we get threading errors)
            double[] powerBins = new double[fftCalc.PowerBins.Length];
            Array.Copy(fftCalc.PowerBins, powerBins, fftCalc.PowerBins.Length);

            ObservableCollection<FFTBinViewModel> newBins = new ObservableCollection<FFTBinViewModel>();

            if (FFTBins == null)
                FFTBins = new ObservableCollection<FFTBinViewModel>();

            for (int i = 0; i < (fftCalc.PowerBins.Length / 2); i++) {
                if (FFTBins.ElementAtOrDefault(i) == null)
                    FFTBins.Insert(i, new FFTBinViewModel());

                FFTBins[i].BinNumber = i;

                FFTBins[i].Power = powerBins[i];
            }

            _averageMaxPower.Enqueue(FFTBins.Max(bin => bin.Power));
            if (_averageMaxPower.Count() > 5)
                _averageMaxPower.Dequeue();
            _maxPower = _averageMaxPower.Sum() / 5;

            if (BarViewModels == null) {
                BarViewModels = new ObservableCollection<BarViewModel>();
            }

            int j = 0;
            foreach (FFTBinViewModel bin in FFTBins) {
                if (BarViewModels.ElementAtOrDefault(j) == null) {
                    BarViewModels.Insert(j, new BarViewModel());
                }

                double height = ((bin.Power / _maxPower) * ControlActualHeight);
       
                BarViewModels[j].Height = height;
                BarViewModels[j].Width = ControlActualWidth / FFTBins.Count;
                j++;
            }

            Note[] notes = _noteSource.GetNotes();
            if (notes.Length > 0)
                Frequency = notes.Last().frequency;
            
            //System.Diagnostics.Debug.Write(FFTBins.Count + "\n");

        }

        #region Frequency
        private double _Frequency;
        private static PropertyChangedEventArgs _Frequency_ChangedEventArgs = new PropertyChangedEventArgs("Frequency");
        public double Frequency
        {
            get { return _Frequency; }
            set { 
                _Frequency = value;
                NotifyPropertyChanged(_Frequency_ChangedEventArgs);
            }
        }
        #endregion

        #region MaxValue
        private static PropertyChangedEventArgs _MaxValue_ChangedEventArgs = new PropertyChangedEventArgs("MaxValue");
        public double MaxValue {
            get { return _maxPower; }
            private set {
                _maxPower = value;
                NotifyPropertyChanged(_MaxValue_ChangedEventArgs);
            }
        }
        #endregion

        #region FFTBins
        private ObservableCollection<FFTBinViewModel> _FFTBins = new ObservableCollection<FFTBinViewModel>();
        private static PropertyChangedEventArgs _FFTBins_ChangedEventArgs = new PropertyChangedEventArgs("FFTBins");
        public ObservableCollection<FFTBinViewModel> FFTBins {
            get { return _FFTBins; }
            set {
                _FFTBins = value;
                NotifyPropertyChanged(_FFTBins_ChangedEventArgs);
            }
        }
        #endregion

        #region ControlActualWidth
        private double _ControlActualWidth;
        private static PropertyChangedEventArgs _ControlActualWidth_ChangedEventArgs = new PropertyChangedEventArgs("ControlActualWidth");

        public double ControlActualWidth {
            get { return _ControlActualWidth; }
            set {
                _ControlActualWidth = value;
                NotifyPropertyChanged(_ControlActualWidth_ChangedEventArgs);
            }
        }
        #endregion

        #region ControlActualHeight
        private double _ControlActualHeight;
        private static PropertyChangedEventArgs _ControlActualHeight_ChangedEventArgs = new PropertyChangedEventArgs("ControlActualHeight");

        public double ControlActualHeight {
            get { return _ControlActualHeight; }
            set {
                _ControlActualHeight = value;
                NotifyPropertyChanged(_ControlActualHeight_ChangedEventArgs);
            }
        }
        #endregion

        #region Bars
        private ObservableCollection<BarViewModel> _BarViewModels;
        private static PropertyChangedEventArgs _Bars_ChangedEventArgs = new PropertyChangedEventArgs("BarViewModels");

        public ObservableCollection<BarViewModel> BarViewModels {
            get { return _BarViewModels; }
            set {
                _BarViewModels = value;
                NotifyPropertyChanged(_Bars_ChangedEventArgs);
            }
        }
        #endregion
    }
}

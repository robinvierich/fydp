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
        private DispatcherTimer _fftUpdateTimer = new DispatcherTimer();

        

        public void StartReadingFFT()
        {
            if (_fftUpdateTimer.IsEnabled)
                return;

            _fftUpdateTimer.Interval = TimeSpan.FromMilliseconds(50);
            _fftUpdateTimer.Tick += new EventHandler(timer_Tick);
            _fftUpdateTimer.Start();
        }

        public void StopReadingFFT()
        {
            if (!_fftUpdateTimer.IsEnabled)
                return;

            _fftUpdateTimer.Stop();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            FFTCalculation fftCalc;
            if (!_fftSource.FFTQueue.TryPeek(out fftCalc))
                return;

            // copy array (otherwise we get threading errors)
            double[] powerBins = new double[fftCalc.PowerBins.Length];
            Array.Copy(fftCalc.PowerBins, powerBins, fftCalc.PowerBins.Length);

            ObservableCollection<FFTBinViewModel> newBins = new ObservableCollection<FFTBinViewModel>();
            for (int i = 0; i < fftCalc.PowerBins.Length; i++)
            {
                FFTBinViewModel newBin = new FFTBinViewModel(){
                    BinNumber = i,
                    Power = powerBins[i]
                };
                newBins.Add(newBin);
            }
            
            FFTBins = newBins;

            double maxPower = FFTBins.Max(bin => 
                {
                    return bin.Power;
                });

            ObservableCollection<BarViewModel> bvms = new ObservableCollection<BarViewModel>();
            foreach (FFTBinViewModel bin in FFTBins)
            {
                BarViewModel bvm = new BarViewModel()
                {
                    Height = (bin.Power / maxPower) * ControlActualHeight,
                    Width = ControlActualWidth / FFTBins.Count
                };
                bvms.Add(bvm);
            }

            BarViewModels = bvms;
        }


        #region FFTBins
        private ObservableCollection<FFTBinViewModel> _FFTBins = new ObservableCollection<FFTBinViewModel>();
        private static PropertyChangedEventArgs _FFTBins_ChangedEventArgs = new PropertyChangedEventArgs("FFTBins");
        public ObservableCollection<FFTBinViewModel> FFTBins
        {
            get { return _FFTBins; }
            set
            {
                _FFTBins = value;
                NotifyPropertyChanged(_FFTBins_ChangedEventArgs);
            }
        }
        #endregion

        #region ControlActualWidth
        private double _ControlActualWidth;
        private static PropertyChangedEventArgs _ControlActualWidth_ChangedEventArgs = new PropertyChangedEventArgs("ControlActualWidth");

        public double ControlActualWidth
        {
            get { return _ControlActualWidth; }
            set
            {
                _ControlActualWidth = value;
                NotifyPropertyChanged(_ControlActualWidth_ChangedEventArgs);
            }
        }
        #endregion

        #region ControlActualHeight
        private double _ControlActualHeight;
        private static PropertyChangedEventArgs _ControlActualHeight_ChangedEventArgs = new PropertyChangedEventArgs("ControlActualHeight");

        public double ControlActualHeight
        {
            get { return _ControlActualHeight; }
            set
            {
                _ControlActualHeight = value;
                NotifyPropertyChanged(_ControlActualHeight_ChangedEventArgs);
            }
        }
        #endregion

        #region Bars
        private ObservableCollection<BarViewModel> _BarViewModels;
        private static PropertyChangedEventArgs _Bars_ChangedEventArgs = new PropertyChangedEventArgs("BarViewModels");

        public ObservableCollection<BarViewModel> BarViewModels
        {
            get { return _BarViewModels; }
            set
            {
                _BarViewModels = value;
                NotifyPropertyChanged(_Bars_ChangedEventArgs);
            }
        }
        #endregion
    }
}

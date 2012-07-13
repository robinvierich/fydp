using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using BlueWave.Interop.Asio;
using Regis.AudioCapture.Services;

namespace Regis.Commands
{

    public class LoadDriverCommandArgs
    {
        private InstalledDriver _driver;
        private uint _sampleRate;

        public LoadDriverCommandArgs(InstalledDriver driver, uint sampleRate)
        {
            _driver = driver;
            _sampleRate = sampleRate;
        }

        public InstalledDriver Driver
        {
            get { return _driver; }
        }

        public uint SampleRate
        {
            get { return _sampleRate; }
        }
    }

    public class LoadDriverCommand: ICommand
    {
        public LoadDriverCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public void Execute(object parameter)
        {
            LoadDriverCommandArgs args = parameter as LoadDriverCommandArgs;
            if (args == null)
                throw new Exception("LoadDriverCommand needs a LoadDriverCommandArgs object as the command parameter");

            AsioDeviceService.LoadDriver(args.Driver, args.SampleRate);
        }

    }
}

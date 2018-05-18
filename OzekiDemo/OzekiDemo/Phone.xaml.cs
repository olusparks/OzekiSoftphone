using Ozeki.Network;
using Ozeki.VoIP;
using OzekiDemo.DataObjects;
using OzekiDemo.Model;
using OzekiDemo.Model.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestSoftphone;
using static OzekiDemo.DataOperation.DataOperation;

namespace OzekiDemo
{
    /// <summary>
    /// Interaction logic for Phone.xaml
    /// </summary>
    public partial class Phone : Window, INotifyPropertyChanged
    {
        public SoftphoneEngine Model { get; private set; }
        public MediaHandlers MediaHandlers { get { return Model.MediaHandlers; } }



        public static bool PowerOn = false;
        public static bool MicOff = false;
        public static bool SoundOff = false;

        public Phone(SoftphoneEngine model)
        {
            Model = model;
            InitializeComponent();

            Model.PhoneLineStateChanged += (Model_PhoneLineStateChanged);
            Model.PhoneCallStateChanged += (Model_PhoneCallStateChanged);
            Model.MessageSummaryReceived += (Model_MessageSummaryReceived);
            Model.NatDiscoveryFinished += Model_NatDiscoveryFinished;
            Model.CallInstantMessageReceived += (Model_CallInstantMessageReceived);
            Model.MediaHandlers.MicrophoneStopped += MediaHandlers_MicrophoneStopped;
            Model.MediaHandlers.SpeakerStopped += MediaHandlers_SpeakerStopped;

            //txtCallStatus.Text = Model.callState.ToString();
        }

        #region Window events

        private void Window_Load(object sender, RoutedEventArgs e)
        {
            /*remoteVideoViewer.SetImageProvider(MediaHandlers.RemoteImageProvider);
            localVideoViewer.SetImageProvider(MediaHandlers.LocalImageProvider);

            remoteVideoViewer.Start();
            localVideoViewer.Start();*/
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Model.PhoneLineStateChanged -= (Model_PhoneLineStateChanged);
            Model.PhoneCallStateChanged -= (Model_PhoneCallStateChanged);
            Model.MessageSummaryReceived -= (Model_MessageSummaryReceived);
            Model.NatDiscoveryFinished -= (Model_NatDiscoveryFinished);
            Model.CallInstantMessageReceived -= (Model_CallInstantMessageReceived);
            Model.Dispose();
        }
        #endregion

        #region Model events

        private void Model_PhoneLineStateChanged(object sender, GEventArgs<IPhoneLine> e)
        {
            //tbPhoneLineStatus.Dispatcher.Invoke(new Action(() => tbPhoneLineStatus.GetBindingExpression(TextBox.TextProperty).UpdateTarget()));
            //txtCallStatus.Dispatcher.Invoke(new Action(() => txtCallStatus.GetBindingExpression(TextBox.TextProperty).UpdateTarget()));
            //txtCallStatus.Dispatcher.Invoke(new Action(() => txtCallStatus.Text = "Registration successful"));
        }

        private void Model_PhoneCallStateChanged(object sender, GEventArgs<IPhoneCall> e)
        {
            UpdatePhoneCalls();
        }

        private void Model_MessageSummaryReceived(object sender, MessageSummaryArgs e)
        {
            /*btnMessageSummary.Dispatcher.Invoke(new Action(() => {
                btnMessageSummary.GetBindingExpression(Button.ContentProperty).UpdateTarget();
                btnMessageSummary.GetBindingExpression(Button.IsEnabledProperty).UpdateTarget();
            }));*/
        }

        private void Model_CallInstantMessageReceived(object sender, PhoneCallInstantMessageArgs e)
        {
            string sipAccount = string.Format("{0}@{1}", e.PhoneCall.PhoneLine.SIPAccount.UserName, e.PhoneCall.PhoneLine.SIPAccount.DomainServerHost);

            StringBuilder sb = new StringBuilder();
            sb.Append("Instant message received\r\n");
            sb.Append(string.Format("Call: {0} - {1}\r\n", sipAccount, e.PhoneCall.DialInfo));
            sb.Append(string.Format("Message: {0}", e.Message.Content));

            MessageBox.Show(sb.ToString());
        }

        private void Model_NatDiscoveryFinished(object sender, GEventArgs<NatInfo> e)
        {
            NatInfo info = e.Item;
            //natDiscoveryWin.Dispatcher.Invoke(new Action(() => natDiscoveryWin.Close()));

            MessageBox.Show(string.Format("NAT discovery finished. NAT Type: {0}, Public address: {1}", info.NatType, info.PublicAddress));
        }

        private void MediaHandlers_SpeakerStopped(object sender, EventArgs e)
        {
            //RefreshSpeakerDevices();
        }

        private void MediaHandlers_MicrophoneStopped(object sender, EventArgs e)
        {
            //RefreshMicrophoneDevices();
        }

        private void UpdatePhoneCalls()
        {
            //lvPhoneCalls.Dispatcher.Invoke(new Action(() => lvPhoneCalls.Items.Refresh()));
        }

        #endregion

        #region GUIEvents

        public IPhoneLine SelectedLine
        {
            get { return Model.SelectedLine; }
        }

        private string dialNumber;
        public string DialNumber
        {
            get { return dialNumber; }
            set
            {
                dialNumber = value;
                OnNotifyPropertyChanged("DialNumber");
            }
        }

        #endregion

        #region PowerOn Softphone
        private void PowerOn_Click(object sender, RoutedEventArgs e)
        {
            if (PowerOn == false)
            {
                if (!GetSIPRegistrationStatus())
                {
                    ShowLicenseError("Unable to Initialize softphone");
                    return;
                }
                if (!GetPhoneLine())
                {
                    ShowLicenseError("Unable to register SoftPhone!");
                    return;
                }
                if (!Register())
                {
                    ShowLicenseError("Unable to register SoftPhone! Err-001");
                    return;
                }

                ImgNetWork.Source = GetImage(@"\Images\signalYes.png");
                ImgIsOn.Source = GetImage(@"\Images\StatusOn.png");
                ImgIsRegistered.Source = GetImage(@"\Images\StatusRegistered.png");
                txtCallStatus.Text = "Connected";
                OnlineStatus.Text = "Online";
                PowerOn = true;
                txtNumber.Focus();
            }
            else
            {
                UnRegister();
                ImgNetWork.Source = GetImage(@"\Images\signalNo.png");
                ImgIsOn.Source = GetImage(@"\Images\StatusOff.png");
                ImgIsRegistered.Source = GetImage(@"\Images\StatusOff.png");
                txtCallStatus.Text = "Not Connected";
                OnlineStatus.Text = "Offline";
                PowerOn = false;
                txtNumber.Focus();
            }

        }
        #endregion


        private void Dial_Click(object sender, RoutedEventArgs e)
        {

            if (Model.Isincoming)
            {
                //txtCallStatus.Text = Model.callState.ToString();
                lblCalleeNumber.Visibility = Visibility.Visible;
                lblCalleeNumber.Content = $"{Model.SelectedCall.DialInfo.CallerID}  --  {Model.SelectedCall.DialInfo.CallerDisplay}";

                Answer();

                txtCallStatus.Text = Model.callState.ToString();
                //InvokeGUIThread(() => { txtCallStatus.Text = ("Call accepted."); });


                //TODO : CTI
                DbContact contact = GetContactByPhone(txtNumber.Text.Trim());
                txtCallerName.Text = contact.FullName;

                Model.Isincoming = false;
                return;
            }
            Dial(CallType.Audio, false);
            Model.Isincoming = false;
        }

        #region Microphone, Speaker

        private void Mute_Click(object sender, RoutedEventArgs e)
        {

            int mic = 0;
            if (MicOff == false)
            {
                MediaHandlers.Microphone.Volume = mic;
                MicSlider.Value = mic;
                BtnMicImage.Source = GetImage(@"\Images\MicNo.png");
                MicOff = true;
            }
            else
            {
                MicSlider.Value = MediaHandlers.Microphone.Volume;
                BtnMicImage.Source = GetImage(@"\Images\MicYes.png");
                MicOff = false;
            }
        }

        private void MuteSound_Click(object sender, RoutedEventArgs e)
        {
            int vol = 0;
            if (SoundOff == false)
            {
                VolSlider.Value = vol;
                MediaHandlers.Speaker.Volume = vol;
                BtnVolImg4.Source = GetImage(@"\Images\SoundNo.png");
                SoundOff = true;
            }
            else
            {
                VolSlider.Value = MediaHandlers.Speaker.Volume;
                BtnVolImg4.Source = GetImage(@"\Images\SoundYes.png");
                SoundOff = false;
            }
        }

        #endregion

        #region Phone call functions
        private void buttonHangUp_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedCall != null)
            {
                if (Model.Isincoming && Model.SelectedCall.CallState == CallState.Ringing)
                {
                    Model.RejectCall();
                    txtCallStatus.Text = Model.callState.ToString();

                }
                else
                {
                    Model.HangUpCall();
                    txtCallStatus.Text = Model.callState.ToString();
                    Model.Isincoming = false;
                }
                Model.SelectedCall = null;
            }
            Clear();

            TimeSpan span = new TimeSpan(0, 0, 0, 0, 300);
            System.Threading.Thread.Sleep(span);
            txtCallStatus.Text = "Connected";
        }

        private void Clear()
        {
            txtCallerName.Text = string.Empty;
            //txtCallStatus.Text = "Connected";
            txtNumber.Text = string.Empty;
            lblCalleeNumber.Content = string.Empty;
        }

        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedCall == null)
                return;

            if (!Model.SelectedCall.CallState.IsInCall())
                return;

            TransferModel transferSettings = new TransferModel(Model.PhoneCalls, Model.SelectedCall);
            TransferWindow tranferWin = new TransferWindow(this, transferSettings);
            bool? ok = tranferWin.ShowDialog();
            if (ok != null && ok == true)
            {
                // blind transfer
                if (transferSettings.TransferMode == TransferMode.Blind)
                {
                    Model.BlindTransfer(transferSettings.BlindTransferTarget);
                    return;
                }

                // attended transfer
                if (transferSettings.TransferMode == TransferMode.Attended)
                {
                    Model.AttendedTransfer(transferSettings.AttendedTransferTarget);
                    return;
                }
            }
        }
        private void ButtonForward_Click(object sender, RoutedEventArgs e)
        {
            if (Model.SelectedCall == null)
                return;

            if (!Model.SelectedCall.CallState.IsInCall())
                return;

            TransferModel transferSettings = new TransferModel(Model.PhoneCalls, Model.SelectedCall);
            TransferWindow tranferWin = new TransferWindow(this, transferSettings);
            bool? ok = tranferWin.ShowDialog();
            if (ok != null && ok == true)
            {
                // blind transfer
                if (transferSettings.TransferMode == TransferMode.Blind)
                {
                    Model.BlindTransfer(transferSettings.BlindTransferTarget);
                    return;
                }

                // attended transfer
                if (transferSettings.TransferMode == TransferMode.Attended)
                {
                    Model.AttendedTransfer(transferSettings.AttendedTransferTarget);
                    return;
                }
            }
        }
        private void Answer()
        {
            try
            {
                Model.AnswerCall();
            }
            catch (Exception ex)
            {
                ShowLicenseError(ex.Message);
            }
        }
        #endregion

        private void txtNumber_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !IsNumberKey(e.Key) && !IsDelOrBackspaceOrTabKey(e.Key);
            if (txtNumber.Text == "")
            {
                //Clear();
            }
            txtNumber.Focus();
        }


        private void ButtonSetting_Click(object sender, RoutedEventArgs e)
        {
            AccountModel model = new AccountModel();
            var settings = new PhoneSetting(this, model);
            bool? ok = settings.ShowDialog();

            model = settings.Model;
            if (ok != null && ok == true)
            {
                var line = Model.AddPhoneLine(model.SIPAccount, model.TransportType, model.NatConfig, model.SRTPMode);

                //if (Model.SelectedLine == null)
                Model.SelectedLine = line;
            }
        }

        #region HELPER METHODS
        public BitmapImage GetImage(string imagePath)
        {
            BitmapImage MImage;
            MImage = new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
            return MImage;
        }
        #endregion

        #region Registeration


        private bool GetPhoneLine()
        {
            bool iSuccess = false;
            try
            {
                AccountModel model = new AccountModel();
                SIPAccount sa = new SIPAccount(model.RegistrationRequired, "Jide", "851", "851", "reliance@123", "192.168.20.35");
                var line = Model.AddPhoneLine(sa, model.TransportType, model.NatConfig, model.SRTPMode);
                Model.SelectedLine = line;
                iSuccess = true;
            }
            catch (Exception)
            {
                iSuccess = false;
            }
            return iSuccess;
        }
        private bool Register()
        {
            bool iSuccess = false;
            try
            {
                Model.RegisterPhoneLine();
                iSuccess = true;
            }
            catch (Exception ex)
            {
                ShowLicenseError(ex.Message);
            }
            return iSuccess;
        }

        private void UnRegister()
        {
            Model.UnregisterPhoneLine();
        }


        private void ShowLicenseError(string message)
        {
            MessageBox.Show(message, "Reliance Ozkei", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region HandlingNumberKey

        private bool IsNumberKey(Key inKey)
        {
            if (inKey < Key.D0 || inKey > Key.D9)
            {
                if (inKey < Key.NumPad0 || inKey > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Delete, Backspace, Tab
        /// </summary>
        /// <param name="inKey"></param>
        /// <returns></returns>
        private bool IsDelOrBackspaceOrTabKey(Key inKey)
        {
            return inKey == Key.Delete || inKey == Key.Back || inKey == Key.Tab;
        }




        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnNotifyPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Dialpad
        private void buttonNumber_Click(object sender, RoutedEventArgs e)
        {
            Button btnPressed = (Button)sender;
            if (btnPressed == null)
                return;

            //txtNumber.Text += btnPressed.Content;

            // if no calls selected, extend the dial number
            if (Model.SelectedCall == null)
                txtNumber.Text += btnPressed.Content.ToString();
            DialNumber = txtNumber.Text;
        }
        private void btnKeyPad_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            // if no calls selected, extend the dial number
            if (Model.SelectedCall == null)
                DialNumber += button.Content.ToString();
        }

        private void btnKeyPad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            // start DTMF
            int signal;
            if (int.TryParse(button.Tag.ToString(), out signal))
                Model.StartDtmfSignal(signal);
        }

        private void btnKeyPad_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            // stop DTMF
            int signal;
            if (int.TryParse(button.Tag.ToString(), out signal))
                Model.StopDtmfSignal(signal);
        }

        private void btnDialAudio_Click(object sender, RoutedEventArgs e)
        {
            Dial(CallType.Audio, false);
        }

        private void Dial(CallType callType, bool dialIP)
        {
            if (SelectedLine == null)
                return;

            if (!SelectedLine.RegState.IsRegistered())
            {
                MessageBox.Show("Cannot start calls while the selected line is not registered.");
                return;
            }
            try
            {
                if (!dialIP)
                {
                    Model.Dial(DialNumber, callType);
                    txtCallStatus.Text = "Calling";
                }
                else
                    Model.DialIP(DialNumber, callType);
            }
            catch (Exception ex)
            {
                ShowLicenseError(ex.Message);
            }

            DialNumber = string.Empty;
        }

        #endregion

        private void InvokeGUIThread(Action action)
        {
            this.Dispatcher.Invoke(action);
        }


    }
}

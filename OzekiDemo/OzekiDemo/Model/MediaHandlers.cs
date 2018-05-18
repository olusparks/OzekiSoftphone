using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ozeki.Media;
using Ozeki.VoIP;
using TestSoftphone;
using System.ComponentModel;
using System.Drawing;
using Ozeki.Camera;

namespace OzekiDemo.Model
{
    public class MediaHandlers : INotifyPropertyChanged, IDisposable
    {
        private bool _initialized;

        #region Audio Handlers

        private MediaConnector _audioConnector;

        public Microphone Microphone { get; private set; }
        public Speaker Speaker { get; private set; }
        public AudioQualityEnhancer AudioEnhancer { get; private set; }
        private DtmfEventWavePlayer _dtmfPlayer;

        // mixers
        //private AudioForwarder outgoingDataMixer;
        //private AudioForwarder speakerMixer;
        //private AudioForwarder recordDataMixer;

        // phone call handlers
        private PhoneCallAudioSender _phoneCallAudioSender;
        private PhoneCallAudioReceiver _phoneCallAudioReceiver;

        // audio files
        private WaveStreamRecorder _wavRecorder;
        private WaveStreamPlayback _wavPlayer;
        private MP3StreamPlayback _mp3StreamPlayback;
        private WaveStreamPlayback _ringtonePlayer;
        private WaveStreamPlayback _ringbackPlayer;

        #endregion

        #region Video Handlers

        private MediaConnector _videoConnector;
        public IWebCamera WebCamera { get; private set; }

        // image providers
        public ImageProvider<Image> LocalImageProvider { get; private set; }
        public ImageProvider<Image> RemoteImageProvider { get; private set; }

        // phone call handlers
        private PhoneCallVideoSender _phoneCallVideoSender;
        private PhoneCallVideoReceiver _phoneCallVideoReceiver;

        #endregion

        #region Other Properties

        /// <summary>
        /// Gets the available noise reduction levels.
        /// </summary>
        public List<NoiseReductionLevel> NoiseReductionLevels { get; private set; }

        /// <summary>
        /// Gets the level of the microphone.
        /// </summary>
        public float MicrophoneLevel { get; private set; }

        /// <summary>
        /// Gets the level of the speaker.
        /// </summary>
        public float SpeakerLevel { get; private set; }

        /// <summary>
        /// Gets the available microphone devices.
        /// </summary>
        public List<AudioDeviceInfo> Microphones
        {
            get { return Microphone.GetDevices(); }
        }

        /// <summary>
        /// Gets the available speaker devices.
        /// </summary>
        public List<AudioDeviceInfo> Speakers
        {
            get { return Speaker.GetDevices(); }
        }

        /// <summary>
        /// Gets the available camera devices.
        /// </summary>
        public List<VideoDeviceInfo> Cameras
        {
            get { return WebCameraFactory.GetDevices(); }
        }

        /// <summary>
        /// Gets the frame rates that can be set to the camera.
        /// </summary>
        public List<int> FrameRates { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the level of the microphone has changed.
        /// </summary>
        public event EventHandler<GEventArgs<float>> MicrophoneLevelChanged;

        /// <summary>
        /// Occurs when the the microphone stopped recording.
        /// </summary>
        public event EventHandler<EventArgs> MicrophoneStopped;

        /// <summary>
        /// Occurs when the level of the speaker has changed.
        /// </summary>
        public event EventHandler<GEventArgs<float>> SpeakerLevelChanged;

        /// <summary>
        /// Occurs when the the speaker stopped playing.
        /// </summary>
        public event EventHandler<EventArgs> SpeakerStopped;

        #endregion

        #region Init

        public MediaHandlers()
        {
            NoiseReductionLevels = new List<NoiseReductionLevel>();
            NoiseReductionLevels.Add(NoiseReductionLevel.NoReduction);
            NoiseReductionLevels.Add(NoiseReductionLevel.Low);
            NoiseReductionLevels.Add(NoiseReductionLevel.Medium);
            NoiseReductionLevels.Add(NoiseReductionLevel.High);

            FrameRates = new List<int> {0, 30, 25, 20, 15, 10, 5};

            _audioConnector = new MediaConnector();
            _videoConnector = new MediaConnector();
        }

        public void Init()
        {
            if (_initialized)
                return;

            InitAudio();
            //InitVideo();

            if (Microphone != null)
            {
                SubscribeToMicrophoneEvents();
                Microphone.Start();
            }

            if (Speaker != null)
            {
                SubscribeToSpeakerEvents();
                Speaker.Start();
            }

            _initialized = true;

            /*
             * Media Connections:
             *
             * +---------------------------------------------------------------+
             * |                           PhoneCall                           |***********
             * +---------------------------------------------------------------+          *
             *              ^^                              VV                            *
             *     +--------------------+          +----------------------+  +----+   +---------+  +-----+
             *     |PhoneCallAudioSender|          |PhoneCallAudioReceiver|  |DMTF|   |Ringtones|  | TTS |
             *     +--------------------+          +----------------------+  +----+   +---------+  +-----+
             *         ^^          ^^                V                  V        V           V       V
             *+--------------+   +---------+    +-----------+        +-------+   |           |       |
             *|AudioEnhancer |   |WavPlayer|    |WavRecorder|        |Speaker|<--/-----------/-------|
             *+--------------+   +---------+    +-----------+        +-------+   
             *        ^^                                                
             *    +----------+                                       
             *    |Microphone|                                       
             *    +----------+                                       
             */
        }

        /// <summary>
        /// Initializes the audio handlers (microphone, speaker, DTMF player etc.).
        /// </summary>
        private void InitAudio()
        {
            // ----- CREATE -----
            // devices
            Microphone = Microphone.GetDefaultDevice();
            Speaker = Speaker.GetDefaultDevice();

            // audio processors
            if (Microphone == null)
                AudioEnhancer = new AudioQualityEnhancer(new AudioFormat());
            else
                AudioEnhancer = new AudioQualityEnhancer(Microphone.MediaFormat);
            AudioEnhancer.SetEchoSource(Speaker);
            _dtmfPlayer = new DtmfEventWavePlayer();

            // ringtones
            var ringbackStream = LoadRingbackStream();
            var ringtoneStream = LoadRingtoneStream();
            _ringtonePlayer = new WaveStreamPlayback(ringtoneStream, true, false);
            _ringbackPlayer = new WaveStreamPlayback(ringbackStream, true, false);

            // mixers
            //outgoingDataMixer = new AudioForwarder();
            //speakerMixer = new AudioForwarder();
            //recordDataMixer = new AudioForwarder();

            // phone handlers
            _phoneCallAudioSender = new PhoneCallAudioSender();
            _phoneCallAudioReceiver = new PhoneCallAudioReceiver();


            // ----- CONNECT -----
            // connect outgoing
            //audioConnector.Connect(AudioEnhancer, outgoingDataMixer);
            //audioConnector.Connect(outgoingDataMixer, phoneCallAudioSender);
            //audioConnector.Connect(outgoingDataMixer, recordDataMixer);
            _audioConnector.Connect(AudioEnhancer, _phoneCallAudioSender);
            if (Microphone != null)
            {
                Microphone.LevelChanged += (Microphone_LevelChanged);
                _audioConnector.Connect(Microphone, AudioEnhancer);
            }

            // connect incoming
            if (Speaker != null)
            {
                Speaker.LevelChanged += (Speaker_LevelChanged);
                _audioConnector.Connect(_phoneCallAudioReceiver, Speaker);
                _audioConnector.Connect(_ringtonePlayer, Speaker);
                _audioConnector.Connect(_ringbackPlayer, Speaker);
                _audioConnector.Connect(_dtmfPlayer, Speaker);
            }
        }

        private static Stream LoadRingbackStream()
        {
            Stream ringback = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OzekiDemo.Resources.ringback.wav");

            if (ringback == null)
                throw new Exception("Cannot load default ringback.");

            return ringback;
        }

        private Stream LoadRingtoneStream()
        {
            Stream basicRing = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OzekiDemo.Resources.basic_ring.wav");

            if (basicRing == null)
                throw new Exception("Cannot load default ringtone.");

            return basicRing;
        }

        /// <summary>
        /// Initializes the video handlers (camera, image providers etc.).
        /// </summary>
        private void InitVideo()
        {
            // ----- CREATE -----
            WebCamera = WebCameraFactory.GetDefaultDevice();
            
            LocalImageProvider = new DrawingImageProvider();
            RemoteImageProvider = new DrawingImageProvider();

            _phoneCallVideoReceiver = new PhoneCallVideoReceiver();
            _phoneCallVideoSender = new PhoneCallVideoSender();

            // ----- CONNECT -----
            _videoConnector.Connect(_phoneCallVideoReceiver, RemoteImageProvider);
            if (WebCamera != null)
            {
                _videoConnector.Connect(WebCamera.VideoChannel, LocalImageProvider);
                _videoConnector.Connect(WebCamera.VideoChannel, _phoneCallVideoSender);
            }
        }

        #endregion

        #region Invocators

        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnSpeakerLevelChanged(float value)
        {
            var handler = SpeakerLevelChanged;
            if (handler != null)
                handler(this, new GEventArgs<float>(value));
        }

        private void OnMicrophoneLevelChanged(float value)
        {
            var handler = MicrophoneLevelChanged;
            if (handler != null)
                handler(this, new GEventArgs<float>(value));
        }

        #endregion

        #region Audio EventHandlers

        /// <summary>
        /// This will be called when the level of the microphone has changed.
        /// </summary>
        private void Microphone_LevelChanged(object sender, VoIPEventArgs<float> e)
        {
            MicrophoneLevel = e.Item;
            OnPropertyChanged("MicrophoneLevel");

            OnMicrophoneLevelChanged(e.Item);
        }

        /// <summary>
        /// This will be called when the level of the speaker has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Speaker_LevelChanged(object sender, VoIPEventArgs<float> e)
        {
            SpeakerLevel = e.Item;
            OnPropertyChanged("SpeakerLevel");

            OnSpeakerLevelChanged(e.Item);
        }

        #endregion

        #region Microphone

        /// <summary>
        /// Changes the microphone device.
        /// </summary>
        public void ChangeMicrophone(AudioDeviceInfo deviceInfo)
        {
            if (!_initialized)
                return;

            float prevVolume = 0;
            bool prevMuted = false;

            if (Microphone != null)
            {
                // same device
                if (Microphone.DeviceInfo.Equals(deviceInfo))
                    return;

                // backup settings
                prevVolume = Microphone.Volume;
                prevMuted = Microphone.Muted;

                // dispose previous device
                _audioConnector.Disconnect(Microphone, AudioEnhancer);
                UnsubscribeFromMicrophoneEvents();
                Microphone.Dispose();
            }

            // create new microphone
            Microphone = Microphone.GetDevice(deviceInfo);

            if (Microphone != null)
            {
                SubscribeToMicrophoneEvents();
                _audioConnector.Connect(Microphone, AudioEnhancer);

                // set prev device settings
                Microphone.Volume = prevVolume;
                Microphone.Muted = prevMuted;
                Microphone.Start();
            }

            OnPropertyChanged("Microphone");
        }

        private void SubscribeToMicrophoneEvents()
        {
            if (Microphone == null)
                return;

            Microphone.LevelChanged += (Microphone_LevelChanged);
            Microphone.Stopped += Microphone_Stopped;
        }

        private void UnsubscribeFromMicrophoneEvents()
        {
            if (Microphone == null)
                return;

            Microphone.LevelChanged -= (Microphone_LevelChanged);
            Microphone.Stopped -= Microphone_Stopped;
        }

        void Microphone_Stopped(object sender, EventArgs e)
        {
            var handler = MicrophoneStopped;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Speaker

        /// <summary>
        /// Changes the speaker device and sets the volume and muted property of the new device.
        /// </summary>
        public void ChangeSpeaker(AudioDeviceInfo deviceInfo)
        {
            if (!_initialized)
                return;

            float prevVolume = 0;
            bool prevMuted = false;

            if (Speaker != null)
            {
                // backup settings
                prevVolume = Speaker.Volume;
                prevMuted = Speaker.Muted;

                // dispose previous device
                _audioConnector.Disconnect(_phoneCallAudioReceiver, Speaker);
                _audioConnector.Disconnect(_ringtonePlayer, Speaker);
                _audioConnector.Disconnect(_ringbackPlayer, Speaker);
                _audioConnector.Disconnect(_dtmfPlayer, Speaker);
                UnsubscribeFromSpeakerEvents();
                Speaker.Dispose();

                AudioEnhancer.SetEchoSource(null);
            }

            // create new microphone
            Speaker = Speaker.GetDevice(deviceInfo);

            if (Speaker != null)
            {
                SubscribeToSpeakerEvents();
                _audioConnector.Connect(_phoneCallAudioReceiver, Speaker);
                _audioConnector.Connect(_ringtonePlayer, Speaker);
                _audioConnector.Connect(_ringbackPlayer, Speaker);
                _audioConnector.Connect(_dtmfPlayer, Speaker);

                // set prev device settings
                Speaker.Volume = prevVolume;
                Speaker.Muted = prevMuted;
                Speaker.Start();

                AudioEnhancer.SetEchoSource(Speaker);
            }

            OnPropertyChanged("Speaker");
        }

        private void SubscribeToSpeakerEvents()
        {
            if (Speaker == null)
                return;

            Speaker.LevelChanged += (Speaker_LevelChanged);
            Speaker.Stopped += Speaker_Stopped;
        }

        private void UnsubscribeFromSpeakerEvents()
        {
            if (Speaker == null)
                return;

            Speaker.LevelChanged -= (Speaker_LevelChanged);
            Speaker.Stopped -= Speaker_Stopped;
        }

        void Speaker_Stopped(object sender, EventArgs e)
        {
            var handler = SpeakerStopped;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region Camera

        /// <summary>
        /// Changes the camera device.
        /// </summary>
        public void ChangeCamera(int deviceID)
        {
            if (!_initialized)
                return;

            // same device
            if (WebCamera != null && WebCamera.DeviceID == deviceID)
                return;

            // find the proper info
            VideoDeviceInfo newDeviceInfo = null;
            foreach (var info in Cameras)
            {
                if (info.DeviceID != deviceID)
                    continue;

                newDeviceInfo = info;
                break;
            }

            if (newDeviceInfo == null)
                return;


            // begin change device
            bool capturing = false;

            if (WebCamera != null)
            {
                // disconnect
                if (LocalImageProvider != null)
                    _audioConnector.Disconnect(WebCamera.VideoChannel, LocalImageProvider);
                _audioConnector.Disconnect(WebCamera.VideoChannel, _phoneCallVideoSender);

                // dispose previous device
                capturing = WebCamera.Capturing;
                WebCamera.Stop();
                WebCamera.Dispose();
            }

            // create new
            WebCamera = WebCameraFactory.GetDevice(newDeviceInfo);

            if (WebCamera != null)
            {
                _audioConnector.Connect(WebCamera.VideoChannel, LocalImageProvider);
                _audioConnector.Connect(WebCamera.VideoChannel, _phoneCallVideoSender);

                if (capturing)
                    WebCamera.Start();
            }

            OnPropertyChanged("WebCamera");
        }

        /// <summary>
        /// Gets the supported video resolutions of the camera.
        /// </summary>
        /// <returns></returns>
        public List<Resolution> GetVideoResolutions()
        {
            var resolutions = new List<Resolution>();

            if (WebCamera == null)
                return resolutions;

            List<VideoCapabilities> capabilities = WebCamera.Capabilities;
            if (capabilities == null)
                return resolutions;

            resolutions.AddRange(capabilities.Select(cap => new Resolution(cap.Resolution.Width, cap.Resolution.Height)));

            return resolutions;
        }

        #endregion

        #region DTMF

        /// <summary>
        /// Starts playing the DTMF signal.
        /// </summary>
        internal void StartDtmf(DtmfNamedEvents signal)
        {
            if (!_initialized)
                return;

            _dtmfPlayer.Start(signal);
        }

        /// <summary>
        /// Starts playing the DTMF signal.
        /// </summary>
        internal void StartDtmf(int signal)
        {
            if (!_initialized)
                return;

            _dtmfPlayer.Start(signal);
        }

        /// <summary>
        /// Stops playing the DTMF signal.
        /// </summary>
        internal void StopDtmf(DtmfNamedEvents signal)
        {
            if (!_initialized)
                return;

            _dtmfPlayer.Stop();
        }

        /// <summary>
        /// Stops playing the DTMF signal.
        /// </summary>
        internal void StopDtmf(int signal)
        {
            if (!_initialized)
                return;

            _dtmfPlayer.Stop();
        }

        #endregion

        #region Wav Playback

        /// <summary>
        /// Loads a wav file for playback.
        /// </summary>
        public void LoadPlaybackWavFile(string filePath)
        {
            if (!_initialized)
                return;

            if (_wavPlayer != null)
            {
                _audioConnector.Disconnect(_wavPlayer, _phoneCallAudioSender);
                _wavPlayer.Dispose();
                _wavRecorder = null;
            }

            _wavPlayer = new WaveStreamPlayback(filePath, false, true);

            _audioConnector.Connect(_wavPlayer, _phoneCallAudioSender);
        }

        /// <summary>
        /// Starts streaming the wav file.
        /// </summary>
        public void StartWavPlayback()
        {
            if (_wavPlayer == null)
                return;

            _wavPlayer.Start();
        }

        /// <summary>
        /// Pauses the wav file streaming.
        /// </summary>
        public void PauseWavPlayback()
        {
            if (_wavPlayer == null)
                return;

            _wavPlayer.Pause();
        }

        /// <summary>
        /// Stops streaming the wav file.
        /// </summary>
        public void StopWavPlayback()
        {
            if (_wavPlayer == null)
                return;

            _wavPlayer.Stop();
        }

        #endregion

        #region Wav Record

        /// <summary>
        /// Loads a wav file for playback.
        /// </summary>
        public void LoadRecordWavFile(string filePath)
        {
            if (!_initialized)
                return;

            if (_wavRecorder != null)
            {
                _audioConnector.Disconnect(_phoneCallAudioReceiver, _wavRecorder);
                _audioConnector.Disconnect(AudioEnhancer, _wavRecorder);
                _wavRecorder.Dispose();
                _wavRecorder = null;
            }

            _wavRecorder = new WaveStreamRecorder(filePath);

            _audioConnector.Connect(_phoneCallAudioReceiver, _wavRecorder);
            _audioConnector.Connect(AudioEnhancer, _wavRecorder);
        }

        /// <summary>
        /// Starts recording the audio into a wav file.
        /// </summary>
        public void StartWavRecording()
        {
            if (_wavRecorder == null)
                return;

            _wavRecorder.Start();
        }

        /// <summary>
        /// Pauses the wav recording.
        /// </summary>
        public void PauseWavRecording()
        {
            if (_wavRecorder == null)
                return;

            _wavRecorder.Pause();
        }

        /// <summary>
        /// Stops wav recording.
        /// </summary>
        public void StopWavRecording()
        {
            if (_wavRecorder == null)
                return;

            _wavRecorder.Stop();
            _wavRecorder.Dispose();
        }

        #endregion

        #region MP3 Playback

        /// <summary>
        /// Loads an mp3 file for playback.
        /// </summary>
        public void LoadPlaybackMP3File(string filePath)
        {
            if (!_initialized)
                return;

            if (_mp3StreamPlayback != null)
            {
                _audioConnector.Disconnect(_mp3StreamPlayback, _phoneCallAudioSender);
                _mp3StreamPlayback.Dispose();
                _mp3StreamPlayback = null;
            }

            _mp3StreamPlayback = new MP3StreamPlayback(filePath, false, true);

            _audioConnector.Connect(_mp3StreamPlayback, _phoneCallAudioSender);
        }

        /// <summary>
        /// Starts recording the audio into a wav file.
        /// </summary>
        public void StartMP3Playback()
        {
            if (_mp3StreamPlayback == null)
                return;

            _mp3StreamPlayback.Start();
        }

        /// <summary>
        /// Pauses the wav recording.
        /// </summary>
        public void PauseMP3Playback()
        {
            if (_mp3StreamPlayback == null)
                return;

            _mp3StreamPlayback.Pause();
        }

        /// <summary>
        /// Stops wav recording.
        /// </summary>
        public void StopMP3Playback()
        {
            if (_mp3StreamPlayback == null)
                return;

            _mp3StreamPlayback.Stop();
        }

        #endregion

        #region Ringtones

        internal void SetRingtone(string filePath)
        {
            bool isStreaming = false;

            // dispose previous
            if (_ringtonePlayer != null)
            {
                isStreaming = _ringtonePlayer.IsStreaming;

                _audioConnector.Disconnect(_ringtonePlayer, Speaker);
                _ringtonePlayer.Dispose();
                _ringtonePlayer = null;
            }

            // create new
            _ringtonePlayer = new WaveStreamPlayback(filePath, true, true);

            _audioConnector.Connect(_ringtonePlayer, Speaker);

            // start if necessary
            if (isStreaming)
                _ringtonePlayer.Start();
        }

        public void StartRingtone()
        {
            if (_ringtonePlayer != null)
                _ringtonePlayer.Start();
        }

        public void StopRingtone()
        {
            if (_ringtonePlayer != null)
                _ringtonePlayer.Stop();
        }


        internal void SetRingback(string filePath)
        {
            bool isStreaming = false;

            // dispose previous
            if (_ringbackPlayer != null)
            {
                isStreaming = _ringbackPlayer.IsStreaming;
                _audioConnector.Disconnect(_ringbackPlayer, Speaker);
                _ringbackPlayer.Dispose();
                _ringbackPlayer = null;
            }

            // create new
            _ringbackPlayer = new WaveStreamPlayback(filePath, true, true);
            _audioConnector.Connect(_ringbackPlayer, Speaker);

            // start if necessary
            if (isStreaming)
                _ringbackPlayer.Start();
        }


        public void StartRingback()
        {
            if (_ringbackPlayer != null)
                _ringbackPlayer.Start();
        }

        public void StopRingback()
        {
            if (_ringbackPlayer != null)
                _ringbackPlayer.Stop();
        }

        #endregion

        #region Audio Control

        /// <summary>
        /// Attaches the media handlers to the given phone call.
        /// </summary>
        public void AttachAudio(IPhoneCall call)
        {
            AudioEnhancer.Refresh();
            AudioEnhancer.Start();
            _phoneCallAudioSender.AttachToCall(call);
            _phoneCallAudioReceiver.AttachToCall(call);
        }

        /// <summary>
        /// Detaches the media handlers from the phone call.
        /// </summary>
        public void DetachAudio()
        {
            _phoneCallAudioSender.Detach();
            _phoneCallAudioReceiver.Detach();
            AudioEnhancer.Stop();
        }

        #endregion

        /*#region Video Control

        / <summary>
        / Attaches the media handlers to the given phone call.
        / </summary>
        public void AttachVideo(IPhoneCall call)
        {
            _phoneCallVideoReceiver.AttachToCall(call);
            _phoneCallVideoSender.AttachToCall(call);
        }

        / <summary>
        / Detaches the media handlers from the phone call.
        / </summary>
        public void DetachVideo()
        {
            _phoneCallVideoReceiver.Detach();
            _phoneCallVideoSender.Detach();
        }

        / <summary>
        / Starts the video handlers.
        / </summary>
        public void StartVideo()
        {
            if (WebCamera != null)
                WebCamera.Start();
        }

        / <summary>
        / Stops the video handlers.
        / </summary>
        public void StopVideo()
        {
            if (WebCamera != null)
                WebCamera.Stop();
        }

        #endregion*/

        #region Dispose

        public void Dispose()
        {
            // audio
            if (_audioConnector != null)
                _audioConnector.Dispose();

            if (Microphone != null)
            {
                UnsubscribeFromMicrophoneEvents();
                Microphone.Dispose();
            }

            if (Speaker != null)
            {
                UnsubscribeFromSpeakerEvents();
                Speaker.Dispose();
            }

            if (_wavRecorder != null)
                _wavRecorder.Dispose();

            if (_wavPlayer != null)
                _wavPlayer.Dispose();

            if (_mp3StreamPlayback != null)
                _mp3StreamPlayback.Dispose();

            if (_ringtonePlayer != null)
                _ringtonePlayer.Dispose();

            if (_ringbackPlayer != null)
                _ringbackPlayer.Dispose();

            if (AudioEnhancer != null)
                AudioEnhancer.Dispose();

            if (_dtmfPlayer != null)
                _dtmfPlayer.Dispose();

            //if (outgoingDataMixer != null)
            //    outgoingDataMixer.Dispose();

            //if (speakerMixer != null)
            //    speakerMixer.Dispose();

            //if (recordDataMixer != null)
            //    recordDataMixer.Dispose();

            if (_phoneCallAudioSender != null)
                _phoneCallAudioSender.Dispose();

            if (_phoneCallAudioReceiver != null)
                _phoneCallAudioReceiver.Dispose();


            // video
            if (_videoConnector != null)
                _videoConnector.Dispose();

            if (WebCamera != null)
                WebCamera.Dispose();

            if (_phoneCallVideoReceiver != null)
                _phoneCallVideoReceiver.Dispose();

            if (_phoneCallVideoSender != null)
                _phoneCallVideoSender.Dispose();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}

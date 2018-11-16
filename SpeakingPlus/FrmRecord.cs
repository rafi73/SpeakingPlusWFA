using DirectShowLib;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpeakingPlus
{
    public partial class FrmRecord : Form
    {
        const int WM_DEVICECHANGE = 0x0219;
        // new device is pluggedin
        const int DBT_DEVICEARRIVAL = 0x8000;
        //device is removed 
        const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        //device is changed
        const int DBT_DEVNODES_CHANGED = 0x0007;

        private const string APP_DIRECORY = "C:\\Users\\";
        private const string APP_PATH = "\\AppData\\Speaking-Plus\\";
        private string appPath = "";
        private bool recordMode = false;

        private WaveIn waveInMicrophone = null;
        private WaveFileWriter waveFileWriterMicrophone;
        private WaveFileWriter waveFileWriterSoundCard;
        private WasapiLoopbackCapture wasApiLoopbackCapture;

        public FrmRecord()
        {
            
            InitializeComponent();
        }

        private void createDirectory()
        {
            appPath = APP_DIRECORY + Environment.UserName + APP_PATH + DateTime.Now.ToString("MM-dd-yyyy hh.mm.ss tt");

            try
            {
                if (!Directory.Exists(appPath))
                {
                    Directory.CreateDirectory(appPath);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Data);
            }
            Console.WriteLine(appPath);
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            if (recordMode)
            {
                stopRecord();
            }
            else
            {
                createDirectory();
                startRecord();
            }
        }

        private void startRecord()
        {

            if (checkMicrophone() && checkSpeaker())
            {
                recordMode = true;
                btnRecord.Text = "Stop";



                recordFromMicrophone();
                recordFromSoundCard();
            }
        }

        private void stopRecord()
        {
            recordMode = false;
            btnRecord.Text = "Record";

            stopRecordMicrophone();
            stopRecordFromSoundCard();
        }

        private bool checkMicrophone()
        {
            try
            {
                MMDeviceEnumerator mmDeviceEnumerator = new MMDeviceEnumerator();
                MMDevice defaultDevice = mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);

                if(defaultDevice.AudioEndpointVolume.MasterVolumeLevel == 0)
                {
                    MessageBox.Show("Microphone Unplugged!");
                    return false;
                }

                if (mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications).AudioEndpointVolume.Mute)
                {
                    MessageBox.Show("Your Default Microphone is in Mute position.Please Unmute it.");
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                if (exception.HResult == -2147023728)
                {
                    MessageBox.Show("No audio input device found");
                }
                return false;
            }
        }

        private bool checkSpeaker()
        {
            try
            {
                MMDeviceEnumerator mmDeviceEnumerator = new MMDeviceEnumerator();
                MMDevice defaultDevice = mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications);

                if (mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications).AudioEndpointVolume.Mute)
                {
                    MessageBox.Show("Your Default Speaker is in Mute position.Please Unmute it.");
                    return false;
                }
                return true;
            }
            catch (Exception exception)
            {
                if (exception.HResult == -2147023728)
                {
                    MessageBox.Show("No audio output device found");
                }
                return false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                if(recordMode)
                {
                    stopRecordFromSoundCard();
                    stopRecordMicrophone();
                    recordMode = false;
                }
                MessageBox.Show("Microphone is Unplugged, Recoeding has been stopped!");
            }
            base.WndProc(ref m);
        }

        private void recordFromMicrophone()
        {
            int deviceNumber = 0;
            waveInMicrophone = new WaveIn();
            waveInMicrophone.DeviceNumber = deviceNumber;
            waveInMicrophone.WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(deviceNumber).Channels);
            waveInMicrophone.DataAvailable += new EventHandler<WaveInEventArgs>(microphoneFileWriter);
            waveFileWriterMicrophone = new WaveFileWriter(appPath + "\\mic.wav", waveInMicrophone.WaveFormat);
            waveInMicrophone.StartRecording();
        }

        private void microphoneFileWriter(object sender, WaveInEventArgs e)
        {
            if (waveFileWriterMicrophone == null) return;
            waveFileWriterMicrophone.Write(e.Buffer, 0, e.BytesRecorded);
            waveFileWriterMicrophone.Flush();
        }

        private void recordFromSoundCard()
        {
            string outputFilePath = appPath + @"\scr.wav";
            wasApiLoopbackCapture = new WasapiLoopbackCapture();
            waveFileWriterSoundCard = new WaveFileWriter(outputFilePath, wasApiLoopbackCapture.WaveFormat); 

            wasApiLoopbackCapture.DataAvailable += (s, a) => 
            {
                waveFileWriterSoundCard.Write(a.Buffer, 0, a.BytesRecorded);
            };

            wasApiLoopbackCapture.RecordingStopped += (s, a) =>
            {
                waveFileWriterSoundCard.Dispose();
                waveFileWriterSoundCard = null;
                wasApiLoopbackCapture.Dispose();
            };
            wasApiLoopbackCapture.StartRecording();
        }

        private void stopRecordFromSoundCard()
        {
            wasApiLoopbackCapture.StopRecording();
            waveFileWriterSoundCard.Dispose();
            convertToMonoChannelAudio(appPath + "\\scr.wav", appPath + "\\scr-mono.wav");
        }
        
        private void stopRecordMicrophone()
        {
            waveInMicrophone.StopRecording();   
            waveInMicrophone.Dispose();
            waveFileWriterMicrophone.Dispose();

            convertToMonoChannelAudio(appPath + "\\mic.wav", appPath + "\\mic-mono.wav");
        }

        private void convertToMonoChannelAudio(string dualChanelAudio, string monoChannelAudio)
        {
            using (var inputReader = new AudioFileReader(dualChanelAudio))
            {
                var mono = new StereoToMonoSampleProvider(inputReader);
                mono.LeftVolume = 0.0f;
                mono.RightVolume = 1.0f;

                WaveFileWriter.CreateWaveFile16(monoChannelAudio, mono);
            }
            File.Delete(dualChanelAudio);
        }
    }
}

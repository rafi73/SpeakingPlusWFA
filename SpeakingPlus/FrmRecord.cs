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

        /* Audio Volume Meter */
        private static Random rnd = new Random();
        private static double audioValueMax = 0;
        private static double audioValueLast = 0;
        private static int audioCount = 0;
        private static int RATE = 44100;
        private static int BUFFER_SAMPLES = 1024;

        private const string APP_DIRECORY = "C:\\Users\\";
        private const string APP_PATH = "\\AppData\\Speaking-Plus\\";
        private string appPath = "";
        private bool recordMode = false;

        private WaveIn waveInMicrophone = null;
        private WaveFileWriter waveFileWriterMicrophone;
        private WaveFileWriter waveFileWriterSoundCard;
        private WasapiLoopbackCapture wasApiLoopbackCapture;

        int timerDuration = 0;
        

        public FrmRecord()
        {
            InitializeComponent();
            volumeMeter();
            timerVolumeMeter.Start();
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            
            if (recordMode)
            {
                timerForDisplay.Stop();
                timerDuration = 0;
                stopRecord();
                
            }
            else
            {
                
                createDirectory();
                startRecord();
                
            }
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

        private void startRecord()
        {

            if (checkSpeaker() && checkMicrophone())
            {
                recordMode = true;
                btnRecord.Text = "Stop";

                timerForDisplay.Start();
                recordFromSoundCard();
                recordFromMicrophone();
                
                
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
        private void stopRecord()
        {
            recordMode = false;
            btnRecord.Text = "Record";
            stopRecordFromSoundCard();
            stopRecordMicrophone();
            
            
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
        
        
        private void MicrophoneVolumeLevel(object sender, WaveInEventArgs args)
        {

            float max = 0;

            // interpret as 16 bit audio
            for (int index = 0; index < args.BytesRecorded; index += 2)
            {
                short sample = (short)((args.Buffer[index + 1] << 8) |
                                        args.Buffer[index + 0]);
                var sample32 = sample / 32768f; // to floating point
                if (sample32 < 0) sample32 = -sample32; // absolute value 
                if (sample32 > max) max = sample32; // is this the max value?
            }

            // calculate what fraction this peak is of previous peaks
            if (max > audioValueMax)
            {
                audioValueMax = (double)max;
            }
            audioValueLast = max;
            audioCount += 1;
        }
        private void volumeMeter()
        {
            WaveInEvent waveIn = new WaveInEvent();
            waveIn.DeviceNumber = 0;
            waveIn.WaveFormat = new NAudio.Wave.WaveFormat(RATE, 1);
            waveIn.DataAvailable += MicrophoneVolumeLevel;
            waveIn.BufferMilliseconds = (int)((double)BUFFER_SAMPLES / (double)RATE * 1000.0);
            waveIn.StartRecording();

        }
        

        private void timerVolumeMeter_Tick(object sender, EventArgs e)
        {
            double percentageOfVolume = audioValueLast / audioValueMax;
            pictureBoxVolumeMeterFront.Width = (int)(percentageOfVolume * pictureBoxVolumeMeterBack.Width);
        }
        
        private void timerForDisplay_Tick(object sender, EventArgs e)
        {
            timerDuration = timerDuration + 1;
            string time = "00:00:00";
            int hh = (int)timerDuration / 3600;
            int mm = (int)((timerDuration % 3600) / 60);
            int ss = (timerDuration % 3600) % 60;
            if (hh < 10)
            {
                time = "0" + hh.ToString();
            }
            else
            {
                time = hh.ToString();
            }
            time = time + ":";
            if (mm < 10)
            {
                time = time + "0" + mm.ToString();
            }
            else
            {
                time = time + mm.ToString();
            }
            time = time + ":";
            if (ss < 10)
            {
                time = time + "0" + ss.ToString();
            }
            else
            {
                time = time + ss.ToString();
            }
            textBoxTimer.Text = time;
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DEVICECHANGE)
            {
                if (recordMode)
                {
                    stopRecord();
                    timerForDisplay.Stop();
                    timerDuration = 0;
                    

                }             
                MessageBox.Show("Microphone is Unplugged, Recording has been stopped!");
                volumeMeter();
            }
            
            base.WndProc(ref m);
        }

    }
}

using System;
using System.IO;
using System.Net;
using System.Windows;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace RadioPlayer
{
    public partial class MainWindow : Window
    {
        private WaveOutEvent outputDevice;
        private Mp3FrameReader mp3Reader;

        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists("lasturl.txt"))
            {
                UrlBox.Text = File.ReadAllText("lasturl.txt");
            }
        }


        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (outputDevice != null)
                {
                    outputDevice.Stop();
                    outputDevice.Dispose();
                }

                string url = UrlBox.Text;

                var reader = new MediaFoundationReader(url);

                outputDevice = new WaveOutEvent();
                outputDevice.Init(reader);
                outputDevice.Play();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            File.WriteAllText("lasturl.txt", UrlBox.Text);

        }

        // Start radio playback

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            StopPlayback();
        }

        private void StopPlayback()
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            mp3Reader?.Dispose();
        }
    }

    // 🔥 вспомогательные классы для стрима
    public class Mp3FrameReader : IDisposable
    {
        private Stream stream;

        public Mp3FrameReader(Stream stream)
        {
            this.stream = stream;
        }

        public Mp3Frame ReadFrame()
        {
            return Mp3Frame.LoadFromStream(stream);
        }

        public void Dispose()
        {
            stream?.Dispose();
        }
    }

    public class Mp3FrameWaveProvider : IWaveProvider
    {
        private Mp3FrameReader reader;
        private WaveFormat format;

        public Mp3FrameWaveProvider(Mp3FrameReader reader)
        {
            this.reader = reader;
            this.format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
        }

        public WaveFormat WaveFormat => format;

        public int Read(byte[] buffer, int offset, int count)
        {
            return 0; // минимальная заглушка для запуска (упрощённый вариант)
        }
    }
}

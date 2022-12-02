using MahApps.Metro.Controls;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using UseMapEditor.Control;
using static Data.Map.MapData;

namespace UseMapEditor.Windows
{
    /// <summary>
    /// SoundImport.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SoundImport : MetroWindow
    {
        MapEditor mapEditor;
        WaveOut waveOut = new WaveOut();
        public class SoundList
        {
            public SoundData soundData;

            public string FileName { get; set; }
            public string Len { get; set; }
            public string Size { get; set; }
            public string Samplerate { get; set; }
            public string Bitrate { get; set; }
        }

        private List<SoundList> soundLists = new List<SoundList>();

        public SoundImport(List<SoundData> soundlists, MapEditor _mapEditor)
        {
            InitializeComponent();

            mapEditor = _mapEditor;
            Rename.Visibility = Visibility.Collapsed;


            foreach (SoundData item in soundlists)
            {
                AddList(item);
            }
        }
        public SoundImport(MapEditor _mapEditor)
        {
            InitializeComponent();

            mapEditor = _mapEditor;
            Rename.Visibility = Visibility.Collapsed;


        }

        private void AddList(SoundData soundData)
        {
            if(soundData == null)
            {
                return;
            }

            bool isrefresh;
            SoundList soundList;
            {
                SoundList fsoundList = soundLists.Find((x) => x.FileName == soundData.path);
                if(fsoundList != null)
                {
                    soundList = fsoundList;
                    isrefresh = true;
                }
                else
                {
                    soundList = new SoundList();
                    isrefresh = false;
                }
            }




            soundList.FileName = soundData.path;
            soundList.Size = soundData.bytes.Length / 1024 + "kb";
            soundList.soundData = soundData;

            WaveFormat waveFormat = null;

            long Samplerate = 0;
            long Bitrate = 0;

            try
            {
                if (System.IO.Path.GetExtension(soundData.path).ToLower() == ".wav")
                {
                    WaveFileReader waveFileReader = new WaveFileReader(new MemoryStream(soundData.bytes));
                    waveFormat = waveFileReader.WaveFormat;


                    soundList.Len = waveFileReader.TotalTime.ToString();
                }
                else if (System.IO.Path.GetExtension(soundData.path).ToLower() == ".ogg")
                {
                    NAudio.Vorbis.VorbisWaveReader vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(new MemoryStream(soundData.bytes));
                    waveFormat = vorbisWaveReader.WaveFormat;
                    soundList.Len = vorbisWaveReader.TotalTime.ToString();
                }
                Samplerate = waveFormat.SampleRate;
                Bitrate = Samplerate * waveFormat.BitsPerSample * waveFormat.Channels;
            }
            catch (Exception)
            {

            }
 




            soundList.Samplerate = Samplerate + "Hz";
            soundList.Bitrate = Bitrate / 1000 + "kb";




            if (!isrefresh)
            {
                soundLists.Add(soundList);
            }


            MainListbox.ItemsSource = null;
            MainListbox.ItemsSource = soundLists;
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "사운드파일|*.ogg;*.wav;*.mp3";

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string item in openFileDialog.FileNames)
                {
                    string chkname;

                    if(openFileDialog.FileNames.Length == 1)
                    {
                        OpenRenamePanel(item);
                    }
                    else
                    {
                        chkname = System.IO.Path.GetFileName(item);
                        LoadSound(item, chkname);
                    }
                }
            }
        }

        string fullpath;
        private void OpenRenamePanel(string item)
        {
            fullpath = item;
            NewName.Text = System.IO.Path.GetFileNameWithoutExtension(item);
            Rename.Visibility = Visibility.Visible;
        }



        public void LoadSound(string openfile, string chkname)
        {
            Process prcFFMPEG = new Process();
            ProcessStartInfo psiProcInfo = new ProcessStartInfo();
            psiProcInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "Data\\ffmpeg.exe";
            psiProcInfo.WindowStyle = ProcessWindowStyle.Hidden;


            prcFFMPEG.StartInfo = psiProcInfo;

            string lchar = char.ConvertFromUtf32(34);
            string output = UseMapEditor.Dialog.ProgramStart.tempfolder + "\\tempsound";


            psiProcInfo.Arguments = "-i " + lchar + openfile + lchar + " -ac 1  -y " + lchar + output + ".wav" + lchar;
            prcFFMPEG.Start();
            prcFFMPEG.WaitForExit();



            int samplerate = int.Parse(((ComboBoxItem)SampleCB.SelectedItem).Tag.ToString());



            // ====================================================================================================================================
            string sampleratestr = "-ar " + samplerate;


            psiProcInfo.Arguments = "-i " + lchar + output + ".wav" + lchar + " "  + sampleratestr + " -y " + lchar + output + "lower.ogg" + lchar;
            prcFFMPEG.Start();
            prcFFMPEG.WaitForExit();
            // ====================================================================================================================================


            AddList(mapEditor.mapdata.AddSound(System.IO.File.ReadAllBytes(output + "lower.ogg"), chkname));
            mapEditor.SetDirty();
        }


        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            string item = fullpath;
            string chkname = NewName.Text;
            LoadSound(item, chkname);
            Rename.Visibility = Visibility.Collapsed;
        }

        private void CloseBnt_Click(object sender, RoutedEventArgs e)
        {
            Rename.Visibility = Visibility.Collapsed;
        }

        private void CompBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (SoundList listitem in soundLists)
            {
                string tempfilepath = UseMapEditor.Dialog.ProgramStart.tempfolder + "\\tempsound" + System.IO.Path.GetExtension(listitem.FileName);

                File.WriteAllBytes(tempfilepath, listitem.soundData.bytes);

                LoadSound(tempfilepath, listitem.FileName);
            }
        }

        private void Playbtn_Click(object sender, RoutedEventArgs e)
        {
            SoundList soundList = (SoundList)MainListbox.SelectedItem;
            if(soundList == null)
            {
                return;
            }

            string filename = soundList.FileName;
            byte[] buffer = soundList.soundData.bytes;

            try
            {
                if (System.IO.Path.GetExtension(filename).ToLower() == ".wav")
                {
                    WaveFileReader waveFileReader = new WaveFileReader(new MemoryStream(buffer));
                    waveOut.Init(waveFileReader);
                    waveOut.Play();
                }
                else if (System.IO.Path.GetExtension(filename).ToLower() == ".ogg")
                {
                    NAudio.Vorbis.VorbisWaveReader vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(new MemoryStream(buffer));
                    waveOut.Init(vorbisWaveReader);
                    waveOut.Play();
                }
            }
            catch (Exception)
            {
                System.Media.SystemSounds.Hand.Play();
            }



        }

        private void Stopbtn_Click(object sender, RoutedEventArgs e)
        {
            waveOut.Pause();
        }
    }
}

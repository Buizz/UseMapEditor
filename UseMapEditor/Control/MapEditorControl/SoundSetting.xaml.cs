using MaterialDesignThemes.Wpf;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Data.Map.MapData;
using Microsoft.WindowsAPICodePack.Dialogs;
using UseMapEditor.Windows;

namespace UseMapEditor.Control.MapEditorControl
{
    /// <summary>
    /// SoundSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SoundSetting : UserControl
    {

        Casc.Data data = new Casc.Data();

        private MapEditor mapEditor;
        public void SetMapEditor(MapEditor _mapEditor)
        {
            mapEditor = _mapEditor;

            soundListItems = new List<SoundListItem>();
            RefreshListBox();
            CreateTreeview();
            MainListbox.Items.SortDescriptions.Add(new SortDescription("FileName",
            ListSortDirection.Ascending));
        }

        public void RefreshListBox()
        {
            MainListbox.ItemsSource = null;
            soundListItems.Clear();
            for (int i = 0; i < mapEditor.mapdata.WAV.Length; i++)
            {
                string d = mapEditor.mapdata.WAV[i].String;
                if (mapEditor.mapdata.WAV[i].IsLoaded)
                {
                    SoundListItem soundListItem = new SoundListItem();
                    soundListItem.FileName = d;

                    SoundData soundData = mapEditor.mapdata.soundDatas.Find((x) => x.path == d);
                    if(soundData == null)
                    {
                        soundListItem.Size = "외부파일";
                    }
                    else
                    {
                        long s = soundData.bytes.Length;
                        s /= 1024;

                        soundListItem.Size = s.ToString() + "kb";
                    }

                    soundListItems.Add(soundListItem);
                }
            }
            MainListbox.ItemsSource = soundListItems;
        }


        public void CreateTreeview()
        {
            MainTreeview.Items.Clear();
            for (int i = 0; i < Global.WindowTool.soundlist.Length; i++)
            {
                CreateTreeviewItem(Global.WindowTool.soundlist[i]);
            }
        }


        List<TreeViewItem> treeViews = new List<TreeViewItem>();
        private void CreateTreeviewItem(string path)
        {
            string[] paths = path.Split('/');

            ItemCollection itemCollection = MainTreeview.Items;
            for (int i = 0; i < paths.Length - 1; i++)
            {
                if (paths[i] == "")
                    continue;

                bool IsExist = false;
                foreach (TreeViewItem item in itemCollection)
                {
                    string header = (string)item.Tag;
                    if (header == paths[i])
                    {
                        IsExist = true;
                        itemCollection = item.Items;
                        break;
                    }
                }
                if (!IsExist)
                {
                    //존재하지 않을 경우 새로 만들기
                    TreeViewItem treeViewItem = new TreeViewItem();
                    treeViewItem.Header = paths[i];
                    treeViewItem.Tag = paths[i];

                    itemCollection.Add(treeViewItem);


                    itemCollection = treeViewItem.Items;
                }
            }


            {
                TreeViewItem treeViewItem = new TreeViewItem();


                treeViewItem.Header = paths.Last();


                treeViewItem.Tag = path;
                itemCollection.Add(treeViewItem);

                treeViews.Add(treeViewItem);
            }
        }



        public List<SoundListItem> soundListItems;
        public class SoundListItem
        {
            public string FileName { get; set; }
            public string Size { get; set; }
            public string Pos { get; set; }

        }





        public SoundSetting()
        {
            InitializeComponent();
        }

        private void BGMPlay(object sender, RoutedEventArgs e)
        {

        }
        WaveOut waveOut = new WaveOut();
        private void StarCraftSoundPlay()
        {
            TreeViewItem titem = (TreeViewItem)MainTreeview.SelectedItem;
            if (titem != null)
            {
                if (titem.Items.Count == 0)
                {
                    ComboBoxItem combo = (ComboBoxItem)SoundLan.SelectedItem;
                    string lanstr = (string)combo.Tag;
                    string filename = (string)titem.Tag;

                    //lanstr = lanstr.ToLower();
                    //filename = filename.ToLower();

                    byte[] buffer = data.ReadFile(lanstr + filename);

                    if (buffer.Length == 0)
                    {
                        buffer = data.ReadFile(filename);
                    }

                    if (buffer.Length == 0)
                    {
                        return;
                    }


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
            }
        }





        private void StarCraftSoundPlay_Click(object sender, RoutedEventArgs e)
        {
            StarCraftSoundPlay();
        }

        private void MapSoundSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = MapSoundSearchBox.Text;
            MainListbox.Items.Filter = delegate (object obj)
            {
                SoundListItem unitdata = (SoundListItem)obj;
                string str = unitdata.FileName;
                if (String.IsNullOrEmpty(str)) return false;
                int index = str.IndexOf(searchText, 0);

                return (index > -1);
            };
        }
        private bool TreeviewFliter(ItemCollection items, string searchText)
        {
            bool returnbool = false;
            for (int i = 0; i < items.Count; i++)
            {
                TreeViewItem titem = (TreeViewItem)items[i];

                if (titem.Items.Count == 0)
                {
                    //마지막 아이템일 경우
                    if (titem.Visibility == Visibility.Visible)
                    {
                        //아이템이 보일 경우
                        return true;
                    }
                }
                else
                {
                    bool rbool = TreeviewFliter(titem.Items, searchText);

                    if (rbool)
                    {
                        //안에 보이는 아이템이 하나 이상 있다는 뜻
                        titem.Visibility = Visibility.Visible;
                        if (searchText == "")
                        {
                            titem.IsExpanded = false;
                        }
                        else
                        {
                            titem.IsExpanded = true;
                        }
                        returnbool = true;
                    }
                    else
                    {
                        titem.Visibility = Visibility.Collapsed;
                        titem.IsExpanded = false;
                    }


                }
            }
            return returnbool;
        }
        private void StarSoundSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = StarSoundSearchBox.Text;


            for (int i = 0; i < treeViews.Count; i++)
            {
                TreeViewItem list = treeViews[i];
                string str = list.Tag.ToString();
                

                if (!String.IsNullOrEmpty(str))
                {
                    int index = str.IndexOf(searchText, 0);

                    if (index > -1)
                    {
                        //보이게하기
                        list.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //안보이게하기
                        list.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    //안보이게하기
                    list.Visibility = Visibility.Collapsed;
                }
            }


            TreeviewFliter(MainTreeview.Items, searchText);
        }


        private void StarCraftSoundImport_Click(object sender, RoutedEventArgs e)
        {
            TreeViewItem titem = (TreeViewItem)MainTreeview.SelectedItem;
            if (titem != null)
            {
                if (titem.Items.Count == 0)
                {
                    ComboBoxItem combo = (ComboBoxItem)SoundLan.SelectedItem;
                    string lanstr = (string)combo.Tag;
                    string filename = (string)titem.Tag;

                    string lastname;

                    //lanstr = lanstr.ToLower();
                    //filename = filename.ToLower();
                    lastname = lanstr + filename;
                    byte[] buffer = data.ReadFile(lastname);

                    if (buffer.Length == 0)
                    {
                        lastname = filename;
                        buffer = data.ReadFile(filename);
                    }

                    if (buffer.Length == 0)
                    {
                        return;
                    }


                    lastname = lastname.Replace("/", "\\");

                    for (int i = 0; i < mapEditor.mapdata.WAV.Length; i++)
                    {
                        string d = mapEditor.mapdata.WAV[i].String;
                        if (mapEditor.mapdata.WAV[i].IsLoaded)
                        {
                            if(lastname == d)
                            {
                                return;
                            }
                        }
                    }


                    for (int i = 0; i < mapEditor.mapdata.WAV.Length; i++)
                    {
                        string d = mapEditor.mapdata.WAV[i].String;
                        if (!mapEditor.mapdata.WAV[i].IsLoaded)
                        {
                            mapEditor.mapdata.WAV[i].String = lastname;

                            SoundListItem soundListItem = new SoundListItem();
                            soundListItem.FileName = lastname;
                            soundListItem.Size = "외부파일";

                            soundListItems.Add(soundListItem);
                            MainListbox.ItemsSource = null;
                            MainListbox.ItemsSource = soundListItems;
                            break;
                        }
                    }


                }
            }
            mapEditor.SetDirty();
        }


        private void SoundStop_Click(object sender, RoutedEventArgs e)
        {
            waveOut.Pause();
        }

        private void MainTreeview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            StarCraftSoundPlay();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            List<SoundListItem> soundlists = new List<SoundListItem>();

            foreach (SoundListItem item in MainListbox.SelectedItems)
            {
                soundlists.Add(item);
            }



            foreach (SoundListItem item in soundlists)
            {
                string soundname = item.FileName;

                DeleteItem(soundname);
            }
            mapEditor.SetDirty();
        }
        private void DeleteItem(string soundname)
        {
            if (mapEditor.mapdata.DeleteSound(soundname))
            {
                SoundListItem soundListItem = soundListItems.Find((x) => x.FileName == soundname);
                soundListItems.Remove(soundListItem);
                MainListbox.ItemsSource = null;
                MainListbox.ItemsSource = soundListItems;
            }
        }



        private void MapSoundPlay()
        {
            SoundListItem soundListItems = (SoundListItem)MainListbox.SelectedItem;
            if (soundListItems != null)
            {
                string soundname = soundListItems.FileName;


                SoundData soundData = mapEditor.mapdata.soundDatas.Find((x) => x.path == soundname);
                if (soundData == null)
                {
                    byte[] buffer = data.ReadFile(soundname);


                    if (System.IO.Path.GetExtension(soundname).ToLower() == ".wav")
                    {
                        WaveFileReader waveFileReader = new WaveFileReader(new MemoryStream(buffer));
                        waveOut.Init(waveFileReader);
                        waveOut.Play();
                    }
                    else if (System.IO.Path.GetExtension(soundname).ToLower() == ".ogg")
                    {
                        NAudio.Vorbis.VorbisWaveReader vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(new MemoryStream(buffer));
                        waveOut.Init(vorbisWaveReader);
                        waveOut.Play();
                    }
                }
                else
                {
                    if (System.IO.Path.GetExtension(soundname).ToLower() == ".wav")
                    {
                        WaveFileReader waveFileReader = new WaveFileReader(new MemoryStream(soundData.bytes));
                        waveOut.Init(waveFileReader);
                        waveOut.Play();
                    }
                    else if (System.IO.Path.GetExtension(soundname).ToLower() == ".ogg")
                    {
                        NAudio.Vorbis.VorbisWaveReader vorbisWaveReader = new NAudio.Vorbis.VorbisWaveReader(new MemoryStream(soundData.bytes));
                        waveOut.Init(vorbisWaveReader);
                        waveOut.Play();
                    }
                }
            }

        }
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            MapSoundPlay();
        }

        private void MainListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MapSoundPlay();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if(MainListbox.SelectedItems.Count == 0)
            {
                return;
            }

            // CommonOpenFileDialog 클래스 생성
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            // 처음 보여줄 폴더 설정(안해도 됨)
            //dialog.InitialDirectory = "";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }


            foreach (SoundListItem item in MainListbox.SelectedItems)
            {
                string soundname = item.FileName;

                byte[] buffer;
                SoundData soundData = mapEditor.mapdata.soundDatas.Find((x) => x.path == soundname);
                if (soundData == null)
                {
                    buffer = data.ReadFile(soundname);
                }
                else
                {
                    buffer = soundData.bytes;
                }

                File.WriteAllBytes(dialog.FileName + "\\" + System.IO.Path.GetFileName(soundname), buffer);
            }
        }

        private void OpenSound_Click(object sender, RoutedEventArgs e)
        {
            List<SoundData> soundlists = new List<SoundData>();
            SoundImport soundImport = new SoundImport(soundlists, mapEditor);
            soundImport.ShowDialog();
            RefreshListBox();
        }

        private void Compression_Click(object sender, RoutedEventArgs e)
        {
            List<SoundData> soundlists = new List<SoundData>();

            foreach (SoundListItem item in MainListbox.SelectedItems)
            {
                string soundname = item.FileName;

                SoundData soundData = mapEditor.mapdata.soundDatas.Find((x) => x.path == soundname);
                if(soundData != null)
                {
                    soundlists.Add(soundData);
                }
            }
            if(soundlists.Count != 0)
            {
                SoundImport soundImport = new SoundImport(soundlists, mapEditor);
                soundImport.ShowDialog();
                RefreshListBox();
            }

        }
    }
}

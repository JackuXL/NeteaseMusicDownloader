using System;
using System.IO;
using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        private async void ShowMessageBox(string title,string content)
        {
            var dialog = new MessageDialog(content, title);

            dialog.Commands.Add(new UICommand("确定", cmd => { }, commandId: 0));

            //设置默认按钮，不设置的话默认的确认按钮是第一个按钮
            dialog.DefaultCommandIndex = 0;

            //获取返回值
            var result = await dialog.ShowAsync();
        }
        public async void HttpDownloadFile(Windows.Storage.StorageFolder folder,string name)
        {
            String url = "http://music.163.com/song/media/outer/url?id=" + tb.Text;
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            
            //创建本地文件写入流
            try
            {
                Windows.Storage.StorageFile f = await folder.CreateFileAsync(name+".mp3", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                Stream stream = await f.OpenStreamForWriteAsync();
                byte[] bArr = new byte[1024];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                while (size > 0)
                {
                    stream.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                }
                stream.Close();
                responseStream.Close();
                ShowMessageBox("下载成功", "已存储" + name + ".mp3" + "至" + folder.Path);
            }
            catch(Exception e)
            {
                ShowMessageBox("拒绝访问",e.Message);
            }
       
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            folderPicker.FileTypeFilter.Add("*");
            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                Windows.Storage.AccessCache.StorageApplicationPermissions.
                FutureAccessList.AddOrReplace("PickedFolderToken", folder);
                HttpDownloadFile(folder,tb.Text);

            }

        }
    }
}

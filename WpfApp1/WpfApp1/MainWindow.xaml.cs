using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using System.IO;
using WpfApp1.Helper;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            AesModels = new ObservableCollection<AesModel>();
            try
            {
                AesModels = AesHelper.GetAesKeys();
            }
            catch (Exception)
            {
                MessageBox.Show("Some Keys in source File are Invalide", "Warning");
            }
        }

        public ObservableCollection<AesModel> AesModels { get; set; }
        Dictionary<string, NamedKey> dictPrivate;

        Dictionary<string, NamedKey> dictPublic;

        private static string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string destinationPrivate = System.IO.Path.Combine(currentDirectory, @"..\..\Keys\RSA\Private");
        private static string destinationPublic = System.IO.Path.Combine(currentDirectory, @"..\..\Keys\RSA\Public");
        private static string pathPrivateKey = System.IO.Path.GetFullPath(destinationPrivate);
        private static string pathPublicKey = System.IO.Path.GetFullPath(destinationPublic);
       

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            fillDitionaries();
            refreshListKeys(LstPrivateKeys, dictPrivate);
            refreshListKeys(LstPublicKeys, dictPublic);
        }

        #region Kyes Generator
        private void BtnAesSkeyGenerator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string keyName = TxtkeyGenerator.Text;
                if (!string.IsNullOrEmpty(keyName))
                {
                    using (Aes myAes = Aes.Create())
                    {
                        AesModel model = new AesModel()
                        {
                            KeyName = keyName,
                            Key = myAes.Key,
                            IV = myAes.IV
                        };
                        AesModels.Add(model);
                        AesHelper.AddKeyToMemory(model);
                        MessageBox.Show("Succes Creation OF AES Key", "Notification");
                        TxtkeyGenerator.Text = string.Empty;
                    }
                    MessageBox.Show("Keys generated", "success");
                }
                else
                {
                    MessageBox.Show("Please Enter Key Name", "Notification");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong", "error");
            }
        }
        #endregion
        #region Aes Encrypt
        private void BtnAeEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CboxEncreptAesKeys.SelectedIndex > -1 && !string.IsNullOrEmpty(TxtPlainAes.Text))
                {
                    AesModel model = CboxEncreptAesKeys.SelectedItem as AesModel;
                    byte[] encrypted = AesHelper.EncryptStringToBytes_Aes(TxtPlainAes.Text, model.Key, model.IV);
                    TxtBlockEncreptedAes.Text = Convert.ToBase64String(encrypted);

                    SpEncreptedAes.Visibility = Visibility.Visible;
                }
                else if (string.IsNullOrEmpty(TxtPlainAes.Text))
                {
                    MessageBox.Show("Give Plain Text", "Notification");
                }
                else
                {
                    MessageBox.Show("Select An Encrypt A Sleutel", "Notification");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Encrypt Key");
                SpEncreptedAes.Visibility = Visibility.Hidden;
                TxtBlockEncreptedAes.Text = string.Empty;
            }
        }
        private void BtnAesEncryptFieText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CboxEncreptAesKeys.SelectedIndex > -1)
                {
                    AesModel model = CboxEncreptAesKeys.SelectedItem as AesModel;

                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    openFileDialog.Filter = "txt files (*.txt)|*.txt";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.Title = "Select File To Encrypt";
                    openFileDialog.RestoreDirectory = true;

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                    saveFileDialog.Title = "Save Encrypted File";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        if (File.Exists(openFileDialog.FileName))
                        {
                            try
                            {
                                string plaiTxt = File.ReadAllText(openFileDialog.FileName);
                                byte[] encrypted = AesHelper.EncryptStringToBytes_Aes(plaiTxt, model.Key, model.IV);

                                MessageBox.Show("Succes Encryption", "Notification");

                                if (saveFileDialog.ShowDialog() == true)
                                {
                                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                                    {
                                        writer.Write(Convert.ToBase64String(encrypted));
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Nothing is saved", "Notification");
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Invalid Encrypt Key");
                            }
                        }
                        else
                        {
                            MessageBox.Show("File Path is not correct", "Notification");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No File Was Selected", "Notification");
                    }
                }
                else
                {
                    MessageBox.Show("Select An Encrypt A Sleutel", "Notification");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong", "error");
            }
        }
        #endregion
        #region Aes Decrypt
        private void BtnAesDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CboxDecryptAesKeys.SelectedIndex > -1 && !string.IsNullOrEmpty(TxtCipherTextAes.Text))
                {
                    AesModel model = CboxDecryptAesKeys.SelectedItem as AesModel;
                    byte[] encrypted = Convert.FromBase64String(TxtCipherTextAes.Text);
                    string decrypted = AesHelper.DecryptStringFromBytes_Aes(encrypted, model.Key, model.IV);
                    TxtBlockDecryptedAes.Text = decrypted;

                    SpDecryptedAes.Visibility = Visibility.Visible;
                }
                else if(string.IsNullOrEmpty(TxtCipherTextAes.Text))
                {
                    MessageBox.Show("Give Cipher Text", "Notification");
                }
                else
                {
                    MessageBox.Show("Select An Decrypt A Sleutel", "Notification");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid Decrypt Key");
                SpDecryptedAes.Visibility = Visibility.Hidden;
                TxtBlockDecryptedAes.Text = string.Empty;
            }
        }
        private void BtnAesDecryptTextFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CboxDecryptAesKeys.SelectedIndex > -1)
                {
                    AesModel model = CboxDecryptAesKeys.SelectedItem as AesModel;

                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    openFileDialog.Filter = "txt files (*.txt)|*.txt";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.Title = "Select File To Decrypt";
                    openFileDialog.RestoreDirectory = true;

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                    saveFileDialog.Title = "Save Decrypted File";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        if (File.Exists(openFileDialog.FileName))
                        {
                            byte[] encrypted = Convert.FromBase64String(File.ReadAllText(openFileDialog.FileName));
                            try
                            {
                                string decrypted = AesHelper.DecryptStringFromBytes_Aes(encrypted, model.Key, model.IV);

                                MessageBox.Show("Succes Decryption", "Notification");

                                if (saveFileDialog.ShowDialog() == true)
                                {
                                    using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                                    {
                                        writer.Write(decrypted);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Nothing is saved", "Notification");
                                }
                            }
                            catch (Exception)
                            {
                                MessageBox.Show("Invalid Decrypt Key");
                            }
                        }
                        else
                        {
                            MessageBox.Show("File Path is not correct", "Notification");
                        }
                    }
                    else
                    {
                        MessageBox.Show("No File Was Selected", "Notification");
                    }
                }
                else
                {
                    MessageBox.Show("Select An Decrypt A Sleutel", "Notification");
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong", "error");
            }
        }
        #endregion
            
        


        private void fillDitionaries()
        {
            dictPublic = new Dictionary<string, NamedKey>();
            DirectoryInfo di = new DirectoryInfo(pathPublicKey);
            foreach (var item in di.GetFiles("*.xml"))
            {
                XmlSerializer reader = new XmlSerializer(typeof(NamedKey));
                StreamReader file = new System.IO.StreamReader($@"{pathPublicKey}\{item}");
                NamedKey namedKey = (NamedKey)reader.Deserialize(file);
                dictPublic.Add(namedKey.Name, namedKey);
                file.Close();
            }

            dictPrivate = new Dictionary<string, NamedKey>();
            di = new DirectoryInfo(pathPrivateKey);
            foreach (var item in di.GetFiles("*.xml"))
            {
                XmlSerializer reader = new XmlSerializer(typeof(NamedKey));
                StreamReader file = new System.IO.StreamReader($@"{pathPrivateKey}\{item}");
                NamedKey namedKey = (NamedKey)reader.Deserialize(file);
                dictPrivate.Add(namedKey.Name, namedKey);
                file.Close();
            }
        }


        private void refreshListKeys(ListBox listBox, Dictionary<string, NamedKey> dict)
        {
            listBox.Items.Clear();
            foreach (KeyValuePair<string, NamedKey> kvp in dict)
            {
                ListBoxItem lbi = new ListBoxItem();
                lbi.Content = kvp.Key;
                listBox.Items.Add(lbi);
            }
        }

        private void BtnRsaSleutels_Click(object sender, RoutedEventArgs e)
        {
            if (TxtkeyGenerator.Text != string.Empty)
            {
                var keyLength = 384;
                var dialog = new Dialog();
                if (dialog.ShowDialog() == true)
                {
                    keyLength = Convert.ToInt32(dialog.ResponseText);
                    try
                    {
                        RSACryptoServiceProvider csp = new RSACryptoServiceProvider(keyLength);
                        string privateKey = csp.ToXmlString(true);
                        var namedKey = new NamedKey() { Key = privateKey, Name = TxtkeyGenerator.Text + " (Private) " };
                        dictPrivate.Add(namedKey.Name, namedKey);
                        refreshListKeys(LstPrivateKeys, dictPrivate);
                        saveKey(namedKey, true);
                        string publicKey = csp.ToXmlString(false);
                        namedKey = new NamedKey() { Key = publicKey, Name = TxtkeyGenerator.Text + " (public) " };
                        dictPublic.Add(namedKey.Name, namedKey);
                        refreshListKeys(LstPublicKeys, dictPublic);
                        saveKey(namedKey, false);
                        MessageBox.Show("Keys generated", "success");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.Replace("sleutel", "naam"), "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please Enter Key Name", "Error");
            }
            
        }

       
        private void saveKey(NamedKey key, bool isPrivate)
        {
            var serializer = new XmlSerializer(typeof(NamedKey));
            StreamWriter writer;
            if (isPrivate)
            {
                writer = new StreamWriter($@"{pathPrivateKey}\{TxtkeyGenerator.Text}.xml");
            }
            else
            {
                writer = new StreamWriter($@"{pathPublicKey}\{TxtkeyGenerator.Text}.xml");
            }
            serializer.Serialize(writer, key);
            writer.Close();
        }

        private void LstPrivateKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstPrivateKeys.SelectedIndex >= 0)
            {
                var lbi = LstPrivateKeys.SelectedItem as ListBoxItem;
                TxtChosenKey.Text = lbi.Content.ToString();
                LstPublicKeys.UnselectAll();
            }
        }


        private void LstPublicKeys_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstPublicKeys.SelectedIndex >= 0)
            {
                var lbi = LstPublicKeys.SelectedItem as ListBoxItem;
                TxtChosenKey.Text = lbi.Content.ToString();
                LstPrivateKeys.UnselectAll();
            }

        }


        private void BtnEncrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                csp.FromXmlString(dictPublic[$"{TxtChosenKey.Text}"].Key);
                string plainText = TxtPlain.Text;
                byte[] plainTextBytes = System.Text.Encoding.Unicode.GetBytes(plainText);
                byte[] cypherTextBytes = csp.Encrypt(plainTextBytes, false);
                string cypherText = Convert.ToBase64String(cypherTextBytes);
                TxtCypher.Text = cypherText;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Can not encrypt!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
           
        }


        private void BtnDecrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                csp.FromXmlString(dictPrivate[TxtChosenKey.Text].Key);
                byte[] cypherTextBytes = Convert.FromBase64String(TxtCypher.Text);
                byte[] plainTextBytes = csp.Decrypt(cypherTextBytes, false);
                string plainText = System.Text.Encoding.Unicode.GetString(plainTextBytes);
                TxtPlain.Text = plainText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Can not decrypt!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        private void BtnSaveCypher_Click(object sender, RoutedEventArgs e)
        {
            saveToTextFile(TxtCypher.Text, TxtChosenKey.Text);   
        }

        private void BtnSavePlain_Click(object sender, RoutedEventArgs e)
        {
            saveToTextFile(TxtPlain.Text, TxtChosenKey.Text);
        }


        private void saveToTextFile(string tekst, string keyName)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                InitialDirectory = @"c:\",
                OverwritePrompt = true,
                AddExtension = true,
                CreatePrompt = true
            };
            if (sfd.ShowDialog() == true)
            {
                FileStream fs = new FileStream($"{sfd.FileName} ({keyName}).txt", FileMode.Create, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(tekst);
                }
            }
        }

        private void BtnLoadCypher_Click(object sender, RoutedEventArgs e)
        {
            TxtCypher.Text = getTextFromFile();
        }

        private void BtnLoadPlain_Click(object sender, RoutedEventArgs e)
        {
            TxtPlain.Text = getTextFromFile();
        }

        private string getTextFromFile()
        {
            string text = "";

            OpenFileDialog ofd = new OpenFileDialog()
            {
                InitialDirectory = @"c:\"
            };
            if (ofd.ShowDialog() == true)
            {
                var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read);
                using (StreamReader sr = new StreamReader(fs))
                {
                   text = sr.ReadToEnd();
                }
            }
            return text;
        }

        private void BtnClearPlain_Click(object sender, RoutedEventArgs e)
        {
            TxtPlain.Clear();
        }

        private void BtnClearCypher_Click(object sender, RoutedEventArgs e)
        {
            TxtCypher.Clear();
        }

       

        //private void BtnPublicFromPrivate_Click(object sender, RoutedEventArgs e)
        //{
        //    RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
        //    csp.FromXmlString(dictPrivate[$"{TxtChosenKey.Text}"].Key);
        //    var key = csp.ToXmlString(false);
        //    NamedKey namedKey = new NamedKey();
        //    if (!dictPublic.ContainsKey(TxtChosenKey.Text))
        //    {
        //        namedKey.Key = key; 
        //        namedKey.Name = TxtChosenKey.Text;
        //    }
        //    else
        //    {
        //        namedKey.Key = key;
        //        namedKey.Name = $"{TxtChosenKey.Text} (1)";
        //    }
        //    dictPublic.Add(namedKey.Name, namedKey);
        //    refreshListKeys(LstPublicKeys, dictPublic);
        //}

        //private void BtnPrivateFromPrivate_Click(object sender, RoutedEventArgs e)
        //{
        //    RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
        //    csp.FromXmlString(dictPrivate[$"{TxtChosenKey.Text}"].Key);
        //    var key = csp.ToXmlString(true);
        //    NamedKey namedKey = new NamedKey();
        //    if (!dictPrivate.ContainsKey(TxtChosenKey.Text))
        //    {
        //        namedKey.Key = key;
        //        namedKey.Name = TxtChosenKey.Text;
        //    }
        //    else
        //    {
        //        namedKey.Key = key;
        //        namedKey.Name = $"{TxtChosenKey.Text} (1)";
        //    }
        //    dictPrivate.Add(namedKey.Name, namedKey);
        //    refreshListKeys(LstPrivateKeys, dictPrivate);
        //}

    }
}

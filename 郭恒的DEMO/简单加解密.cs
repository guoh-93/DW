using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.IO;
namespace 郭恒的DEMO
{
    public partial class 简单加解密 : Form
    {
     

        public 简单加解密()
        {
            InitializeComponent();
        }

        private void 简单加解密_Load(object sender, EventArgs e)
        {

        }


        #region 加密
        /// <summary>
        /// 直接就ascII码加三
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string Encrypt(string s)
        {
            Encoding ascii = Encoding.ASCII;//实例化。
            string EncryptString = "";//定义。
            for (int i = 0; i < s.Length; i++)//遍历。
            {
                int j;
                byte[] b = new byte[1];
                j = Convert.ToInt32(ascii.GetBytes(s[i].ToString())[0]);//获取字符的ASCII。
                j = j + 3; 
                b[0] = Convert.ToByte(j);//转换为八位无符号整数。
                EncryptString = EncryptString + ascii.GetString(b);//显示。

            }
            return EncryptString;

            

        }
        #endregion
        //解密
        #region 解密
        public string Decryptor(string s)
        {
            Encoding ascii = Encoding.ASCII;
            string DecryptorString = ""; 
            for (int i = 0; i < s.Length; i++) 
            {
                int j;
                byte[] b = new byte[1];
                j = Convert.ToInt32(ascii.GetBytes(s[i].ToString())[0]); 
                j = j - 3;
                b[0] = Convert.ToByte(j);
                DecryptorString = DecryptorString + ascii.GetString(b); 

            }
            return DecryptorString;
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string  Encrypt_md5(string s)
        {
            string ss = "";
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] palindata = Encoding.Default.GetBytes(s);//将要加密的字符串转换为字节数组
            byte[] encryptdata = md5.ComputeHash(palindata);//将字符串加密后也转换为字符数组
            ss = Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为加密字符串
            return ss;
        }

        /// <summary>
        /// RSA加密
        /// </summary>
        /// <param name="express"></param>
        /// <returns></returns>
        //加密 加密不能超过117位
        private string Encryption(string express)
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = "oa_erp_dowork";//密匙容器的名称，保持加密解密一致才能解密成功
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] plaindata = Encoding.Default.GetBytes(express);//将要加密的字符串转换为字节数组
                byte[] encryptdata = rsa.Encrypt(plaindata, false);//将加密后的字节数据转换为新的加密字节数组
                return Convert.ToBase64String(encryptdata);//将加密后的字节数组转换为字符串
            }
        }

        //解密密文 不能超过 128 位
        private string Decrypt(string ciphertext)
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = "oa_erp_dowork";
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] encryptdata = Convert.FromBase64String(ciphertext);
                byte[] decryptdata = rsa.Decrypt(encryptdata, false);
                return Encoding.Default.GetString(decryptdata);
            }
        }




        #region DES加解密

        //先定义一个全局的字节数组和实例化一个全局的DESCryptoServiceProvider对象
   
        DESCryptoServiceProvider DesCSP = new DESCryptoServiceProvider();
        // <summary>  
        /// C# DES解密方法  
        /// </summary>  
        /// <param name="encryptedValue">待解密的字符串</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <returns>解密后的字符串</returns>  
        public static string DESDecrypt(string encryptedValue, string key, string iv)
        {
            using (DESCryptoServiceProvider sa =
                new DESCryptoServiceProvider
                { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
            {
                using (ICryptoTransform ct = sa.CreateDecryptor())
                {
                    byte[] byt = Convert.FromBase64String(encryptedValue);

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, ct, CryptoStreamMode.Write))
                        {
                            cs.Write(byt, 0, byt.Length);
                            cs.FlushFinalBlock();
                        }
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
        /// <summary>  
        /// C# DES加密方法  
        /// </summary>  
        /// <param name="encryptedValue">要加密的字符串</param>  
        /// <param name="key">密钥</param>  
        /// <param name="iv">向量</param>  
        /// <returns>加密后的字符串</returns>  
        public static string DESEncrypt(string originalValue, string key, string iv)
        {
            using (DESCryptoServiceProvider sa
                = new DESCryptoServiceProvider { Key = Encoding.UTF8.GetBytes(key), IV = Encoding.UTF8.GetBytes(iv) })
            {
                using (ICryptoTransform ct = sa.CreateEncryptor())
                {
                    byte[] by = Encoding.UTF8.GetBytes(originalValue);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, ct,
                                                         CryptoStreamMode.Write))
                        {
                            cs.Write(by, 0, by.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }


        #endregion



        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = DESEncrypt(textBox2.Text.Trim(),"s","s");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        
         
        }

        private void button2_Click(object sender, EventArgs e)
        {
          
            try
            {
                textBox1.Text = DESDecrypt(textBox2.Text.Trim(),"s","s");
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
    }

}

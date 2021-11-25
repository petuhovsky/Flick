using System;
using System.Security.Cryptography;
using System.Text;

namespace Flick
{
    public class ViewImage
    {
        public string id => calcId(previewURL);
        public string title { get; set; }
        public string tags { get; set; }
        public string previewURL { get; set; }
        public string pageURL { get; set; }

        private string calcId(string iUrl)
        {
            MD5 md5 = MD5.Create();

            byte[] inputBytes = Encoding.UTF8.GetBytes(iUrl);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Convert the byte array to hexadecimal string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }
            return sb.ToString();
        }

        internal ViewImage setPreviewURLToCached(string previewURL)
        {
            ViewImage clone = (ViewImage)MemberwiseClone();
            clone.previewURL = previewURL;
            return clone;
        }
    }
}

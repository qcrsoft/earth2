using System;
using System.Text;

namespace Earth.Library
{
    class Block
    {
        private StringBuilder buffer = new StringBuilder();

        public string Html
        {
            get
            {
                return buffer.ToString();
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string Text
        {
            get;
            private set;
        }

        /// <summary>
        /// 归属
        /// </summary>
        public StringBuilder StringBuilder
        {
            get;
            set;
        }

        public string InnerText
        {
            get;
            private set;
        }

        public void Append(string text)
        {
            buffer.Append(text);
        }

        public Block(string name, StringBuilder sb)
        {
            this.Name = name;

            string beginTag = "<!-- start " + name + " -->";
            string endTag = "<!-- end " + name + " -->";

            //开始、结束标签的起始位置
            int startIndex = sb.IndexOf(beginTag);
            if (startIndex == -1)
            {
                beginTag = beginTag.Replace("<!-- start", "<!--start");
                startIndex = sb.IndexOf(beginTag);
            }

            int endIndex = sb.IndexOf(endTag);
            if (endIndex == -1)
            {
                endTag = endTag.Replace("<!-- end", "<!--end");
                endIndex = sb.IndexOf(endTag);
            }

            if (startIndex == -1 || endIndex == -1 || endIndex < startIndex)
            {
                string message = "区块" + name + "不存在或不完整";
                throw new Exception(message);
            }

            int length = endIndex - startIndex + endTag.Length;
            this.Text = sb.Substring(startIndex, length);

            length = endIndex - startIndex - beginTag.Length;
            this.InnerText = sb.Substring(startIndex + beginTag.Length, length);

            this.StringBuilder = new StringBuilder(this.InnerText);
        }


        public Block(StringBuilder sb)
        {
            this.StringBuilder = sb;

            this.Text = sb.ToString();
            this.InnerText = this.Text;
        }
    }
}
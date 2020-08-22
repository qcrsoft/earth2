using System.IO;
namespace Earth.Library
{

    /// <summary>
    /// 模板文件
    /// </summary>
    class TemplateFile
    {
        #region 属性
        /// <summary>
        /// 文件名
        /// </summary>
        internal string Filename
        {
            get;
            set;
        }

        private string _text;

        /// <summary>
        /// 文件内容
        /// </summary>
        public string Text
        {
            get
            {
                if (_text == null)
                {
                    //转换成屋里路径，如有必要（2020.1.1)
                    string fullpath;
                    if (Filename.Contains("\\"))
                    {
                        fullpath = this.Filename;
                    }
                    else
                    {
                        fullpath = "";//!@#$ System.Web.HttpContext.Current.Server.MapPath(Filename);
                    }
                    string pPath = System.IO.Path.GetDirectoryName(fullpath);
                    string filename = System.IO.Path.GetFileName(fullpath);
                    _text = System.IO.File.ReadAllText(fullpath);
                    FileSystemWatcher fsw = new FileSystemWatcher(pPath, filename);
                    fsw.Changed += new FileSystemEventHandler(fsw_Changed);
                    fsw.EnableRaisingEvents = true;
                }
                return _text;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造 Tempate 实例
        /// </summary>
        /// <param name="filename"></param>
        public TemplateFile(string filename)
        {
            Filename = filename;
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 文件变动事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            _text = null;
        }
        #endregion
    }
}
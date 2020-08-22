using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Earth.Library
{

    public delegate bool TagProcessDelegate(DataRow dr, string columnName, ref string result);
    public delegate bool EntityTagProcessDelegate(object entity, string columnName, ref string result);

    /// <summary>
    /// 模板标签处理
    /// </summary>
    public class HtmlTemplate
    {
        #region 私有变量
        /// <summary>
        /// 缓存的模板文件
        /// </summary>
        private static Dictionary<string, TemplateFile> tempateFiles = new Dictionary<string, TemplateFile>();

        /// <summary>
        /// 处理本模板文件的 StringBuilder，主要用户字符串的替换
        /// </summary>
        private StringBuilder sb;

        /// <summary>
        /// 当前块
        /// </summary>
        private Block currentBlock;

        /// <summary>
        /// 块堆栈
        /// </summary>
        private Stack<Block> stack = new Stack<Block>();
        #endregion

        #region 属性
        /// <summary>
        /// 输出结果
        /// </summary>
        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(currentBlock.Html))
                {
                    Flush();
                }
                return currentBlock.Html;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造 HTMLTemp 实例
        /// </summary>
        /// <param name="filename">模板文件名</param>
        public HtmlTemplate(string filename)
        {
            filename = filename.ToLower();

            TemplateFile tempateFile;
            if (tempateFiles.ContainsKey(filename))
            {
                tempateFile = tempateFiles[filename];
            }
            else
            {
                tempateFile = new TemplateFile(filename);
                //tempateFiles.Add(filename, tempateFile); 2018.09.17临时去掉：在win10不更新
            }

            sb = new StringBuilder(tempateFile.Text);
            currentBlock = new Block(sb);
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 打开块
        /// </summary>
        /// <param name="blockName">块名称</param>
        public void OpenBlock(string blockName)
        {
            stack.Push(currentBlock);

            currentBlock = new Block(blockName, sb);
            sb = currentBlock.StringBuilder;
        }

        /// <summary>
        /// 关闭块
        /// </summary>
        public void CloseBlock()
        {
            Block block = stack.Pop();
            block.StringBuilder.Replace(currentBlock.Text, currentBlock.Html);
            currentBlock = block;
            sb = currentBlock.StringBuilder;
            //currentBlock.iii();
        }

        /// <summary>
        /// 输出缓存文本
        /// </summary>
        public void Flush()
        {
            currentBlock.Append(sb.ToString());
            sb = new StringBuilder(currentBlock.InnerText);
            currentBlock.StringBuilder = sb;

        }

        public void Append(string text)
        {
            currentBlock.Append(text);
        }

        #endregion

        #region ReplaceVar函数
        /// <summary>
        /// 替换标签
        /// </summary>
        /// <param name="tagName">标签名</param>
        /// <param name="value">要替换的值</param>
        public void ReplaceVar(string tagName, object value)
        {
            string v = "";
            if (value != null)
            {
                v = value.ToString();
            }
            sb.Replace("%%" + tagName + "%%", v);
            sb.Replace("%{" + tagName + "}", v);
            sb.Replace("$" + tagName, v);
        }

        /// <summary>
        /// 用DataRow里的数据替换标签
        /// </summary>
        /// <param name="dr">数据行</param>
        public void ReplaceVar(DataRow dr)
        {
            ReplaceVar(dr, null);
        }

        /// <summary>
        /// 用DataRow里的数据替换标签
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="tagProcess">自定义标签处理委托</param>
        public void ReplaceVar(DataRow dr, string blockName)
        {
            ReplaceVar(dr, blockName, null);
        }

        /// <summary>
        /// 用DataRow里的数据替换标签
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="blockName">块名称</param>
        /// <param name="tagProcess">自定义标签处理委托</param>
        public void ReplaceVar(DataRow dr, string blockName, TagProcessDelegate tagProcess)
        {
            //if (!string.IsNullOrEmpty(blockName))
            //OpenBlock(blockName);

            foreach (DataColumn col in dr.Table.Columns)
            {
                string tagName = col.ColumnName;
                string newValue = "";
                if (tagProcess != null && tagProcess(dr, tagName, ref newValue))
                {
                }
                else
                {
                    newValue = dr[tagName].ToString();
                }
                ReplaceVar(tagName, newValue);
            }
        }

        /// <summary>
        /// 用DataTable里的数据替换标签
        /// </summary>
        /// <param name="dt">数据表DataTable</param>
        /// <param name="blockName">块名称</param>
        public void ReplaceVar(DataTable dt, string blockName)
        {
            ReplaceVar(dt, blockName, null);
        }

        /// <summary>
        /// 用DataTable里的数据替换标签
        /// </summary>
        /// <param name="dt">数据表DataTable</param>
        /// <param name="blockName">块名称</param>
        /// <param name="tagProcess">自定义标签处理委托</param>
        public void ReplaceVar(DataTable dt, string blockName, TagProcessDelegate tagProcess)
        {
            OpenBlock(blockName);

            foreach (DataRow dr in dt.Rows)
            {
                ReplaceVar(dr, "", tagProcess);
                Flush();
            }

            CloseBlock();
        }

        /// <summary>
        /// 用DataTable里的数据替换标签，用于Table形式的表格
        /// </summary>
        /// <param name="dt">数据表DataTable</param>
        /// <param name="blockName">块名称</param>
        /// <param name="columnCount">表格列数</param>
        public void ReplaceVar(DataTable dt, string blockName, int columnCount)
        {
            ReplaceVar(dt, blockName, columnCount, null, null, null);
        }

        /// <summary>
        /// 用DataTable里的数据替换标签，用于Table形式的表格
        /// </summary>
        /// <param name="dt">数据表DataTable</param>
        /// <param name="blockName">块名称</param>
        /// <param name="columnCount">表格列数</param>
        /// <param name="tagProcess">自定义标签处理委托</param>
        public void ReplaceVar(DataTable dt, string blockName, int columnCount, TagProcessDelegate tagProcess)
        {
            ReplaceVar(dt, blockName, columnCount, tagProcess, null, null);
        }

        /// <summary>
        /// 用DataTable里的数据替换标签，用于Table形式的表格
        /// </summary>
        /// <param name="dt">数据表DataTable</param>
        /// <param name="blockName">块名称</param>
        /// <param name="columnCount">表格列数</param>
        /// <param name="rowBeginTag">行开始HTML标签</param>
        /// <param name="emptyCellTag">空单元格标签</param>
        /// <param name="tagProcess">自定义标签处理委托</param>
        public void ReplaceVar(DataTable dt, string blockName, int columnCount, TagProcessDelegate tagProcess, string rowBeginTag, string emptyCellTag)
        {
            if (string.IsNullOrEmpty(rowBeginTag))
                rowBeginTag = "<tr>";

            if (string.IsNullOrEmpty(emptyCellTag))
                emptyCellTag = "<td>&nbsp;</td>";

            int n = 0;
            OpenBlock("ListByBoardNews");
            for (int h = 0; h < 999; h++)
            {
                Append(rowBeginTag);

                for (int L = 0; L < columnCount; L++)
                {
                    if (n < dt.Rows.Count)
                    {
                        ReplaceVar(dt.Rows[n]);
                        Flush();
                        n++;
                    }
                    else
                    {
                        Append(emptyCellTag);
                    }
                }

                Append("</tr>");
                if (n == dt.Rows.Count)
                    break;
            }
            CloseBlock();
        }



        #endregion

        public override string ToString()
        {
            return this.Text;
        }
    }
}
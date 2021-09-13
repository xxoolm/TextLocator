﻿using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using TextLocator.Core;
using TextLocator.Enums;

namespace TextLocator.Util
{
    /// <summary>
    /// 文件工具类
    /// </summary>
    public class FileUtil
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 根据文件类型获取文件图标
        /// </summary>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static BitmapImage GetFileIcon(FileType fileType)
        {
            Bitmap bitmap = null;
            switch (fileType)
            {
                case FileType.Word文档:
                    bitmap = Properties.Resources.word;
                    break;
                case FileType.Excel表格:
                    bitmap = Properties.Resources.excel;
                    break;
                case FileType.PPT演示文稿:
                    bitmap = Properties.Resources.ppt;
                    break;
                case FileType.PDF文档:
                    bitmap = Properties.Resources.pdf;
                    break;
                case FileType.HTML和XML:
                    bitmap = Properties.Resources.html;
                    break;
                case FileType.常用图片:
                    bitmap = Properties.Resources.image;
                    break;
                case FileType.代码文件:
                    bitmap = Properties.Resources.code;
                    break;
                case FileType.文本文件:
                    bitmap = Properties.Resources.txt;
                    break;
                default:
                    bitmap = Properties.Resources.rtf;
                    break;
            }
            BitmapImage bi = new BitmapImage();
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, bitmap.RawFormat);
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnLoad;
                bi.StreamSource = ms;
                bi.EndInit();
                bi.Freeze();
            }

            try
            {
                bitmap.Dispose();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            return bi;
        }

        /// <summary>
        /// 获取文件大小友好显示
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public static string GetFileSizeFriendly(long fileSize)
        {
            string fileSizeUnit = "b";
            if (fileSize > 1024)
            {
                fileSize = fileSize / 1024;
                fileSizeUnit = "KB";
            }
            if (fileSize > 1024)
            {
                fileSize = fileSize / 1024;
                fileSizeUnit = "MB";
            }
            if (fileSize > 1024)
            {
                fileSize = fileSize / 1024;
                fileSizeUnit = "GB";
            }
            return fileSize + "" + fileSizeUnit;
        }

        /// <summary>
        /// 超出范围
        /// </summary>
        /// <param name="fileSize"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool OutOfRange(long fileSize, int range = 10)
        {
            return false;
            /*if (fileSize <= 0)
            {
                return false;
            }
            return fileSize / 1024 / 1024 > range;*/
        }

        /// <summary>
        /// 获取指定根目录下的子目录及其文档
        /// </summary>
        /// <param name="rootPath">根目录路径</param>
        /// <param name="filePaths">文档列表</param>
        public static void GetAllFiles(string rootPath, List<string> filePaths)
        {
            // 根目录
            DirectoryInfo rootDir = new DirectoryInfo(rootPath);

            // 文件夹处理
            try
            {
                string[] dirs = Directory.GetDirectories(rootPath);
                foreach (string dir in dirs)
                {
                    string du = dir.ToUpper();
                    // $开始、360REC开头、SYSTEM、TEMP
                    if (du.StartsWith("$") || du.StartsWith("360REC") || du.Contains("SYSTEM") || du.Contains("TEMP"))
                    {
                        continue;
                    }
                    // 递归调用
                    GetAllFiles(dir, filePaths);
                }
            }
            catch { }

            // 文件处理
            try
            {
                // 查找word文件
                string[] paths = Directory.GetFiles(rootDir.FullName)
                    .Where(file => AppConst.REGIX_FILE_EXT.IsMatch(file))
                    .ToArray();
                // 遍历每个文档
                foreach (string path in paths)
                {
                    string fileName = path.Substring(path.LastIndexOf("\\") + 1);
                    if (fileName.StartsWith("`") || fileName.StartsWith("$"))
                    {
                        continue;
                    }
                    filePaths.Add(path);
                }
            }
            catch { }
        }
    }
}

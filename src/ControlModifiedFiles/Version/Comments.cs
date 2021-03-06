﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlModifiedFiles
{
    internal class Comments
    {
        internal FileSubscriber Subscriber { get; }
        internal FileInfo FileInfo { get; }
        private FileInfo _fileInfoComment;

        public Comments(FileSubscriber subscriber, FileInfo fileInfo = null)
        {
            Subscriber = subscriber ?? throw new ArgumentNullException("subscriber");
            FileInfo = fileInfo ?? new FileInfo(subscriber.Path);

            DirectoryInfo directoryInfo = new DirectoryInfo(Subscriber.DirectoryVersion);

            string pathFileComment = Path.Combine(directoryInfo.FullName, "comments.js");

            _fileInfoComment = new FileInfo(pathFileComment);
        }

        internal void UpdateCommentFile(string textComment)
        {
            List<CommentsVersion> comments = GetListComments();

            comments.Add(new CommentsVersion()
            {
                Comment = textComment,
                FileName = FileInfo.Name,
                DateTime = DateTime.Now
            });

            string dataFile = new Json<CommentsVersion>().Serialize(comments);

            if (!string.IsNullOrWhiteSpace(dataFile))
            {
                try
                {
                    using (StreamWriter stream = new StreamWriter(_fileInfoComment.FullName, false))
                    {
                        stream.WriteLine(dataFile);
                        stream.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Dialog.ShowMessage("Не удалось записать комментарий к версии.");
                    Errors.Save(ex);
                }
            }
        }

        internal List<CommentsVersion> GetListComments()
        {
            string dataFile = string.Empty;
            if (_fileInfoComment.Exists)
            {
                using (StreamReader reader = new StreamReader(_fileInfoComment.FullName))
                {
                    dataFile = reader.ReadToEnd();
                }
            }

            List<CommentsVersion> comments = null;
            if (string.IsNullOrWhiteSpace(dataFile))
                comments = new List<CommentsVersion>();
            else
                comments = new Json<CommentsVersion>().Deserialize(dataFile);

            return comments;
        }

        internal void OpenFormComment(int? numberVersion = null)
        {
            var mainWindow = App.Current.MainWindow;
            mainWindow.Activate();

            FormComment form = new FormComment(Subscriber, numberVersion) { Owner = mainWindow };
            form.ShowDialog();
            if (form.ClickOK)
            {
                string textComment = form.TextBoxComment.Text;
                if (!string.IsNullOrWhiteSpace(textComment))
                {
                    new Versions() { Subscriber = Subscriber }.SetCommentFile(
                        numberVersion == null ? form.Version : (int)numberVersion,
                        textComment);
                }
            }
        }

    }
}
